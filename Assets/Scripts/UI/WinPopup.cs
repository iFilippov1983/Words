using Lofelt.NiceVibrations;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinPopup : MonoBehaviour
{
    public GameObject winPopup;
    public GameObject nextButtonObject;
    public GameObject exitButtonObject;
    public GameObject messageField;
    public ParticleSystem winParticle;
    [ReadOnly]
    public bool _categoryCompleted;

    private Button _nextButton;
    private Button _exitButton;

    void Start()
    {
        winPopup.SetActive(false);
        _nextButton = nextButtonObject.GetComponent<Button>();
        _nextButton.onClick.AddListener(LoadNextLevel);

        _exitButton = exitButtonObject.GetComponent<Button>();
        _exitButton.onClick.AddListener(Exit);
    }

    private void OnEnable()
    {
        GameEvents.OnBoardComleted += ShowWinPoppup;
    }

    private void OnDisable()
    {
        GameEvents.OnBoardComleted -= ShowWinPoppup;
    }

    private async void ShowWinPoppup(bool categoryCompleted)
    {
        CancellationToken token = new CancellationToken();
        token.ThrowIfCancellationRequested();
        await Task.Delay(2500, token);
        if (token.IsCancellationRequested) return;

        winPopup.SetActive(true);

        SoundManager.PalaySound(Sound.Win);
        Debug.Log("[Sound] WinPopup - ShowWinPoppup");

        GameEvents.MenuIsActiveMethod(true);
        winParticle.Play();

        var animation = winPopup.GetComponent<Animation>();
        if(animation)
            animation.Play();

        _categoryCompleted = categoryCompleted;
    }

    private async void LoadNextLevel()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        SoundManager.PalaySound(Sound.ButtonClicked);
        Debug.Log("[Haptic + Sound] WinPopup - LoadNextLevel");

        if (_categoryCompleted)
        {
            await ShowMessage();
            SceneManager.LoadScene(Literal.Scene_MainMenu);
            //SceneManager.LoadScene(Literal.Scene_SelectCategory);// Comment previous two lines of code and comment this line out if category selection avalable
        }
        else
        {
            winPopup.SetActive(false);
            GameEvents.LoadNextLevelMethod();
        }  
    }

    private void Exit()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        SoundManager.PalaySound(Sound.ButtonClicked);
        Debug.Log("[Haptic + Sound] WinPopup - Exit");

        GameUtility.LoadScene(Literal.Scene_MainMenu);
    }

    private async Task ShowMessage()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);
        Debug.Log("[Haptic] WinPopup - ShowMessage");

        var animation = messageField.GetComponent<Animation>();
        animation.Play();
        while (animation.isPlaying)
            await Task.Yield();
    }
}
