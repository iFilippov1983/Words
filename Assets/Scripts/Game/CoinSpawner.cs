using System;
using System.Collections;
using System.Collections.Generic;
using Lofelt.NiceVibrations;
using UnityEngine;

namespace Game
{
    public class CoinSpawner : MonoBehaviour
    {
        public const int noCoins = -1;
        public event Action CoinArrived;
        [SerializeField] private GameObject coinPrefab;
        [SerializeField] private Transform uiParent;
        [SerializeField] private RectTransform target;
        [Space] [SerializeField] private float spawnInterval;
        [SerializeField] private float distanceToCamera;
        [SerializeField] private float flySpeed;
        [SerializeField] private AnimationCurve timeCurve;
        [SerializeField] private ParticleSystem.MinMaxCurve xCurve;
        [SerializeField] private ParticleSystem.MinMaxCurve yCurve;
        [SerializeField] private ParticleSystem.MinMaxCurve zCurve;

        private List<Vector3> spawnPositions = new List<Vector3>();
        private int coinsToSpawn = noCoins;

        private void Start()
        {
            GameEvents.OnCheckSquare += OnCheckSquare;
            GameEvents.OnEnableSquareSelection += OnEnableSquareSelection;
            GameEvents.OnDisableSquareSelection += OnDisableSquareSelection;
            GameEvents.OnCorrectExtraWord += OnCorrectExtraWord;
            GameEvents.OnCorrectWord += OnCorrectWord;
        }

        private void OnCorrectWord(string word, List<int> squareindexes)
        {
            coinsToSpawn = noCoins;
        }

        private void OnCheckSquare(string letter, Vector3 squareposition, int squareindex)
        {
            spawnPositions.Add(squareposition);
        }

        private void OnDestroy()
        {
            GameEvents.OnCheckSquare -= OnCheckSquare;
            GameEvents.OnEnableSquareSelection -= OnEnableSquareSelection;
            GameEvents.OnDisableSquareSelection -= OnDisableSquareSelection;
            GameEvents.OnCorrectExtraWord -= OnCorrectExtraWord;
            GameEvents.OnCorrectWord -= OnCorrectWord;
        }

        private void OnDisableSquareSelection()
        {
            if (coinsToSpawn != noCoins)
                StartCoroutine(SpawnCoinsWithInterval(coinsToSpawn, spawnInterval));
            
            coinsToSpawn = noCoins;
        }

        private void OnCorrectExtraWord(List<int> squareindexes)
        {
            coinsToSpawn = squareindexes.Count;
        }

        private void OnEnableSquareSelection()
        {
            coinsToSpawn = noCoins;
            spawnPositions.Clear();
        }

        private IEnumerator SpawnCoinsWithInterval(int coinCount, float interval)
        {
            for (int i = 0; i < coinCount; i++)
            {
                SpawnCoin(spawnPositions[i]);
                yield return new WaitForSeconds(interval);
            }
        }

        private void SpawnCoin(Vector3 position)
        {
            var coin = Instantiate(coinPrefab, position, Quaternion.identity, uiParent);

            StartCoroutine(MoveCoin(coin.transform));
        }

        private IEnumerator MoveCoin(Transform coin)
        {
            var travelPercent = 0f;
            var startPos = coin.position;
            var targetPos = target.transform.position;
            targetPos.z = distanceToCamera;

            while (travelPercent < 1)
            {
                coin.position = Vector3.Lerp(startPos, targetPos, timeCurve.Evaluate(travelPercent));

                var offset = new Vector3(
                    xCurve.Evaluate(travelPercent),
                    yCurve.Evaluate(travelPercent),
                    zCurve.Evaluate(travelPercent)
                );

                coin.position += offset;

                travelPercent += flySpeed * Time.deltaTime;
                yield return null;
            }

            HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
            Debug.Log("[Haptic] CoinSpawner - MoveCoin[arrived]");

            CoinArrived?.Invoke();
            Destroy(coin.gameObject);
        }
    }
}