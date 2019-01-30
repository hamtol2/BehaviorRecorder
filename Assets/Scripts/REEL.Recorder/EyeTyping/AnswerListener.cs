using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.Recorder
{
    public class AnswerListener : MonoBehaviour
    {
        public enum ButtonType
        {
            YES, NO, ETC
        }

        public SurveyController surveyController;

        [SerializeField] private ButtonType buttonType;
        public ButtonType GetButtonType { get { return buttonType; } }

        private string yesString = "오";
        private string noString = "엑스";

        public void TimerListener()
        {
            surveyController.GetReply(this.ToString());
            surveyController.CloseAnswerButton();
        }

        public override string ToString()
        {
            return GetButtonSting;
        }

        string GetButtonSting
        {
            get { return buttonType == ButtonType.YES ? yesString : noString; }
        }
    }
}