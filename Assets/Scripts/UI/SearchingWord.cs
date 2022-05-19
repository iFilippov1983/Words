using System.Collections;
using System.Collections.Generic;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SearchingWord : MonoBehaviour
{
    [SerializeField] private ParticleSystem hitParticleSystem;
    public TextMeshProUGUI displayedText;
    public Image crossLine;

    private string _word;
    public string Word => _word;

    void Start()
    {
        
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
        hitParticleSystem.Play();
    }

    //private void CorrectWord(string word, List<int> squareIndexes)
    //{
    //    if (word.Equals(_word))
    //    {
    //        crossLine.gameObject.SetActive(true);
    //    }
    //}
}
