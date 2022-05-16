using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectPuzzleButton : MonoBehaviour
{
    public GameData gameData;
    public GameLevelData levelData;
    public Text categoryText;
    public Image progressBarFilling;
    
    private string _gameSceneName = Literal.Scene_GameScene;
    private bool _levelLocked;

    void Start()
    {
        _levelLocked = false;
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
        UpdateButtonInformation();

        button.interactable = _levelLocked
            ? false
            : true;
    }

    void Update()
    {
        
    }

    private void UpdateButtonInformation()
    {
        int currentIndex = -1;
        int totalBoards = 0;

        foreach (var data in levelData.Data)
        {
            if (data.CategoryName.Equals(gameObject.name))
            {
                currentIndex = DataSaver.LoadIntData(gameObject.name);
                totalBoards = data.BoardData.Count;

                if (levelData.Data[0].CategoryName.Equals(gameObject.name) && currentIndex < 0)
                {
                    DataSaver.SaveIntData(levelData.Data[0].CategoryName, 0);//Unlocks first level
                    currentIndex = DataSaver.LoadIntData(gameObject.name);
                    totalBoards = data.BoardData.Count;
                }
            }
        }

        if (currentIndex.Equals(-1))
            _levelLocked = true;

        categoryText.text = _levelLocked 
            ? string.Empty 
            : (currentIndex.ToString() + "/" + totalBoards.ToString());

        progressBarFilling.fillAmount = (currentIndex > 0 && totalBoards > 0) 
            ? (float)currentIndex / (float) totalBoards 
            : 0f;
    }

    private void OnButtonClick()
    { 
        gameData.selectedCategoryName = gameObject.name;
        SceneManager.LoadScene(_gameSceneName);
    }
}
