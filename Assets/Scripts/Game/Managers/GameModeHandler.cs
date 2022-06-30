using UnityEngine;

public class GameModeHandler : MonoBehaviour
{
    public static readonly string GameModeKey = "GameMode";

    [SerializeField] private GameData _currentGameData;

    private void Start()
    {
        var gameMode = DataSaver.LoadIntData(GameModeKey);
        _currentGameData.selectedGameMode = (GameModeType)gameMode;
        GameEvents.OnGameModeChanged += SetGameMode;

        Debug.Log($"Handler: {_currentGameData.selectedGameMode}");
    }

    private void OnDestroy()
    {
        DataSaver.SaveIntData(GameModeKey, (int)_currentGameData.selectedGameMode);
        GameEvents.OnGameModeChanged -= SetGameMode;
    }

    public void SetGameMode(GameModeType gameModeType)
    {
        _currentGameData.selectedGameMode = gameModeType;
        DataSaver.SaveIntData(GameModeKey, (int)_currentGameData.selectedGameMode);
    }
}
