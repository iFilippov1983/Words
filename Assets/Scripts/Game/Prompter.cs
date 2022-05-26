using Game.WordComparison;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Prompter : MonoBehaviour
{
    [SerializeField] private GameData _currentGameData;

    private string _word;
    private int _assignedPoints = 0;
    private bool _usePrompts;

    private Ray _rayUp, _rayDown;
    private Ray _rayLeft, _rayRight;
    private Ray _rayDiagLeftUp, _rayDiagLeftDown;
    private Ray _rayDiagRightUp, _rayDiagRightDown;
    private Ray _currenRay = new Ray();
    private Vector3 _rayStartPosition;
    private List<int> _correcSquaresIndexes = new List<int>();
    private List<int> _checkedSquaresIndexes = new List<int>();
    private List<Ray> _raysList = new List<Ray>();
    private List<GridSquare> _squaresList = new List<GridSquare>();

    private void Start()
    {
        _assignedPoints = 0;
        _usePrompts = _currentGameData.selectedBoardData.UsePrompts;

        MakeSquaresList();
    }

    private void Update()
    {

    }

    private async void MakeSquaresList()
    {
        var squaresArray = FindObjectsOfType<GridSquare>();
        int squaresAmount = _currentGameData.selectedBoardData.Columns * _currentGameData.selectedBoardData.Rows;
        while (squaresArray.Length < squaresAmount)
        {
            squaresArray = FindObjectsOfType<GridSquare>();
            await Task.Yield();
        }

        for (int i = 0; i < squaresArray.Length; i++)
        {
            if(squaresArray[i].NotVisible == false)
                _squaresList.Add(squaresArray[i]);
        }

        Debug.Log($"Squares amont: {squaresAmount}, List count: {_squaresList.Count}");
    }

    private void SquareSelected(string letter, Vector3 squarePosition, int squareIndex)
    {
        if (_assignedPoints == 0)
        {
            _rayStartPosition = squarePosition;
            _correcSquaresIndexes.Add(squareIndex);
            _word += letter;

            
        }
        else if (_assignedPoints == 1)
        {
            _correcSquaresIndexes.Add(squareIndex);
            GameEvents.SelectSquareMethod(squarePosition);
            _word += letter;
            CheckWord();
        }
        else
        {

            _correcSquaresIndexes.Add(squareIndex);
            GameEvents.SelectSquareMethod(squarePosition);
            _word += letter;
            CheckWord();

        }

        _assignedPoints++;
    }

    private void CheckWord()
    {
        foreach (var searchingWord in _currentGameData.selectedBoardData.SearchingWords)
        {
            if (_word.Equals(searchingWord.Word))
            {
                GameEvents.WordToPromptFoundMethod(_correcSquaresIndexes);
                _word = string.Empty;
                _correcSquaresIndexes.Clear();
            }
        }
    }

    private void SetRays(Vector3 squarePosition)
    {
        _raysList.Clear();

        _rayUp = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(0f, 1f));
        _raysList.Add(_rayUp);

        _rayDown = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(0f, -1f));
        _raysList.Add(_rayDown);

        _rayLeft = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(-1f, 0f));
        _raysList.Add(_rayLeft);

        _rayRight = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(1f, 0f));
        _raysList.Add(_rayRight);

        _rayDiagLeftUp = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(-1f, 1f));
        _raysList.Add(_rayDiagLeftUp);

        _rayDiagLeftDown = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(-1f, -1f));
        _raysList.Add(_rayDiagLeftDown);

        _rayDiagRightUp = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(1f, 1f));
        _raysList.Add(_rayDiagRightUp);

        _rayDiagRightDown = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(1f, -1f));
        _raysList.Add(_rayDiagRightDown);
    }

    private void ClearSelection()
    {
        _assignedPoints = 0;
        _correcSquaresIndexes.Clear();
        _word = string.Empty;
    }
}
