using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "GameData", menuName = "Data/GameData")]
public class GameData : ScriptableObject
{
    public GameModeType selectedGameMode;
    public BoardData selectedBoardData;
}
