using System.Threading.Tasks;
using UnityEngine;

public class WinPopup : MonoBehaviour
{
    public GameObject winPopup;

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

    private async void ShowWinPoppup()
    {
        await Task.Delay(2500);
        winPopup.SetActive(true);
    }

    public void LoadNextLevel()
    {
        GameEvents.LoadNextLevelMethod();
    }
}
