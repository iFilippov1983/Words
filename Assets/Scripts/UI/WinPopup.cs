using Lofelt.NiceVibrations;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinPopup : MonoBehaviour
{
    public GameObject winPopup;
    public GameObject messageField;
    public bool _categoryCompleted;

    void Start()
    {
        winPopup.SetActive(false);
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
        await Task.Delay(2500);
        winPopup.SetActive(true);
        _categoryCompleted = categoryCompleted;
    }

    public async void LoadNextLevel()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        Debug.Log("[Haptic] WinPopup - LoadNextLevel");

        if (_categoryCompleted)
        {
            await ShowMessage();
            SceneManager.LoadScene(Literal.Scene_MainMenu);
            //SceneManager.LoadScene(Literal.Scene_SelectCategory);// Comment previous two lines of code and comment this line out if category selection avalable
        }
        else
            GameEvents.LoadNextLevelMethod();
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
