using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.EAIEditor
{
    public class IFBranchItem : GraphItem
    {
        [SerializeField]
        private BranchCondition ifBranchData = null;

        [SerializeField]
        private ExecuteIFPoint executeTrue;
        [SerializeField]
        private ExecuteIFPoint executeFalse;

        public void SetParameters(BranchCondition branchCondition)
        {
            if (ifBranchData == null) ifBranchData = new BranchCondition(branchCondition);
            else ifBranchData.SetValues(branchCondition);
        }

        public BranchCondition GetIFBranchData()
        {
            return ifBranchData;
        }
    }
}