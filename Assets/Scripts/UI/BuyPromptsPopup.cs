using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Game;
using Lofelt.NiceVibrations;

public class BuyPromptsPopup : MonoBehaviour
{
    [SerializeField] private GameObject _buyPromptsPopup;
    [SerializeField] private GameObject _buyPromptsButtonObject;
    [SerializeField] private GameObject _backButtonObject;
    [SerializeField] private TextMeshProUGUI _messageText;
    [SerializeField] private int _promptsBuyCost = 20;
    [SerializeField] private int _promptsBuyAmount = 1;

    private Button _buyPrompstButton;
    private Button _backButton;

    public static Action<int> ContinueWhithExtraPrompts;
 
    void Start()
    {
        _buyPrompstButton = _buyPromptsButtonObject.GetComponent<Button>();
        _buyPrompstButton.onClick.AddListener(TryBuyPrompts);

        _backButton = _backButtonObject.GetComponent<Button>();
        _backButton.onClick.AddListener(HidePopup);

        CurrencyManager.CoinsAmountChangeImpossible += ShowMessage;
    }

    private void OnDestroy()
    {
        CurrencyManager.CoinsAmountChangeImpossible -= ShowMessage;
    }

    private void TryBuyPrompts()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        SoundManager.PalaySound(Sound.ButtonClicked);
        Debug.Log("[Haptic + sound] BuyPromptsPopup - TryBuyPrompts");

        int cost = -_promptsBuyCost;
        bool succes = CurrencyManager.TryChangeCoinsAmountMethod(cost);
        if (succes)
        {
            ContinueWhithExtraPrompts?.Invoke(_promptsBuyAmount);
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
            await System.Threading.Tasks.Task.Yield();

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
    
}
