using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUtility : MonoBehaviour
{
    [SerializeField] private GameLevelData _levelData;
    [SerializeField] private DataProfile _dataProfile;

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void CloseApplication()
    { 
        Application.Quit();

#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#endif
    }

    [Button]
    private void ResetProgress()
    {
        DataSaver.ClearGameData(_levelData);
        _dataProfile.UsedExtraWords.Clear();
    }
}
