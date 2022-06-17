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

    private const string UsedWordsKey = "UsedWords";
    private const string CyclesCountKey = "CyclesCount";
    private string _word;
    private string _extraWord;
    private int _assignedPoints = 0;
    private int _completedWords = 0;
    private int _gameCyclesCount = 0;
    private bool _currentLevelNotCompleted;
    private bool _dotsMode;
    private List<int> _correctSquareList = new List<int>();

    [SerializeField] private int _levelNumberToCycleFrom = 10;
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

        //If there's NO category selection
        _gameCyclesCount = DataSaver.LoadIntData(CyclesCountKey); 
        //
        _dataProfile.SetUsedExtraWordsList(DataSaver.LoadSavedStringList(UsedWordsKey));

        //If there's category selection
        //_dataProfile.CurrenLevelNumber = DataSaver.LoadIntData(currentGameData.selectedCategoryName) + 1; 
        //

        //If there's NO category selection
        int number = DataSaver.LoadIntData(currentGameData.selectedCategoryName)
        + gameLevelData.Data[0].BoardData.Count * _gameCyclesCount
        - (_levelNumberToCycleFrom - 1) * _gameCyclesCount + 1;

        _dataProfile.CurrentLevelNumber = number;
        _dataProfile.isUpdated = true;
        //    

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

            DataSaver.SaveStringDataFromList(UsedWordsKey, _dataProfile.UsedWords);
        }
        else
        {
            DataSaver.ClearSavedStringListData(UsedWordsKey);
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
            //Save current level progress
            var categoryName = currentGameData.selectedCategoryName;
            var currentBoardIndex = DataSaver.LoadIntData(categoryName);
            int nextBoardIndex = -1;
            int currentCategoryIndex = 0;
            bool readNextCategoryName = false;
            _currentLevelNotCompleted = false;

            for (int index = 0; index < gameLevelData.Data.Count; index++)
            {
                if (readNextCategoryName)
                {
                    nextBoardIndex = DataSaver.LoadIntData(gameLevelData.Data[index].CategoryName);
                    readNextCategoryName = false;
                }

                if (gameLevelData.Data[index].CategoryName.Equals(categoryName))
                { 
                    readNextCategoryName = true;
                    currentCategoryIndex = index;
                }
            }

            int currentCategorySize = gameLevelData.Data[currentCategoryIndex].BoardData.Count;
            if (currentBoardIndex < currentCategorySize)
                currentBoardIndex++;

            DataSaver.SaveIntData(categoryName, currentBoardIndex);

            //Unlock next category
            if (currentBoardIndex >= currentCategorySize)
            {
                currentCategoryIndex++;
                if (currentCategoryIndex < gameLevelData.Data.Count) //If this is not the last category
                {
                    categoryName = gameLevelData.Data[currentCategoryIndex].CategoryName;
                    currentBoardIndex = 0;
                    loadNextCategory = true;

                    if (nextBoardIndex <= 0)
                    {
                        DataSaver.SaveIntData(categoryName, currentBoardIndex);
                    }

                    _dataProfile.isUpdated = false;
                    GameEvents.BoardCompletedMethod(loadNextCategory);
                }
                else //If there's NO category selection
                {
                    _gameCyclesCount++;
                    currentCategoryIndex = 0;
                    currentBoardIndex = _levelNumberToCycleFrom - 1;
                    loadNextCategory = false;
                    categoryName = gameLevelData.Data[currentCategoryIndex].CategoryName;

                    DataSaver.SaveIntData(categoryName, currentBoardIndex);
                    DataSaver.SaveIntData(CyclesCountKey, _gameCyclesCount);
                    _dataProfile.isUpdated = false;
                    GameEvents.BoardCompletedMethod(loadNextCategory);
                }
                //else //If there's category selection
                //{
                //    SceneManager.LoadScene(Literal.Scene_SelectCategory);
                //}
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

    #region Legacy
    ////Use this code if selection direction change limitation is needed
    //public GameData _currentGameData;

    //private string _word;
    //private int _assignedPoints = 0;
    //private int _completedWords = 0;
    ////private Ray _rayUp, _rayDown;
    ////private Ray _rayLeft, _rayRight;
    ////private Ray _rayDiagLeftUp, _rayDiagLeftDown;
    ////private Ray _rayDiagRightUp, _rayDiagRightDown;
    ////private Ray _currenRay = new Ray();
    ////private Vector3 _rayStartPosition;
    //private List<int> _correcSquareList = new List<int>();

    //[SerializeField] private DataProfile _dataProfile;
    //[SerializeField] private List<TextAsset> _dictionaries;
    //private WordFinder _wordFinder;

    //private void OnEnable()
    //{
    //    _dataProfile.UsedExtraWords.Clear();//TODO: delete after debug and save system implementation

    //    GameEvents.OnCheckSquare += SquareSelected;
    //    GameEvents.OnClearSelection += ClearSelection;
    //}

    //private void OnDisable()
    //{
    //    GameEvents.OnCheckSquare -= SquareSelected;
    //    GameEvents.OnClearSelection -= ClearSelection;
    //}

    //private void Start()
    //{
    //    _assignedPoints = 0;
    //    _completedWords = 0;

    //    _wordFinder = new WordFinder(_dictionaries, ' ');
    //}

    //private void Update()
    //{
    //    ////Use this code if selection direction change limitation is needed (in Editor visualization)
    //    //if (_assignedPoints > 0 && Application.isEditor)
    //    //{
    //    //    Debug.DrawRay(_rayUp.origin, _rayUp.direction * 4);
    //    //    Debug.DrawRay(_rayDown.origin, _rayDown.direction * 4);
    //    //    Debug.DrawRay(_rayLeft.origin, _rayLeft.direction * 4);
    //    //    Debug.DrawRay(_rayRight.origin, _rayRight.direction * 4);
    //    //    Debug.DrawRay(_rayDiagLeftUp.origin, _rayDiagLeftUp.direction * 4);
    //    //    Debug.DrawRay(_rayDiagLeftDown.origin, _rayDiagLeftDown.direction * 4);
    //    //    Debug.DrawRay(_rayDiagRightUp.origin, _rayDiagRightUp.direction * 4);
    //    //    Debug.DrawRay(_rayDiagRightDown.origin, _rayDiagRightDown.direction * 4);
    //    //}
    //}

    //private void SquareSelected(string letter, Vector3 squarePosition, int squareIndex)
    //{
    //    if (_assignedPoints == 0)
    //    {
    //        //_rayStartPosition = squarePosition;
    //        _correcSquareList.Add(squareIndex);
    //        _word += letter;

    //        ////Use this code if selection direction change limitation is needed
    //        //_rayUp = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(0f, 1f));
    //        //_rayDown = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(0f, -1f));
    //        //_rayLeft = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(-1f, 0f));
    //        //_rayRight = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(1f, 0f));
    //        //_rayDiagLeftUp = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(-1f, 1f));
    //        //_rayDiagLeftDown = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(-1f, -1f));
    //        //_rayDiagRightUp = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(1f, 1f));
    //        //_rayDiagRightDown = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(1f, -1f));
    //    }
    //    else
    //    {
    //        _correcSquareList.Add(squareIndex);
    //        //_currenRay = SelectRay(_rayStartPosition, squarePosition); //Use this code if selection direction change limitation is needed
    //        GameEvents.SelectSquareMethod(squarePosition);
    //        _word += letter;
    //        CheckWord();
    //    }

    //    ////Use this code if selection direction change limitation is needed
    //    //else if (_assignedPoints == 1)
    //    //{
    //    //    _correcSquareList.Add(squareIndex);
    //    //    _currenRay = SelectRay(_rayStartPosition, squarePosition);
    //    //    GameEvents.SelectSquareMethod(squarePosition);
    //    //    _word += letter;
    //    //    CheckWord();
    //    //}
    //    //else
    //    //{
    //    //    if (IsPoinOnTheRay(_currenRay, squarePosition))
    //    //    {
    //    //        _correcSquareList.Add(squareIndex);
    //    //        GameEvents.SelectSquareMethod(squarePosition);
    //    //        _word += letter;
    //    //        CheckWord();
    //    //    }
    //    //}

    //    _assignedPoints++;
    //}

    //private void CheckWord()
    //{
    //    foreach (var searchingWord in _currentGameData.selectedBoardData.SearchingWords)
    //    {
    //        if (_word.Equals(searchingWord.Word))
    //        {
    //            GameEvents.CorrectWordMethod(_word, _correcSquareList);
    //            //_word = string.Empty;
    //            //_correcSquareList.Clear();
    //            return;
    //        }
    //    }

    //    bool foundExtraWord = _wordFinder.FindWord(_word);
    //    bool alreadyUsedWord = _dataProfile.UsedExtraWords.Contains(_word);

    //    //Debug.Log($"extra: {foundExtraWord}");
    //    //Debug.Log($"in List: {alreadyUsedWord}");

    //    if (foundExtraWord && !alreadyUsedWord)
    //    {
    //        GameEvents.OnCorrectExtraWordMethod(_correcSquareList);
    //        _dataProfile.UsedExtraWords.Add(_word);
    //        //_word = string.Empty;
    //        //_correcSquareList.Clear();
    //    }

    //    //Debug.Log(foundExtraWord ? $"{_word} found" : $"{_word} not found");
    //}

    //private bool IsPoinOnTheRay(Ray currentRay, Vector3 point)
    //{
    //    var hits = Physics.RaycastAll(currentRay, 100.0f);
    //    for (int i = 0; i < hits.Length; i++)
    //    {
    //        if (hits[i].transform.position == point)
    //            return true;
    //    }

    //    return false;
    //}

    //////Use this code if selection direction change limitation is needed
    ////private Ray SelectRay(Vector2 firstPosition, Vector2 secondPosition)
    ////{
    ////    var direction = (secondPosition - firstPosition).normalized;
    ////    float tolerance = 0.01f;

    ////    if ((Mathf.Abs(direction.x) < tolerance) && (Mathf.Abs(direction.y) - 1f < tolerance))
    ////    {
    ////        return _rayUp;
    ////    }

    ////    if ((Mathf.Abs(direction.x) < tolerance) && (Mathf.Abs(direction.y) - (-1f) < tolerance))
    ////    {
    ////        return _rayDown;
    ////    }

    ////    if ((Mathf.Abs(direction.x) - (-1f) < tolerance) && (Mathf.Abs(direction.y) < tolerance))
    ////    {
    ////        return _rayLeft;
    ////    }

    ////    if ((Mathf.Abs(direction.x) - 1f < tolerance) && (Mathf.Abs(direction.y) < tolerance))
    ////    {
    ////        return _rayRight;
    ////    }

    ////    if (direction.x < 0f && direction.y > 0f)
    ////    {
    ////        return _rayDiagLeftUp;
    ////    }

    ////    if (direction.x < 0f && direction.y < 0f)
    ////    {
    ////        return _rayDiagLeftDown;
    ////    }

    ////    if (direction.x > 0f && direction.y > 0f)
    ////    {
    ////        return _rayDiagRightUp;
    ////    }

    ////    if (direction.x > 0f && direction.y < 0f)
    ////    {
    ////        return _rayDiagRightDown;
    ////    }

    ////    return _rayDown;
    ////}

    //private void ClearSelection()
    //{
    //    _assignedPoints = 0;
    //    _correcSquareList.Clear();
    //    _word = string.Empty;
    //}
    #endregion
}
