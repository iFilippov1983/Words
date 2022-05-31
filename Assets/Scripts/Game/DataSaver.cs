using Game;
using System.Collections.Generic;
using UnityEngine;

public class DataSaver
{
    private static List<string> _listNames = new List<string>();
    private static List<string> _stringList;
    private static int _countIndex = -1;

    public static int LoadIntData(string dataName)
    {
        int value = 0;// -1?
        if (PlayerPrefs.HasKey(dataName))
        { 
            value = PlayerPrefs.GetInt(dataName);
        }

        return value;
    }

    public static float LoadFloatData(string dataName)
    { 
        float value = 0f;
        if (PlayerPrefs.HasKey(dataName))
        { 
            value = PlayerPrefs.GetFloat(dataName);
        }

        return value;
    }

    public static void SaveIntData(string dataName, int dataIntValue)
    {
        PlayerPrefs.SetInt(dataName, dataIntValue);
        PlayerPrefs.Save();
    }

    public static void SaveFloatData(string dataName, float dataValue)
    {
        PlayerPrefs.SetFloat(dataName, dataValue);
        PlayerPrefs.Save();
    }

    public static List<string> LoadSavedStringList(string listName)
    { 
        _stringList = new List<string>();
        if (PlayerPrefs.HasKey(listName))
        { 
            _countIndex = PlayerPrefs.GetInt(listName);
            for (int index = 0; index < _countIndex; index++)
            {
                string key = listName + index.ToString();
                _stringList.Add(PlayerPrefs.GetString(key));
            }
        }

        return _stringList;
    }

    public static void SaveStringDataFromList(string listName, List<string> listToSave)
    {
        _listNames.Add(listName);
        _countIndex = listToSave.Count;
        PlayerPrefs.SetInt(listName, _countIndex);

        for (int index = 0; index < listToSave.Count; index++)
        {
            string key = listName + index.ToString();
            PlayerPrefs.SetString(key, listToSave[index]);
        }

        //Debug.Log("Data saver count index: " + _countIndex);

        PlayerPrefs.Save();
    }

    public static void ClearGameData(GameLevelData levelData)
    {
        PlayerPrefs.DeleteAll();
        _listNames.Clear();

        //Ulock first level
        PlayerPrefs.SetInt(levelData.Data[0].CategoryName, 0);
        
        PlayerPrefs.SetInt(CurrencyManager.coinsKey, 0);
       
        PlayerPrefs.Save();
    }

    public static void ClearSavedStringListData(string listName)
    {
        if (PlayerPrefs.HasKey(listName))
        {
            _countIndex = PlayerPrefs.GetInt(listName);
            for (int index = 0; index < _countIndex; index++)
            {
                string key = listName + index.ToString();
                PlayerPrefs.DeleteKey(key);
            }

            PlayerPrefs.DeleteKey(listName);
        }
        _countIndex = -1;
        _stringList.Clear();

        PlayerPrefs.Save();
    }
}
