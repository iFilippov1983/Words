using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Game
{
    public class CurrencyManager : MonoBehaviour
    {
        public const string coinsKey = "coins";
       
        public static event Action<int> CoinsAdded;

        [SerializeField] private TMP_Text coinsText;
        [SerializeField] private CoinSpawner coinSpawner;
        [Space] [SerializeField] private int coinsForLevelComplete;
        public int Coins { get; private set; }
     
        private void Start()
        {
            var loadedCoins = DataSaver.LoadIntData(coinsKey);
            SetCoins(Mathf.Max(0, loadedCoins));
            
            coinSpawner.CoinArrived += OnCoinArrived;
        }

        private void OnCoinArrived()
        {
            AddCoins(1);
        }

        private void OnDestroy()
        {
            DataSaver.SaveIntData(coinsKey, Coins);
        }

        public void SetCoins(int coins)
        {
            Coins = coins;
            UpdateText();
        }
        
        public void AddCoinsForLevelComplete()
        {
            AddCoins(coinsForLevelComplete);
        }

        private void UpdateText()
        {
            coinsText.text = Coins.ToString();
        }

        [Button]
        [HideInEditorMode]
        public void AddCoins(int amount)
        {
            SetCoins(Coins + amount);
            CoinsAdded?.Invoke(amount);
        }
    }
}