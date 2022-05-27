using Game.WordComparison;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Prompter : MonoBehaviour
{
    [SerializeField] private GameData _currentGameData;
    [SerializeField] private SearchingWordsList _searchingWordsParent;

    private string _word;
    private bool _usePrompts;

    private Ray _rayUp, _rayDown;
    private Ray _rayLeft, _rayRight;
    private Ray _rayDiagLeftUp, _rayDiagLeftDown;
    private Ray _rayDiagRightUp, _rayDiagRightDown;
    private Ray _currenRay = new Ray();
    private Vector3 _rayOriginPosition;

    private List<int> _checkedSquaresIndexes = new List<int>();
    private List<Ray> _raysList = new List<Ray>();
    private List<GridSquare> _squaresList = new List<GridSquare>();
    private List<GridSquare> _unvisibleSquares = new List<GridSquare>();
    private List<GridSquare> _visibleSquares = new List<GridSquare>();
    private List<SearchingWord> _searchingWords = new List<SearchingWord>();

    private async void Start()
    {
        _usePrompts = _currentGameData.selectedBoardData.UsePrompts;
        _searchingWords = UpdateSearchingWordsList(_searchingWordsParent);
        _squaresList = await MakeAllSquaresList();

        SortSquaresLists(_word, _checkedSquaresIndexes);

        GameEvents.OnCorrectWord += SortSquaresLists;
        GameEvents.OnTimeToPrompt += MakePrompt;
    }

    private void OnDestroy()
    {
        GameEvents.OnCorrectWord -= SortSquaresLists;
        GameEvents.OnTimeToPrompt += MakePrompt;
    }

    private async Task<List<GridSquare>> MakeAllSquaresList()
    {
        var list = new List<GridSquare>();
        var squaresArray = FindObjectsOfType<GridSquare>();
        int squaresAmount = _currentGameData.selectedBoardData.Columns * _currentGameData.selectedBoardData.Rows;
        while (squaresArray.Length < squaresAmount)
        {
            squaresArray = FindObjectsOfType<GridSquare>();
            await Task.Yield();
        }

        for (int i = 0; i < squaresArray.Length; i++)
            list.Add(squaresArray[i]);

        return list;
    }

    private async void SortSquaresLists(string word, List<int> squareIndexes)
    {
        await Task.Delay(3000);

        _visibleSquares.Clear();
        _unvisibleSquares.Clear();
        foreach (var square in _squaresList)
        {
            if (square == null) continue;
            if (square.NotVisible) _unvisibleSquares.Add(square);
            else _visibleSquares.Add(square);
        }

        _searchingWords = UpdateSearchingWordsList(_searchingWordsParent);
        //Debug.Log($"Visible list count: {_visibleSquares.Count}, Unvisible: {_unvisibleSquares.Count}");
    }

    [Button]
    private void MakePrompt()
    {
        if (_usePrompts == false)
            return;

        foreach (var word in _searchingWords)
        {
            bool wordFound = TryFindWord(word);
            if (wordFound)
            {
                GameEvents.WordToPromptFoundMethod(_checkedSquaresIndexes);
                ClearData();
                return;
            }
        }

        ClearData();
    }

    private bool TryFindWord(SearchingWord searchingWord)
    {
        foreach (var visibleSquare in _visibleSquares)
        {
            bool found = SearchStartingFrom(visibleSquare, searchingWord.Word);
            if(found) return true;
        }

        return false;
    }

    private bool SearchStartingFrom(GridSquare square, string word)
    {
        ClearData();

        _word += square.Letter;
        _checkedSquaresIndexes.Add(square.Index);
        _raysList = SetRaysFrom(square.BodyPosition);
        foreach (var ray in _raysList)
        {
            CheckAllSequencesToFind(word, ray);
        }

        return false;
    }

    private void CheckAllSequencesToFind(string word, Ray ray)
    {
        var hits = Physics.RaycastAll(ray, 2f, LayerMask.NameToLayer(Literal.LayerMask_SquareBody));
        if (hits.Length != 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                var square = hits[i].collider.GetComponentInParent<GridSquare>();
                if (_checkedSquaresIndexes.Contains(square.Index))
                    continue;

            }
            
            
        }
        else return;
    }

    private List<Ray> SetRaysFrom(Vector3 origin)
    {
        var raysList = new List<Ray>();

        _rayUp = new Ray(new Vector2(origin.x, origin.y), new Vector2(0f, 1f));
        raysList.Add(_rayUp);

        _rayDown = new Ray(new Vector2(origin.x, origin.y), new Vector2(0f, -1f));
        raysList.Add(_rayDown);

        _rayLeft = new Ray(new Vector2(origin.x, origin.y), new Vector2(-1f, 0f));
        raysList.Add(_rayLeft);

        _rayRight = new Ray(new Vector2(origin.x, origin.y), new Vector2(1f, 0f));
        raysList.Add(_rayRight);

        _rayDiagLeftUp = new Ray(new Vector2(origin.x, origin.y), new Vector2(-1f, 1f));
        raysList.Add(_rayDiagLeftUp);

        _rayDiagLeftDown = new Ray(new Vector2(origin.x, origin.y), new Vector2(-1f, -1f));
        raysList.Add(_rayDiagLeftDown);

        _rayDiagRightUp = new Ray(new Vector2(origin.x, origin.y), new Vector2(1f, 1f));
        raysList.Add(_rayDiagRightUp);

        _rayDiagRightDown = new Ray(new Vector2(origin.x, origin.y), new Vector2(1f, -1f));
        raysList.Add(_rayDiagRightDown);

        return raysList;
    }

    private void ClearData()
    {
        _word = string.Empty;
        _checkedSquaresIndexes.Clear();
        _raysList.Clear();
    }

    private List<SearchingWord> UpdateSearchingWordsList(SearchingWordsList parent)
    { 
        var list = new List<SearchingWord>();
        foreach (var word in parent.SearchingWords)
        { 
            if(word.isFound == false)
                list.Add(word);
        }

        return list;
    }

    #region Legacy
    //private void SquareSelected(string letter, Vector3 squarePosition, int squareIndex)
    //{
    //    if (_assignedPoints == 0)
    //    {
    //        _rayStartPosition = squarePosition;
    //        _correcSquaresIndexes.Add(squareIndex);
    //        _word += letter;


    //    }
    //    else if (_assignedPoints == 1)
    //    {
    //        _correcSquaresIndexes.Add(squareIndex);
    //        GameEvents.SelectSquareMethod(squarePosition);
    //        _word += letter;
    //        CheckWord();
    //    }
    //    else
    //    {

    //        _correcSquaresIndexes.Add(squareIndex);
    //        GameEvents.SelectSquareMethod(squarePosition);
    //        _word += letter;
    //        CheckWord();

    //    }

    //    _assignedPoints++;
    //}

    //private void SetRays(Vector3 squarePosition)
    //{
    //    _raysList.Clear();

    //    _rayUp = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(0f, 1f));
    //    _raysList.Add(_rayUp);

    //    _rayDown = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(0f, -1f));
    //    _raysList.Add(_rayDown);

    //    _rayLeft = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(-1f, 0f));
    //    _raysList.Add(_rayLeft);

    //    _rayRight = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(1f, 0f));
    //    _raysList.Add(_rayRight);

    //    _rayDiagLeftUp = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(-1f, 1f));
    //    _raysList.Add(_rayDiagLeftUp);

    //    _rayDiagLeftDown = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(-1f, -1f));
    //    _raysList.Add(_rayDiagLeftDown);

    //    _rayDiagRightUp = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(1f, 1f));
    //    _raysList.Add(_rayDiagRightUp);

    //    _rayDiagRightDown = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(1f, -1f));
    //    _raysList.Add(_rayDiagRightDown);
    //}
    #endregion
}
