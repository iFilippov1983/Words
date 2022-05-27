using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Prompter : MonoBehaviour
{
    [SerializeField] private GameData _currentGameData;
    [SerializeField] private SearchingWordsList _searchingWordsParent;

    private string _word;
    private bool _usePrompts;
    private float _rayLength = 1.5f;

    private Ray _rayUp, _rayDown;
    private Ray _rayLeft, _rayRight;
    private Ray _rayDiagLeftUp, _rayDiagLeftDown;
    private Ray _rayDiagRightUp, _rayDiagRightDown;

    private List<int> _checkedSquaresIndexes = new List<int>();
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

        if (!this.gameObject) return;

        _visibleSquares.Clear();
        _unvisibleSquares.Clear();
        foreach (var square in _squaresList)
        {
            if (square == null) continue;
            if (square.NotVisible) _unvisibleSquares.Add(square);
            else _visibleSquares.Add(square);
        }

        _searchingWords = UpdateSearchingWordsList(_searchingWordsParent);
    }

    [Button]
    private void MakePrompt()
    {
        if (_usePrompts == false)
            return;

        foreach (var word in _searchingWords)
        {
            //Debug.Log($"Trying to find: {word.Word}");
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
        if (word.StartsWith(square.Letter) == false)
        {
            return false;
        }

        //Debug.Log($"First letter: {square.Letter}");

        ClearData();

        _word += square.Letter;
        _checkedSquaresIndexes.Add(square.Index);
         var raysList = SetRaysFrom(square.BodyPosition);
        foreach (var ray in raysList)
        {
            bool found = RecursivelyFind(word, ray);
            if (found) return true;

            _word = string.Empty;
            _word += square.Letter;
            _checkedSquaresIndexes.Clear();
            _checkedSquaresIndexes.Add(square.Index);
        }

        return false;
    }

    private bool RecursivelyFind(string word, Ray rayToCheck)
    {
        var hasHit = Physics.Raycast(rayToCheck, out RaycastHit hit, _rayLength);
        if (hasHit)
        {
            var isSquareBody = hit.collider.gameObject.layer.Equals(LayerMask.NameToLayer(Literal.LayerMask_SquareBody));
            if (isSquareBody)
            {
                var square = hit.collider.gameObject.GetComponentInParent<GridSquare>();

                if (_checkedSquaresIndexes.Contains(square.Index) || square.NotVisible)
                {
                    return false;
                }
                else
                {
                    _checkedSquaresIndexes.Add(square.Index);
                    _word += square.Letter;

                    if (word.Equals(_word))
                    {
                        //Debug.Log($"Found: {_word}");
                        return true;
                    }
                    if (word.StartsWith(_word))
                    {
                        //Debug.Log($"Word starts with: {_word}");
                        var raysList = SetRaysFrom(square.BodyPosition);
                        foreach (var ray in raysList)
                        {
                            bool found = RecursivelyFind(word, ray);
                            if (found) return true;
                        }
                    }
                    else
                    {
                        _word = _word.Remove(_word.Length - 1);
                        _checkedSquaresIndexes.Remove(square.Index);
                        return false;
                    } 
                }
            }
            else return false;
        }
        
        return false;
    }

    private List<Ray> SetRaysFrom(Vector3 origin)
    {
        var raysList = new List<Ray>();

        _rayUp = new Ray(origin, new Vector2(0f, 1f));
        raysList.Add(_rayUp);

        _rayDown = new Ray(origin, new Vector2(0f, -1f));
        raysList.Add(_rayDown);

        _rayLeft = new Ray(origin, new Vector2(-1f, 0f));
        raysList.Add(_rayLeft);

        _rayRight = new Ray(origin, new Vector2(1f, 0f));
        raysList.Add(_rayRight);

        _rayDiagLeftUp = new Ray(origin, new Vector2(-1f, 1f));
        raysList.Add(_rayDiagLeftUp);

        _rayDiagLeftDown = new Ray(origin, new Vector2(-1f, -1f));
        raysList.Add(_rayDiagLeftDown);

        _rayDiagRightUp = new Ray(origin, new Vector2(1f, 1f));
        raysList.Add(_rayDiagRightUp);

        _rayDiagRightDown = new Ray(origin, new Vector2(1f, -1f));
        raysList.Add(_rayDiagRightDown);

        return raysList;
    }

    private void ClearData()
    {
        _word = string.Empty;
        _checkedSquaresIndexes.Clear();
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
}
