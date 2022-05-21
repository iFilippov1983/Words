using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class WordConnectionLine : MonoBehaviour
    {
        [SerializeField] private GameObject linePrefab;
        [SerializeField] private float distanceToCamera;
        [SerializeField] private float _maxLineLength = 2f;
        [SerializeField] private Vector3 selectedPointOffset;
        private Vector3 _lastAnchorPosition;
        private readonly Stack<LineRenderer> despawnedLines = new Stack<LineRenderer>();
        private readonly Stack<LineRenderer> lines = new Stack<LineRenderer>();
       
        private Camera _camera;
        private bool canUpdate;
        private LineRenderer lastLine => lines.Peek();
        private int lastPointIndex => lastLine.positionCount - 1;
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
            _lastAnchorPosition = position + selectedPointOffset;

            // on first selection
            if (canUpdate == false)
                AddLinePosition(_lastAnchorPosition);

            canUpdate = true;

            lastLine.SetPosition(lastPointIndex, _lastAnchorPosition);
            AddLinePosition(_lastAnchorPosition);
        }

        private void Update()
        {
            if (!canUpdate)
                return;

            if (Input.GetMouseButton(0))
            {
                var mousePosition = Input.mousePosition;
                mousePosition.z = distanceToCamera;

                var lineEndPosition = _camera.ScreenToWorldPoint(mousePosition);
                lastLine.SetPosition(lastPointIndex, lineEndPosition);

                lineEndPosition.z = 0f;
                CheckLineLength(lineEndPosition);
            }

            if (Input.GetMouseButtonUp(0))
            {
                canUpdate = false;
                ResetLinePositionCount();
            }
        }

        private void CheckLineLength(Vector3 endPosition)
        { 
            float length = Vector3.Distance(_lastAnchorPosition, endPosition);

            if (length > _maxLineLength)
            {
                GameEvents.ClearSelectionMethod();
                GameEvents.DisableSquareSelectionMethod();
                canUpdate = false;
                ResetLinePositionCount();
            }
        }

        private void ResetLinePositionCount()
        {
            foreach (var line in lines)
            {
                line.positionCount = 0;
                line.gameObject.SetActive(false);
                
                despawnedLines.Push(line);
            }
            
            lines.Clear();
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