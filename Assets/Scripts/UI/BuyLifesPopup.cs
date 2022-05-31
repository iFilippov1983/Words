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
    [SerializeField] private GameObject _buyLifesButtonObject;
    [SerializeField] private TextMeshProUGUI _messageText;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private Text _lifesText;
    [SerializeField] private int _lifesBuyCost = 100;
    [SerializeField] private int _lifesBuyAmount = 5;

    private Button _buyLifesButton;

    public static Action<int> ContinueWhithExtraLifes;

    public TextMeshProUGUI TimerText => _timerText;
    public Text LifesText => _lifesText;

    private void Start()
    {
        _buyLifesButton = _buyLifesButtonObject.GetComponent<Button>();
        _buyLifesButton.onClick.AddListener(TryBuyLifes);

        ShowPopup(false);

        CurrencyManager.CoinsAmountChangeImpossible += ShowMessage;
    }

    private void OnDestroy()
    {
        CurrencyManager.CoinsAmountChangeImpossible -= ShowMessage;
    }

    private void TryBuyLifes()
    {
        int cost = -_lifesBuyCost;
        bool succes = CurrencyManager.TryChangeCoinsAmountMethod(cost);
        if (succes)
        {
            ContinueWhithExtraLifes?.Invoke(_lifesBuyAmount);
            _buyLifesPopup.SetActive(false);
        }
    }

    private async void ShowMessage(string animationName)
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);
        Debug.Log("[Haptic] BuyLifesPopup - ShowMessage");

        _messageText.text = NoCoinsMessageText;
        var animation = _messageText.GetComponentInChildren<Animation>();
        animation.Play();
        while (animation.isPlaying)
            await System.Threading.Tasks.Task.Yield();

        _messageText.text = DefaultMessageText;
    }

    public void ShowPopup(bool show) => _buyLifesPopup.SetActive(show);
}
