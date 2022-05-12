using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    public Vector3 worldPosition;
    [SerializeField] private LineRenderer _lineRenderer;
    private Vector3 _startPoint;
    private Vector3 _anchorPoint;
    private Vector3 _finishPoint;
    private bool _isMouseDown;
    private GridSquare _currentSquare;

    private bool _lineNotDrawed;

    Plane plane = new Plane(Vector3.up, 0);

    private void OnEnable()
    {
        _lineRenderer.positionCount = 0;
        _lineNotDrawed = true;

        GameEvents.OnSelectSquare += DrawLine;
        GameEvents.OnEnableSquareSelection += SetStartPoint;
        GameEvents.OnClearSelection += EraseLine;
    }

    private void OnDisable()
    {
        GameEvents.OnSelectSquare -= DrawLine;
        GameEvents.OnEnableSquareSelection -= SetStartPoint;
        GameEvents.OnClearSelection -= EraseLine;
    }

    private void DrawLine(Vector3 squarePosition)
    {
        if (_lineNotDrawed)
        {
            _lineRenderer.positionCount = 2;
            if(_startPoint == null)
                _startPoint = transform.position;
            _lineRenderer.SetPosition(0, new Vector3(_startPoint.x, _startPoint.y, 0f));
            _lineRenderer.SetPosition(1, new Vector3(squarePosition.x, squarePosition.y, 0f));
            _lineNotDrawed = false;
        }
    }

    private void EraseLine()
    {
        if (!_lineNotDrawed)
        {
            _lineRenderer.positionCount = 0;
            _lineNotDrawed = true;
        }
    }

    private void SetStartPoint()
    {
        if(_startPoint == null && _lineNotDrawed)
            _startPoint = transform.position;

        Debug.Log("Set start");
    }

    //private void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        RaycastHit hit;

    //        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
    //        {
    //            print(hit.collider.gameObject.name);
    //            if (hit.collider.gameObject.TryGetComponent(out GridSquare square))
    //            {
    //                _currentSquare = square;
    //                _startPoint = square.transform.position;
    //                _isMouseDown = true;

    //            }
    //            else
    //            {
    //                _isMouseDown = false;
    //            }
    //        }
    //    }
    //    if (Input.GetMouseButton(0) && _isMouseDown)
    //    {
    //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        RaycastHit hitData;
    //        if (Physics.Raycast(ray, out hitData, Mathf.Infinity))
    //        {
    //            worldPosition = hitData.point;
    //            _anchorPoint = hitData.transform.position;
    //        }
    //        _lineRenderer.SetPosition(0, new Vector3(_startPoint.x, _startPoint.y, 0f));
    //        _lineRenderer.SetPosition(1, new Vector3(worldPosition.x, worldPosition.y, 0f));
    //    }


    //    if (Input.GetMouseButtonUp(0) && _isMouseDown)
    //    {
    //        float distance;
    //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        if (plane.Raycast(ray, out distance))
    //        {
    //            worldPosition = ray.GetPoint(distance);
    //        }

    //        _finishPoint = new Vector3(worldPosition.x, 1, worldPosition.z);

    //        _lineRenderer.SetPosition(0, new Vector3(0, 0f, 0));
    //        _lineRenderer.SetPosition(1, new Vector3(0, 0f, 0));


    //        //StartCalculation();

    //        //_currentSquare = new Ball();
    //        _isMouseDown = false;
    //    }
    //}


    //private void StartCalculation()
    //{
    //    var direction = _finishPoint - _startPoint;
    //    Debug.DrawRay(_finishPoint, direction, Color.black);

    //    float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

    //    var direct = 0;

    //    if ((angle >= -45 && angle <= 0) || (angle >= 0 && angle <= 45))
    //    {
    //        direct = 0;
    //    }
    //    else if ((angle >= 45 && angle <= 90) || (angle >= 90 && angle <= 135))
    //    {
    //        direct = 1;
    //    }
    //    else if ((angle >= 135 && angle <= 180) || (angle >= -180 && angle <= -135))
    //    {
    //        direct = 2;
    //    }
    //    else if ((angle >= -135 && angle <= -90) || (angle >= -90 && angle <= -45))
    //    {
    //        direct = 3;
    //    }
    //    //_currentSquare.MoveToDirection(direct);
    //}
}
