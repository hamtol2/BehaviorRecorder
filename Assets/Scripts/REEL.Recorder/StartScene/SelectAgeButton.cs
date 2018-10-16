using UnityEngine;

namespace REEL.Recorder
{
    public class SelectAgeButton : MonoBehaviour
    {
        [SerializeField] private int age = 20;

        private string ageKey = "age";

        public void SelectAge()
        {
            PlayerPrefs.SetString(ageKey, age.ToString());
            SurveyStartManager.Instance.GoingForward();
        }
    }
}