using Game;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Lofelt.NiceVibrations;

public class BuyLifesPopup : MonoBehaviour
{
    private const string DefaultMessageText = "MORE LIFES?";
    private const string NoCoinsMessageText = "Not enough coins!";

    [SerializeField] private GameObject _buyLifesPopup;
    [SerializeField] private GameObject _buyLifesPopupObject;
    [SerializeField] private GameObject _buyLifesButtonObject;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private Text _lifesText;
    [SerializeField] private int lifesBuyCost;

    private Button _buyLifesButton;

    private void Start()
    {
        _buyLifesButton = _buyLifesButtonObject.AddComponent<Button>();

        _buyLifesPopup.SetActive(false);
    }

    private void TryBuyLifes()
    { 
        
    }


}
