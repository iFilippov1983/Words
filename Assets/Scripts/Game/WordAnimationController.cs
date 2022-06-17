using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public class WordAnimationController : MonoBehaviour
    {
        [SerializeField] private WordAnimation wordAnimation;
        [SerializeField] private LetterFactory letterFactory;
        [SerializeField] private Transform taskWordsParent;
        private readonly List<GameObject> letters = new List<GameObject>();
        private readonly List<SearchingWord> taskWords = new List<SearchingWord>();
        private readonly List<int> _squareIndexes = new List<int>();
        private Camera _camera;
        private string correctWord;
        private bool canCreateLetters = true;
        private bool isCorrectWordFound => !string.IsNullOrEmpty(correctWord);
        
        private IEnumerator Start()
        {
            _camera = Camera.main;

            GameEvents.OnCheckSquare += OnCheckSquare;
            GameEvents.OnUnselectSquare += OnUnselectSquare;
            GameEvents.OnEnableSquareSelection += OnEnableSquareSelection;
            GameEvents.OnDisableSquareSelection += OnDisableSquareSelection;
            GameEvents.OnCorrectWord += OnCorrectWord;
            
            yield return new WaitWhile(() => taskWordsParent.childCount == 0);

            taskWordsParent.GetComponentsInChildren(taskWords);
        }

        private void OnDestroy()
        {
            GameEvents.OnCheckSquare -= OnCheckSquare;
            GameEvents.OnUnselectSquare -= OnUnselectSquare;
            GameEvents.OnEnableSquareSelection -= OnEnableSquareSelection;
            GameEvents.OnDisableSquareSelection -= OnDisableSquareSelection;
            GameEvents.OnCorrectWord -= OnCorrectWord;

            StopAllCoroutines();
        }

        private void OnEnableSquareSelection()
        {
            letters.Clear();
            _squareIndexes.Clear();
            canCreateLetters = true;
        }

        private void OnCorrectWord(string word, List<int> squareindexes)
        {
            correctWord = word;
        }

        private void OnDisableSquareSelection()
        {
            if (!isCorrectWordFound)
            {
                foreach (var letter in letters)
                    Destroy(letter.gameObject);

                return;
            }

            wordAnimation.Play(new List<GameObject>(letters), GetSearchingWord(correctWord));

            correctWord = string.Empty;
            _squareIndexes.Clear();
        }
        
        private SearchingWord GetSearchingWord(string word)
        {
            var taskWord = taskWords.Find(x => string.Equals(x.displayedText.text.ToLower(), word.ToLower()));

            if (taskWord != null)
                return taskWord;

            Debug.LogError("task word not found");
            return null;
        }
        
        private void OnCheckSquare(string letterValue, Vector3 squareposition, int squareIndex)
        {
            if (!canCreateLetters || _squareIndexes.Contains(squareIndex))
                return;

            _squareIndexes.Add(squareIndex);
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

        private void OnUnselectSquare(string letter, Vector3 squarePosition, int squareIndex)
        {
            if (_squareIndexes.Contains(squareIndex))
            {
                letters.Remove(letters[letters.Count - 1]);
                _squareIndexes.Remove(squareIndex);
            }
        }
    }
}