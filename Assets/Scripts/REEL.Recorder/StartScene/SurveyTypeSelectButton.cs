using UnityEngine;

namespace REEL.Recorder
{
    public class SurveyTypeSelectButton : EyeSelectionBase
    {
        public enum SurveyType
        {
            TypeGA, TypeNA, None
        }

        [SerializeField] private SurveyType surveyType = SurveyType.None;

        private readonly string surveyTypeKey = "surveyType";

        public override void OnGazeComplete()
        {
            base.OnGazeComplete();

            Debug.Log(surveyType.ToString());
            PlayerPrefs.SetString(surveyTypeKey, this.surveyType.ToString());
            SurveyStartManager.Instance.GoingForward();
        }
    }
}