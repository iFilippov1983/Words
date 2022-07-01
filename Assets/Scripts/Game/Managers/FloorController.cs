using UnityEngine;

public class FloorController : MonoBehaviour
{
    [SerializeField] private DataProfile _dataProfile;
    [SerializeField] private int firstBoardExpandLevel = 10;
    [SerializeField] private int secondBoardExpandLevel = 20;

    public void SetPosition()
    {
        var currentBoardIndex = DataSaver.LoadIntData(_dataProfile.ProgressKey);
        bool moveOnes = currentBoardIndex >= firstBoardExpandLevel - 1;
        bool moveTwice = currentBoardIndex >= secondBoardExpandLevel - 1;

        float yOffset = 0;
        if (moveOnes) yOffset = 1f;
        if (moveTwice) yOffset = 2f;

        Vector3 offsetVector = new Vector3(0f, yOffset, 0f);
        transform.position -= offsetVector;
    }
}
