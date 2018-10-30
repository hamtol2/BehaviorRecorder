using System;
using UnityEngine;

namespace REEL.Recorder
{
    public enum SurveyType
    {
        TypeGA,
        TypeNA,
        TypeDA,
        TypeRA,
        None
    }

    public class SurveyTypeSelectButton : EyeSelectionBase
    {
        [SerializeField] private SurveyType surveyType = SurveyType.None;

        public override void OnGazeComplete()
        {
            base.OnGazeComplete();

            Debug.Log(surveyType.ToString());
            PlayerPrefs.SetString(SurveyUtil.surveyTypeKey, this.surveyType.ToString());
            SurveyStartManager.Instance.GoingForward();
        }
    }
}