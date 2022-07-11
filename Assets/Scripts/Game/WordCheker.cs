using System.Collections.Generic;
using System.Threading.Tasks;
using Game.WordComparison;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WordCheker : MonoBehaviour
{
    public GameData currentGameData;
    public GameLevelData gameLevelData;

    //private const string UsedWordsKey = "UsedWords";
    //private const string CyclesCountKey = "CyclesCount";
    private string _word;
    private string _extraWord;
    private int _assignedPoints = 0;
    private int _completedWords = 0;
    private int _gameCyclesCount = 0;
    private bool _currentLevelNotCompleted;
    private bool _dotsMode;
    private List<int> _correctSquareList = new List<int>();

    [SerializeField] private SearchingWordsList _searchingWordsList;
    [SerializeField] private DataProfile _dataProfile;
    [SerializeField] private List<TextAsset> _dictionaries;
    private List<SearchingWord> _searchingWords = new List<SearchingWord>();
    private WordFinder _wordFinder;

    private async void OnEnable()
    {
        GameEvents.OnCheckSquare += SquareSelected;
        GameEvents.OnClearSelection += ClearSelection;
        GameEvents.OnLoadLevel += LoadNextGameLevel;
        GameEvents.OnGameOver += OnGameOver;

        while (_searchingWordsList.ListDone == false)
            await Task.Yield();

        Init();
        TinySauce.OnGameStarted(_dataProfile.CurrentLevelNumber.ToString());
    }

    private void OnDisable()
    {
        GameEvents.OnCheckSquare -= SquareSelected;
        GameEvents.OnClearSelection -= ClearSelection;
        GameEvents.OnLoadLevel -= LoadNextGameLevel;
        GameEvents.OnGameOver -= OnGameOver;
        Cleanup();
    }

    private void Start()
    {
        _assignedPoints = 0;
        _completedWords = 0;
        _dotsMode = currentGameData.selectedBoardData.UseDotsMode;
        _wordFinder = new WordFinder(_dictionaries, ' ');
    }

    private void Init()
    {
        _currentLevelNotCompleted = true;
        _gameCyclesCount = DataSaver.LoadIntData(DataKey.CyclesCountKey); 
        _dataProfile.SetUsedExtraWordsList(DataSaver.LoadSavedStringList(DataKey.UsedWordsKey));

        int number = DataSaver.LoadIntData(DataKey.ProgressKey)
        + gameLevelData.Data[0].BoardData.Count * _gameCyclesCount
        - (_dataProfile.LevelNumberToCycleFrom - 1) * _gameCyclesCount + 1;

        _dataProfile.CurrentLevelNumber = number;
        _dataProfile.isUpdated = true;

        foreach (var sw in currentGameData.selectedBoardData.SearchingWords)
            sw.isFound = false;

        _searchingWords = _searchingWordsList.SearchingWords;
    }

    private void Cleanup()
    {
        if (_currentLevelNotCompleted)
        {
            if(_dotsMode)
                _dataProfile.UsedWords.Clear();

            DataSaver.SaveStringDataFromList(DataKey.UsedWordsKey, _dataProfile.UsedWords);
        }
        else
        {
            DataSaver.ClearSavedStringListData(DataKey.UsedWordsKey);
        }

        _dataProfile.UsedWords.Clear();

        TinySauce.OnGameFinished(!_currentLevelNotCompleted, 0f, _dataProfile.CurrentLevelNumber.ToString());
    }

    private void LoadNextGameLevel() => SceneManager.LoadScene(Literal.Scene_GameScene);

    private void OnGameOver() => _currentLevelNotCompleted = false;// Lets player reuse allready used extra words if he've lost

    private void SquareSelected(string letter, Vector3 squarePosition, int squareIndex)
    {
        if (_dataProfile.MousePositionIsFar)
            return;

        if (_assignedPoints == 0)
        {
            _correctSquareList.Add(squareIndex);
            _word += letter;
        }
        else if (_correctSquareList.Count >= 2 && _correctSquareList[_correctSquareList.Count - 2].Equals(squareIndex)) //If this is previous letter
        {
            GameEvents.UnselectSquareMethod(letter, squarePosition, _correctSquareList[_correctSquareList.Count - 1]); //Unselect last letter
            _correctSquareList.RemoveAt(_correctSquareList.Count - 1);
            _word = _word.Remove(_word.Length - 1, 1);
            _assignedPoints--;

            return;
        }
        else if (_correctSquareList.Contains(squareIndex) == false)
        {
            _correctSquareList.Add(squareIndex);
            GameEvents.SelectSquareMethod(squarePosition);
            _word += letter;
        }

        _assignedPoints++;
    }

    private void CheckWord()
    {
        foreach (var searchingWord in currentGameData.selectedBoardData.SearchingWords)
        {
            bool caseOne = 
                _word.Equals(searchingWord.Word) 
                && searchingWord.isFound == false;

            if (caseOne)
            {
                Debug.Log("Case 1");
                searchingWord.isFound = true;
                GameEvents.CorrectWordMethod(_word, _correctSquareList);
                _dataProfile.UsedWords.Add(_word);
                _completedWords++;
                CheckBoardCompleted();
                return;
            }
        }

        bool foundExtraWord = _wordFinder.FindWord(_word);
        bool alreadyUsedWord = _dataProfile.UsedWords.Contains(_word);

        foreach (var searchingWord in currentGameData.selectedBoardData.SearchingWords)
        {
            bool caseTwo =
                foundExtraWord
                && !alreadyUsedWord
                && _word.Length.Equals(searchingWord.Word.Length)
                && searchingWord.isFound == false
                && _dotsMode;

            if (caseTwo)
            {
                Debug.Log("Case 2");
                ModifySearchingWords(_searchingWords, searchingWord);
                searchingWord.isFound = true;
                GameEvents.CorrectWordMethod(_word, _correctSquareList);
                _dataProfile.UsedWords.Add(_word);
                _completedWords++;
                CheckBoardCompleted();
                return;
            }
        }

        if (foundExtraWord && !alreadyUsedWord)
        {
            Debug.Log("Case 3");
            GameEvents.OnCorrectExtraWordMethod(_correctSquareList);
            _dataProfile.UsedWords.Add(_word);

            _extraWord = _word;
        }
    }

    private void ClearSelection()
    {
        CheckWord();

        _assignedPoints = 0;
        _correctSquareList.Clear();

        _word = string.Empty;
        _extraWord = string.Empty;
    }

    private void CheckBoardCompleted()
    {
        bool loadNextCategory = false;

        if (currentGameData.selectedBoardData.SearchingWords.Count.Equals(_completedWords))
        {
            var currentBoardIndex = DataSaver.LoadIntData(DataKey.ProgressKey);
            int currentCategoryIndex = 0;
            _currentLevelNotCompleted = false;

            int currentCategorySize = gameLevelData.Data[currentCategoryIndex].BoardData.Count;
            if (currentBoardIndex < currentCategorySize)
                currentBoardIndex++;

            DataSaver.SaveIntData(DataKey.ProgressKey, currentBoardIndex);

            //Make game cycle
            if (currentBoardIndex >= currentCategorySize)
            {
                _gameCyclesCount++;
                currentCategoryIndex = 0;
                currentBoardIndex = _dataProfile.LevelNumberToCycleFrom - 1;
                loadNextCategory = false;

                DataSaver.SaveIntData(DataKey.ProgressKey, currentBoardIndex);
                DataSaver.SaveIntData(DataKey.CyclesCountKey, _gameCyclesCount);
                _dataProfile.isUpdated = false;
                GameEvents.BoardCompletedMethod(loadNextCategory);
            }
            else
            {
                _dataProfile.isUpdated = false;
                GameEvents.BoardCompletedMethod(loadNextCategory);
            }

            if (loadNextCategory)
            {
                GameEvents.UnlockNextCategoryMethod();
            }
        }
    }

    private void ModifySearchingWords(List<SearchingWord> wordsList, BoardData.BDSearchingWord bdSearchingWord)
    {
        foreach (SearchingWord word in wordsList)
        {
            if (word.Word.Equals(bdSearchingWord.Word))
            {
                word.SetWord(_word);
                word.isFound = true;
                bdSearchingWord.isFound = true;
            }
        }
    }

    [Button]
    private void CompleteLevel()
    {
        _completedWords = currentGameData.selectedBoardData.SearchingWords.Count;
        CheckBoardCompleted();
    }
}
