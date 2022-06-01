using Lofelt.NiceVibrations;
using UnityEngine;
using UnityEngine.UI;

public class AwarePopup : MonoBehaviour
{
    private const int ExitCost = -1;

    [SerializeField] private GameObject _popupObject;
    [SerializeField] private Button _yesButton;
    [SerializeField] private Button _backButton;

    void Start()
    {
        _yesButton.onClick.AddListener(Exit);
        _backButton.onClick.AddListener(Back);
    }

    private void Exit()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        SoundManager.PalaySound(Sound.ButtonClicked);
        Debug.Log("[Haptic + sound] AwarePopup - Exit");

        _ = LifesManager.TryChangeLifesAmountMethod(ExitCost);
        GameUtility.LoadScene(Literal.Scene_MainMenu);
    }

    private void Back()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        SoundManager.PalaySound(Sound.ButtonClicked);
        Debug.Log("[Haptic + sound] AwarePopup - Back");

        _popupObject.SetActive(false);
    }
}
