using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.EAIEditor
{
    public class SwitchBranchPopup : Popup
    {
        [SerializeField]
        private float blockHeight = 85f;

        [SerializeField]
        private BranchConditionComponent[] switchBlocks;

        private RectTransform refRectTransform;

        private int blockMinNum = 2;
        private int blockMaxNum = 5;

        [SerializeField]
        private int currentBlockNumber = 2;

        private SwitchBranchItem switchItem;

        private void Awake()
        {
            refRectTransform = GetComponent<RectTransform>();
        }

        public override void OnOKClicked()
        {
            if (switchItem)
            {
                switchItem.SetBlockData(currentBlockNumber, switchBlocks);
            }

            base.OnOKClicked();
        }

        public override void OnCancelClicked()
        {
            base.OnCancelClicked();
        }

        public override void HidePopup()
        {
            base.HidePopup();

            for (int ix = 0; ix < switchBlocks.Length; ++ix)
            {
                DeleteBlock();
            }
        }

        public void OnPlusButtonClicked()
        {
            InsertBlock();
        }

        public void OnMinusButtonClicked()
        {
            DeleteBlock();
        }

        public override void ShowPopup(GraphItem targetItem = null)
        {
            base.ShowPopup(targetItem);

            switchItem = targetItem as SwitchBranchItem;
            if (switchItem)
            {
                BranchCondition[] conditionData = switchItem.GetConditionData();
                for (int ix = 0; ix < conditionData.Length; ++ix)
                {
                    if (ix < 2)
                    {
                        switchBlocks[ix].SetDataToComponent(conditionData[ix]);
                        continue;
                    }

                    InsertBlock(conditionData[ix]);
                }
            }
        }

        private void InsertBlock()
        {
            if (currentBlockNumber < 5)
            {
                refRectTransform.sizeDelta += new Vector2(0f, blockHeight);
                switchBlocks[currentBlockNumber].gameObject.SetActive(true);
                ++currentBlockNumber;
            }
        }

        private void InsertBlock(BranchCondition condition)
        {
            InsertBlock();
            switchBlocks[currentBlockNumber - 1].SetDataToComponent(condition);
        }

        private void DeleteBlock()
        {
            if (currentBlockNumber > 2)
            {
                refRectTransform.sizeDelta -= new Vector2(0f, blockHeight);
                --currentBlockNumber;
                switchBlocks[currentBlockNumber].gameObject.SetActive(false);
            }
        }
    }
}