using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace REEL.Recorder
{
    public class SurveyStart : MonoBehaviour
    {
        [SerializeField] private Dropdown ageDropdown;
        [SerializeField] private Dropdown genderDropdown;
        [SerializeField] private string surveySceneName;

        public static string ageKey = "age";
        public static string genderKey = "gender";
        public static string countKey = "fileCount";

        public void OnStartClicked()
        {
            PlayerPrefs.SetString(ageKey, GetAgeValue(ageDropdown.options[ageDropdown.value].text));
            PlayerPrefs.SetString(genderKey, GetGenderValue(genderDropdown.options[genderDropdown.value].text));
            PlayerPrefs.SetInt(countKey, GetFileCount());

            //Debug.Log(string.Format("{0:yyyy_MM_dd}", DateTime.Today));

            SceneManager.LoadScene(surveySceneName);
        }

        string GetAgeValue(string age)
        {
            return age.Remove(age.Length - 1, 1);
        }

        string GetGenderValue(string gender)
        {
            return gender.Contains("남") ? "m" : "f";
        }

        int GetFileCount()
        {
            if (PlayerPrefs.HasKey(countKey)) return PlayerPrefs.GetInt(countKey) + 1;
            else return 1;
        }
    }
}