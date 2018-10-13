using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using REEL.Recorder;

namespace REEL.Recorder
{
    public class EyeTypingButton : MonoBehaviour
    {
        public enum ButtonType
        {
            YES, NO, ETC
        }

        [SerializeField] private Image gaugeImage;
        [SerializeField] private Color gaugeColor;
        [SerializeField] private ButtonType buttonType;
        [SerializeField] private Timer timer = new Timer();
        private Button button;

        private float clickCheckTime = 1f;

        private float lastRecoredTime = 0f;
        private float onExitCheckTime = 0.2f;

        private string yesString = "오";
        private string noString = "엑스";

        private void Awake()
        {
            timer.SetTimer(clickCheckTime, EyeClickHandler);
            gaugeImage.color = gaugeColor;
        }

        private void Update()
        {
            if (lastRecoredTime != 0f && Time.realtimeSinceStartup - lastRecoredTime > onExitCheckTime)
            {
                ResetButton();
            }
        }

        public void ResetButton()
        {
            lastRecoredTime = 0f;
            timer.Reset();
            gaugeImage.fillAmount = 0f;
        }

        void EyeClickHandler()
        {
            WebSurvey.Instance.GetReply(this.ToString());
            WebSurvey.Instance.CloseAnswerButton();
            //transform.parent.gameObject.SetActive(false);
        }

        //public void UpdateTimer(float deltaTime)
        public void UpdateTimer()
        {
            timer.Update(Time.deltaTime);
            lastRecoredTime = Time.realtimeSinceStartup;
            UpdateGauge(timer.GetElapsedTime / clickCheckTime);
        }

        void UpdateGauge(float amount)
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