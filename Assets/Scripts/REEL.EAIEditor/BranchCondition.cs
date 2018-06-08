using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.EAIEditor
{
    [System.Serializable]
    public class BranchCondition
    {
        public enum IFBranchParamType
        {
            Variable, Value, Other, None
        }

        public enum IFBranchOpParamType
        {
            Equal, NotEqual, Greater, GreaterOrEqual, Less, LessOrEqual
        }

        public IFBranchParamType lParamType = IFBranchParamType.None;
        public IFBranchParamType rParamType = IFBranchParamType.None;

        public string lParameter = string.Empty;
        public IFBranchOpParamType opParameter;
        public string rParameter = string.Empty;

        public BranchCondition(int lType, int rType, string lParam, int opParam, string rParam)
        {
            SetValues(lType, rType, lParam, opParam, rParam);
        }

        public BranchCondition(BranchCondition branchCondition)
        {
            SetValues(branchCondition);
        }

        public void SetValues(int lType, int rType, string lParam, int opParam, string rParam)
        {
            lParamType = (IFBranchParamType)lType;
            rParamType = (IFBranchParamType)rType;
            lParameter = lParam;
            opParameter = (IFBranchOpParamType)opParam;
            rParameter = rParam;
        }

        public void SetValues(BranchCondition branchCondition)
        {
            lParamType = branchCondition.lParamType;
            rParamType = branchCondition.rParamType;
            lParameter = branchCondition.lParameter;
            opParameter = branchCondition.opParameter;
            rParameter = branchCondition.rParameter;
        }
    }
}