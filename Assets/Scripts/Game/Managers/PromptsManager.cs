using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PromptsManager : MonoBehaviour
{
    //private const string PromptsKey = "Prompts";
    private const int PromptUseCost = 1;

    [SerializeField] private Prompter _prompter;
    [Space][SerializeField] private BuyPromptsPopup _buyPromptPopup;
    [SerializeField] private Button _useOrBuyPromptButton;
    [SerializeField] private TextMeshProUGUI _promptsAmountText;
    [SerializeField] private Image _plusImage;
    [SerializeField] private Animation _buisyAnimation;
    [SerializeField] private int _defaultPromptsAmount = 3;

    private static bool _havePrompts;
    private bool _canBuy;
    private bool _inMainMenu;

    public int Prompts { get; private set; }

    public static Action<int> PromptsAmountChanged;
    public static Action<int> TryChangePromptsAmount;
    public static Action<string> PromptsAmountChangeImpossible;

    void Start()
    {
        _inMainMenu = SceneManager.GetActiveScene().name.Equals(Literal.Scene_MainMenu);
        LoadData();
        Init();
    }

    private void OnDestroy()
    {
        SaveData();
        Cleanup();
    }

    private void Init()
    {
        TryChangePromptsAmount += ChangePromptsAmount;
        if (_inMainMenu) return;
        GameEvents.OnCorrectWord += DisableUseButtonClickability;
        GameEvents.OnBoardConfigurationChanged += EnableUseButtonClickability;
        BoardResetManager.OnDisactivateMenuInteraction += DisableUseButtonClickability;
        BuyPromptsPopup.ContinueWhithExtraPrompts += ChangePromptsAmount;

        _useOrBuyPromptButton.onClick.AddListener(ShowBuyPromptsPopup);
        _useOrBuyPromptButton.onClick.AddListener(UsePrompt);
    }

    private void Cleanup()
    {
        CancelInvoke();
        TryChangePromptsAmount -= ChangePromptsAmount;
        GameEvents.OnCorrectWord -= DisableUseButtonClickability;
        GameEvents.OnBoardConfigurationChanged -= EnableUseButtonClickability;
        BoardResetManager.OnDisactivateMenuInteraction -= DisableUseButtonClickability;
        BuyPromptsPopup.ContinueWhithExtraPrompts -= ChangePromptsAmount;
    }

    private void ShowBuyPromptsPopup()
    {
        if (_canBuy == false)
            return;
        _buyPromptPopup.ShowBuyPromptsPopup();
    }

    private async void UsePrompt()
    {
        if (_inMainMenu || _canBuy)
            return;

        int amount = -PromptUseCost;
        bool success = TryChangePromptAmountMethod(amount);
        if (success)
        {
            DisableUseButtonClickability();
            await Prompter.MakeManualPrompt();
            Timer.ResetPromptTimerMethod();
            EnableUseButtonClickability();
        }
    }

    [Button]
    private void ChangePromptsAmount(int amount)
    { 
        var newAmount = Prompts + amount;
        if (newAmount < 0)
        {
            _havePrompts = false;
            PromptsAmountChangeImpossible?.Invoke(Literal.AnimName_NoPrompts);
        }
        else
        {
            _havePrompts = true;
            SetPrompts(newAmount);
            PromptsAmountChanged?.Invoke(amount);
        }
    }

    private void SetPrompts(int prompts)
    {
        Prompts = prompts;
        _canBuy = Prompts > 0
            ? false : true;

        if (!_inMainMenu)
        {
            _promptsAmountText.text = Prompts.ToString();
            _plusImage.gameObject.SetActive(_canBuy);
        }
    }

    private void LoadData()
    {
        var startPrompts = DataSaver.HasKey(DataKey.PromptsKey)
            ? DataSaver.LoadIntData(DataKey.PromptsKey)
            : _defaultPromptsAmount;
        SetPrompts(Mathf.Max(0, startPrompts));
    }

    private void SaveData()
    {
        DataSaver.SaveIntData(DataKey.PromptsKey, Prompts);
    }

    public static bool TryChangePromptAmountMethod(int amount)
    { 
        TryChangePromptsAmount?.Invoke(amount);
        return _havePrompts;
    }

    private async void PauseUseButtonClickability(IEnumerable<int> obj)
    {
        _useOrBuyPromptButton.interactable = false;
        CancellationToken token = new CancellationToken();
        token.ThrowIfCancellationRequested();
        await Task.Delay(3000, token);

        if (token.IsCancellationRequested) return;

        _useOrBuyPromptButton.interactable = true;
    }

    private void DisableUseButtonClickability()
        => DisableUseButtonClickability(string.Empty, new int[0]);
    private void DisableUseButtonClickability(string word, IEnumerable<int> obj) 
        => _useOrBuyPromptButton.interactable = false;
    private void EnableUseButtonClickability() 
        => _useOrBuyPromptButton.interactable = true;
}
