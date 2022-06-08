using System;
using System.Collections;
using System.Collections.Generic;
using Lofelt.NiceVibrations;
using UnityEngine;

namespace Game
{
    public class WordAnimation : MonoBehaviour
    {
        [SerializeField] private AnimationCurve timeCurve;
        [SerializeField] private AnimationCurve trajectoryCurve;
        [Space] [SerializeField] private float moveInterval;
        [SerializeField] private float endScalingDelay;
        [Space] [SerializeField] private float firstLetterFlySpeed;
        [SerializeField] private float letterFollowSpeed;
        [Space] [SerializeField] private float endScaleSpeed;
        [SerializeField] private AnimationCurve scaleCurve;
        [Space] [SerializeField] private float resizeSpeed;
        [SerializeField] private AnimationCurve resizeCurve;
        [SerializeField] private Vector3 endLetterSize;
        private WaitForSeconds endScalingDelayFirstWfs;
        private WaitForSeconds endScalingDelayWfs;
        private WaitForSeconds moveIntervalWfs;
        private static SearchingWord _searchingWord;
        private static bool _isWordAnimated;

        private void Start()
        {
            moveIntervalWfs = new WaitForSeconds(moveInterval);
            endScalingDelayWfs = new WaitForSeconds(endScalingDelay);
            endScalingDelayFirstWfs = new WaitForSeconds(endScalingDelay + 0.2f);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        public static event Action<SearchingWord> LetterReachedSearchingWord;
        public static event Action<GameObject> LetterReachedTarget;

        public void Play(List<GameObject> letters, SearchingWord searchingWord)
        {
            _isWordAnimated = true;
            _searchingWord = searchingWord;
            StartCoroutine(StartAnimation(letters, _searchingWord));
        }

        public void Play(GameObject objectToAnimate, GameObject target, Action animationFinishCallBack)
        {
            _isWordAnimated = false;
            StartCoroutine(StartAnimation(objectToAnimate, target, animationFinishCallBack));
        }

        private IEnumerator StartAnimation(List<GameObject> letters, SearchingWord searchingWord)
        {
            Transform moveTarget = null;

            foreach (var letter in letters)
                letter.SetActive(true);

            var endPositions = GetLettersEndPosition(searchingWord.transform.position,
                                                     letters.Count,
                                                     endLetterSize.x / 2f);
            
            var count = letters.Count;

            LetterReachedSearchingWord += OnLetterReachedSearchingWord;

            void OnLetterReachedSearchingWord(SearchingWord obj)
            {
                count--;

                if (count > 0)
                    return;

                StartCoroutine(ScaleLettersToZero(letters, searchingWord));
                LetterReachedSearchingWord -= OnLetterReachedSearchingWord;
            }

            for (var i = 0; i < letters.Count; i++)
            {
                var letter = letters[i];

                StartCoroutine(Animate(letter.transform, moveTarget, endPositions[i], searchingWord));

                moveTarget = letter.transform;

                yield return moveIntervalWfs;
            }
        }

        private IEnumerator StartAnimation(GameObject letter, GameObject target, Action animationFinishCallBack)
        {
            letter.SetActive(true);

            var endPosition = target.transform.position;

            LetterReachedTarget += OnLetterReachedTarget;

            void OnLetterReachedTarget(GameObject target)
            {
                StartCoroutine(ScaleLetterToZero(letter, target));
                animationFinishCallBack?.Invoke();
                LetterReachedTarget -= OnLetterReachedTarget;
            }

            StartCoroutine(Animate(letter.transform, endPosition, target));

            yield return null;
        }

        private IEnumerator ScaleLettersToZero(List<GameObject> letters, SearchingWord searchingWord)
        {
            foreach (var letter in letters)
            {
               yield return  ScaleToZero(letter.transform, endScaleSpeed, scaleCurve);

               GameEvents.WordGetTargetMethod(searchingWord.Word);
               Destroy(letter.gameObject);
            }
        }

        private IEnumerator ScaleLetterToZero(GameObject letter, GameObject target)
        {
            yield return ScaleToZero(letter.transform, endScaleSpeed, scaleCurve);

            Destroy(letter.gameObject);
        }

        private IEnumerator Animate(Transform letter,
                                    Transform targetLetter,
                                    Vector3 endPosition,
                                    SearchingWord searchingWord)
        {
            StartCoroutine(Rescale(letter, endLetterSize, resizeSpeed, resizeCurve));

            if (targetLetter == null)
            {
                yield return MoveToPosition(letter, endPosition, firstLetterFlySpeed, timeCurve);
                // yield return endScalingDelayFirstWfs;
            }
            else
            {
                yield return MoveToTarget(letter, targetLetter);
                yield return MoveToPosition(letter, endPosition, firstLetterFlySpeed * 6f);
                // yield return endScalingDelayWfs;
            }

            // yield return ScaleToZero(letter, endScaleSpeed, scaleCurve);

            LetterReachedSearchingWord?.Invoke(searchingWord);
          
        }

        private IEnumerator Animate(Transform letter,
                                    Vector3 endPosition,
                                    GameObject targetObject)
        {
            StartCoroutine(Rescale(letter, endLetterSize, resizeSpeed, resizeCurve));

            yield return MoveToPosition(letter, endPosition, firstLetterFlySpeed, timeCurve, trajectoryCurve);

            LetterReachedTarget?.Invoke(targetObject);
        }

        private IEnumerator MoveToTarget(Transform letter, Transform target)
        {
            var lastTargetPos = target.position;
            var travelPercent = 0f;
            var initialPercentOffset = 0.25f;

            while (Vector3.Distance(letter.position, lastTargetPos) > 0.85f || travelPercent < 0.8f)
            {
                var speed = Time.deltaTime *
                            (travelPercent * letterFollowSpeed *
                             timeCurve.Evaluate(travelPercent + initialPercentOffset));

                letter.position = Vector3.Lerp(letter.position, lastTargetPos, speed);

                travelPercent += firstLetterFlySpeed * Time.deltaTime;

                yield return null;

                if (target != null)
                    lastTargetPos = target.position;
            }
        }

        private static IEnumerator MoveToPosition
            (
            Transform transformToMove, 
            Vector3 endPosition, 
            float speed, 
            AnimationCurve timeCurve,
            AnimationCurve trajectoryCurve = null)
        {
            var startPosition = transformToMove.position;
            var travelPercent = 0f;

            while (travelPercent <= 1f)
            {
                Vector3 posVector = Vector3.Lerp(startPosition,
                                               endPosition,
                                               timeCurve.Evaluate(travelPercent));
                if (trajectoryCurve != null)
                {
                    posVector.x += trajectoryCurve.Evaluate(travelPercent);
                }
                    

                transformToMove.position = posVector;

                travelPercent += speed * Time.deltaTime;

                yield return null;
            }

            transformToMove.position = endPosition;

            if(_isWordAnimated)
                _searchingWord.PlayAnimation();
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
            SoundManager.PalaySound(Sound.Word_InCell);
            Debug.Log("[Haptic + Sound] WordAnimation - Animate[MoveToPosition]");
        }

        private static IEnumerator MoveToPosition(Transform target, Vector3 position, float speed)
        {
            var lettersStartPosition = target.position;
            var travelPercent = 0f;

            while (travelPercent <= 1)
            {
                target.position = Vector3.Lerp(lettersStartPosition,
                                               position,
                                               travelPercent);

                travelPercent += speed * Time.deltaTime;

                yield return null;
            }

            target.position = position;

            if(_isWordAnimated)
                _searchingWord.PlayAnimation();
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
            SoundManager.PalaySound(Sound.Word_InCell);
            Debug.Log("[Haptic + Sound] WordAnimation - Animate[MoveToPosition 2]");
        }

        private static IEnumerator Rescale(Transform target, Vector3 targetScale, float speed, AnimationCurve curve)
        {
            var percent = 0f;
            var startScale = target.localScale;

            while (percent < 1f)
            {
                percent += speed * Time.deltaTime;
                target.localScale = Vector3.Lerp(startScale, targetScale, curve.Evaluate(percent));
                yield return null;
            }
        }

        private static IEnumerator ScaleToZero(Transform letter, float speed, AnimationCurve curve)
        {
            var percent = 0f;
            var startScale = letter.localScale;

            while (percent <= 1f)
            {
                percent += speed * Time.deltaTime;
                letter.localScale = startScale * curve.Evaluate(percent);
                yield return null;
            }
        }

        private static Vector3[] GetLettersEndPosition(Vector3 endPosition, int letterCount, float letterSpacing)
        {
            var positions = new Vector3[letterCount];

            for (var i = 0; i < positions.Length; i++)
                positions[i] = endPosition + Vector3.right * (i * letterSpacing);

            var center = Vector3.Lerp(positions[0],
                                      positions[positions.Length - 1],
                                      0.5f);

            var centerOffset = positions[0] - center;

            for (var i = 0; i < positions.Length; i++)
                positions[i] += centerOffset;

            return positions;
        }
    }
}