using TMPro;
using UnityEngine;

public class LevelTextView : MonoBehaviour
{
    public DataProfile dataProfile;
    public TextMeshProUGUI levelText;

    void Start()
    {
        levelText.text = dataProfile.CurrenLevelNumber.ToString();
    }

}
