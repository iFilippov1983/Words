using Lofelt.NiceVibrations;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoWordsAvailablePopup : MonoBehaviour
{
    [SerializeField] private GameObject _noWordsPopup;
    [SerializeField] private GameObject _resetBoardButtonObject;
    [SerializeField] private GameObject _restatButtonObject;
    [SerializeField] private GameObject _backButtonObject;
    [SerializeField] private GameObject _exitButtonObject;
    [SerializeField] private GameObject _messageFieldObject;
    [SerializeField] private int _resetBoardCost = 1;
    [SerializeField] private int _lifesLostCost = 1;

    private Button _resetBoardButton;
    private Button _restatButton;
    private Button _backButton;
    private Button _exitButton;

    public static Action ContinueWhithBoardReset;

    private void Start()
    {
        _resetBoardButton = _resetBoardButtonObject.GetComponent<Button>();
        _resetBoardButton.onClick.AddListener(TryContinueWhithBoardReset);

        _restatButton = _restatButtonObject.GetComponent<Button>();
        _restatButton.onClick.AddListener(TryRestart);

        _backButton = _backButtonObject.GetComponent<Button>();
        _backButton.onClick.AddListener(Back);

        _exitButton = _exitButtonObject.GetComponent<Button>();
        _exitButton.onClick.AddListener(Exit);

        _noWordsPopup.SetActive(false);

        GameEvents.OnNoWordsAvailable += ShowNoWordsPopup;
        BoardResetManager.BoardResetAmountChangeImpossible += ShowMessage;
        LifesManager.LifesAmountChangeImpossible += ShowMessage;
    }

    private void OnDestroy()
    {
        CancelInvoke();

        GameEvents.OnNoWordsAvailable -= ShowNoWordsPopup;
        BoardResetManager.BoardResetAmountChangeImpossible -= ShowMessage;
        LifesManager.LifesAmountChangeImpossible -= ShowMessage;
    }

    [Button]
    private void ShowNoWordsPopup()
    {
        _noWordsPopup.SetActive(true);
        GameEvents.MenuIsActiveMethod(true);

        var animation = _noWordsPopup.GetComponent<Animation>();
        if(animation)
            animation.Play();
    }

    private void HideNoWordsPopup()
    { 
        _noWordsPopup.SetActive(false);
        GameEvents.MenuIsActiveMethod(false);
    }

    private void TryContinueWhithBoardReset()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        SoundManager.PalaySound(Sound.ButtonClicked);
        Debug.Log("[Haptic + Sound] NoWordsAvailablePopup - TryContinueWhithBoardReset");

        var cost = -_resetBoardCost;
        bool success = BoardResetManager.TryChangeBoardResetsAmountMethod(cost);
        if (success)
        { 
            HideNoWordsPopup();
            ContinueWhithBoardReset?.Invoke();
            return;
        }

        ShowMessage(Literal.AnimName_NoBoardResets);
    }

    private void TryRestart()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        SoundManager.PalaySound(Sound.ButtonClicked);
        Debug.Log("[Haptic + Sound] NoWordsAvailablePopup - TryRestart");

        int cost = -_lifesLostCost;
        bool succes = LifesManager.TryChangeLifesAmountMethod(cost);
        if (succes)
        {
            GameUtility.LoadScene(Literal.Scene_GameScene);
            return;
        }

        ShowMessage(Literal.AnimName_NoLifes);
    }

    private async void ShowMessage(string animationName)
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);
        Debug.Log("[Haptic] NoWordsAvailablePopup - ShowMessage");

        var animation = _messageFieldObject.GetComponent<Animation>();
        animation.Play(animationName);
        while (animation.isPlaying)
            await System.Threading.Tasks.Task.Yield();
    }

    private void Back()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        SoundManager.PalaySound(Sound.ButtonClicked);
        Debug.Log("[Haptic + Sound] NoWordsAvailablePopup - Back");

        HideNoWordsPopup();
    }

    private void Exit()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        SoundManager.PalaySound(Sound.ButtonClicked);
        Debug.Log("[Haptic + Sound] NoWordsAvailablePopup - Exit");

        int cost = -_lifesLostCost;
        _ = LifesManager.TryChangeLifesAmountMethod(cost);
        GameUtility.LoadScene(Literal.Scene_MainMenu);
    }
}
