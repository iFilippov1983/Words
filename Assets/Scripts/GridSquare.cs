using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AlphabetData;

public class GridSquare : MonoBehaviour
{
    public int SquareIndex { get; set; }

    private LetterData _normalLetterData;
    private LetterData _selectedLetterData;
    private LetterData _correctLetterData;

    private SpriteRenderer _displayedSprite;
    private bool _isSelected;
    private bool _isClicked;
    private int _index = -1;
    private bool _isCorrect;

    private bool _toBeDestroyed;

    private void Start()
    {
        _displayedSprite = GetComponent<SpriteRenderer>();
        _isSelected = false;
        _isClicked = false;
        _isCorrect = false;

        _toBeDestroyed = false;
    }

    private void OnEnable()
    {
        GameEvents.OnEnableSquareSelection += OnEnableSquareSelection;
        GameEvents.OnDisableSquareSelection += OnDisableSquareSelection;
        GameEvents.OnSelectSquare += OnSelectSquare;
        GameEvents.OnCorrectWord += CorrectWord;
    }

    private void OnDisable()
    {
        GameEvents.OnEnableSquareSelection -= OnEnableSquareSelection;
        GameEvents.OnDisableSquareSelection -= OnDisableSquareSelection;
        GameEvents.OnSelectSquare -= OnSelectSquare;
        GameEvents.OnCorrectWord -= CorrectWord;
    }

    private void CorrectWord(string word, List<int> squareIndexes)
    {
        if (_isSelected && squareIndexes.Contains(_index))
        {
            _isCorrect = true;
            _displayedSprite.sprite = _correctLetterData.Sprite;
        }

        _isSelected = false;
        _isClicked = false;
    }

    public void SetSprite
        (
        LetterData normalLetterData,
        LetterData selectedLetterData,
        LetterData correctLetterData
        )
    {
        _normalLetterData = normalLetterData;
        _selectedLetterData = selectedLetterData;
        _correctLetterData = correctLetterData;

        //_displayedSprite.sprite = _normalLetterData.Sprite; //
        GetComponent<SpriteRenderer>().sprite = _normalLetterData.Sprite;
    }

    public void SetIndex(int index) => _index = index;
    public int GetIndex() => _index;

    public void OnEnableSquareSelection()
    {
        _isClicked = true;
        _isSelected = false;
    }

    public void OnDisableSquareSelection()
    { 
        _isSelected = false;
        _isClicked = false;

        if(_isCorrect)
            _displayedSprite.sprite = _correctLetterData.Sprite;
        else
            _displayedSprite.sprite = _normalLetterData.Sprite;

        if(_toBeDestroyed && _isCorrect)
            Destroy(gameObject);
    }

    private void OnSelectSquare(Vector3 position)
    {
        if (this.gameObject.transform.position == position)
            _displayedSprite.sprite = _selectedLetterData.Sprite;
    }

    private void OnMouseDown()
    {
        GameEvents.EnableSquareSelectionMethod();
        CheckSquare();
        _displayedSprite.sprite = _selectedLetterData.Sprite;
    }

    private void OnMouseEnter()
    {
        CheckSquare();
    }

    private void OnMouseExit()
    {
        
    }

    private void OnMouseUp()
    {
        GameEvents.ClearSelectionMethod();
        GameEvents.DisableSquareSelectionMethod();
    }

    public void CheckSquare()
    {
        if (_isSelected == false && _isClicked)
        {
            _isSelected = true;
            GameEvents.CheckSquareMethod(_normalLetterData.Letter, gameObject.transform.position, _index);

            _toBeDestroyed = true;
        }
    }
}
