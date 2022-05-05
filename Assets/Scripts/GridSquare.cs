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

    void Start()
    {
        _displayedSprite = GetComponent<SpriteRenderer>();
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
}
