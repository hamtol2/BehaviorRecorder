using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.EAIEditor
{
    public class Popup : MenuPopup
    {
        protected GraphItem targetItem;

        public virtual void OnOKClicked()
        {
            HidePopup();
        }

        public virtual void OnCancelClicked()
        {
            HidePopup();
        }

        public override void ShowPopup(GraphItem targetItem = null)
        {
            gameObject.SetActive(true);
            if (targetItem) SetTargetItem(targetItem);
        }

        public override void HidePopup()
        {
            gameObject.SetActive(false);
        }

        public virtual void SetTargetItem(GraphItem targetItem)
        {
            this.targetItem = targetItem;
        }

        public void SetTargetItemData(object data)
        {
            this.targetItem.SetItemData(data);
        }
    }
}