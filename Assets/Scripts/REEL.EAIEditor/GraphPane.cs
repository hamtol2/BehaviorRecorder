using UnityEngine;

namespace REEL.EAIEditor
{
    public class GraphPane : MonoBehaviour
    {
        [SerializeField]
        private Transform blockPane;

        public void AddLibraryItem(GameObject itemPrefab, Vector3 itemPosition)
        {
            GameObject newObj = Instantiate(itemPrefab);
            RectTransform rectTransform = newObj.GetComponent<RectTransform>();
            rectTransform.SetParent(blockPane);
            rectTransform.position = itemPosition;
        }
    }
}