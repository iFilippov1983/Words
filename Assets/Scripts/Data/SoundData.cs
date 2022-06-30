using UnityEngine;

[CreateAssetMenu(fileName = "SoundData", menuName = "Data/SoundData")]
public class SoundData : ScriptableObject
{
    public AudioClip ButtonClickClip;
    public AudioClip SelectClip;
    public AudioClip WordInCellClip;
    public AudioClip WordBlowClip;
    public AudioClip WordWrongClip;
    public AudioClip CoinClip;
    public AudioClip WinClip;
    public AudioClip LooseClip;
    public AudioClip PromptClip;
}
