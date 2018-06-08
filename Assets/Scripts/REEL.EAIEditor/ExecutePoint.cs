using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace REEL.EAIEditor
{
    public class ExecutePoint : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IDragHandler
    {
        public enum PointPosition
        {
            ExecutePoint_Left, ExecutePoint_Right
        }

        [SerializeField]
        protected PointPosition pointPosition = PointPosition.ExecutePoint_Left;

        [SerializeField]
        protected GameObject lineBasePrefab = null;
        protected GraphLine graphLine;
        protected GraphLine.LinePoint linePoint;
        protected RectTransform refRectTransform;

        private DragItem dragItem;

        [SerializeField]
        protected bool isAltPressed = false;

        [SerializeField]
        protected bool hasLine = false;

        protected virtual void Awake()
        {
            dragItem = GetComponentInParent<DragItem>();
            refRectTransform = GetComponent<RectTransform>();
            linePoint = new GraphLine.LinePoint(Vector2.zero, Vector2.zero);
        }

        protected virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                isAltPressed = true;
            }

            if (Input.GetKeyUp(KeyCode.LeftAlt))
            {
                isAltPressed = false;
            }
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            List<RaycastResult> result = new List<RaycastResult>();
            EditorManager.Instance.UIRaycaster.Raycast(eventData, result);

            if (pointPosition == PointPosition.ExecutePoint_Left)
            {
                if (SetLine(result, PointPosition.ExecutePoint_Right)) return;
            }
            else
            {
                if (SetLine(result, PointPosition.ExecutePoint_Left)) return;
            }

            if (!hasLine && graphLine)
                Destroy(graphLine.gameObject);
        }

        private bool SetLine(List<RaycastResult> result, PointPosition position)
        {
            GameObject targetLinePosition = IsOnExecutePoint(result, position);
            if (targetLinePosition && graphLine)
            {
                linePoint.start = SelfPosition;
                linePoint.end = targetLinePosition.transform.position;
                graphLine.SetLinePoint(linePoint);
                graphLine.SetExecutePoints(this, targetLinePosition.GetComponent<ExecutePoint>());

                SetHasLine(true);
                targetLinePosition.GetComponent<ExecutePoint>().SetHasLine(true);

                return true;
            }

            return false;
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (isAltPressed && graphLine)
            {
                Destroy(graphLine.gameObject);
                return;
            }

            if (hasLine) return;

            // create and set graph line.
            if (HasDragItem) dragItem.SetEnableDrag(false);
            GameObject newObj = Instantiate(lineBasePrefab);
            newObj.transform.SetParent(transform);
            newObj.transform.localPosition = Vector3.zero;
            graphLine = newObj.GetComponent<GraphLine>();
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (hasLine) return;

            if (graphLine)
            {
                linePoint.start = SelfPosition;
                linePoint.end = eventData.position;
                graphLine.SetLinePoint(linePoint);
            }
        }

        protected bool HasDragItem
        {
            get { return dragItem != null; }
        }

        protected Vector2 SelfPosition
        {
            get { return refRectTransform.position; }
        }

        public DragItem GetTargetDragItem()
        {
            return dragItem;
        }

        public void SetHasLine(bool hasLine)
        {
            this.hasLine = hasLine;
        }

        protected GameObject IsOnExecutePoint(List<RaycastResult> rayResults, PointPosition point)
        {
            for (int ix = 0; ix < rayResults.Count; ++ix)
            {
                if (Util.CompareTwoStrings(rayResults[ix].gameObject.tag, point.ToString()))
                    return rayResults[ix].gameObject;
            }

            return null;
        }
    }
}