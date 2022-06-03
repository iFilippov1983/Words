using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PromptsManager : MonoBehaviour
{
    private const string PromptsKey = "Prompts";

    [SerializeField] private Prompter _prompter;
    [Space][SerializeField] private BuyPromptsPopup _buyPromptPopup;
    [SerializeField] private Button _buyPromptsButton;
    [SerializeField] private Button _usePromptButton;
    [SerializeField] private Text _promptsAmountText;
    [SerializeField] private int _defaultPromptsAmount = 3;
    private static bool _havePrompts;

    private bool _canUse;
    private bool _canBuy;
    private bool _inGame;
    private bool _inMainMenu;
    public int Prompts { get; private set; }

    public static Action<int> PromptsAmountChanged;
    public static Action<int> TryChangePromptsAmount;
    public static Action<string> PromptsAmountChangeImpossible;

    void Start()
    {
        LoadData();

        _inGame = (_usePromptButton != null 
            && SceneManager.GetActiveScene().name.Equals(Literal.Scene_GameScene));
        if(_inGame)
            _usePromptButton.onClick.AddListener(ChangePromptsAmount);

        _inMainMenu = (_buyPromptPopup != null && _buyPromptsButton
            && SceneManager.GetActiveScene().name.Equals(Literal.Scene_MainMenu));
        if (_inMainMenu)
        { 
        
        }
    }

    private void OnDestroy()
    {
        SaveData();
    }

    private void ChangePromptsAmount()
    { 
    
    }

    private void SaveData()
    { 
    
    }

    private void LoadData()
    { 
    
    }
}
