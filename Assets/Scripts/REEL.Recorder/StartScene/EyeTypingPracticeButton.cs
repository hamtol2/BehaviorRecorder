using UnityEngine;

namespace REEL.Recorder
{
    public class EyeTypingPracticeButton : EyeSelectionBase
    {
        public override void OnGazeComplete()
        {
            base.OnGazeComplete();
            SurveyStartManager.Instance.AddHitCount();
        }
    }
}