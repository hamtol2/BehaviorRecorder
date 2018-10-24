using UnityEngine;

namespace REEL.Recorder
{
    public class SelectGenderButton : EyeSelectionBase
    {
        public enum Gender
        {
            Male, Female
        }

        public Gender gender = Gender.Female;

        private string genderKey = "gender";

        public override void OnGazeComplete()
        {
            base.OnGazeComplete();
            PlayerPrefs.SetString(genderKey, this.ToString());
            SurveyStartManager.Instance.GoingForward();
        }

        public override string ToString()
        {
            return gender == Gender.Male ? "m" : "f";
        }
    }
}