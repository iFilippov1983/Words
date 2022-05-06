using System.Collections;
using System.Collections.Generic;
using Game.WordComparison;
using UnityEngine;

public class WordCheker : MonoBehaviour
{
    public GameData _currentGameData;

    private string _word;
    private int _assignedPoints = 0;
    private int _completedWords = 0;
    private Ray _rayUp, _rayDown;
    private Ray _rayLeft, _rayRight;
    private Ray _rayDiagLeftUp, _rayDiagLeftDown;
    private Ray _rayDiagRightUp, _rayDiagRightDown;
    private Ray _currenRay = new Ray();
    private Vector3 _rayStartPosition;
    private List<int> _correcSquareList = new List<int>();

    [SerializeField] private List<TextAsset> dictionaries;
    private WordFinder _wordFinder;

    private void OnEnable()
    {
        GameEvents.OnCheckSquare += SquareSelected;
        GameEvents.OnClearSelection += ClearSelection;
    }

    private void OnDisable()
    {
        GameEvents.OnCheckSquare -= SquareSelected;
        GameEvents.OnClearSelection -= ClearSelection;
    }

    private void Start()
    {
        _assignedPoints = 0;
        _completedWords = 0;

        _wordFinder = new WordFinder(dictionaries, ' ');
    }

    private void Update()
    {
        if (_assignedPoints > 0 && Application.isEditor)
        {
            Debug.DrawRay(_rayUp.origin, _rayUp.direction * 4);
            Debug.DrawRay(_rayDown.origin, _rayDown.direction * 4);
            Debug.DrawRay(_rayLeft.origin, _rayLeft.direction * 4);
            Debug.DrawRay(_rayRight.origin, _rayRight.direction * 4);
            Debug.DrawRay(_rayDiagLeftUp.origin, _rayDiagLeftUp.direction * 4);
            Debug.DrawRay(_rayDiagLeftDown.origin, _rayDiagLeftDown.direction * 4);
            Debug.DrawRay(_rayDiagRightUp.origin, _rayDiagRightUp.direction * 4);
            Debug.DrawRay(_rayDiagRightDown.origin, _rayDiagRightDown.direction * 4);
        }
    }

    private void SquareSelected(string letter, Vector3 squarePosition, int squareIndex)
    {
        if (_assignedPoints == 0)
        {
            _rayStartPosition = squarePosition;
            _correcSquareList.Add(squareIndex);
            _word += letter;

            _rayUp = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(0f, 1f));
            _rayDown = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(0f, -1f));
            _rayLeft = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(-1f, 0f));
            _rayRight = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(1f, 0f));
            _rayDiagLeftUp = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(-1f, 1f));
            _rayDiagLeftDown = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(-1f, -1f));
            _rayDiagRightUp = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(1f, 1f));
            _rayDiagRightDown = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(1f, -1f));
        }
        else
        {
            _correcSquareList.Add(squareIndex);
            _currenRay = SelectRay(_rayStartPosition, squarePosition);
            GameEvents.SelectSquareMethod(squarePosition);
            _word += letter;
            CheckWord();
        }
        //else if (_assignedPoints == 1)
        //{
        //    _correcSquareList.Add(squareIndex);
        //    _currenRay = SelectRay(_rayStartPosition, squarePosition);
        //    GameEvents.SelectSquareMethod(squarePosition);
        //    _word += letter;
        //    CheckWord();
        //}
        //else
        //{
        //    if (IsPoinOnTheRay(_currenRay, squarePosition))
        //    {
        //        _correcSquareList.Add(squareIndex);
        //        GameEvents.SelectSquareMethod(squarePosition);
        //        _word += letter;
        //        CheckWord();
        //    }
        //}

        _assignedPoints++;
    }

    private void CheckWord()
    {
        var isWordFound = _wordFinder.FindWord(_word);
        Debug.Log(isWordFound ? $"{_word} found" : $"{_word} not found");

        foreach (var searchingWord in _currentGameData.selectedBoardData.SearchingWords)
        {
            if (_word.Equals(searchingWord))
            {
                _word = string.Empty;
                return;
                //TODO implement success method
            }
        }
    }

    private bool IsPoinOnTheRay(Ray currentRay, Vector3 point)
    {
        var hits = Physics.RaycastAll(currentRay, 100.0f);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.position == point)
                return true;
        }

        return false;
    }

    private Ray SelectRay(Vector2 firstPosition, Vector2 secondPosition)
    {
        var direction = (secondPosition - firstPosition).normalized;
        float tolerance = 0.01f;

        if ((Mathf.Abs(direction.x) < tolerance) && (Mathf.Abs(direction.y) - 1f < tolerance))
        {
            return _rayUp;
        }

        if ((Mathf.Abs(direction.x) < tolerance) && (Mathf.Abs(direction.y) - (-1f) < tolerance))
        {
            return _rayDown;
        }

        if ((Mathf.Abs(direction.x) - (-1f) < tolerance) && (Mathf.Abs(direction.y) < tolerance))
        {
            return _rayLeft;
        }

        if ((Mathf.Abs(direction.x) - 1f < tolerance) && (Mathf.Abs(direction.y) < tolerance))
        {
            return _rayRight;
        }

        if (direction.x < 0f && direction.y > 0f)
        {
            return _rayDiagLeftUp;
        }

        if (direction.x < 0f && direction.y < 0f)
        {
            return _rayDiagLeftDown;
        }

        if (direction.x > 0f && direction.y > 0f)
        {
            return _rayDiagRightUp;
        }

        if (direction.x > 0f && direction.y < 0f)
        {
            return _rayDiagRightDown;
        }

        return _rayDown;
    }

    private void ClearSelection()
    {
        _assignedPoints = 0;
        _correcSquareList.Clear();
        _word = string.Empty;
    }
}
