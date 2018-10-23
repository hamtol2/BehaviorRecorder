using UnityEngine;

namespace REEL.Recorder
{
    public class SurveyTypeSelectButton : MonoBehaviour
    {
        public enum SurveyType
        {
            TypeGA, TypeNA, None
        }

        [SerializeField] private SurveyType surveyType = SurveyType.None;

        private readonly string surveyTypeKey = "surveyType";

        public void OnGazeComplete()
        {
            Debug.Log(surveyType.ToString());
            PlayerPrefs.SetString(surveyTypeKey, this.surveyType.ToString());
            SurveyStartManager.Instance.GoingForward();
        }
    }
}