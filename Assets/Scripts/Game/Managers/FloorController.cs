using UnityEngine;

public class FloorController : MonoBehaviour
{
    [SerializeField] private GameData _currentGameData;
    [SerializeField] private int firstBoardExpandLevel = 10;
    [SerializeField] private int secondBoardExpandLevel = 20;

    public void SetPosition()
    {
        //var categoryName = _currentGameData.selectedCategoryName;
        var gameMode = _currentGameData.selectedGameMode.ToString();
        var currentBoardIndex = DataSaver.LoadIntData(gameMode);
        bool moveOnes = currentBoardIndex >= firstBoardExpandLevel - 1;
        bool moveTwice = currentBoardIndex >= secondBoardExpandLevel - 1;

        float yOffset = 0;
        if (moveOnes) yOffset = 1f;
        if (moveTwice) yOffset = 2f;

        Vector3 offsetVector = new Vector3(0f, yOffset, 0f);

        transform.position -= offsetVector;
    }
}
