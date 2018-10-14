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

        [SerializeField] private ButtonType buttonType;

        private string yesString = "오";
        private string noString = "엑스";

        public void TimerListener()
        {
            WebSurvey.Instance.GetReply(this.ToString());
            WebSurvey.Instance.CloseAnswerButton();
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