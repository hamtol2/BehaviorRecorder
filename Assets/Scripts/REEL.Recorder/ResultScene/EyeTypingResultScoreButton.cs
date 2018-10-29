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

            SurveyResultManager.Instance.ScoreQuestion(questionNumber, score);
        }
    }
}