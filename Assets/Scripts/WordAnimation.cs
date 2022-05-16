using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class WordAnimation : MonoBehaviour
    {
        public static Action<SearchingWord> LetterReachedSearchingWord;
        [SerializeField] private Transform taskWordsParent;
        [SerializeField] private LetterFactory letterFactory;
        [Space] [SerializeField] private float moveInterval;
        [SerializeField] private float firstLetterFlySpeed;
        [SerializeField] private float letterFollowSpeed;
        [SerializeField] private float endScaleSpeed;
        [SerializeField] private AnimationCurve scaleCurve;
        [SerializeField] private AnimationCurve timeCurve;
        private readonly List<GameObject> letters = new List<GameObject>();
        private readonly List<SearchingWord> taskWords = new List<SearchingWord>();
        private Camera _camera;
        private string correctWord;
        private bool canCreateLetters = true;
        private bool isCorrectWordFound => !string.IsNullOrEmpty(correctWord);
        private WaitForSeconds moveStartInterval;
        private IEnumerator Start()
        {
            _camera = Camera.main;

            GameEvents.OnCheckSquare += OnCheckSquare;
            GameEvents.OnEnableSquareSelection += OnEnableSquareSelection;
            GameEvents.OnDisableSquareSelection += OnDisableSquareSelection;
            GameEvents.OnCorrectWord += OnCorrectWord;

            moveStartInterval = new WaitForSeconds(moveInterval);

            yield return new WaitWhile(() => taskWordsParent.childCount == 0);

            taskWordsParent.GetComponentsInChildren(taskWords);
        }

        private void OnDestroy()
        {
            GameEvents.OnCheckSquare -= OnCheckSquare;
            GameEvents.OnEnableSquareSelection -= OnEnableSquareSelection;
            GameEvents.OnDisableSquareSelection -= OnDisableSquareSelection;
            GameEvents.OnCorrectWord -= OnCorrectWord;

            StopAllCoroutines();
        }

        private void OnEnableSquareSelection()
        {
            foreach (var flyingLetter in letters)
                Destroy(flyingLetter);

            letters.Clear();

            correctWord = string.Empty;
            canCreateLetters = true;
        }

        private void OnCorrectWord(string word, List<int> squareindexes)
        {
            correctWord = word;
        }

        private void OnDisableSquareSelection()
        {
            if (!isCorrectWordFound)
                return;
            
            StartCoroutine(MoveLettersWithInterval(moveStartInterval));
        }

        private IEnumerator MoveLettersWithInterval(WaitForSeconds interval)
        {
            Transform moveTarget = null;
            var word = correctWord;

            foreach (var letter in letters)
                letter.SetActive(true);

            while (letters.Count > 0)
            {
                var letter = letters[0];
                
                letters.Remove(letter);

                StartCoroutine(MoveLetter(letter.transform, moveTarget, moveTarget == null, word));

                moveTarget = letter.transform;

                yield return interval;
            }
        }

        private SearchingWord GetSearchingWord(string word)
        {
            var taskWord = taskWords.Find(x => string.Equals(x.displayedText.text.ToLower(), word.ToLower()));

            if (taskWord != null)
                return taskWord;

            Debug.LogError("task word not found");
            return null;
        }

        private IEnumerator MoveLetter(Transform letter, Transform target, bool isMovingToSearchedWord, string word)
        {
            var searchingWord = GetSearchingWord(word);

            if (isMovingToSearchedWord)
                yield return MoveLetterToPosition(letter, searchingWord.transform.position);
            else
                yield return MoveLetterToTarget(letter, target);

            yield return HideLetter(letter);

            LetterReachedSearchingWord?.Invoke(searchingWord);
            GameEvents.WordGetTargetMethod(word);

            Destroy(letter.gameObject);
        }

        private IEnumerator MoveLetterToTarget(Transform letter, Transform target)
        {
            var lastTargetPos = target.position;
            var travelPercent = 0f;

            while (Vector3.Distance(letter.position, lastTargetPos) > 0.05f)
            {
                letter.position = Vector3.Lerp(
                    letter.position,
                    lastTargetPos,
                    Time.deltaTime * (travelPercent * letterFollowSpeed));

                travelPercent += firstLetterFlySpeed * Time.deltaTime;

                yield return null;

                if (target != null)
                    lastTargetPos = target.position;
            }
        }

        private IEnumerator MoveLetterToPosition(Transform letter, Vector3 position)
        {
            var lettersStartPosition = letter.position;
            var travelPercent = 0f;

            while (travelPercent < 1)
            {
                letter.position = Vector3.Lerp(
                    lettersStartPosition,
                    position,
                    timeCurve.Evaluate(travelPercent));

                travelPercent += firstLetterFlySpeed * Time.deltaTime;

                yield return null;
            }
        }

        private IEnumerator HideLetter(Transform letter)
        {
            var percent = 0f;
            var defaultScale = letter.localScale;

            while (percent < 1f)
            {
                percent += endScaleSpeed * Time.deltaTime;
                letter.localScale = defaultScale * scaleCurve.Evaluate(percent);
                yield return null;
            }
        }

        private void OnCheckSquare(string letterValue, Vector3 squareposition, int squareindex)
        {
            if (!canCreateLetters)
                return;

            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            var isHit = Physics.Raycast(ray, out var hit);

            if (!isHit)
            {
                Debug.LogError("raycast not hit on square check");
                return;
            }
            
            var letter = letterFactory.Create(hit.transform.GetComponent<GridSquare>());
            letters.Add(letter);

            if (isCorrectWordFound)
                canCreateLetters = false;
        }
    }
}