using UnityEngine;

namespace REEL.Recorder
{
    public class EyeTypingPracticeButton : MonoBehaviour
    {
        public void OnGazeComplete()
        {
            SurveyStartManager.Instance.AddHitCount();
        }
    }
}