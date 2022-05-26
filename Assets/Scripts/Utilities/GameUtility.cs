using Lofelt.NiceVibrations;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUtility : MonoBehaviour
{
    [SerializeField] private GameLevelData _levelData;
    [SerializeField] private DataProfile _dataProfile;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    public void LoadScene(string sceneName)
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
        DataSaver.ClearGameData(_levelData);
        _dataProfile.UsedExtraWords.Clear();
    }
    
    [Button]
    private void ShowPrompt()
    {
        GameEvents.WordToPromptFoundMethod(new List<int>());
    }

    [Button]
    [HideInPlayMode]
    private void SetLevel(int number, string categoryName = "Easy")
    {
        var index = number - 1;
        if (index >= 0)
            DataSaver.SaveIntData(categoryName, number - 1);
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
