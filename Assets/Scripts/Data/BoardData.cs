using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "BoardData", menuName = "Data/BoardData")]
public class BoardData : ScriptableObject
{
    [Serializable]
    public class BDSearchingWord
    { 
        public string Word;
        public bool isFound;
    }

    [Serializable]
    public class BoardRow
    {
        public int Size;
        public string[] Row;

        public BoardRow() { }
        public BoardRow(int size)
        {
            CreateRow(size);
        }

        public void CreateRow(int size)
        { 
            Size = size;
            Row = new string[Size];
            ClearRow();
        }

        public void ClearRow()
        {
            for (int i = 0; i < Size; i++)
            {
                Row[i] = string.Empty;
            }
        }
    }

    public float TimeInSeconds;
    public int Columns = 0;
    public int Rows = 0;
    public bool UseDotsMode = false;
    public bool UsePrompts = false;
    public float TimeToPrompt = 0;
    public bool IsHard;
    public bool HasExtraReward;

    public BoardRow[] Board;
    public List<BDSearchingWord> SearchingWords = new List<BDSearchingWord>();

    public void ClearWithEmptyString()
    {
        for (int i = 0; i < Columns; i++)
        {
            Board[i].ClearRow();
        }
    }

    public void CreateNewBoard()
    { 
        Board = new BoardRow[Columns];
        for (int i = 0; i < Columns; i++)
        {
            Board[i] = new BoardRow(Rows);
        }
    }
}
