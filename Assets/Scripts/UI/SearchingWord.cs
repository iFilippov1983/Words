using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SearchingWord : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> hitParticleSystems;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject _dotPrefab;
    [SerializeField] private Vector3 _dotsEndSize;
    private List<GameObject> _dots = new List<GameObject>();

    public TextMeshProUGUI displayedText;
    public Image crossLine;
    [ReadOnly]
    public bool isFound;
    [ShowInInspector, ReadOnly]
    private string _word;
    private int hash;
    public string Word => _word;

    void Start()
    {
        hash = Animator.StringToHash("searching-word-hit");
    }

    private void OnEnable()
    {
        isFound = false;

        GameEvents.OnWordGetTarget += WordGetTarget;
        GameEvents.OnCorrectWord += OnCorrestWord;
    }

    private void OnDisable()
    {
        GameEvents.OnWordGetTarget -= WordGetTarget;
        GameEvents.OnCorrectWord -= OnCorrestWord;
    }

    public void SetWord(string word)
    { 
        _word = word;
        displayedText.text = _word;
    }

    private void OnCorrestWord(string word, List<int> squareindexes)
    {
        if (word.Equals(_word))
            DisplayWord();
    }

    private void DisplayWord()
    {
        if(_dots.Count != 0)
            foreach (GameObject go in _dots)
                go.SetActive(false);

        displayedText.text = _word;
    }

    private void WordGetTarget(string word)
    {
        if (word.Equals(_word))
        {
            crossLine.gameObject.SetActive(true);
        }
    }

    public void PlayAnimation()
    {
        foreach (var hitParticleSystem in hitParticleSystems)
        {
            hitParticleSystem.Play();
        }

        isFound = true;
        animator.Play(hash);
    }

    public void DisplayDots()
    { 
        displayedText.text = string.Empty;

        for (int i = 0; i < _word.Length; i++)
        {
            var dot = Instantiate(_dotPrefab, transform, false);
            dot.name = _word[i].ToString();
            _dots.Add(dot);
        }
        var positions = GetDotsPositions(transform.position, _word.Length, _dotsEndSize.x / 2f);

        for (int k = 0; k < _dots.Count; k++)
        {
            _dots[k].transform.position = positions[k];
        }
    }

    private Vector3[] GetDotsPositions(Vector3 selfPosition, int letterCount, float letterSpacing)
    {
        var positions = new Vector3[letterCount];

        for (var i = 0; i < positions.Length; i++)
        {
            positions[i] = selfPosition + Vector3.right * (i * letterSpacing);
        }

        var center = Vector3.Lerp(positions[0],
                                  positions[positions.Length - 1],
                                  0.5f);

        var centerOffset = positions[0] - center;

        for (var i = 0; i < positions.Length; i++)
            positions[i] += centerOffset;

        return positions;
    }
}
