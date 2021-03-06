using Game;
using UnityEngine;
using UnityEngine.UI;
using System;
using Lofelt.NiceVibrations;

public class GameOverPopup : MonoBehaviour
{
    public GameObject gameOverPopup;
    public GameObject continueForCoinsButtonObject;
    public GameObject continueForAdsButtonObject;
    public GameObject restartButtonObject;
    public GameObject exitButtonObject;
    public GameObject messageField;
    public int continueCoinsCost = 30;
    public int lifesLostCost = 1;

    private Button _continueForAdsButton;
    private Button _continueForCoinsButton;
    private Button _restartButton;
    private Button _exitButton;

    public static Action ContinueWhithExtraTime;

    private void Start()
    {
        _continueForAdsButton = continueForAdsButtonObject.GetComponent<Button>();

        _continueForCoinsButton = continueForCoinsButtonObject.GetComponent<Button>();
        _continueForCoinsButton.onClick.AddListener(TryBuySeconds);

        _restartButton = restartButtonObject.GetComponent<Button>();
        _restartButton.onClick.AddListener(TryRestart);

        _exitButton = exitButtonObject.GetComponent<Button>();
        _exitButton.onClick.AddListener(Exit);

        gameOverPopup.SetActive(false);

        GameEvents.OnGameOver += ShowGameOverPopup;
        CurrencyManager.CoinsAmountChangeImpossible += ShowMessage;
    }

    private void OnDestroy()
    {
        CancelInvoke();

        GameEvents.OnGameOver -= ShowGameOverPopup;
        CurrencyManager.CoinsAmountChangeImpossible -= ShowMessage;
    }

    private void ShowGameOverPopup()
    {
        gameOverPopup.SetActive(true);

        SoundManager.PalaySound(Sound.Loose);
        Debug.Log("[Sound] GameOverPopup - ShowGameOverPopup");

        GameEvents.MenuIsActiveMethod(true);
        _continueForAdsButton.interactable = false;
        _continueForCoinsButton.interactable = true;

        var animation = gameOverPopup.GetComponent<Animation>();
        if (animation)
            animation.Play();
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
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        SoundManager.PalaySound(Sound.ButtonClicked);
        Debug.Log("[Haptic + Sound] GameOverPopup - TryBuySeconds");

        var cost = -continueCoinsCost;
        var succes = CurrencyManager.TryChangeCoinsAmountMethod(cost);
        if (succes)
        {
            ContinueWhithExtraTime?.Invoke();
            HideGameOverPopup();
        }   
    }

    private void TryRestart()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        SoundManager.PalaySound(Sound.ButtonClicked);
        Debug.Log("[Haptic + Sound] GameOverPopup - TryRestart");

        int cost = -lifesLostCost;
        bool success = LifesManager.TryChangeLifesAmountMethod(cost);
        if (success)
        {
            GameUtility.LoadScene(Literal.Scene_GameScene);
            return;
        }

        ShowMessage(Literal.AnimName_NoLifes);
    }

    private async void ShowMessage(string animationName)
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);
        Debug.Log("[Haptic] GameOverPopup - ShowMessage");

        var animation = messageField.GetComponent<Animation>();
        animation.Play(animationName);
        while(animation.isPlaying)
            await System.Threading.Tasks.Task.Yield();
    }

    private void Exit()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        SoundManager.PalaySound(Sound.ButtonClicked);
        Debug.Log("[Haptic + Sound] GameOverPopup - Exit");

        int cost = -lifesLostCost;
        _ = LifesManager.TryChangeLifesAmountMethod(cost);
        GameUtility.LoadScene(Literal.Scene_MainMenu);
    }
}
