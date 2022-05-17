using System.Collections.Generic;
using Game.WordComparison;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WordCheker : MonoBehaviour
{
    public GameData currentGameData;
    public GameLevelData gameLevelData;

    private const string UsedWords = "UsedWords";
    private string _word;
    private string _extraWord;
    private int _assignedPoints = 0;
    private int _completedWords = 0;
    private bool _currentLevelNotCompleted;
    private List<int> _correctSquareList = new List<int>();

    [SerializeField] private DataProfile _dataProfile;
    [SerializeField] private List<TextAsset> _dictionaries;
    private WordFinder _wordFinder;

    private void OnEnable()
    {
        Init();

        GameEvents.OnCheckSquare += SquareSelected;
        GameEvents.OnClearSelection += ClearSelection;
        GameEvents.OnLoadLevel += LoadNextGameLevel;
        GameEvents.OnGameOver += OnGameOver;
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

        _wordFinder = new WordFinder(_dictionaries, ' ');
    }

    private void Init()
    {
        _dataProfile.SetUsedExtraWordsList(DataSaver.LoadSavedStringList(UsedWords));
        Debug.Log("Current used extra words list count: " + _dataProfile.UsedExtraWords.Count);

        _dataProfile.CurrenLevelNumber = DataSaver.LoadIntData(currentGameData.selectedCategoryName) + 1;

        TinySauce.OnGameStarted(_dataProfile.CurrenLevelNumber.ToString());

        _currentLevelNotCompleted = true;
    }

    private void Cleanup()
    {
        if (_currentLevelNotCompleted)
        {
            DataSaver.SaveStringDataFromList(UsedWords, _dataProfile.UsedExtraWords);

            Debug.Log("Extra words list data SAVED");
        }
        else
        {
            DataSaver.ClearSavedStringListData(UsedWords);

            Debug.Log("Extra words list data CLEARED");
        }

        TinySauce.OnGameFinished(!_currentLevelNotCompleted, 0f, _dataProfile.CurrenLevelNumber.ToString());

        _dataProfile.UsedExtraWords.Clear();
    }

    private void LoadNextGameLevel() => SceneManager.LoadScene(Literal.Scene_GameScene);

    private void OnGameOver() => _currentLevelNotCompleted = false;// Lets player reuse allready used extra words if he've lost

    private void SquareSelected(string letter, Vector3 squarePosition, int squareIndex)
    {
        if (_assignedPoints == 0)
        {
            _correctSquareList.Add(squareIndex);
            _word += letter;
        }
        else
        {
            _correctSquareList.Add(squareIndex);
            GameEvents.SelectSquareMethod(squarePosition);
            _word += letter;
            //CheckWord();
        }

        _assignedPoints++;
    }

    private void CheckWord()
    {
        foreach (var searchingWord in currentGameData.selectedBoardData.SearchingWords)
        {
            if (_word.Equals(searchingWord.Word))
            {
                GameEvents.CorrectWordMethod(_word, _correctSquareList);
                _completedWords++;
                CheckBoardCompleted();
                return;
            }
        }

        bool foundExtraWord = _wordFinder.FindWord(_word);
        bool alreadyUsedWord = _dataProfile.UsedExtraWords.Contains(_word);
        
        //Debug.Log($"extra: {foundExtraWord}");
        //Debug.Log($"in List: {alreadyUsedWord}");

        if (foundExtraWord && !alreadyUsedWord)
        {
            GameEvents.OnCorrectExtraWordMethod(_correctSquareList);
            _dataProfile.UsedExtraWords.Add(_word);

            _extraWord = _word;
            Debug.Log($"Extra word: {_extraWord} is ADDED to list");
        }

        //Debug.Log(foundExtraWord ? $"{_word} found" : $"{_word} not found");
    }

    private void ClearSelection()
    {
        CheckWord();

        _assignedPoints = 0;
        _correctSquareList.Clear();

        //if (_word.Equals(_extraWord) == false && string.IsNullOrEmpty(_extraWord) == false)
        //{
        //    _dataProfile.UsedExtraWords.Remove(_extraWord);

        //    Debug.Log($"Extra word: {_extraWord} is REMOVED from list");
        //}
            
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

                    GameEvents.BoardCompletedMethod(loadNextCategory);
                }
                else
                {
                    SceneManager.LoadScene(Literal.Scene_SelectCategory);
                }
            }
            else
            {
                GameEvents.BoardCompletedMethod(loadNextCategory);
            }

            if (loadNextCategory)
            {
                GameEvents.UnlockNextCategoryMethod();
            }
                
        }
    }

    
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
}
