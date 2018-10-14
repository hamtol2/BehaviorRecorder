using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace REEL.Recorder
{
    public class EyeTypingButton : MonoBehaviour
    {   
        [SerializeField] [Range(0.1f, 2.0f)] protected float clickCheckTime = 1f;
        [SerializeField] protected Image gaugeImage;
        [SerializeField] protected Color gaugeColor;
        protected Timer timer = new Timer();
        protected float lastRecoredTime = 0f;
        protected float onExitCheckTime = 0.2f;

        [SerializeField] protected UnityEvent timerCallback;

        protected virtual void Awake()
        {
            timer.SetTimer(clickCheckTime, EyeClickHandler);
            gaugeImage.color = gaugeColor;
        }

        protected virtual void Update()
        {
            if (lastRecoredTime != 0f && Time.realtimeSinceStartup - lastRecoredTime > onExitCheckTime)
            {
                ResetButton();
            }
        }

        protected virtual void ResetButton()
        {
            lastRecoredTime = 0f;
            timer.Reset();
            gaugeImage.fillAmount = 0f;
        }

        protected virtual void EyeClickHandler()
        {
            if (timerCallback != null)
                timerCallback.Invoke();
        }

        public virtual void UpdateTimer()
        {
            timer.Update(Time.deltaTime);
            lastRecoredTime = Time.realtimeSinceStartup;
            UpdateGauge(timer.GetElapsedTime / clickCheckTime);
        }

        protected virtual void UpdateGauge(float amount)
        {
            gaugeImage.fillAmount = amount;
        }
    }
}