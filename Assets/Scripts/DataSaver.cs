using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSaver //: MonoBehaviour
{
    public static int ReadCategoryCurrentIndexValues(string name)
    {
        int value = -1;
        if (PlayerPrefs.HasKey(name))
        { 
            value = PlayerPrefs.GetInt(name);
        }

        return value;
    }

    public static void SaveCategoryData(string categoryName, int currentIndex)
    {
        PlayerPrefs.SetInt(categoryName, currentIndex);
        PlayerPrefs.Save();
    }

    public static void ClearGameData(GameLevelData levelData)
    {
        foreach (var data in levelData.Data)
        {
            PlayerPrefs.SetInt(data.CategoryName, -1);
        }

        //Ulock first level
        PlayerPrefs.SetInt(levelData.Data[0].CategoryName, 0);
        PlayerPrefs.Save();
    }
}
