using Game;
using Lofelt.NiceVibrations;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyBoardResetPopup : MonoBehaviour
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

    public static Action<int> ContinueWhithExtraResets;

    void Start()
    {
        _buyResetButton = _buyBoardResetButtonObject.GetComponent<Button>();
        _buyResetButton.onClick.AddListener(TryBuyResets);

        _backButton = _backButtonObject.GetComponent<Button>();
        _backButton.onClick.AddListener(HidePopup);

        _buyButtonText.text = _resetBuyCost.ToString();
        CurrencyManager.CoinsAmountChangeImpossible += ShowMessage;
    }

    private void OnDestroy()
    {
        CurrencyManager.CoinsAmountChangeImpossible -= ShowMessage;
    }

    private void TryBuyResets()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        SoundManager.PalaySound(Sound.ButtonClicked);
        Debug.Log("[Haptic + sound] BuyBoardResetPopup - TryBuyResets");

        int cost = -_resetBuyCost;
        bool succes = CurrencyManager.TryChangeCoinsAmountMethod(cost);
        if (succes)
        {
            ContinueWhithExtraResets?.Invoke(_resetBuyAmount);
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
            await System.Threading.Tasks.Task.Yield();

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
}
