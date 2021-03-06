using Game.WordComparison;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Prompter : MonoBehaviour
{
    [SerializeField] private GameData _currentGameData;
    [SerializeField] private SearchingWordsList _searchingWordsParent;
    [SerializeField] private List<TextAsset> _dictionaries;
    [SerializeField] private DataProfile _dataProfile;
    private WordFinder _wordFinder;

    private static bool _usePrompts;
    private bool _useDotsMod;

    private string _word;
    private int _counter = 0;
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

    private static Func<Task> OnManualPrompt;

    private async void Start()
    {
        _usePrompts = _currentGameData.selectedBoardData.UsePrompts;
        _useDotsMod = _currentGameData.selectedBoardData.UseDotsMode;
        _searchingWords = UpdateSearchingWordsList(_searchingWordsParent);
        _wordFinder = new WordFinder(_dictionaries, ' ');
        _squaresList = await MakeAllSquaresList();

        OnManualPrompt += MakePrompt;
        GameEvents.OnTimeToPrompt += MakePrompt;
        GameEvents.OnBoardConfigurationChanged += SortSquaresLists;
        BoardResetManager.OnBoardIsReset += ResetLists;
    }

    private void OnDestroy()
    {
        CancelInvoke();
        OnManualPrompt -= MakePrompt;
        GameEvents.OnTimeToPrompt -= MakePrompt;
        GameEvents.OnBoardConfigurationChanged += SortSquaresLists;
        BoardResetManager.OnBoardIsReset -= ResetLists;
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

    private void SortSquaresLists()
    {
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

    private async void ResetLists()
    {
        _squaresList = await MakeAllSquaresList();
        SortSquaresLists();
    }

    [Button]
    private async Task MakePrompt()
    {
        if (_usePrompts == false)
            return;

        bool wordFound = false;
        foreach (var word in _searchingWords)
        {
            if (word.isFound == false)
            {
                Debug.Log($"TryFindWord: {word.Word}");

                wordFound = await TryFindWord(word);
                if (wordFound)
                {
                    GameEvents.WordToPromptFoundMethod(_checkedSquaresIndexes);
                    ClearData();
                    return;
                }
            }
        }

        if (_useDotsMod)
        {
            foreach (var word in _searchingWords)
            {
                if (word.isFound == false)
                {
                    Debug.Log($"TryFindAdditionalWord: {word.Word}");

                    wordFound = await TryFindAdditionalWord(word.Word);
                    if (wordFound)
                    {
                        GameEvents.WordToPromptFoundMethod(_checkedSquaresIndexes);
                        ClearData();
                        return;
                    }
                }
            }
        }

        if (!wordFound)
            GameEvents.NoWordsAvailableMethod();

        ClearData();
    }

    private async Task<bool> TryFindWord(SearchingWord searchingWord)
    {
        bool found = false;
        foreach (var visibleSquare in _visibleSquares)
        {
            _counter = 0;
            if (visibleSquare != null)
            {
                found = await SearchStartingFrom(visibleSquare, searchingWord.Word);
                if (found) return true;
            }
        }
        return found;
    }

    private async Task<bool> TryFindAdditionalWord(string aWord)
    {
        bool found = false;
        var wordsToTry = _wordFinder.GetWordsListWhithLength(aWord.Length);

        foreach (var visibleSquare in _visibleSquares)
        {
            _counter = 0;
            if (visibleSquare != null)
            {
                foreach (var word in wordsToTry)
                {
                    found = await SearchStartingFrom(visibleSquare, word.ToUpper());
                    if (found) return true;
                }
            }
        }
        return found;
    }

    private async Task<bool> SearchStartingFrom(GridSquare square, string word)
    {
        if (word.StartsWith(square.Letter) == false)
        {
            return false;
        }

        ClearData();

        _word += square.Letter;
        _checkedSquaresIndexes.Add(square.Index);
         var raysList = SetRaysFrom(square.BodyPosition);
        foreach (var ray in raysList)
        {
            bool found = await RecursivelyFind(word, ray);
            if (found) return true;

            _word = string.Empty;
            _word += square.Letter;
            _checkedSquaresIndexes.Clear();
            _checkedSquaresIndexes.Add(square.Index);
        }

        return false;
    }

    private async Task<bool> RecursivelyFind(string word, Ray rayToCheck)
    {
        _counter++;
        if (_counter >= 10000)
        {
            Debug.Log($"Yeld - counter: {_counter}");
            await Task.Yield();
            _counter = 0;
        }
            

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
                        Debug.Log($"Found: {_word}");
                        return true;
                    }
                    if (word.StartsWith(_word))
                    {
                        //Debug.Log($"Word starts with: {_word}");
                        var raysList = SetRaysFrom(square.BodyPosition);
                        foreach (var ray in raysList)
                        {
                            bool found = await RecursivelyFind(word, ray);
                            if (found) return true;
                        }
                    }

                    _word = _word.Remove(_word.Length - 1);
                    _checkedSquaresIndexes.Remove(square.Index);
                    return false;
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

    public static async Task MakeManualPrompt()
    {
        bool promptsModeOn = _usePrompts;
        _usePrompts = true;
        await OnManualPrompt?.Invoke();
        _usePrompts = promptsModeOn == true
            ? true : false;
    }
}
