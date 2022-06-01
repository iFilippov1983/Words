using Lofelt.NiceVibrations;
using UnityEngine;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    [SerializeField] private GameObject _buyLifesPopup;
    [SerializeField] private LifesManager _lifesManager;
    private Button _playButton;

    void Start()
    {
        _playButton = GetComponent<Button>();
        _playButton.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (_lifesManager.Lifes > 0)
        {
            GameUtility.LoadScene(Literal.Scene_GameScene);
        }
        else
        { 
            _buyLifesPopup.gameObject.SetActive(true);
        }

        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        SoundManager.PalaySound(Sound.ButtonClicked);
        Debug.Log("[Haptic + sound] PlayButton - OnClick");
    }
}
