using Lofelt.NiceVibrations;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{
    public delegate void EnableSquareSelection();
    public static event EnableSquareSelection OnEnableSquareSelection;

    public static void EnableSquareSelectionMethod()
    { 
        OnEnableSquareSelection?.Invoke();
    }
    //************

    public delegate void DisableSquareSelection();
    public static event DisableSquareSelection OnDisableSquareSelection;

    public static void DisableSquareSelectionMethod()
    {
        OnDisableSquareSelection?.Invoke();
    }
    //************

    public delegate void SelectSquare(Vector3 position);
    public static event SelectSquare OnSelectSquare;

    public static void SelectSquareMethod(Vector3 position)
    {
        OnSelectSquare?.Invoke(position);
    }
    //************

    public delegate void CheckSquare(string letter, Vector3 squarePosition, int squareIndex);
    public static event CheckSquare OnCheckSquare;

    public static void CheckSquareMethod(string letter, Vector3 squarePosition, int squareIndex)
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        Debug.Log("[Haptic] GameEvents - CheckSquareMethod");

        OnCheckSquare?.Invoke(letter, squarePosition, squareIndex);
    }
    //************

    public delegate void ClearSelection();
    public static event ClearSelection OnClearSelection;

    public static void ClearSelectionMethod()
    {
        OnClearSelection?.Invoke();
    }
    //************

    public delegate void CorrectWord(string word, List<int> squareIndexes);
    public static event CorrectWord OnCorrectWord;

    public static void CorrectWordMethod(string word, List<int> squareIndexes)
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);
        Debug.Log("[Haptic] GameEvents - CorrectWordMethod");

        OnCorrectWord?.Invoke(word, squareIndexes);
    }
    //************

    public delegate void CorrectExtraWord(List<int> squareIndexes);
    public static event CorrectExtraWord OnCorrectExtraWord;

    public static void OnCorrectExtraWordMethod(List<int> squareIndexes)
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);
        Debug.Log("[Haptic] GameEvents - OnCorrectExtraWordMethod");

        OnCorrectExtraWord?.Invoke(squareIndexes);
    }
    //************

    public delegate void BoardComleted(bool categoryCompleted);
    public static event BoardComleted OnBoardComleted;

    public static void BoardCompletedMethod(bool categoryCompleted)
    {
        OnBoardComleted?.Invoke(categoryCompleted);
    }
    //************

    public delegate void UnlockNextCategory();
    public static event UnlockNextCategory OnUnlockNextCategory;

    public static void UnlockNextCategoryMethod()
    {
        OnUnlockNextCategory?.Invoke();
    }
    //************

    public delegate void LoadLevel();
    public static event LoadLevel OnLoadLevel;

    public static void LoadNextLevelMethod()
    { 
        OnLoadLevel?.Invoke();
    }
    //************

    public delegate void GameOver();
    public static event GameOver OnGameOver;

    public static void GameOverMethod()
    {
        OnGameOver?.Invoke();
    }
    //************

    public delegate void WordGetTarget(string word);
    public static event WordGetTarget OnWordGetTarget;

    public static void WordGetTargetMethod(string word)
    { 
        OnWordGetTarget?.Invoke(word);
    }
    //************

    public delegate void MenuIsActive(bool menuIsActive);
    public static event MenuIsActive OnMenuIsActive;

    public static void MenuIsActiveMethod(bool menuIsActive)
    { 
        OnMenuIsActive?.Invoke(menuIsActive);
    }
    //************
}
