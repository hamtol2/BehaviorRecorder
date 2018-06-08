using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using EPaneType = REEL.EAIEditor.EditorManager.EPaneType;

namespace REEL.EAIEditor
{
    public class LibraryItem : DragItem
    {
        public enum LibraryItemType
        {
            Event, Hearing, Say, If, Switch
        }

        public LibraryItemType libraryItemType;
        public GameObject itemButtonPrefab;

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);

            List<RaycastResult> result = new List<RaycastResult>();
            EditorManager.Instance.UIRaycaster.Raycast(eventData, result);

            if (IsOnGraphPane(result) && itemButtonPrefab)
            {
                GameObject paneObj = EditorManager.Instance.GetPaneObject(EPaneType.Graph_Pane);
                GraphPane pane = paneObj.GetComponent<GraphPane>();
                pane.AddLibraryItem(itemButtonPrefab, eventData.position);
                //GameObject newButton = Instantiate(itemButtonPrefab);
                //RectTransform rectTransform = newButton.GetComponent<RectTransform>();
                //GameObject graphPane = EditorManager.Instance.GetPaneObject(EPaneType.Graph_Pane);
                //rectTransform.SetParent(graphPane.transform);
                //rectTransform.position = eventData.position;
            }

            SetToOrigin();
        }

        private bool IsOnGraphPane(List<RaycastResult> rayResults)
        {
            for (int ix = 0; ix < rayResults.Count; ++ix)
            {
                if (Util.CompareTwoStrings(rayResults[ix].gameObject.tag, EPaneType.Graph_Pane.ToString()))
                    return true;
            }

            return false;
        }
    }
}