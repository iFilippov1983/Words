using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class WordAnimation : MonoBehaviour
    {
        [SerializeField] private Transform taskWordsParent;
        [SerializeField] private Transform flyingLetterTarget;
        [SerializeField] private GameObject flyingLetterPrefab;
        [Space] [SerializeField] private float flySpeed;
         [SerializeField] private float normalizeSpeed;
        [SerializeField] private float letterSpacingNormalized;
        [SerializeField] private float letterSpacingEnd;
        [SerializeField] private ParticleSystem.MinMaxCurve xCurve;
        [SerializeField] private ParticleSystem.MinMaxCurve yCurve;
        [SerializeField] private ParticleSystem.MinMaxCurve zCurve;
        [SerializeField] private ParticleSystem.MinMaxCurve sizeOffset;
        private Camera _camera;
        private readonly List<GameObject> flyingLetters = new List<GameObject>();
        private bool isCorrectWordFound;
        private bool shouldCreateFlyingLetters = true;
        private string correctWord;
        private List<SearchingWord> taskWords = new List<SearchingWord>();

        private IEnumerator Start()
        {
            _camera = Camera.main;

            GameEvents.OnCheckSquare += OnCheckSquare;
            GameEvents.OnDisableSquareSelection += OnDisableSquareSelection;
            GameEvents.OnCorrectWord += OnCorrectWord;

            yield return new WaitWhile(() => taskWordsParent.childCount == 0);

            taskWordsParent.GetComponentsInChildren(taskWords);
        }

        private void OnCorrectWord(string word, List<int> squareindexes)
        {
            correctWord = word;
            isCorrectWordFound = true;
        }

        private void OnDisableSquareSelection()
        {
            shouldCreateFlyingLetters = true;

            if (isCorrectWordFound)
            {
                foreach (var flyingLetter in flyingLetters)
                    flyingLetter.SetActive(true);

                StartCoroutine(MoveLetters());
                isCorrectWordFound = false;
                return;
            }

            ClearFlyingLetters();
        }

        private Vector3 GetSearchingWordPosition(string word)
        {
            var taskWord = taskWords.Find(x => string.Equals(x.displayedText.text.ToLower(), word.ToLower()));

            if (taskWord != null) 
                return taskWord.transform.position;
            
            Debug.LogError("task word not found");
            return Vector3.zero;
        }

        private IEnumerator MoveLetters()
        {
            var lettersStartPositions = GetLettersStartPosition(flyingLetters);
            var center = Vector3.Lerp(flyingLetters[0].transform.position,
                flyingLetters[flyingLetters.Count - 1].transform.position, 0.5f);
            var lettersEndPositionsNormalized = GetLettersEndPosition(center, flyingLetters ,letterSpacingNormalized);
            var lettersEndPositions = GetLettersEndPosition(GetSearchingWordPosition(correctWord), flyingLetters, letterSpacingEnd);
            var scale = flyingLetters[0].transform.localScale;

            var travelPercent = 0f;
            while (travelPercent < 1)
            {
                for (var i = 0; i < flyingLetters.Count; i++)
                {
                    var letterTransform = flyingLetters[i].transform;

                    letterTransform.position = Vector3.Lerp(
                        lettersStartPositions[i],
                        lettersEndPositionsNormalized[i],
                        travelPercent);
                }

                travelPercent += normalizeSpeed * Time.deltaTime;

                yield return null;
            }

            
            travelPercent = 0f;
            lettersStartPositions = GetLettersStartPosition(flyingLetters);
            
            while (travelPercent < 1)
            {
                for (var i = 0; i < flyingLetters.Count; i++)
                {
                    var letterTransform = flyingLetters[i].transform;

                    letterTransform.position = Vector3.Lerp(
                        lettersStartPositions[i],
                        lettersEndPositions[i],
                        travelPercent);

                    var offset = new Vector3(
                        xCurve.Evaluate(travelPercent),
                        yCurve.Evaluate(travelPercent),
                        zCurve.Evaluate(travelPercent)
                    );

                    letterTransform.position += offset;
                }

                foreach (var flyingLetter in flyingLetters)
                    flyingLetter.transform.localScale = scale * sizeOffset.Evaluate(travelPercent);

                travelPercent += flySpeed * Time.deltaTime;

                yield return null;
            }

            ClearFlyingLetters();
        }

        private Vector3[] GetLettersStartPosition(IReadOnlyList<GameObject> letters)
        {
            var positions = new Vector3[letters.Count];

            for (var i = 0; i < positions.Length; i++)
                positions[i] = letters[i].transform.position;

            return positions;
        }

        private Vector3[] GetLettersEndPosition(Vector3 targetPos, ICollection letters, float letterSpacing)
        {
            var positions = new Vector3[letters.Count];

            for (var i = 0; i < positions.Length; i++)
                positions[i] = targetPos + Vector3.right * (i * letterSpacing);

            var center = Vector3.Lerp(
                positions[0],
                positions[positions.Length - 1],
                0.5f);

            var centerOffset = positions[0] - center;

            for (var i = 0; i < positions.Length; i++)
                positions[i] += centerOffset;

            return positions;
        }

        private void ClearFlyingLetters()
        {
            foreach (var flyingLetter in flyingLetters)
                Destroy(flyingLetter);

            flyingLetters.Clear();
        }

        private void OnDestroy()
        {
            GameEvents.OnCheckSquare -= OnCheckSquare;
            GameEvents.OnDisableSquareSelection -= OnDisableSquareSelection;
            GameEvents.OnCorrectWord -= OnCorrectWord;

            StopAllCoroutines();
        }

        private void OnCheckSquare(string letter, Vector3 squareposition, int squareindex)
        {
            if (!shouldCreateFlyingLetters)
                return;

            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            var isHit = Physics.Raycast(ray, out var hit);

            if (!isHit)
            {
                Debug.LogError("raycast not hit on square check");
                return;
            }

            var flyingLetter = Instantiate(flyingLetterPrefab, squareposition, Quaternion.identity, transform);
            flyingLetters.Add(flyingLetter);

            var gridSquare = hit.transform.GetComponent<GridSquare>();
            var letterSprite = gridSquare.NormalLetterData.Sprite;

            flyingLetter.GetComponent<SpriteRenderer>().sprite = letterSprite;

            if (isCorrectWordFound)
                shouldCreateFlyingLetters = false;
        }
    }
}