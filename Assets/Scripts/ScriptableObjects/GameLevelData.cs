using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "GameLevelData", menuName = "Data/GameLevelData")]
public class GameLevelData : ScriptableObject
{
    [Serializable]
    public struct CategoryRecord
    { 
        public string CategoryName;
        public List<BoardData> BoardData;
    }

    public List<CategoryRecord> Data;
}
