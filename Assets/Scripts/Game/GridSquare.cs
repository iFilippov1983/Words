using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static AlphabetData;

public class GridSquare : MonoBehaviour
{
    [SerializeField] private DataProfile _dataProfile;
    [SerializeField] private SpriteRenderer _displayedSprite;
    [SerializeField] private GameObject _bodyObject;
    [SerializeField] private Material _bodyMatNormal;
    [SerializeField] private Material _bodyMatHighlighted;
    [SerializeField] private Material _bodyMatWrong;
    [SerializeField] private Material _bodyMatExtra;
    [SerializeField] private ParticleSystem _highlightedEffect;
    [SerializeField] private ParticleSystem _destroyEffect;
    [SerializeField] private bool _isInCorrectWord;

    private LetterData _normalLetterData;
    private LetterData _selectedLetterData;
    private LetterData _correctLetterData;
    private LetterData _planeLetterData;

    private MeshRenderer _bodyMesh;
    private Animator _animator;
    private Transform _thisTransform;
    private Transform _bodyTransform;
    private Vector3 _thresholdPoint;

    private int _showDelay = 1000;
    private int _maxDelay = 1000;
    private int _index = -1;
    private string _letter;

    private bool _isClickable;
    private bool _notVisible;
    private bool _isSelected;
    private bool _isClicked;
    private bool _isCorrect;
    private bool _isInExtraWord;
    private bool _toBeDestroyed;
    private bool _checkIsUnavailable;

    [ShowInInspector]
    public string Letter => _letter;
    public int Index => _index;
    public bool NotVisible => _notVisible;
    public Vector3 BodyPosition => _bodyTransform.position;
    public LetterData PlaneLetterData => _planeLetterData;

    private void Start()
    {
        _thresholdPoint = FindObjectOfType<ThresholdView>().transform.position;
        _bodyMesh = _bodyObject.GetComponent<MeshRenderer>();
        _animator = GetComponent<Animator>();
        _thisTransform = gameObject.transform;
        _bodyTransform = _bodyObject.transform;

        _notVisible = true;
        _isClickable = true;
        _isSelected = false;
        _isClicked = false;
        _isCorrect = false;
        _isInExtraWord = false;
        _toBeDestroyed = false;
        _checkIsUnavailable = (_notVisible || _isClickable == false || _dataProfile.MousePositionIsFar);
    }

    private void OnEnable()
    {
        GameEvents.OnEnableSquareSelection += OnEnableSquareSelection;
        GameEvents.OnDisableSquareSelection += OnDisableSquareSelection;
        GameEvents.OnSelectSquare += OnSelectSquare;
        GameEvents.OnUnselectSquare += OnUnselectSquare;
        GameEvents.OnCorrectWord += CorrectWord;
        GameEvents.OnCorrectExtraWord += CorrectExtraWord;
        GameEvents.OnMenuIsActive += SetClickability;
        GameEvents.OnWordToPromptFound += ShowPrompt;
    }

    private void OnDisable()
    {
        CancelInvoke();

        GameEvents.OnEnableSquareSelection -= OnEnableSquareSelection;
        GameEvents.OnDisableSquareSelection -= OnDisableSquareSelection;
        GameEvents.OnSelectSquare -= OnSelectSquare;
        GameEvents.OnUnselectSquare -= OnUnselectSquare;
        GameEvents.OnCorrectWord -= CorrectWord;
        GameEvents.OnCorrectExtraWord -= CorrectExtraWord;
        GameEvents.OnMenuIsActive -= SetClickability;
        GameEvents.OnWordToPromptFound -= ShowPrompt;
    }

    private void OnMouseDown()
    {
        if (_checkIsUnavailable) 
            return;

        GameEvents.EnableSquareSelectionMethod();
        CheckSquare();
        _displayedSprite.sprite = _selectedLetterData.Sprite;
        _bodyMesh.material = _bodyMatHighlighted;
        _animator.SetBool(Literal.AnimBool_isHighlighted, true);
    }

    private void OnMouseEnter()
    {
        if (_checkIsUnavailable) 
            return;

        CheckSquare();
    }

    private void OnMouseUp()
    {
        if (_checkIsUnavailable)
            return;

        GameEvents.ClearSelectionMethod();
        GameEvents.DisableAllSquaresSelectionMethod();
    }

    private void FixedUpdate()
    {
        CheckGlobalPosition();
        _checkIsUnavailable = (_notVisible || _isClickable == false || _dataProfile.MousePositionIsFar);
    }

    public void SetSprite
       (
       LetterData normalLetterData,
       LetterData selectedLetterData,
       LetterData correctLetterData,
       LetterData planeLetterData
       )
    {
        _normalLetterData = normalLetterData;
        _selectedLetterData = selectedLetterData;
        _correctLetterData = correctLetterData;
        _planeLetterData = planeLetterData;

        _displayedSprite.sprite = _normalLetterData.Sprite;
        _letter = _normalLetterData.Letter;
    }

    public async Task ManualDestroy()
    {
        if (_notVisible == false)
        {
            _displayedSprite.enabled = false;
            _bodyObject.gameObject.SetActive(false);

            _destroyEffect.gameObject.SetActive(true);
            _destroyEffect.Play();

            while (_destroyEffect.isPlaying)
                await Task.Yield();
        }
        Destroy(gameObject);
    }

    public void SetIndex(int index) => _index = index;
    public int GetIndex() => _index;
    private void SetClickability(bool menuIsActive) => _isClickable = !menuIsActive;

    private void OnEnableSquareSelection()
    {
        _isClicked = true;
        _isSelected = false;
    }

    private void CheckSquare()
    {
        if (_isSelected == false && _isClicked)
        {
            _highlightedEffect.gameObject.SetActive(true);
            _highlightedEffect.Play();

            _isSelected = true;

            GameEvents.CheckSquareMethod(_normalLetterData.Letter, transform.position, _index);
        }
        else if (_isSelected)
        {
            GameEvents.CheckSquareMethod(_normalLetterData.Letter, transform.position, _index);
        }
    }

    private async void OnDisableSquareSelection()
    {
        if (_isCorrect)
        {
            _displayedSprite.sprite = _correctLetterData.Sprite;
            _bodyMesh.material = _bodyMatHighlighted;
        }
        else if (_isSelected && !_isInExtraWord)
        {
            ShowWrongWord();
            _displayedSprite.sprite = _normalLetterData.Sprite;
            _highlightedEffect.gameObject.SetActive(false);
            _animator.SetBool(Literal.AnimBool_isHighlighted, false);
        }

        if (_toBeDestroyed && _isCorrect)
        {
            _displayedSprite.enabled = false;
            _bodyObject.gameObject.SetActive(false);

            _destroyEffect.gameObject.SetActive(true);
            _destroyEffect.Play();

            while (_destroyEffect.isPlaying)
                await Task.Yield();
            
            Destroy(gameObject);
        }

        if (_isInExtraWord)
        {
            _animator.SetBool(Literal.AnimBool_isHighlighted, false);
            ShowExtraWord();
            _displayedSprite.sprite = _normalLetterData.Sprite;
            _highlightedEffect.gameObject.SetActive(false);
            _isInExtraWord = false;
        }

        _isSelected = false;
        _isClicked = false;
    }

    private async void CheckGlobalPosition()
    {
        if (_thisTransform.position.y < _thresholdPoint.y && _notVisible)
        {
            GameEvents.BoardConfigurationChangedMethod();
            _animator.SetBool(Literal.AnimBool_isVisible, true);
            _notVisible = false;
            var delay = Random.Range(0, _maxDelay);
            await Task.Delay(delay);
            _animator.SetTrigger(Literal.AnimTrigger_Idle);
        }
    }

    private void CorrectWord(string word, List<int> squareIndexes)
    {
        if (_isSelected && squareIndexes.Contains(_index))
        {
            _isCorrect = true;
            _displayedSprite.sprite = _correctLetterData.Sprite;
            _bodyMesh.material = _bodyMatHighlighted;
            _toBeDestroyed = true;
            _isClickable = false;
        }
    }

    private void CorrectExtraWord(List<int> squareIndexes)
    {
        if (_isSelected && squareIndexes.Contains(_index))
        {
            _isInExtraWord = true;
        }
    }

    private async void ShowExtraWord()
    {
        _bodyMesh.material = _bodyMatExtra;
        await Task.Delay(_showDelay);
        if(_bodyMesh != null)
            _bodyMesh.material = _bodyMatNormal;
    }

    private async void ShowWrongWord()
    {
        SoundManager.PalaySound(Sound.Word_Wrong);
        Debug.Log("[Sound] GridSquare - ShowWrongWord");

        _bodyMesh.material = _bodyMatWrong;
        _animator.SetBool(Literal.AnimBool_isWrong, true);
        await Task.Delay(_showDelay);
        if (_bodyMesh != null)
        {
            _bodyMesh.material = _bodyMatNormal;
            _animator.SetBool(Literal.AnimBool_isWrong, false);
        } 
    }

    private async void ShowPrompt(List<int> squareIndexes)
    {
        squareIndexes.Contains(_index);
        if (squareIndexes.Contains(_index) || _isInCorrectWord)
        {
            _animator.SetBool(Literal.AnimBool_showPrompt, true);

            CancellationToken token = new CancellationToken();
            token.ThrowIfCancellationRequested();
            await Task.Delay(2000, token);
            if (token.IsCancellationRequested) return;

            if(_animator)
                _animator.SetBool(Literal.AnimBool_showPrompt, false);
        }
    }

    private void OnSelectSquare(Vector3 position)
    {
        if (this.gameObject.transform.position == position)
        {
            _displayedSprite.sprite = _selectedLetterData.Sprite;
            _bodyMesh.material = _bodyMatHighlighted;
            _animator.SetBool(Literal.AnimBool_isHighlighted, true);
        } 
    }

    private void OnUnselectSquare(string letter, Vector3 squarePosition, int squareIndex)
    {
        if (squareIndex.Equals(_index))
        {
            _displayedSprite.sprite = _normalLetterData.Sprite;
            _bodyMesh.material = _bodyMatNormal;
            _highlightedEffect.gameObject.SetActive(false);
            _animator.SetBool(Literal.AnimBool_isHighlighted, false);

            _isSelected = false;
        }
    }

    private void PlayPromptSound()
    {
        SoundManager.PalaySound(Sound.Prompt);
        Debug.Log("[Sound] GridSquare - PlayPromptSound");
    }
}
