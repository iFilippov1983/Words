using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelCell : MonoBehaviour
{
    public Sprite SpriteToPlay;
    public Color ColorToPlay;
    public Sprite SpritePassive;
    public Color ColorPassive;
    public Sprite SpriteCompleted;
    public Color ColorCompleted;
    [Space]   

    public Image LevelImage;
    public Image ToLevelLine;
    public TextMeshProUGUI LevelNumber;
}
