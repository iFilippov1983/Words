using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "AlphabetData", menuName = "Data/AlphabetData")]
public class AlphabetData : ScriptableObject
{
    [Serializable]
    public class LetterData
    {
        public string Letter;
        public Sprite Sprite;
    }

    public List<LetterData> AlphabetPlane = new List<LetterData>();
    public List<LetterData> AlphabetNormal = new List<LetterData>();
    public List<LetterData> AlphabetHighlighted = new List<LetterData>();
    public List<LetterData> AlphabetCorrect = new List<LetterData>();
}
