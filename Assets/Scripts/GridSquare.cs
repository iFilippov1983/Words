using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static AlphabetData;

public class GridSquare : MonoBehaviour
{
    public int SquareIndex { get; set; }

    [SerializeField] private GameObject _bodyObject;
    [SerializeField] private ParticleSystem _highlightedEffect;
    [SerializeField] private ParticleSystem _destroyEffect;

    private LetterData _normalLetterData;
    private LetterData _selectedLetterData;
    private LetterData _correctLetterData;

    private SpriteRenderer _displayedSprite;
    private bool _isSelected;
    private bool _isClicked;
    private int _index = -1;
    private bool _isCorrect;

    private bool _isInExtraWord;
    private bool _toBeDestroyed;

    public LetterData NormalLetterData => _normalLetterData;

    private void Start()
    {
        _displayedSprite = GetComponent<SpriteRenderer>();
        _isSelected = false;
        _isClicked = false;
        _isCorrect = false;

        _isInExtraWord = false;
        _toBeDestroyed = false;
    }

    private void OnEnable()
    {
        GameEvents.OnEnableSquareSelection += OnEnableSquareSelection;
        GameEvents.OnDisableSquareSelection += OnDisableSquareSelection;
        GameEvents.OnSelectSquare += OnSelectSquare;
        GameEvents.OnCorrectWord += CorrectWord;
        GameEvents.OnCorrectExtraWord += CorrectExtraWord;
    }

    private void OnDisable()
    {
        GameEvents.OnEnableSquareSelection -= OnEnableSquareSelection;
        GameEvents.OnDisableSquareSelection -= OnDisableSquareSelection;
        GameEvents.OnSelectSquare -= OnSelectSquare;
        GameEvents.OnCorrectWord -= CorrectWord;
        GameEvents.OnCorrectExtraWord -= CorrectExtraWord;
    }

    private void CorrectWord(string word, List<int> squareIndexes)
    {
        if (_isSelected && squareIndexes.Contains(_index))
        {
            _isCorrect = true;
            _displayedSprite.sprite = _correctLetterData.Sprite;
            _toBeDestroyed = true;
        }

        //_isSelected = false;
        //_isClicked = false;
    }

    private void CorrectExtraWord(List<int> squareIndexes)
    {

        if (_isSelected && squareIndexes.Contains(_index))
        {
            _isInExtraWord = true;
            //_displayedSprite.sprite = _correctLetterData.Sprite;
        }

        //_isSelected = false;
        //_isClicked = false;
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

        GetComponent<SpriteRenderer>().sprite = _normalLetterData.Sprite;
    }

    public void SetIndex(int index) => _index = index;
    public int GetIndex() => _index;

    public void OnEnableSquareSelection()
    {       
        _isClicked = true;
        _isSelected = false;
    }

    public async void OnDisableSquareSelection()
    { 
        _isSelected = false;
        _isClicked = false;

        if (_isCorrect || _isInExtraWord)
        {
            _displayedSprite.sprite = _correctLetterData.Sprite;
        }
        else
        {
            _displayedSprite.sprite = _normalLetterData.Sprite;
            _highlightedEffect.gameObject.SetActive(false);
        }

        if (_toBeDestroyed && _isCorrect)
        {
            _displayedSprite.enabled = false;
            _bodyObject.gameObject.SetActive(false);
            _destroyEffect.gameObject.SetActive(true);
            _destroyEffect.Play();
            while(_destroyEffect.isPlaying)
                await Task.Yield();

            Destroy(gameObject);
        }
    }

    private void OnSelectSquare(Vector3 position)
    {
        if (this.gameObject.transform.position == position)
        {
            _displayedSprite.sprite = _selectedLetterData.Sprite;
        }
            
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
            _highlightedEffect.gameObject.SetActive(true);
            _highlightedEffect.Play();

            _isSelected = true;
            GameEvents.CheckSquareMethod(_normalLetterData.Letter, transform.position, _index);
        }
    }
}
