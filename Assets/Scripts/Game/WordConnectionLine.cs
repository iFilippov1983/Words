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
        private int _lastSquareIndex;
        private readonly Stack<LineRenderer> despawnedLines = new Stack<LineRenderer>();
        private readonly Stack<LineRenderer> lines = new Stack<LineRenderer>();
        private readonly List<int> anchorsIndexes = new List<int>();
       
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
            Debug.Log($"Enter: {index}");

            bool desactivate = anchorsIndexes.Count >= 2 && anchorsIndexes[anchorsIndexes.Count - 2].Equals(index);
            if (desactivate)
                DesactivatePreviousLine();
            else
            {
                anchorsIndexes.Add(index);
                Debug.Log($"Last: {anchorsIndexes[anchorsIndexes.Count - 1]}");
            }
                
            _lastAnchorPosition = position + selectedPointOffset;

            // on first selection
            if (canUpdate == false)
                AddLinePosition(_lastAnchorPosition);

            canUpdate = true;

            if(anchorsIndexes.Contains(index) == false)
                lastLine.SetPosition(lastPointIndex, _lastAnchorPosition);

            if(desactivate == false)
                AddLinePosition(_lastAnchorPosition);
        }

        private void DesactivatePreviousLine()
        {
            lastLine.positionCount = 0;
            lastLine.gameObject.SetActive(false);
            despawnedLines.Push(lines.Pop());

            bool removed = anchorsIndexes.Remove(anchorsIndexes[anchorsIndexes.Count - 1]);

            Debug.Log($"Removed: {removed} - Last: {anchorsIndexes[anchorsIndexes.Count - 1]}");
            //Debug.Log($"Despawned: {despawnedLines.Count}");
            //Debug.Log("Active: " + lines.Count);
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
                GameEvents.DisableAllSquaresSelectionMethod();
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