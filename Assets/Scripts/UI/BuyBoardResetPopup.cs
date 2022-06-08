using Game;
using Lofelt.NiceVibrations;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyBoardResetPopup : MonoBehaviour, IAnimatableObjectParent
{
    [SerializeField] private GameObject _buyBoardResetPopup;
    [SerializeField] private GameObject _buyBoardResetButtonObject;
    [SerializeField] private GameObject _backButtonObject;
    [SerializeField] private Text _buyButtonText;
    [SerializeField] private TextMeshProUGUI _messageText;
    [SerializeField] private int _resetBuyCost = 30;
    [SerializeField] private int _resetBuyAmount = 1;

    private Button _buyResetButton;
    private Button _backButton;

    private bool _animationFinished;

    public static Action<int> ContinueWhithExtraResets;
    public event Action OnAnimationInitialize;
    public event Action OnAnimationFinish;

    void Start()
    {
        _buyResetButton = _buyBoardResetButtonObject.GetComponent<Button>();
        _buyResetButton.onClick.AddListener(TryBuyResets);

        _backButton = _backButtonObject.GetComponent<Button>();
        _backButton.onClick.AddListener(HidePopup);

        _buyButtonText.text = _resetBuyCost.ToString();

        OnAnimationFinish += OnAnimationFinishedMethod;
        CurrencyManager.CoinsAmountChangeImpossible += ShowMessage;
    }

    private void OnDestroy()
    {
        OnAnimationFinish -= OnAnimationFinishedMethod;
        CurrencyManager.CoinsAmountChangeImpossible -= ShowMessage;
    }

    private async void TryBuyResets()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        SoundManager.PalaySound(Sound.ButtonClicked);
        Debug.Log("[Haptic + sound] BuyBoardResetPopup - TryBuyResets");

        int cost = -_resetBuyCost;
        bool succes = CurrencyManager.TryChangeCoinsAmountMethod(cost);
        if (succes)
        {
            OnAnimationInitialize?.Invoke();

            await DesactivateButtonInteraction();
        }
    }

    private async void ShowMessage(string animationName)
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);
        Debug.Log("[Haptic] BuyBoardResetPopup - ShowMessage");

        _messageText.gameObject.SetActive(true);

        var animation = _messageText.GetComponent<Animation>();
        animation.Play();
        while (animation.isPlaying)
            await Task.Yield();

        _messageText.gameObject.SetActive(false);
    }

    private void HidePopup()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        SoundManager.PalaySound(Sound.ButtonClicked);
        Debug.Log("[Haptic + sound] BuyBoardResetPopup - HidePopup");

        _buyBoardResetPopup.SetActive(false);
        GameEvents.MenuIsActiveMethod(false);
    }

    public void ShowBuyBoardResetPopup()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        SoundManager.PalaySound(Sound.ButtonClicked);
        Debug.Log("[Haptic + sound] BuyBoardResetPopup - ShowBuyPromptsPopup");

        _buyBoardResetPopup.SetActive(true);
        GameEvents.MenuIsActiveMethod(true);
    }

    private void OnAnimationFinishedMethod()
    {
        ContinueWhithExtraResets?.Invoke(_resetBuyAmount);
        _animationFinished = true;
    }

    private async Task DesactivateButtonInteraction()
    {
        _buyResetButton.interactable = false;
        while(!_animationFinished)
            await Task.Yield();
        _buyResetButton.interactable |= true;
        _animationFinished = false;
    }

    public void AnimationFinishCallback()
    {
        OnAnimationFinish?.Invoke();
    }
}
