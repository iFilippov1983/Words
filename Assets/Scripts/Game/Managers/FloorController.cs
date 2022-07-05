using System;
using System.Collections.Generic;
using UnityEngine;

public class FloorController : MonoBehaviour
{
    [SerializeField] private DataProfile _dataProfile;
    [SerializeField] private List<Level> _levelsToOffsetPosition;
    //[SerializeField] private int firstBoardExpandLevel = 10;
    //[SerializeField] private int secondBoardExpandLevel = 20;

    //public void SetPosition()
    //{
    //    var currentBoardIndex = DataSaver.LoadIntData(_dataProfile.ProgressKey);
    //    bool moveOnes = currentBoardIndex >= firstBoardExpandLevel - 1;
    //    bool moveTwice = currentBoardIndex >= secondBoardExpandLevel - 1;

    //    float yOffset = 0;
    //    if (moveOnes) yOffset = 1f;
    //    if (moveTwice) yOffset = 2f;

    //    Vector3 offsetVector = new Vector3(0f, yOffset, 0f);
    //    transform.position -= offsetVector;
    //}

    public void SetPosition()
    {
        int currentBoardIndex = DataSaver.LoadIntData(_dataProfile.ProgressKey);
        float yOffset = 0f;
        foreach (Level level in _levelsToOffsetPosition)
            if (currentBoardIndex >= level.number - 1)
                yOffset = level.offset;

        Vector3 offsetVector = new Vector3(0f, yOffset, 0f);
        transform.position -= offsetVector;
    }

    [Serializable]
    private class Level
    {
        public int number = 0;
        [Range(0f, 10f, order = 1)]
        public float offset = 0;
    }
}
