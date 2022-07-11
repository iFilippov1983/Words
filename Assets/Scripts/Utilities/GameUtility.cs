using Game;
using Lofelt.NiceVibrations;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUtility : MonoBehaviour
{
    [SerializeField] private GameLevelData _gameLevelData;
    [SerializeField] private GameModeHandler _gameModeHandler;
    [SerializeField] private DataProfile _dataProfile;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    public static void LoadScene(string sceneName)
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        Debug.Log("[Haptic] GameUtility - LoadScene");

        SceneManager.LoadScene(sceneName);
    }

    public void SetOverlayMenuActivityStatus(bool isActive)
    {
        GameEvents.MenuIsActiveMethod(isActive);
    }

    public void CloseApplication()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        Debug.Log("[Haptic] GameUtility - CloseApplication");

        Application.Quit();

#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#endif
    }

    [Button]
    private void ResetProgress()
    {
        DataSaver.ClearGameData();
        //Ulock first level
        DataSaver.SaveIntData(DataKey.ProgressKey, 0);
        DataSaver.SaveIntData(DataKey.CoinsKey, 0);
        
        _gameModeHandler.SetGameMode(GameModeType.WordsMode);

        _dataProfile.UsedWords.Clear();
        _dataProfile.isUpdated = false;
    }
    
    [Button]
    [HideInEditorMode]
    private void ShowPrompt()
    {
        GameEvents.WordToPromptFoundMethod(new List<int>());
    }

    [Button]
    [HideInPlayMode]
    private void SetLevel(int number, GameModeType gameMode = GameModeType.WordsMode)
    {
        int gameCyclesCount = 0;
        int index = number - 1;
        if (index >= _gameLevelData.Data[0].BoardData.Count)
        {
            index -= _gameLevelData.Data[0].BoardData.Count;
            gameCyclesCount++;

            while (index > _gameLevelData.Data[0].BoardData.Count - _dataProfile.LevelNumberToCycleFrom)
            {
                index -= (_gameLevelData.Data[0].BoardData.Count - (_dataProfile.LevelNumberToCycleFrom - 1));
                gameCyclesCount++;
                Debug.Log($"1 - Number: {number}, Index: {index}, Cycles: {gameCyclesCount}");
            }
            index += _dataProfile.LevelNumberToCycleFrom - 1;
        }

        Debug.Log($"2 - Number: {number}, Index: {index}, Cycles: {gameCyclesCount}");

        if (index >= 0)
        {
            DataSaver.SaveIntData(DataKey.CyclesCountKey, gameCyclesCount);
            DataSaver.SaveIntData(DataKey.ProgressKey, index);
            _gameModeHandler.SetGameMode(gameMode);
            _dataProfile.CurrentLevelNumber = number;
        }
        else
            Debug.LogError("Level number can't be less than 1");
    }

    
    // Haptic types
    //
    // Selection : a light vibration on Android, and a light impact on iOS
    // Success : a light then heavy vibration on Android, and a success impact on iOS
    // Warning : a heavy then medium vibration on Android, and a warning impact on iOS
    // Failure : a medium / heavy / heavy / light vibration pattern on Android, and a failure impact on iOS
    // Light : a light impact on iOS and a short and light vibration on Android.
    // Medium : a medium impact on iOS and a medium and regular vibration on Android
    // Heavy : a heavy impact on iOS and a long and heavy vibration on Android
    // Rigid : a short and hard impact
    // Soft : a slightly longer and softer impact
}
