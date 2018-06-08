using UnityEngine;
using UnityEngine.UI;

namespace REEL.EAIEditor
{
    public class BranchConditionComponent : MonoBehaviour
    {
        [SerializeField]
        private Dropdown lParamDropdown;
        [SerializeField]
        private Dropdown rParamDropdown;

        [SerializeField]
        public InputField leftInputField;
        [SerializeField]
        public InputField rightInputField;
        [SerializeField]
        public Dropdown operatorDropdown;

        // Init values.
        private int lDropdownInitValue = 0;
        private int rDropdownInitValue = 1;
        private string lInputInitValue = "VARIABLE";
        private int opInitValue = 0;
        private string rInputInitValue = "VALUE";

        private void OnDisable()
        {
            Init();
        }

        private void Init()
        {
            lParamDropdown.value = lDropdownInitValue;
            rParamDropdown.value = rDropdownInitValue;
            leftInputField.text = lInputInitValue;
            rightInputField.text = rInputInitValue;
            operatorDropdown.value = opInitValue;
        }

        public BranchCondition GetBranchCondition()
        {
            return new BranchCondition(
                lParamDropdown.value,
                rParamDropdown.value,
                leftInputField.text,
                operatorDropdown.value,
                rightInputField.text);
        }

        public void SetDataToComponent(BranchCondition condition)
        {
            lParamDropdown.value = (int)condition.lParamType;
            rParamDropdown.value = (int)condition.rParamType;
            leftInputField.text = condition.lParameter;
            rightInputField.text = condition.rParameter;
            operatorDropdown.value = (int)condition.opParameter;
        }
    }
}