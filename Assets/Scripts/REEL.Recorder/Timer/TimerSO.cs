using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.Recorder
{
    public class SimpleTimer
    {
        private float interval = 0f;
        private float elapsedTime = 0f;

        public SimpleTimer() { }
        public SimpleTimer(float interval)
        {
            elapsedTime = 0f;
            this.interval = interval;
        }

        public void Update(float deltaTime)
        {
            elapsedTime = Mathf.Min(elapsedTime + deltaTime, interval);
        }

        public bool IsTimeout { get { return elapsedTime >= interval; } }
        public void Reset()
        {
            elapsedTime = 0f;
        }
    }

    [CreateAssetMenu(menuName = "ScriptableObject/Timer")]
    public class TimerSO : ScriptableObject
    {
        public MOCCAEvent intervalEvent;

        private SimpleTimer mainTimer = null;
        private SimpleTimer intervalTimer = null;

        public bool isTurnedOn = false;

        private void OnEnable()
        {
            isTurnedOn = false;
        }

        private void OnDisable()
        {
            mainTimer = null;
            intervalTimer = null;
            isTurnedOn = false;
        }

        public void SetTimer(float interval)
        {
            if (mainTimer == null) mainTimer = new SimpleTimer();
            if(intervalTimer == null) intervalTimer = new SimpleTimer(interval);
            isTurnedOn = true;
        }

        public void UpdateTimer(float deltaTime)
        {
            if (!isTurnedOn) return;

            mainTimer.Update(deltaTime);

            if (intervalTimer != null)
            {
                intervalTimer.Update(deltaTime);
                if (intervalTimer.IsTimeout)
                {
                    intervalEvent.Raise();
                    intervalTimer.Reset();
                }
            }
        }

        public void TurnOn()
        {
            isTurnedOn = true;
        }

        public void TurnOff()
        {
            isTurnedOn = false;
            if (mainTimer != null) mainTimer.Reset();
            if (intervalTimer != null) intervalTimer.Reset();
        }

        public bool IsTurnedOn { get { return isTurnedOn; } }
    }
}