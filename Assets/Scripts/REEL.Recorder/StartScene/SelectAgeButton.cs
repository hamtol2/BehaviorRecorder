using UnityEngine;

namespace REEL.Recorder
{
    public class SelectAgeButton : EyeSelectionBase
    {
        [SerializeField] private int age = 20;

        private string ageKey = "age";

        public override void OnGazeComplete()
        {
            base.OnGazeComplete();
            PlayerPrefs.SetString(ageKey, age.ToString());
            SurveyStartManager.Instance.GoingForward();
        }
    }
}