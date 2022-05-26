using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class WordConnectionLine : MonoBehaviour
    {
        [SerializeField] private DataProfile _dataProfile;
        [SerializeField] private GameObject linePrefab;
        [SerializeField] private float distanceToCamera;
        [SerializeField] private float _maxLineLength = 2f;
        [SerializeField] private Vector3 selectedPointOffset;
        private Vector3 _lastAnchorPosition;
        private int _lastSquareIndex;
        private readonly Stack<LineRenderer> despawnedLines = new Stack<LineRenderer>();
        private readonly Stack<LineRenderer> lines = new Stack<LineRenderer>();
        private readonly List<int> anchorsIndexes = new List<int>();
       
        private Camera _camera;
        private bool canUpdate;
        private LineRenderer lastLine => lines.Peek();
        private int lastPointIndex => lastLine.positionCount - 1;
        private bool _canDrawLine => !_dataProfile.MousePositionIsFar;

        private void Start()
        {
            _camera = Camera.main;

            ResetLinePositionCount();

            GameEvents.OnCheckSquare += OnSelectSquare;
        }

        private void OnDestroy()
        {
            GameEvents.OnCheckSquare -= OnSelectSquare;
        }

        private void OnSelectSquare(string letter, Vector3 position, int index)
        {
            bool desactivate = anchorsIndexes.Count >= 2 && anchorsIndexes[anchorsIndexes.Count - 2].Equals(index);
            if (desactivate)
                DesactivatePreviousLine();
            else if (anchorsIndexes.Contains(index))
                return;
            else
                anchorsIndexes.Add(index);
                
            _lastAnchorPosition = position + selectedPointOffset;

            // on first selection
            if (canUpdate == false)
                AddLinePosition(_lastAnchorPosition);

            canUpdate = true;

            if (_canDrawLine)
            {
                lastLine.SetPosition(lastPointIndex, _lastAnchorPosition);

                if (desactivate == false)
                    AddLinePosition(_lastAnchorPosition);
            }
        }

        private void DesactivatePreviousLine()
        {
            lastLine.positionCount = 0;
            lastLine.gameObject.SetActive(false);
            despawnedLines.Push(lines.Pop());

            anchorsIndexes.Remove(anchorsIndexes[anchorsIndexes.Count - 1]);
        }

        private void Update()
        {
            if (!canUpdate)
                return;

            if (Input.GetMouseButton(0))
            {
                DrawLine();
            }

            if (Input.GetMouseButtonUp(0))
            {
                canUpdate = false;
                ResetLinePositionCount();
            }
        }

        private void DrawLine()
        {
            var mousePosition = Input.mousePosition;
            mousePosition.z = distanceToCamera;

            var lineEndPosition = _camera.ScreenToWorldPoint(mousePosition);
            lineEndPosition.z = 0f;

            float length = Vector3.Distance(_lastAnchorPosition, lineEndPosition);
            
            if (length > _maxLineLength)
            {
                _dataProfile.MousePositionIsFar = true;
                
                GameEvents.ClearSelectionMethod();
                GameEvents.DisableAllSquaresSelectionMethod();
                canUpdate = false;
                ResetLinePositionCount();
            }
            else
                _dataProfile.MousePositionIsFar = false;

            if (lines.Count != 0)
                lastLine.SetPosition(lastPointIndex, lineEndPosition);
        }

        private void CheckLineLength(ref Vector3 endPosition)
        { 
            float length = Vector3.Distance(_lastAnchorPosition, endPosition);

            if (length > _maxLineLength)
            {
                _dataProfile.MousePositionIsFar = true;

                GameEvents.ClearSelectionMethod();
                GameEvents.DisableAllSquaresSelectionMethod();
                canUpdate = false;
                ResetLinePositionCount();
            }
            else
                _dataProfile.MousePositionIsFar = false;
        }

        private void ResetLinePositionCount()
        {
            foreach (var line in despawnedLines)
                line.positionCount = 0;

            foreach (var line in lines)
            {
                line.positionCount = 0;
                line.gameObject.SetActive(false);
                
                despawnedLines.Push(line);
            }

            anchorsIndexes.Clear();
            lines.Clear();
            _dataProfile.MousePositionIsFar = false;
        }

        private void AddLinePosition(Vector3 position)
        {
            var line = despawnedLines.Count == 0
                ? Instantiate(linePrefab, transform).GetComponent<LineRenderer>()
                : despawnedLines.Pop();
            
            line.gameObject.SetActive(true);
            line.positionCount = 2;

            for (var i = 0; i < line.positionCount; i++)
                line.SetPosition(i, position);
            
            lines.Push(line);
        }
    }
}