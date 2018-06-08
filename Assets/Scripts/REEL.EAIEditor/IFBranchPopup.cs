using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.EAIEditor
{
    public class IFBranchPopup : Popup
    {
        [SerializeField]
        private BranchConditionComponent branchData; 

        [SerializeField]
        private IFBranchItem targetIfItem;

        // Init values.
        private int lDropdownInitValue = 0;
        private int rDropdownInitValue = 1;
        private string lInputInitValue = "VARIABLE";
        private int opInitValue = 0;
        private string rInputInitValue = "VALUE";

        public override void OnOKClicked()
        {   
            if (targetIfItem)
            {
                targetIfItem.SetParameters(branchData.GetBranchCondition());
            }

            base.OnOKClicked();
        }

        public override void SetTargetItem(GraphItem targetItem)
        {
            base.SetTargetItem(targetItem);
            targetIfItem = targetItem as IFBranchItem;
        }

        public override void ShowPopup(GraphItem targetItem = null)
        {
            base.ShowPopup(targetItem);
            SetTargetItem(targetItem);
            SetPopupValues();
        }

        private void SetPopupValues()
        {
            if (targetIfItem)
            {
                BranchCondition ifBranchItem = targetIfItem.GetIFBranchData();
                if (ifBranchItem == null || ifBranchItem.lParamType == BranchCondition.IFBranchParamType.None) return;

                branchData.SetDataToComponent(ifBranchItem);
            }
        }

        public override void HidePopup()
        {
            base.HidePopup();
        }
    }
}