using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Game
{
    public class CurrencyManager : MonoBehaviour
    {
        public const string coinsKey = "coins";
       
        public static event Action<int> CoinsAmountChanged;
        public static event Action<int> TryChangeCoinsAmount;
        public static event Action<string> CoinsAmountChangeImpossible;

        [SerializeField] private TMP_Text coinsText;
        [SerializeField] private CoinSpawner coinSpawner;
        [Space] [SerializeField] private int coinsForLevelComplete;
        [SerializeField] private int _coinCost;
        private static bool _canBuy;

        public int Coins { get; private set; }
     
        private void Start()
        {
            var loadedCoins = DataSaver.LoadIntData(coinsKey);
            SetCoins(Mathf.Max(0, loadedCoins));
            
            coinSpawner.CoinArrived += OnCoinArrived;
            TryChangeCoinsAmount += ChangeCoinsAmount;
        }

        private void OnCoinArrived()
        {
            ChangeCoinsAmount(_coinCost);
        }

        private void OnDestroy()
        {
            DataSaver.SaveIntData(coinsKey, Coins);

            coinSpawner.CoinArrived -= OnCoinArrived;
            TryChangeCoinsAmount -= ChangeCoinsAmount;
        }

        public void SetCoins(int coins)
        {
            Coins = coins;
            UpdateText();
        }
        
        public void AddCoinsForLevelComplete()
        {
            ChangeCoinsAmount(coinsForLevelComplete);
        }

        private void UpdateText()
        {
            coinsText.text = Coins.ToString();
        }

        [Button]
        [HideInEditorMode]
        private void ChangeCoinsAmount(int amount)
        {
            var newAmount = Coins + amount;
            if (newAmount <= 0)
            {
                _canBuy = false;
                CoinsAmountChangeImpossible?.Invoke(Literal.AnimName_NoCoins);
            }
            else
            {
                _canBuy = true;
                SetCoins(newAmount);
                CoinsAmountChanged?.Invoke(amount);
            }
        }

        public static bool TryChangeCoinsAmountMethod(int amount)
        {
            TryChangeCoinsAmount?.Invoke(amount);
            return _canBuy;
        }
    }
}