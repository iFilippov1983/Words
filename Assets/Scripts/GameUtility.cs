using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUtility : MonoBehaviour
{
    public GameLevelData levelData;

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    [Button]
    private void ResetProgress()
    {
        DataSaver.ClearGameData(levelData);
    }

}
