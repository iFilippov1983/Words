using UnityEngine;

public class GameModeHandler : MonoBehaviour
{
    [SerializeField] private GameData _currentGameData;

    private void Start()
    {
        var gameMode = DataSaver.LoadIntData(DataKey.GameModeKey);
        _currentGameData.selectedGameMode = (GameModeType)gameMode;
        GameEvents.OnGameModeChanged += SetGameMode;
    }

    private void OnDestroy()
    {
        DataSaver.SaveIntData(DataKey.GameModeKey, (int)_currentGameData.selectedGameMode);
        GameEvents.OnGameModeChanged -= SetGameMode;
    }

    public void SetGameMode(GameModeType gameModeType)
    {
        _currentGameData.selectedGameMode = gameModeType;
        DataSaver.SaveIntData(DataKey.GameModeKey, (int)_currentGameData.selectedGameMode);
    }
}
