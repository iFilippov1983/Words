using UnityEngine;
using UnityEngine.UI;

public class GameOverPopup : MonoBehaviour
{
    public GameObject gameOverPopup;
    public GameObject continueGameForCoins;
    public GameObject continueGameAfterAdsButton;

    private void Start()
    {
        continueGameAfterAdsButton.GetComponent<Button>().interactable = false;
        gameOverPopup.SetActive(false);

        GameEvents.OnGameOver += ShowGameOverPopup;
    }

    private void OnDisable()
    {
        GameEvents.OnGameOver -= ShowGameOverPopup;
    }

    private void ShowGameOverPopup()
    {
        gameOverPopup.SetActive(true);
        //continueGameForCoins.GetComponent<Button>().interactable = true;
        continueGameAfterAdsButton.GetComponent <Button>().interactable = false;
    }
}
