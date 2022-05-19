using UnityEngine;

namespace Game
{
    public class LetterFactory : MonoBehaviour
    {
        [SerializeField] private GameObject flyingLetterPrefab;
     
        public GameObject Create(GridSquare gridSquare)
        {
            var letter = Instantiate(flyingLetterPrefab, gridSquare.transform.position, Quaternion.identity, transform);
            var letterSprite = gridSquare.SelectedLetterData.Sprite;

            letter.GetComponent<SpriteRenderer>().sprite = letterSprite;

            return letter;
        }
    }
}