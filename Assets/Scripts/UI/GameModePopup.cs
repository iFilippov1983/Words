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
        _wordsModeButton.ChooseModeButton.onClick.AddListener(() => SetGameMode(GameModeType.WordsMode));
        _wordsModeButton.DescriptionButton.onClick.AddListener(() => ShowDescription(_wordsModeButton, true));
        _wordsModeButton.CloseDescriptionButton.onClick.AddListener(() => ShowDescription(_wordsModeButton, false));

        _dotsModeButton.ChooseModeButton.onClick.AddListener(() => SetGameMode(GameModeType.DotsMode));
        _dotsModeButton.DescriptionButton.onClick.AddListener(() => ShowDescription(_dotsModeButton, true));
        _dotsModeButton.CloseDescriptionButton.onClick.AddListener(() => ShowDescription(_dotsModeButton, false));

        _backButton = _backButtonObject.GetComponent<Button>();
        _backButton.onClick.AddListener(HidePopup);
    }

    private void HidePopup()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        SoundManager.PalaySound(Sound.ButtonClicked);
        Debug.Log("[Haptic + sound] GameModePopup - HidePopup");

        ShowDescription(_wordsModeButton, false);
        ShowDescription(_dotsModeButton, false);
        ShowPopup(false);
    }

    private void SetGameMode(GameModeType gameModeType) => GameEvents.GameModeChangedMethod(gameModeType);
    private void ShowDescription(GameModeButtonView buttonView, bool show) => buttonView.DescriptionImage.gameObject.SetActive(show);
    public void ShowPopup(bool show) => _gameModePopup.SetActive(show);
}
