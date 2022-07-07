using System;
using System.Collections.Generic;
using UnityEngine;

public class FloorController : MonoBehaviour
{
    [SerializeField] private DataProfile _dataProfile;
    [SerializeField] private List<Level> _levelsToOffsetPosition;

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
