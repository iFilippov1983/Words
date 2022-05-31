using UnityEngine;
using UnityEngine.UI;

public class AwarePopup : MonoBehaviour
{
    private const int ExitCost = -1;
    [SerializeField] private Button _yesButton;

    void Start()
    {
        _yesButton.onClick.AddListener(Exit);
    }

    private void Exit()
    {
        _ = LifesManager.TryChangeLifesAmountMethod(ExitCost);
        GameUtility.LoadScene(Literal.Scene_MainMenu);
    }
}
