using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinPopup : MonoBehaviour
{
    public GameObject winPopup;
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

    public void LoadNextLevel()
    {
        if(_categoryCompleted)
            SceneManager.LoadScene(Literal.Scene_SelectCategory);
        else
            GameEvents.LoadNextLevelMethod();
    }
}
