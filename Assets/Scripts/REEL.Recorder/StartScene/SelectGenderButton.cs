using UnityEngine;

namespace REEL.Recorder
{
    public class SelectGenderButton : MonoBehaviour
    {
        public enum Gender
        {
            Male, Female
        }

        public Gender gender = Gender.Female;

        private string genderKey = "gender";

        public void SelectGender()
        {
            PlayerPrefs.SetString(genderKey, this.ToString());
            SurveyStartManager.Instance.GoingForward();
        }

        public override string ToString()
        {
            return gender == Gender.Male ? "m" : "f";
        }
    }
}