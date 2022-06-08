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
    private const string BoardResetKey = "BoardReset";
    private const int BoardResetUseCost = 1;

    [SerializeField] private WordsGrid _grid;
    [Space][SerializeField] private BuyBoardResetPopup _buyBoardResetPopup;
    [SerializeField] private Button _useOrBuyResetButton;
    [SerializeField] private TextMeshProUGUI _resetsAmountText;
    [SerializeField] private Image _plusImage;
    [SerializeField] private int _defaultResetsAmount = 3;

    private bool _haveResets;
    private bool _canBuy;
    private bool _inMainMenu;

    public int BoardResets { get; private set; }

    public static Action OnBoardIsReset;
    public static Action OnDisactivateMenuInteraction;
    public static Action<int> TryChangeBoardResetsAmount;
    public static Action<string> BoardResetAmountChangeImpossible;

    void Start()
    {
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
        _inMainMenu = (_useOrBuyResetButton != null
            && SceneManager.GetActiveScene().name.Equals(Literal.Scene_MainMenu));

        _useOrBuyResetButton.onClick.AddListener(ShowBuyResetsPopup);
        _useOrBuyResetButton.onClick.AddListener(UseBoardReset);


        OnDisactivateMenuInteraction += DisableUseButtonClickability;
        TryChangeBoardResetsAmount += ChangeBoardResetsAmount;
        GameEvents.OnCorrectWord += DisableUseButtonClickability;
        GameEvents.OnBoardConfigurationChanged += EnableUseButtonClickability;
        GameEvents.OnWordToPromptFound += PauseUseButtonClickability;
        BuyBoardResetPopup.ContinueWhithExtraResets += ChangeBoardResetsAmount;
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
        _resetsAmountText.text = BoardResets.ToString();
        _canBuy = BoardResets > 0
            ? false : true;

        if (!_inMainMenu)
            _plusImage.gameObject.SetActive(_canBuy);
    }

    private void LoadData()
    {
        var startAmount = DataSaver.HasKey(BoardResetKey)
            ? DataSaver.LoadIntData(BoardResetKey)
            : _defaultResetsAmount;
        SetBoardResets(Mathf.Max(0, startAmount));
    }

    private void SaveData()
    { 
        DataSaver.SaveIntData(BoardResetKey, BoardResets);
    }

    private bool TryChangeBoardResetsAmountMethod(int amount)
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
        => DisableUseButtonClickability(string.Empty, new int[0]);
    private void DisableUseButtonClickability(string word, IEnumerable<int> obj)
        => _useOrBuyResetButton.interactable = false;
    private void EnableUseButtonClickability()
        => _useOrBuyResetButton.interactable = true;
}
