using System.Collections;
using System.Collections.Generic;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SearchingWord : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> hitParticleSystems;
    [SerializeField] private Animator animator;
    public TextMeshProUGUI displayedText;
    public Image crossLine;

    private string _word;
    private int hash;
    public string Word => _word;

    void Start()
    {
        hash = Animator.StringToHash("searching-word-hit");
    }

    private void OnEnable()
    {
        GameEvents.OnWordGetTarget += WordGetTarget;
        //GameEvents.OnCorrectWord += CorrectWord;
    }

    private void OnDisable()
    {
        GameEvents.OnWordGetTarget -= WordGetTarget;
        //GameEvents.OnCorrectWord -= CorrectWord;
    }

    public void SetWord(string word)
    { 
        _word = word;
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

        animator.Play(hash);
    }

    //private void CorrectWord(string word, List<int> squareIndexes)
    //{
    //    if (word.Equals(_word))
    //    {
    //        crossLine.gameObject.SetActive(true);
    //    }
    //}
}
