using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class WordAnimation: MonoBehaviour
    {
        public static Action<SearchingWord> LetterReachedSearchingWord;
        [SerializeField] private float moveInterval;
        [SerializeField] private float firstLetterFlySpeed;
        [SerializeField] private float letterFollowSpeed;
        [SerializeField] private float endScaleSpeed;
        [SerializeField] private AnimationCurve scaleCurve;
        [SerializeField] private AnimationCurve timeCurve;
        private WaitForSeconds moveStartInterval;

        private void Start()
        {
            moveStartInterval = new WaitForSeconds(moveInterval);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        public void Play(List<GameObject> letters, SearchingWord searchingWord)
        {
            StartCoroutine(StartAnimation(letters, searchingWord));
        }

        private IEnumerator StartAnimation(List<GameObject> letters,  SearchingWord searchingWord)
        {
            Transform moveTarget = null;

            foreach (var letter in letters)
                letter.SetActive(true);

            foreach (var letter in letters)
            {
                StartCoroutine(MoveLetter(letter.transform, moveTarget, searchingWord));

                moveTarget = letter.transform;

                yield return moveStartInterval;
            }
        }

        private IEnumerator MoveLetter(Transform letter, Transform targetLetter,SearchingWord searchingWord)
        {
            if (targetLetter == null)
                yield return MoveLetterToPosition(letter, searchingWord.transform.position);
            else
                yield return MoveLetterToTarget(letter, targetLetter);

            yield return HideLetter(letter);

            LetterReachedSearchingWord?.Invoke(searchingWord);
            GameEvents.WordGetTargetMethod(searchingWord.Word);

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
    }
}