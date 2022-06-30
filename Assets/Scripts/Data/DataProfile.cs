using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "DataProfile", menuName = "Data/DataProfile")]
public class DataProfile : ScriptableObject
{
    public int CurrentLevelNumber;
    public GameModeType GameMode;
    public bool MousePositionIsFar = false;
    public bool BoardCanPrompt = false;
    public bool isUpdated = false;

    private List<string> _usedExtraWords = new List<string>();

    public List<string> UsedWords => _usedExtraWords;

    public void SetUsedExtraWordsList(List<string> list) => _usedExtraWords = list;
}
