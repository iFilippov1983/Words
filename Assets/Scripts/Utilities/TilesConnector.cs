using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TilesConnector : MonoBehaviour
{
    [SerializeField] private float _verticalOffsetValue;
    [SerializeField] private float _horizontalOffsetValue;
    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;
    private Dictionary<NeighbourType, bool> _existingNeighbours;

    private void Awake()
    {
        _existingNeighbours = GetExistingNeighbours();
        _propBlock = new MaterialPropertyBlock();
        _renderer = GetComponent<Renderer>();

        SetRenderer();
    }

    private void SetRenderer()
    {
        _renderer.GetPropertyBlock(_propBlock);
        foreach (var pair in _existingNeighbours)
        {
            float value = pair.Value
                ? 1f
                : 0f;
            _propBlock.SetFloat(pair.Key.ToString(), value);
        }
        _renderer.SetPropertyBlock(_propBlock);
    }

    private Dictionary<NeighbourType, bool> GetExistingNeighbours()
    {
        var raysDictionary = SetRaysFrom(transform.position);
        foreach (var ray in raysDictionary)
        {
            bool exists = TryFindNeighbourOn(ray.Value);
            _existingNeighbours.Add(ray.Key, exists);
        }
        return _existingNeighbours;
    }

    private bool TryFindNeighbourOn(Ray ray)
    {
        bool hasHit = Physics.Raycast(ray, out RaycastHit hit, _verticalOffsetValue * 1.1f);
        if (hasHit)
        { 
            bool hasSameTag = hit.collider.tag.Equals(gameObject.tag);
            return hasSameTag;
        }

        return hasHit;
    }

    private Dictionary<NeighbourType, Ray> SetRaysFrom(Vector3 origin)
    {
        Dictionary<NeighbourType, Ray> raysDictionary = new Dictionary<NeighbourType, Ray>();
        Vector3 offsetedOrigin;

        offsetedOrigin = new Vector3
            (
            0f, 
            origin.y + _verticalOffsetValue, 
            origin.z + _horizontalOffsetValue
            );
        Ray rayUp = new Ray(offsetedOrigin, Vector3.down);
        raysDictionary.Add(NeighbourType.Up, rayUp);

        offsetedOrigin = new Vector3
            (
            0f, 
            origin.y + _verticalOffsetValue, 
            origin.z -_horizontalOffsetValue
            );
        Ray rayDown = new Ray(offsetedOrigin, Vector3.down);
        raysDictionary.Add(NeighbourType.Down, rayDown);

        offsetedOrigin = new Vector3
            (
            origin.x -_horizontalOffsetValue, 
            origin.y + _verticalOffsetValue, 
            0f
            );
        Ray rayLeft = new Ray(offsetedOrigin, Vector3.down);
        raysDictionary.Add(NeighbourType.Left, rayLeft);

        offsetedOrigin = new Vector3
            (
            origin.x + _horizontalOffsetValue, 
            origin.y + _verticalOffsetValue, 
            0f
            );
        Ray rayRight = new Ray(offsetedOrigin, Vector3.down);
        raysDictionary.Add(NeighbourType.Right, rayRight);

        offsetedOrigin = new Vector3
            (
            origin.x - _horizontalOffsetValue, 
            origin.y + _verticalOffsetValue, 
            origin.z + _horizontalOffsetValue
            );
        Ray rayDiagLeftUp = new Ray(offsetedOrigin, Vector3.down);
        raysDictionary.Add(NeighbourType.DiagLeftUp, rayDiagLeftUp);

        offsetedOrigin = new Vector3
            (
            origin.x - _horizontalOffsetValue,
            origin.y + _verticalOffsetValue,
            origin.z - _horizontalOffsetValue
            );
        Ray rayDiagLeftDown = new Ray(offsetedOrigin, Vector3.down);
        raysDictionary.Add(NeighbourType.DiagLeftDown, rayDiagLeftDown);

        offsetedOrigin = new Vector3
            (
            origin.x + _horizontalOffsetValue,
            origin.y + _verticalOffsetValue,
            origin.z + _horizontalOffsetValue
            );
        Ray rayDiagRightUp = new Ray(offsetedOrigin, Vector3.down);
        raysDictionary.Add(NeighbourType.DiagRightUp, rayDiagRightUp);

        offsetedOrigin = new Vector3
            (
            origin.x + _horizontalOffsetValue,
            origin.y + _verticalOffsetValue,
            origin.z - _horizontalOffsetValue
            );
        Ray rayDiagRightDown = new Ray(offsetedOrigin, Vector3.down);
        raysDictionary.Add(NeighbourType.DiagRightDown, rayDiagRightDown);

        return raysDictionary;
    }
}

public enum NeighbourType
{ 
    Up,
    Down,
    Left,
    Right,
    DiagLeftUp,
    DiagRightUp,
    DiagLeftDown,
    DiagRightDown
}
