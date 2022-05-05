using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordsGrid : MonoBehaviour
{
    public GameData currentGameData;
    public GameObject gridSquarePrefab;
    public AlphabetData alphabetData;

    public float squareOffset = 0.0f;
    public float topPosition;

    [SerializeField] private Vector3 _defaultScale = new Vector3(1.5f, 1.5f, 1f);
    private List<GameObject> _squareList = new List<GameObject>();
    private Camera _camera;
    

    void Start()
    {
        _camera = Camera.main;
        SpawnGridSquares();
        SetSquaresPosition();
    }

    private void SetSquaresPosition()
    {
        Rect squareRect = _squareList[0].GetComponent<SpriteRenderer>().sprite.rect;
        Transform squareTransform = _squareList[0].GetComponent<Transform>();
        Vector2 offset = new Vector2
        {
            x = (squareRect.width * squareTransform.localScale.x + squareOffset) * 0.01f,
            y = (squareRect.height * squareTransform.localScale.y + squareOffset) * 0.01f
        };

        var startPosition = GetFirstSquarePosition();
        int columnNumber = 0;
        int rowNumber = 0;

        foreach (var square in _squareList)
        {
            if (rowNumber + 1 > currentGameData.selectedBoardData.Rows)
            {
                columnNumber++;
                rowNumber = 0;
            }
            var positionX = startPosition.x + offset.x * columnNumber;
            var positionY = startPosition.y - offset.y * rowNumber;

            square.GetComponent<Transform>().position = new Vector2(positionX, positionY);
            rowNumber++;
        }
    }

    private Vector2 GetFirstSquarePosition()
    {
        var startPosition = new Vector2(0f, transform.position.y);
        var squareRect = _squareList[0].GetComponent<SpriteRenderer>().sprite.rect;
        var squareTransform = _squareList[0].GetComponent<Transform>();
        var squareSize = new Vector2(0f, 0f);

        squareSize.x = squareRect.width * squareTransform.localScale.x;
        squareSize.y = squareRect.height * squareTransform.localScale.y;

        var midWidthPosition = (((currentGameData.selectedBoardData.Columns - 1) * squareSize.x) / 2) * 0.01f;
        float midWidthHeight = (((currentGameData.selectedBoardData.Rows - 1) * squareSize.y) / 2) * 0.01f;

        startPosition.x = (midWidthPosition != 0) ? midWidthPosition * -1 : midWidthPosition;
        startPosition.y += midWidthHeight;

        return startPosition;
    }

    private void SpawnGridSquares()
    {
        if (currentGameData != null)
        {
            var squareScale = GetSquareScale(_defaultScale);
            foreach (var squares in currentGameData.selectedBoardData.Board)
            {
                foreach (var squareLetter in squares.Row)
                {
                    var normalLetterData = alphabetData.AlphabetNormal.Find(data => data.Letter == squareLetter);
                    var selectedLetterData = alphabetData.AlphabetHighlighted.Find(data => data.Letter == squareLetter);
                    var correctLetterData = alphabetData.AlphabetWrong.Find(data => data.Letter == squareLetter);

                    if (normalLetterData.Sprite == null || selectedLetterData.Sprite == null)
                    {
                        Debug.LogError
                            ("All fields in your array should have some letters. Press \"Fill Up With Random\" in your board data to add random letter. Letter: " + squareLetter);
#if UNITY_EDITOR
                        if (UnityEditor.EditorApplication.isPlaying)
                        {
                            UnityEditor.EditorApplication.isPlaying = false;
                        }
#endif
                    }
                    else 
                    {
                        var square = Instantiate(gridSquarePrefab);
                        square.GetComponent<GridSquare>().SetSprite(normalLetterData, selectedLetterData, correctLetterData);
                        var squareTransform = square.GetComponent<Transform>();
                        squareTransform.SetParent(this.transform);
                        squareTransform.position = Vector3.zero;
                        squareTransform.localScale = squareScale;
                        _squareList.Add(square);

                        //var square = Instantiate(gridSquarePrefab);
                        //square.GetComponent<GridSquare>().SetSprite(normalLetterData, selectedLetterData, correctLetterData);
                        //square.transform.SetParent(this.transform);
                        //square.transform.position = new Vector3(0f, 0f, 0f);
                        //square.transform.localScale = squareScale;
                        //_squareList.Add(square);

                        //_squareList.Add(Instantiate(gridSquarePrefab));
                        //_squareList[_squareList.Count - 1].GetComponent<GridSquare>().SetSprite(normalLetterData, selectedLetterData, correctLetterData);
                        //_squareList[_squareList.Count - 1].transform.SetParent(this.transform);
                        //_squareList[_squareList.Count - 1].GetComponent<Transform>().position = new Vector3(0f, 0f, 0f);
                        //_squareList[_squareList.Count - 1].transform.localScale = squareScale;
                    }
                }
            }
        }
    }

    private Vector3 GetSquareScale(Vector3 defaultScale)
    {
        Vector3 finalScale = defaultScale;
        float adjustment = 0.01f;

        while (ShouldScaleDown(finalScale))
        {
            finalScale.x -= adjustment;
            finalScale.y -= adjustment;
            finalScale.z -= adjustment;

            if (finalScale.x <= 0 || finalScale.y <= 0 || finalScale.z < 0)
            {
                finalScale.x = adjustment;
                finalScale.y = adjustment;
                finalScale.z = adjustment;
                return finalScale;
            }
        }
        return finalScale;
    }

    private bool ShouldScaleDown(Vector3 targetScale)
    { 
        Rect squareRect = gridSquarePrefab.GetComponent<SpriteRenderer>().sprite.rect;
        var squareSize = new Vector2(0f, 0f);
        var startPosition = new Vector2(0f, 0f);

        squareSize.x = (squareRect.width * targetScale.x) + squareOffset;
        squareSize.y = (squareRect.height * targetScale.y) + squareOffset;

        float midWidthPosition = ((currentGameData.selectedBoardData.Columns * squareSize.x) / 2) * 0.01f;
        float midWidthHeight = ((currentGameData.selectedBoardData.Rows * squareSize.y) / 2) * 0.01f;

        startPosition.x = (midWidthPosition != 0) ? midWidthPosition * -1 : midWidthPosition;
        startPosition.y = midWidthHeight;

        return startPosition.x < GetHalfScreenWidth() * -1 || startPosition.y > topPosition;
    }

    private float GetHalfScreenWidth()
    {
        float height = _camera.orthographicSize * 2;
        float width = (1.7f * height) * Screen.width / Screen.height;
        return width / 2;
    }
}
