using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BoardResetManager : MonoBehaviour
{
    //private const string BoardResetKey = "BoardReset";
    private const int BoardResetUseCost = 1;

    [SerializeField] private WordsGrid _grid;
    [Space][SerializeField] private BuyBoardResetPopup _buyBoardResetPopup;
    [SerializeField] private Button _useOrBuyResetButton;
    [SerializeField] private TextMeshProUGUI _resetsAmountText;
    [SerializeField] private Image _plusImage;
    [SerializeField] private int _defaultResetsAmount = 3;

    private static bool _haveResets;
    private bool _canBuy;
    private bool _inMainMenu;

    public int BoardResets { get; private set; }

    public static Action OnBoardIsReset;
    public static Action OnDisactivateMenuInteraction;
    public static Action<int> TryChangeBoardResetsAmount;
    public static Action<string> BoardResetAmountChangeImpossible;

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
        TryChangeBoardResetsAmount += ChangeBoardResetsAmount;
        if (_inMainMenu) return;
        OnDisactivateMenuInteraction += DisableUseButtonClickability;
        GameEvents.OnCorrectWord += DisableUseButtonClickability;
        GameEvents.OnBoardConfigurationChanged += EnableUseButtonClickability;
        GameEvents.OnWordToPromptFound += PauseUseButtonClickability;
        BuyBoardResetPopup.ContinueWhithExtraResets += ChangeBoardResetsAmount;
        NoWordsAvailablePopup.ContinueWhithBoardReset += UseBoardReset;

        _useOrBuyResetButton.onClick.AddListener(ShowBuyResetsPopup);
        _useOrBuyResetButton.onClick.AddListener(UseBoardReset);
    }

    private void Cleanup()
    {
        CancelInvoke();
        OnDisactivateMenuInteraction -= DisableUseButtonClickability;
        TryChangeBoardResetsAmount -= ChangeBoardResetsAmount;
        GameEvents.OnCorrectWord -= DisableUseButtonClickability;
        GameEvents.OnBoardConfigurationChanged -= EnableUseButtonClickability;
        GameEvents.OnWordToPromptFound -= PauseUseButtonClickability;
        BuyBoardResetPopup.ContinueWhithExtraResets -= ChangeBoardResetsAmount;
        NoWordsAvailablePopup.ContinueWhithBoardReset -= UseBoardReset;
    }

    private void ShowBuyResetsPopup()
    {
        if (_canBuy == false)
            return;
        _buyBoardResetPopup.ShowBuyBoardResetPopup();
    }

    private async void UseBoardReset()
    {
        if (_inMainMenu || _canBuy)
            return;

        int amount = -BoardResetUseCost;
        bool success = TryChangeBoardResetsAmountMethod(amount);
        if (success)
        {
            OnDisactivateMenuInteraction?.Invoke();
            await _grid.ResetBoard();
            OnBoardIsReset?.Invoke();
        }
    }

    [Button]
    private void ChangeBoardResetsAmount(int amount)
    {
        var newAmount = BoardResets + amount;
        if (newAmount < 0)
        {
            _haveResets = false;
            BoardResetAmountChangeImpossible?.Invoke(Literal.AnimName_NoBoardResets);
        }
        else
        {
            _haveResets = true;
            SetBoardResets(newAmount);
        }
    }

    private void SetBoardResets(int boardResets)
    { 
        BoardResets = boardResets;
        _canBuy = BoardResets > 0
            ? false : true;

        if (!_inMainMenu)
        {
            _resetsAmountText.text = BoardResets.ToString();
            _plusImage.gameObject.SetActive(_canBuy);
        }
    }

    private void LoadData()
    {
        var startAmount = DataSaver.HasKey(DataKey.BoardResetKey)
            ? DataSaver.LoadIntData(DataKey.BoardResetKey)
            : _defaultResetsAmount;
        SetBoardResets(Mathf.Max(0, startAmount));
    }

    private void SaveData()
    { 
        DataSaver.SaveIntData(DataKey.BoardResetKey, BoardResets);
    }

    public static bool TryChangeBoardResetsAmountMethod(int amount)
    {
        TryChangeBoardResetsAmount?.Invoke(amount);
        return _haveResets;
    }

    private async void PauseUseButtonClickability(IEnumerable<int> obj)
    {
        _useOrBuyResetButton.interactable = false;
        CancellationToken token = new CancellationToken();
        token.ThrowIfCancellationRequested();
        await Task.Delay(3000, token);

        if (token.IsCancellationRequested) return;

        _useOrBuyResetButton.interactable = true;
    }

    private void DisableUseButtonClickability()
        => _useOrBuyResetButton.interactable = false;
    private void DisableUseButtonClickability(string word, IEnumerable<int> obj)
        => DisableUseButtonClickability();
    private void EnableUseButtonClickability()
        => _useOrBuyResetButton.interactable = true;
}
