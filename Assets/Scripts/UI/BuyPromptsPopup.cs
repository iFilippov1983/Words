using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Game;
using Lofelt.NiceVibrations;
using System.Threading.Tasks;

public class BuyPromptsPopup : MonoBehaviour, IAnimatableObjectParent
{
    [SerializeField] private GameObject _buyPromptsPopup;
    [SerializeField] private GameObject _buyPromptsButtonObject;
    [SerializeField] private GameObject _backButtonObject;
    [SerializeField] private Text _buyButtonText;
    [SerializeField] private TextMeshProUGUI _messageText;
    [SerializeField] private int _promptsBuyCost = 20;
    [SerializeField] private int _promptsBuyAmount = 1;

    private Button _buyPrompstButton;
    private Button _backButton;

    private bool _animationFinished;

    public static Action<int> ContinueWhithExtraPrompts;
    public event Action OnAnimationInitialize;
    public event Action OnAnimationFinish;

    private void Start()
    {
        _buyPrompstButton = _buyPromptsButtonObject.GetComponent<Button>();
        _buyPrompstButton.onClick.AddListener(TryBuyPrompts);

        _backButton = _backButtonObject.GetComponent<Button>();
        _backButton.onClick.AddListener(HidePopup);

        _buyButtonText.text = _promptsBuyCost.ToString();

        OnAnimationFinish += OnAnimationFinishMethod;
        CurrencyManager.CoinsAmountChangeImpossible += ShowMessage;
    }

    private void OnDestroy()
    {
        OnAnimationFinish -= OnAnimationFinishMethod;
        CurrencyManager.CoinsAmountChangeImpossible -= ShowMessage;
    }

    private async void TryBuyPrompts()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        SoundManager.PalaySound(Sound.ButtonClicked);
        Debug.Log("[Haptic + sound] BuyPromptsPopup - TryBuyPrompts");

        int cost = -_promptsBuyCost;
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
        Debug.Log("[Haptic] BuyPromptsPopup - ShowMessage");

        _messageText.gameObject.SetActive(true);

        var animation = _messageText.GetComponent<Animation>();
        animation.Play();
        while(animation.isPlaying)
            await Task.Yield();

        _messageText.gameObject.SetActive(false);
    }

    private void HidePopup()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        SoundManager.PalaySound(Sound.ButtonClicked);
        Debug.Log("[Haptic + sound] BuyPromptsPopup - HidePopup");

        _buyPromptsPopup.SetActive(false);
        GameEvents.MenuIsActiveMethod(false);
    }

    public void ShowBuyPromptsPopup()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        SoundManager.PalaySound(Sound.ButtonClicked);
        Debug.Log("[Haptic + sound] BuyPromptsPopup - ShowBuyPromptsPopup");

        _buyPromptsPopup.SetActive(true);
        GameEvents.MenuIsActiveMethod(true);
    }

    private void OnAnimationFinishMethod()
    {
        ContinueWhithExtraPrompts?.Invoke(_promptsBuyAmount);
        _animationFinished = true;
    }

    private async Task DesactivateButtonInteraction()
    {
        _buyPrompstButton.interactable = false;
        while (!_animationFinished)
            await Task.Yield();
        _buyPrompstButton.interactable = true;
        _animationFinished = false;
    }

    public void AnimationFinishCallback()
    {
        OnAnimationFinish?.Invoke();
    }
}
