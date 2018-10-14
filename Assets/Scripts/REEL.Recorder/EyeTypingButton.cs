using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using REEL.Recorder;

namespace REEL.Recorder
{
    public class EyeTypingButton : TimerButton
    {
        public enum ButtonType
        {
            YES, NO, ETC
        }
        
        [SerializeField] protected Image gaugeImage;
        [SerializeField] protected Color gaugeColor;
        [SerializeField] private ButtonType buttonType;

        private string yesString = "오";
        private string noString = "엑스";

        protected override void Awake()
        {
            base.Awake();
            gaugeImage.color = gaugeColor;
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void ResetButton()
        {
            base.ResetButton();
            gaugeImage.fillAmount = 0f;
        }

        protected override void EyeClickHandler()
        {
            WebSurvey.Instance.GetReply(this.ToString());
            WebSurvey.Instance.CloseAnswerButton();
        }

        public override void UpdateTimer()
        {
            base.UpdateTimer();
            UpdateGauge(timer.GetElapsedTime / clickCheckTime);
        }

        protected void UpdateGauge(float amount)
        {
            gaugeImage.fillAmount = amount;
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