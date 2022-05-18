using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class WordConnectionLine : MonoBehaviour
    {
        [SerializeField] private float distanceToCamera;
        [SerializeField] private LineRenderer _lineRenderer;
        private Camera _camera;
        private bool canUpdate;

        private int lineEndPositionIndex => _lineRenderer.positionCount - 1;

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
            if (canUpdate == false)
                AddLinePosition(position);
            
            canUpdate = true;

            if (_lineRenderer.positionCount > 0)
                _lineRenderer.SetPosition(lineEndPositionIndex, position);

            AddLinePosition(position);
        }

        private void Update()
        {
            if (!canUpdate)
                return;
            
            if (Input.GetMouseButton(0))
            {
                var mousePos = Input.mousePosition;
                mousePos.z = distanceToCamera;

                var lineEndPos = _camera.ScreenToWorldPoint(mousePos);
                _lineRenderer.SetPosition(lineEndPositionIndex, lineEndPos);
            }

            if (Input.GetMouseButtonUp(0))
            {
                canUpdate = false;
                ResetLinePositionCount();
            }
        }

        private void ResetLinePositionCount()
        {
            _lineRenderer.positionCount = 0;
        }

        private void AddLinePosition(Vector3 position)
        {
            _lineRenderer.positionCount += 1;
            _lineRenderer.SetPosition(lineEndPositionIndex, position);
        }
    }
}