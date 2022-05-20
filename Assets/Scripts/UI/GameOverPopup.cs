using Game;
using UnityEngine;
using UnityEngine.UI;
using System;
using Lofelt.NiceVibrations;

public class GameOverPopup : MonoBehaviour
{
    public GameObject gameOverPopup;
    public GameObject continueGameForCoins;
    public GameObject continueGameAfterAdsButton;
    public GameObject messageField;
    public int continueCost = 30;

    private Button _continueForAdsButton;
    private Button _continueForCoinsButton;

    public static Action ContinueWhithExtraTime;

    private void Start()
    {
        _continueForAdsButton = continueGameAfterAdsButton.GetComponent<Button>();

        _continueForCoinsButton = continueGameForCoins.GetComponent<Button>();
        _continueForCoinsButton.onClick.AddListener(TryBuySeconds);

        gameOverPopup.SetActive(false);

        GameEvents.OnGameOver += ShowGameOverPopup;
        CurrencyManager.CoinsAmountChangeImpossible += ShowMessage;
    }

    private void OnDestroy()
    {
        GameEvents.OnGameOver -= ShowGameOverPopup;
        CurrencyManager.CoinsAmountChangeImpossible -= ShowMessage;
    }

    private void ShowGameOverPopup()
    {
        gameOverPopup.SetActive(true);
        GameEvents.MenuIsActiveMethod(true);
        _continueForAdsButton.interactable = false;
        _continueForCoinsButton.interactable = true;
    }

    private void HideGameOverPopup()
    {
        gameOverPopup.SetActive(false);
        GameEvents.MenuIsActiveMethod(false);
        _continueForAdsButton.interactable = false;
        _continueForCoinsButton.interactable = false;
    }

    private void TryBuySeconds()
    {
        var cost = -continueCost;
        var succes = CurrencyManager.TryChangeCoinsAmountMethod(cost);
        if (succes)
        {
            ContinueWhithExtraTime?.Invoke();
            HideGameOverPopup();
        }   
    }

    private async void ShowMessage()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);
        Debug.Log("[Haptic] GameOverPopup - ShowMessage");

        var animation = messageField.GetComponent<Animation>();
        animation.Play();
        while(animation.isPlaying)
            await System.Threading.Tasks.Task.Yield();
    }
}
