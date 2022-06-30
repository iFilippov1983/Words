using UnityEngine;

public class GameDataSelector : MonoBehaviour
{
    public GameData currentGameData;
    public GameLevelData levelData;

    void Awake()
    {
        SelectSequentialBoardData();
    }

    private void SelectSequentialBoardData()
    {
        //foreach (var data in levelData.Data)
        //{
        //    if (data.CategoryName.Equals(currentGameData.selectedCategoryName))
        //    {
        //        int boardIndex = DataSaver.LoadIntData(currentGameData.selectedCategoryName);

        //        if (boardIndex < data.BoardData.Count)
        //        {
        //            currentGameData.selectedBoardData = data.BoardData[boardIndex];
        //        }
        //        else
        //        { 
        //            var randomIndex = Random.Range(0, data.BoardData.Count);
        //            currentGameData.selectedBoardData = data.BoardData[randomIndex];
        //        }
        //    }
        //}

        foreach (var data in levelData.Data)
        {
            if (data.GameMode.Equals(currentGameData.selectedGameMode))
            {
                int boardIndex = DataSaver.LoadIntData(currentGameData.selectedGameMode.ToString());


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
