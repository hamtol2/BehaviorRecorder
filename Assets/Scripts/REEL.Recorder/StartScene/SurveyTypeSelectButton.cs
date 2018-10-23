using UnityEngine;

namespace REEL.Recorder
{
    public class SurveyTypeSelectButton : MonoBehaviour
    {
        public enum SurveyType
        {
            TypeGA, TypeNA
        }

        public SurveyType surveyType = SurveyType.TypeGA;

        private readonly string surveyTypeKey = "surveyType";

        public void OnGazeComplete()
        {
            PlayerPrefs.SetString(surveyTypeKey, surveyType.ToString());
            SurveyStartManager.Instance.GoingForward();
        }
    }
}