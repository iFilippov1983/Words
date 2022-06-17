using TMPro;
using UnityEngine;

public class LevelTextView : MonoBehaviour
{
    public DataProfile dataProfile;
    public TextMeshProUGUI levelText;

    private async void Start()
    {
        while(dataProfile.isUpdated == false)
            await System.Threading.Tasks.Task.Yield();
        levelText.text = dataProfile.CurrentLevelNumber.ToString();
    }
}
