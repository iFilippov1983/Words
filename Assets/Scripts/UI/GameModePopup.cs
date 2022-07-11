using Lofelt.NiceVibrations;
using UnityEngine;
using UnityEngine.UI;

public class GameModePopup : MonoBehaviour
{
    [SerializeField] private Button _showPopupButton;
    [SerializeField] private GameObject _gameModePopup;
    [SerializeField] private GameObject _backButtonObject;
    [SerializeField] private GameModeButtonView _wordsModeButton;
    [SerializeField] private GameModeButtonView _dotsModeButton;

    private Button _backButton;

    void Start()
    {
        _showPopupButton.onClick.AddListener(() => ShowPopup(true));

        _wordsModeButton.ChooseModeButton.onClick.AddListener(() => SetGameMode(_wordsModeButton, _dotsModeButton));
        _wordsModeButton.DescriptionButton.onClick.AddListener(() => ShowDescription(_wordsModeButton, true));
        _wordsModeButton.CloseDescriptionButton.onClick.AddListener(() => ShowDescription(_wordsModeButton, false));

        _dotsModeButton.ChooseModeButton.onClick.AddListener(() => SetGameMode(_dotsModeButton, _wordsModeButton));
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

    private void SetRepresentation()
    {
        GameModeType currentGameMode = (GameModeType)DataSaver.LoadIntData(DataKey.GameModeKey);
        if (currentGameMode.Equals(GameModeType.WordsMode))
        {
            _wordsModeButton.MarkerImage.gameObject.SetActive(true);
            _dotsModeButton.MarkerImage.gameObject.SetActive(false);
        }
        else if (currentGameMode.Equals(GameModeType.DotsMode))
        {
            _wordsModeButton.MarkerImage.gameObject.SetActive(false);
            _dotsModeButton.MarkerImage.gameObject.SetActive(true);
        }
    }

    private void SetGameMode(GameModeButtonView representativeToGameMode, GameModeButtonView representativeFromGameMode)
    {
        GameEvents.GameModeChangedMethod(representativeToGameMode.PresentedGameMode);
        representativeToGameMode.MarkerImage.gameObject.SetActive(true);

        representativeFromGameMode.MarkerImage.gameObject.SetActive(false);
    }

    private void ShowDescription(GameModeButtonView buttonView, bool show) => buttonView.DescriptionImage.gameObject.SetActive(show);
    public void ShowPopup(bool show)
    {
        _gameModePopup.SetActive(show);
        if (show)
            SetRepresentation();
    }
}
