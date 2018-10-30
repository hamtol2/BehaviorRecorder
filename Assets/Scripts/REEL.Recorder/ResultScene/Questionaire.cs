using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.Recorder
{
    public class Questionaire : MonoBehaviour
    {
        [SerializeField] private Text questionaireText;
        [SerializeField] private bool isActiveState = true;

        public string GetQuestionaireText
        {
            get
            {
                return questionaireText.text;
            }
        }

        public void SetActiveState(bool state)
        {
            isActiveState = state;
        }

        public bool GetActiveState { get { return isActiveState; } }
    }
}