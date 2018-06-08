using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.EAIEditor
{
    public class TextBoxPopup : Popup
    {
        [SerializeField]
        private InputField inputField;

        public override void OnOKClicked()
        {
            if (targetItem && !inputField.text.Equals(string.Empty))
                targetItem.SetItemData(inputField.text);

            base.OnOKClicked();
        }

        public override void OnCancelClicked()
        {
            base.OnCancelClicked();
        }

        public override void ShowPopup(GraphItem targetItem = null)
        {
            if (targetItem && targetItem.GetItemData() != null)
            {
                inputField.text = targetItem.GetItemData() as string;
            }

            base.ShowPopup(targetItem);
        }

        public override void HidePopup()
        {
            inputField.text = string.Empty;
            base.HidePopup();
        }
    }
}