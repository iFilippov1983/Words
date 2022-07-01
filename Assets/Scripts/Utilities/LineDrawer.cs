using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    public Vector3 worldPosition;

    [SerializeField] private LineRenderer _lineRenderer;

    private Vector3 _startPoint;
    private bool _lineNotDrawed;

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
    }
}
