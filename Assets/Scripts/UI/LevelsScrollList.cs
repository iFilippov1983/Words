using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelsScrollList : MonoBehaviour
{
    private const int MinimumVisibleSquaresAmount = 7;

    [SerializeField] private DataProfile _dataProfile;
    [SerializeField] private GameLevelData _gameLevelData;
    [Space]
    [SerializeField] private GameObject _levelCellPrefab;
    [SerializeField] private VerticalLayoutGroup _listContent;
    [Space]
    [SerializeField] private int _initialCellsSpacing = -700;
    [SerializeField] private int _initialTopPadding = 600;
    [SerializeField] private int _paddingStep = -700;

    private Dictionary<int, BoardData> _boardDataDic;
    private int _currentLevelIndex = 0;
    private int _levelIndexInProgress = 0;

    private void Start()
    {
        SetData();
        MakeScrollList();
        AdjustLayoutGroup();
    }

    private void SetData()
    {
        _currentLevelIndex = DataSaver.LoadIntData(DataKey.ProgressKey);
        foreach (var d in _gameLevelData.Data)
        {
            if (d.GameMode.Equals(_dataProfile.GameMode))
            {
                _boardDataDic = MakeBoardDataDictionary(d.BoardData);
            }
        }
    }

    private Dictionary<int, BoardData> MakeBoardDataDictionary(List<BoardData> boardDataList)
    {
        int gameCyclesCount = DataSaver.LoadIntData(DataKey.CyclesCountKey);
        var dic = new Dictionary<int, BoardData>();

        _levelIndexInProgress = _currentLevelIndex 
            + boardDataList.Count * gameCyclesCount 
            - (_dataProfile.LevelNumberToCycleFrom - 1) * gameCyclesCount;

        bool timeToSpawnExtraCells = _currentLevelIndex > boardDataList.Count - MinimumVisibleSquaresAmount;
        if (timeToSpawnExtraCells)
            gameCyclesCount++;

        do
        {
            for (int i = boardDataList.Count; i > 0 ; i--)
            {
                int number = i + boardDataList.Count * gameCyclesCount;
                dic.Add(number, boardDataList[i - 1]);
            }
            gameCyclesCount--;
        } while (gameCyclesCount >= 0);

        return dic;
    }

    private void MakeScrollList()
    {
        Quaternion rotation = _listContent.GetComponent<RectTransform>().rotation;
        _levelCellPrefab.GetComponent<RectTransform>().rotation = rotation;

        Transform parent = _listContent.gameObject.transform;

        foreach (var pair in _boardDataDic)
        {
            GameObject cellObj = Instantiate(_levelCellPrefab, parent);
            cellObj.name = $"Level {pair.Key}";
            var cell = cellObj.GetComponent<LevelCell>();
            cell.LevelNumber.text = pair.Key.ToString();
            cell.IsHardImage.gameObject.SetActive(pair.Value.IsHard);
            cell.HasExtraRewardImage.gameObject.SetActive(pair.Value.HasExtraReward);

            if (pair.Key == _levelIndexInProgress + 1)
            {
                cell.LevelImage.color = cell.ColorToPlay;
                cell.ToLevelLine.color = cell.ColorCompleted;
            }
            else if (pair.Key < _levelIndexInProgress + 1)
            {
                cell.LevelImage.color = cell.ColorCompleted;
                cell.ToLevelLine.color = cell.ColorCompleted;
            }
            else
                cell.LevelImage.color = cell.ColorPassive;
            if (pair.Key == 1)
                cell.ToLevelLine.gameObject.SetActive(false);
        }
    }

    private void AdjustLayoutGroup()
    {
        var padding = _initialTopPadding + _levelIndexInProgress * _paddingStep;
        _listContent.padding.top = padding;
        _listContent.spacing = _initialCellsSpacing;
    }
}
