using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{
    public delegate void EnableSquareSelection();
    public static event EnableSquareSelection OnEnableSquareSelection;

    public static void EnableSquareSelectionMethod()
    { 
        //if(OnEnableScoreSelection != null)
        //    OnEnableScoreSelection();
        OnEnableSquareSelection?.Invoke();
    }

    public delegate void DisableSquareSelection();
    public static event DisableSquareSelection OnDisableSquareSelection;

    public static void DisableSquareSelectionMethod()
    {
        //if(OnDisableScoreSelection != null)
        //    OnDisableScoreSelection();
        OnDisableSquareSelection?.Invoke();
    }

    public delegate void SelectSquare(Vector3 position);
    public static event SelectSquare OnSelectSquare;

    public static void SelectSquareMethod(Vector3 position)
    {
        //if(OnSelectSquare != null)
        //    OnSelectSquare();
        OnSelectSquare?.Invoke(position);
    }

    public delegate void CheckSquare(string letter, Vector3 squarePosition, int squareIndex);
    public static event CheckSquare OnCheckSquare;

    public static void CheckSquareMethod(string letter, Vector3 squarePosition, int squareIndex)
    {
        //if(OnCheckSquare != null)
        //    OnCheckSquare();
        OnCheckSquare?.Invoke(letter, squarePosition, squareIndex);
    }

    public delegate void ClearSelection();
    public static event ClearSelection OnClearSelection;

    public static void ClearSelectionMethod()
    {
        //if(OnClearSelection != null)
        //    OnClearSelection();
        OnClearSelection?.Invoke();
    }
}
