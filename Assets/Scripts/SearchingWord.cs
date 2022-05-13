using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SearchingWord : MonoBehaviour
{
    public TextMeshProUGUI displayedText;
    public Image crossLine;

    private string _word;

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

    private void CorrectWord(string word, List<int> squareIndexes)
    {
        if (word.Equals(_word))
        {
            crossLine.gameObject.SetActive(true);
        }
    }
}
