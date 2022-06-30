using Lofelt.NiceVibrations;
using UnityEngine;
using UnityEngine.UI;

public class GameModePopup : MonoBehaviour
{
    [SerializeField] private GameObject _gameModePopup;
    [SerializeField] private GameObject _backButtonObject;
    [SerializeField] private GameModeButtonView _wordsModeButton;
    [SerializeField] private GameModeButtonView _dotsModeButton;

    private Button _backButton;

    void Start()
    {

        _backButton = _backButtonObject.GetComponent<Button>();
        _backButton.onClick.AddListener(HidePopup);
    }

    private void HidePopup()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        SoundManager.PalaySound(Sound.ButtonClicked);
        Debug.Log("[Haptic + sound] GameModePopup - HidePopup");

        ShowPopup(false);
    }

    public void ShowPopup(bool show) => _gameModePopup.SetActive(show);
}
