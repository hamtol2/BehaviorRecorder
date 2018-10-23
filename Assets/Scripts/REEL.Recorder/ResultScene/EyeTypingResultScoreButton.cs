using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.Recorder
{
    public class EyeTypingResultScoreButton : MonoBehaviour
    {
        public int questionNumber = 1;
        public int score;

        public void SetScore()
        {
            switch (questionNumber)
            {
                case 1:
                {
                    SurveyResultManager.Instance.ScoreFirstQuestion(score);
                    break;
                }
                case 2:
                {
                    SurveyResultManager.Instance.ScoreSecondQuestion(score);
                    break;
                }
                default: break;
            }
        }
    }
}