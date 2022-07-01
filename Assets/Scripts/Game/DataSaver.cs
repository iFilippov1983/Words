using Game;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class DataSaver
{
    private static List<string> _listNames = new List<string>();
    private static List<string> _stringList;
    private static int _countIndex = -1;

    public static bool HasKey(string key) => PlayerPrefs.HasKey(key);

    public static int LoadIntData(string key)
    {
        int value = 0;
        if (PlayerPrefs.HasKey(key))
            value = PlayerPrefs.GetInt(key);

        return value;
    }

    public static float LoadFloatData(string key)
    { 
        float value = 0f;
        if (PlayerPrefs.HasKey(key))
            value = PlayerPrefs.GetFloat(key);

        return value;
    }

    public static void SaveIntData(string key, int dataIntValue)
    {
        PlayerPrefs.SetInt(key, dataIntValue);
        PlayerPrefs.Save();
    }

    public static void SaveFloatData(string key, float dataValue)
    {
        PlayerPrefs.SetFloat(key, dataValue);
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

        PlayerPrefs.Save();
    }

    public static void ClearGameData()
    {
        PlayerPrefs.DeleteAll();
        _listNames.Clear();
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

    public static void SaveDateTime(string key, DateTime value)
    {
        string convertedToString = value.ToString("u", CultureInfo.InvariantCulture);
        PlayerPrefs.SetString(key, convertedToString);
        PlayerPrefs.Save();
    }

    public static DateTime LoadDateTime(string key, DateTime defaultValue)
    {
        if (PlayerPrefs.HasKey(key))
        { 
            string stored = PlayerPrefs.GetString(key);
            DateTime result = DateTime.ParseExact(stored, "u", CultureInfo.InvariantCulture);
            return result;
        }
        else
            return defaultValue;
    }
}
