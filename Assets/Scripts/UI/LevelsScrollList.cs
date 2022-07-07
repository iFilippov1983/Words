using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelsScrollList : MonoBehaviour
{
    [SerializeField] private DataProfile _dataProfile;
    [SerializeField] private GameLevelData _gameLevelData;
    [Space]
    [SerializeField] private GameObject _levelCellPrefab;
    [SerializeField] private VerticalLayoutGroup _listContent;
    [Space]
    [SerializeField] private int _initialCellsSpacing = -700;
    [SerializeField] private int _initialTopPadding = 600;
    [SerializeField] private int _paddingStep = -700;

    private List<LevelCell> _levelCells;
    private int _currentLevelNumber = 0;
    private int _scrollListSize = 0;

    private void Start()
    {
        Initialize();
        MakeScrollList();
    }

    private void Initialize()
    {
        _levelCells = new List<LevelCell>();
        _currentLevelNumber = DataSaver.LoadIntData(_dataProfile.ProgressKey);
        var padding = _initialTopPadding + _currentLevelNumber * _paddingStep;
        _listContent.padding.top = padding;
        _listContent.spacing = _initialCellsSpacing;
    }

    private void MakeScrollList()
    {
        foreach (var d in _gameLevelData.Data)
        {
            if (d.GameMode.Equals(_dataProfile.GameMode))
            { 
                _scrollListSize = d.BoardData.Count;
            }
        }

        Quaternion rotation = _listContent.GetComponent<RectTransform>().rotation;
        Transform parent = _listContent.gameObject.transform;
        _levelCellPrefab.GetComponent<RectTransform>().rotation = rotation;
        if (_scrollListSize != 0)
        {
            for (int i = _scrollListSize; i > 0; i--)
            {
                GameObject cellObj = Instantiate(_levelCellPrefab, parent);
                var cell = cellObj.GetComponent<LevelCell>();
                cell.LevelNumber.text = i.ToString();
                if (i == 1)
                    cell.ToLevelLine.gameObject.SetActive(false);
            }
        }
    }
}
