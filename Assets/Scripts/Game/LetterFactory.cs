using UnityEngine;

namespace Game
{
    public class LetterFactory : MonoBehaviour
    {
        [SerializeField] private GameObject flyingLetterPrefab;
        [SerializeField] private GameObject letterTrailPrefab;
        [SerializeField] private Vector3 trailSpawnOffset = new Vector3(0f, 0f, 0.5f);

        public GameObject Create(GridSquare gridSquare)
        {
            var letter = Instantiate(flyingLetterPrefab, gridSquare.transform.position, Quaternion.identity, transform);
            var letterSprite = gridSquare?.PlaneLetterData.Sprite;

            letter.GetComponent<SpriteRenderer>().sprite = letterSprite;

            return letter;
        }

        public GameObject Create(GameObject objectToCopy)
        {
            var letter = Instantiate(objectToCopy, objectToCopy.transform.position, Quaternion.identity, transform);
            Vector3 trailSpawnPosition = objectToCopy.transform.position + trailSpawnOffset;
            var trail = Instantiate(letterTrailPrefab, trailSpawnPosition, Quaternion.identity, letter.transform);
            return letter;
        }
    }
}