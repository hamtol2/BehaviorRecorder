using UnityEngine;
using UnityEngine.UI;

namespace REEL.Recorder
{
    public abstract class TimerButton : MonoBehaviour
    {
        protected Timer timer = new Timer();
        [SerializeField] [Range(0.1f, 2.0f)] protected float clickCheckTime = 1f;
        protected float lastRecoredTime = 0f;
        protected float onExitCheckTime = 0.2f;

        protected virtual void Awake()
        {
            timer.SetTimer(clickCheckTime, EyeClickHandler);
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
        }

        protected abstract void EyeClickHandler();

        public virtual void UpdateTimer()
        {
            timer.Update(Time.deltaTime);
            lastRecoredTime = Time.realtimeSinceStartup;
        }
    }
}