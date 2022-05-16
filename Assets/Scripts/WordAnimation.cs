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
        [SerializeField] private GameObject flyingLetterPrefab;
        [Space] [SerializeField] private float flySpeed;
        [SerializeField] private float moveInterval;
        [SerializeField] private float scaleSpeed;
        [SerializeField] private AnimationCurve scaleCurve;
        [SerializeField] private AnimationCurve timeCurve;
        private readonly List<GameObject> letters = new List<GameObject>();
        private readonly List<SearchingWord> taskWords = new List<SearchingWord>();
        private Camera _camera;
        private string correctWord;
        private bool isCorrectWordFound;
        private bool shouldCreateFlyingLetters = true;

        private IEnumerator Start()
        {
            _camera = Camera.main;

            GameEvents.OnCheckSquare += OnCheckSquare;
            GameEvents.OnEnableSquareSelection += OnEnableSquareSelection;
            GameEvents.OnDisableSquareSelection += OnDisableSquareSelection;
            GameEvents.OnCorrectWord += OnCorrectWord;

            yield return new WaitWhile(() => taskWordsParent.childCount == 0);

            taskWordsParent.GetComponentsInChildren(taskWords);
        }

        private void OnEnableSquareSelection()
        {
            shouldCreateFlyingLetters = true;
            ClearFlyingLetters();
        }

        private void OnDestroy()
        {
            GameEvents.OnCheckSquare -= OnCheckSquare;
            GameEvents.OnEnableSquareSelection -= OnEnableSquareSelection;
            GameEvents.OnDisableSquareSelection -= OnDisableSquareSelection;
            GameEvents.OnCorrectWord -= OnCorrectWord;

            StopAllCoroutines();
        }

        private void OnCorrectWord(string word, List<int> squareindexes)
        {
            correctWord = word;
            isCorrectWordFound = true;
        }

        private void OnDisableSquareSelection()
        {
            if (!isCorrectWordFound)
                return;

            isCorrectWordFound = false;

            StartCoroutine(MoveLettersWithInterval(moveInterval));
        }

        private IEnumerator MoveLettersWithInterval(float interval)
        {
            Transform moveTarget = null;

            while (letters.Count > 0)
            {
                var letter = letters[0];

                letter.SetActive(true);
                letters.Remove(letter);

                StartCoroutine(MoveLetter(letter.transform, moveTarget, isMovingToSearchedWord: moveTarget == null));

                moveTarget = letter.transform;

                yield return new WaitForSeconds(interval);
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

        private IEnumerator MoveLetter(Transform letter, Transform target,bool isMovingToSearchedWord)
        {
            var searchingWord = GetSearchingWord(correctWord);

            if (isMovingToSearchedWord)
                yield return MoveLetterToPosition(letter, searchingWord.transform.position);
            else
                yield return MoveLetterToTarget(letter, target);

            yield return HideLetter(letter);

            LetterReachedSearchingWord?.Invoke(searchingWord);
            GameEvents.WordGetTargetMethod(correctWord);
            
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
                    Time.deltaTime * (travelPercent * 14f));

                travelPercent += flySpeed * Time.deltaTime;

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
                
                travelPercent += flySpeed * Time.deltaTime;

                yield return null;
            }
        }
        
        private IEnumerator HideLetter(Transform letter)
        {
            var percent = 0f;
            var defaultScale = letter.localScale;

            while (percent < 1f)
            {
                percent += scaleSpeed * Time.deltaTime;
                letter.localScale = defaultScale * scaleCurve.Evaluate(percent);
                yield return null;
            }
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
            foreach (var flyingLetter in letters)
                Destroy(flyingLetter);

            letters.Clear();
        }

        private void OnCheckSquare(string letterValue, Vector3 squareposition, int squareindex)
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

            var letter = Instantiate(flyingLetterPrefab, squareposition, Quaternion.identity, transform);
            letters.Add(letter);

            var gridSquare = hit.transform.GetComponent<GridSquare>();
            var letterSprite = gridSquare.NormalLetterData.Sprite;

            letter.GetComponent<SpriteRenderer>().sprite = letterSprite;

            if (isCorrectWordFound)
                shouldCreateFlyingLetters = false;
        }
    }
}