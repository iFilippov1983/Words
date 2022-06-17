using System.Collections.Generic;
using UnityEngine;

public class SearchingWordsList : MonoBehaviour
{
    public GameData currentGameData;
    public GameObject searchingWordPrefab;
    public float offset = 0.0f;
    public int maxColumns = 5;
    public int maxRows = 4;

    private int _columns = 2;
    private int _rows;
    private int _wordsNumber;

    private List<GameObject> _words = new List<GameObject>();
    private List<SearchingWord> _searchingWords = new List<SearchingWord>();
    private RectTransform _prefabSquareRect;
    private RectTransform _parentRect;

    private bool _hideWords;
    public bool ListDone { get; private set; }

    public List<SearchingWord> SearchingWords => _searchingWords;

    private void Start()
    {
        _prefabSquareRect = searchingWordPrefab.GetComponent<RectTransform>();
        _parentRect = this.GetComponent<RectTransform>();
        _wordsNumber = currentGameData.selectedBoardData.SearchingWords.Count;
        _hideWords = currentGameData.selectedBoardData.UseDotsMode;

        if (_wordsNumber < _columns)
            _rows = 1;
        else
            CalculateColumnsAndRowsNumber();

        CreateWordObjects();
        SetWordsPosition();

        ListDone = false;
        _searchingWords = MakeSearchingWordsList();
    }

    private void CalculateColumnsAndRowsNumber()
    {
        do
        {
            _columns++;
            _rows = _wordsNumber / _columns;
        } while (_rows > maxRows);

        if (_columns > maxColumns)
        {
            _columns = maxColumns;
            _rows = _wordsNumber / _columns;
        }
    }

    private bool TryIncreaseColumnNumber()
    {
        _columns++;
        _rows = _wordsNumber / _columns;

        if (_columns > maxColumns)
        {
            _columns = maxColumns;
            _rows = _wordsNumber / _columns;

            return false;
        }

        if (_wordsNumber % _columns > 0)
            _rows++;

        return true;
    }

    private void CreateWordObjects()
    {
        var defaultScale = new Vector3(1f, 1f, 0.1f);
        var squareScale = GetSquareScale(defaultScale);

        for (int index = 0; index < _wordsNumber; index++)
        {
            var wordObject = Instantiate(searchingWordPrefab);
            _words.Add(wordObject as GameObject);
            _words[index].transform.SetParent(this.transform);
            var rectTransform = _words[index].GetComponent<RectTransform>();
            rectTransform.localScale = squareScale;
            rectTransform.localPosition = Vector3.zero;

            var wordToSet = currentGameData.selectedBoardData.SearchingWords[index].Word;
            var searchingWord = _words[index].GetComponent<SearchingWord>();
            searchingWord.SetWord(wordToSet);
            
            if (_hideWords)
                searchingWord.DisplayDots();    
        }
    }

    private Vector3 GetSquareScale(Vector3 defaultScale)
    {
        Vector3 finalScale = defaultScale;
        float adjustment = 0.01f;

        while (ShouldScaleDown(finalScale))
        { 
            finalScale.x -= adjustment;
            finalScale.y -= adjustment;

            if (finalScale.x <= 0 || finalScale.y <= 0)
            {
                finalScale.x = adjustment;
                finalScale.y = adjustment;

                return finalScale;
            }
        }

        return finalScale;
    }

    private bool ShouldScaleDown(Vector3 targetScale)
    {
        var squareSize = Vector3.zero;
        squareSize.x = _prefabSquareRect.rect.width * targetScale.x + offset;
        squareSize.y = _prefabSquareRect.rect.height * targetScale.y + offset;

        float totalSquareHeight = squareSize.y * _rows;
        //Make sure all of the square fit in the parent rectangle area
        bool notFit = totalSquareHeight > _parentRect.rect.height;

        while (notFit)
        {
            if (TryIncreaseColumnNumber())
                totalSquareHeight = squareSize.y * _rows;
            else
                return true;
        }
        
        float totalSquareWidth = squareSize.x * _columns;

        if(totalSquareWidth > _parentRect.rect.width)
            return true;

        return false;   
    }

    private void SetWordsPosition()
    {
        var squareRect = _words[0].GetComponent<RectTransform>();
        Vector2 wordOffset = new Vector2
        {
            x = squareRect.rect.width * squareRect.transform.localScale.x + offset,
            y = squareRect.rect.height * squareRect.transform.localScale.y + offset
        };

        int columnNumber = 0;
        int rowNumber = 0;
        int wordsInRow = 1;
        var startPosition = GetFirstSquarePosition();

        foreach (var word in _words)
        {
            if (columnNumber + 1 > _columns)
            {
                columnNumber = 0;
                wordsInRow = 1;
                rowNumber++;
            }

            var positionX = startPosition.x + wordOffset.x * columnNumber;
            var positionY = startPosition.y - wordOffset.y * rowNumber;

            word.GetComponent<RectTransform>().localPosition = new Vector2(positionX, positionY);
            columnNumber++;
            wordsInRow++;
        }
    }

    private Vector2 GetFirstSquarePosition()
    { 
        Vector2 startPosition = new Vector2(0f, transform.position.y);
        var squareRect = _words[0].GetComponent<RectTransform>();
        Vector2 squareSize = Vector2.zero;

        squareSize.x = squareRect.rect.width * squareRect.transform.localScale.x + offset;
        squareSize.y = squareRect.rect.height * squareRect.transform.localScale.y + offset;

        //Make sure they are in center
        float shiftByX = (_parentRect.rect.width - (squareSize.x * _columns)) / 2;
        startPosition.x = ((_parentRect.rect.width - squareSize.x) / 2) * (-1);
        startPosition.x += shiftByX;

        float shiftByY = (_parentRect.rect.height - (squareSize.y * _rows)) / 2;
        startPosition.y = (_parentRect.rect.height - squareSize.y) / 2;
        startPosition.y -= shiftByY;

        return startPosition;
    }

    private List<SearchingWord> MakeSearchingWordsList()
    { 
        var list = new List<SearchingWord>();
        foreach (var word in _words)
            if (word)
                list.Add(word.GetComponent<SearchingWord>());
        ListDone = true;
        return list;
    }
}
