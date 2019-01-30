using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.Recorder
{
    public class TimerManager : MonoBehaviour
    {
        public TimerSO answerTimer;
        public TimerSO hintTimer;
        public TimerSO robotmovementTimer;
        public TimerSO resultSceneTimer;

        public void SetAnswerTimer(float interval)
        {
            answerTimer.SetTimer(interval);
        }

        public void SetHintTimer(float interval)
        {
            hintTimer.SetTimer(interval);
        }

        public void SetRobotMovementTimer(float interval)
        {
            robotmovementTimer.SetTimer(interval);
        }

        public void SetResultSceneTimer(float interval)
        {
            resultSceneTimer.SetTimer(interval);
        }

        private void Update()
        {
            if (answerTimer.IsTurnedOn)
                answerTimer.UpdateTimer(Time.deltaTime);

            if (hintTimer.IsTurnedOn)
                hintTimer.UpdateTimer(Time.deltaTime);

            if (robotmovementTimer.IsTurnedOn)
                robotmovementTimer.UpdateTimer(Time.deltaTime);

            if (resultSceneTimer.IsTurnedOn)
                resultSceneTimer.UpdateTimer(Time.deltaTime);
        }
    }
}