using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyPromptsPopup : MonoBehaviour
{
    private const string DefaultMessageText = "MORE PROMPTS?";
    private const string NoCoinsMessageText = "Not enough coins!";

    [SerializeField] private GameObject _buyPromptsPopup;

    public Button BuyPrompstButton;
    public Button BackButton;

    void Start()
    {
        
    }


}
