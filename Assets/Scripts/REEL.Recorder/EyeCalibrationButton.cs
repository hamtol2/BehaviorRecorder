using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.Recorder
{
    //[ExecuteInEditMode]
    public class EyeCalibrationButton : TimerButton
    {
        [SerializeField] protected Image gaugeImage;
        [SerializeField] protected Color gaugeColor;
        [SerializeField] protected GameObject scatterParticle;

        private bool isFinished = false;

        protected override void Awake()
        {
            base.Awake();
            if (gaugeImage == null)
                gaugeImage = transform.GetChild(0).GetComponent<Image>();

            gaugeImage.color = gaugeColor;
        }

        protected override void Update()
        {
            if (isFinished) return;

            base.Update();
        }

        public override void UpdateTimer()
        {
            //if (isFinished) return;

            base.UpdateTimer();
            UpdateGauge();
        }

        protected override void ResetButton()
        {
            base.ResetButton();
            gaugeImage.fillAmount = 0f;
        }

        private void UpdateGauge()
        {
            gaugeImage.fillAmount = timer.GetElapsedTime / clickCheckTime;
        }

        protected override void EyeClickHandler()
        {
            CalibrationManager.Instance.AddHitCount();
            isFinished = true;
            scatterParticle.SetActive(true);
            this.GetComponent<Image>().enabled = false;
            gaugeImage.enabled = false;
        }
    }
}