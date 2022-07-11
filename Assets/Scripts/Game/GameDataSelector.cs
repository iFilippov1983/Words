using UnityEngine;

public class GameDataSelector : MonoBehaviour
{
    public GameData currentGameData;
    public GameLevelData levelData;
    public DataProfile dataProfile;

    void Awake()
    {
        SelectSequentialBoardData();
    }

    private void SelectSequentialBoardData()
    {
        foreach (var data in levelData.Data)
        {
            if (data.GameMode.Equals(currentGameData.selectedGameMode))
            {
                int boardIndex = DataSaver.LoadIntData(DataKey.ProgressKey);

                if (boardIndex < data.BoardData.Count)
                {
                    currentGameData.selectedBoardData = data.BoardData[boardIndex];
                }
                else
                {
                    var randomIndex = Random.Range(0, data.BoardData.Count);
                    currentGameData.selectedBoardData = data.BoardData[randomIndex];
                }
            }
        }
    }
}
