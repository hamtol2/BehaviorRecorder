using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.EAIEditor
{
    public class GraphLine : MonoBehaviour
    {
        public struct LinePoint
        {
            public Vector2 start;
            public Vector2 end;

            public LinePoint(Vector2 start, Vector2 end)
            {
                this.start = start;
                this.end = end;
            }
        }

        private ExecutePoint left;
        private ExecutePoint right;

        private LinePoint linePoint;
        private Image lineImage;

        private bool isSet = false;

        [SerializeField]
        private float lineHeight = 5f;

        private void Awake()
        {
            lineImage = GetComponent<Image>();
        }

        private void UpdateLine()
        {
            if (isSet)
            {
                linePoint.start = left.transform.position;
                linePoint.end = right.transform.position;
                SetLinePoint(linePoint);
            }
        }

        public void SetExecutePoints(ExecutePoint left, ExecutePoint right)
        {
            this.left = left;
            this.right = right;
            isSet = true;

            // Subscribe dragitem's OnChanged event to update line.
            left.GetTargetDragItem().SubscribeOnChanged(UpdateLine);
            right.GetTargetDragItem().SubscribeOnChanged(UpdateLine);
        }

        private void OnDestroy()
        {
            if (left != null && left.GetTargetDragItem() != null)
            {
                left.GetTargetDragItem().UnsubscribeOnChanged(UpdateLine);
                left.SetHasLine(false);
            }
                
            if (right != null && right.GetTargetDragItem() != null)
            {
                right.GetTargetDragItem().UnsubscribeOnChanged(UpdateLine);
                right.SetHasLine(false);
            }
        }

        public void SetLinePoint(LinePoint linePoint)
        {
            this.linePoint = linePoint;
            SetLineAngle();
            SetLineLength();
        }

        void SetLineAngle()
        {
            float angle = Util.GetAngleBetween(linePoint.start, linePoint.end);
            lineImage.rectTransform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        void SetLineLength()
        {
            float length = Util.GetDistanceBetween(linePoint.start, linePoint.end);
            lineImage.rectTransform.sizeDelta = new Vector2(length, lineHeight);
        }
    }
}