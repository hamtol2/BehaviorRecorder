using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.EAIEditor
{
    public class SwitchBranchItem : GraphItem
    {
        [SerializeField]
        private BranchCondition[] switchBranchDatas;

        [SerializeField]
        private int currentBlockNumber = 2;

        [SerializeField]
        private GameObject[] caseBlocks;

        public void SetBlockData(int blockCount, BranchConditionComponent[] switchBlocks)
        {
            if (IsBlockCountOK(blockCount))
            {
                DisableAllBlock();
                currentBlockNumber = blockCount;
                
                for (int ix = 0; ix < blockCount - 2; ++ix)
                {   
                    caseBlocks[ix].SetActive(true);
                }

                SetConditionData(switchBlocks);
            }
        }

        private void SetConditionData(BranchConditionComponent[] switchBlocks)
        {
            switchBranchDatas = new BranchCondition[currentBlockNumber];
            for (int ix = 0; ix < currentBlockNumber; ++ix)
            {
                switchBranchDatas[ix] = switchBlocks[ix].GetBranchCondition();
            }
        }

        public BranchCondition[] GetConditionData()
        {
            return switchBranchDatas;
        }

        private void DisableAllBlock()
        {
            for (int ix = 0; ix < caseBlocks.Length; ++ix)
            {
                caseBlocks[ix].SetActive(false);
            }
        }

        bool IsBlockCountOK(int blockCount)
        {
            return blockCount >= 2 && blockCount <= 5;
        }
    }
}