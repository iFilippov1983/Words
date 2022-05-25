using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "DataProfile", menuName = "Data/DataProfile")]
public class DataProfile : ScriptableObject
{
    public int CurrenLevelNumber;
    public bool MousePositionIsFar = false;

    private List<string> _usedExtraWords = new List<string>();

    public List<string> UsedExtraWords => _usedExtraWords;

    public void SetUsedExtraWordsList(List<string> list) => _usedExtraWords = list;
}
