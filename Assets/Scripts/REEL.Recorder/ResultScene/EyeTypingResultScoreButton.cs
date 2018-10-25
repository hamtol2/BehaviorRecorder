using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.Recorder
{
    public class EyeTypingResultScoreButton : EyeSelectionBase
    {
        public int questionNumber = 1;
        public int score;

        public override void OnGazeComplete()
        {
            base.OnGazeComplete();

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