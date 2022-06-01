using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private SoundData _soundData;

    private AudioSource _audioSource;
    private Dictionary<Sound, AudioClip> _clips;

    private static Action<Sound> PlaySoundAction;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _clips = MakeSoundsDictionary(_soundData);

        PlaySoundAction += Play;
    }

    private void OnDestroy()
    {
        PlaySoundAction -= Play;
    }

    private void Play(Sound sound)
    {
        _audioSource.clip = _clips[sound];
        _audioSource.Play();
    }

    private Dictionary<Sound, AudioClip> MakeSoundsDictionary(SoundData soundData)
    { 
        var dic = new Dictionary<Sound, AudioClip>();

        dic.Add(Sound.ButtonClicked, soundData.ButtonClickClip);
        dic.Add(Sound.Selected, soundData.SelectClip);
        dic.Add(Sound.Word_Blow, soundData.WordBlowClip);
        dic.Add(Sound.Word_InCell, soundData.WordInCellClip);
        dic.Add(Sound.Word_Wrong, soundData.WordWrongClip);
        dic.Add(Sound.Coin, soundData.CoinClip);
        dic.Add(Sound.Win, soundData.WinClip);
        dic.Add(Sound.Loose, soundData.LooseClip);
        dic.Add(Sound.Prompt, soundData.PromptClip);

        return dic;
    }

    public static void PalaySound(Sound sound)
    {
        PlaySoundAction?.Invoke(sound);
    }
}

public enum Sound
{
    ButtonClicked,
    Selected,
    Word_Blow,
    Word_InCell,
    Word_Wrong,
    Coin,
    Win,
    Loose,
    Prompt
}
