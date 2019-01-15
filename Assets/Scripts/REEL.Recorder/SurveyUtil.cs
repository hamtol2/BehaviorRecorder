using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace REEL.Recorder
{
    public static class SurveyUtil
    {
        public static string ageKey = "age";
        public static string genderKey = "gender";
        public static string countKey = "fileCount";
        public static string surveyTypeKey = "surveyType";

        public static string GetSurveyFilePath
        {
            get { return GetFolderPath + "/" + GetFileName; }
        }

        public static string GetSurveyAddtionalQuestionPath
        {
            get { return GetFolderPath + "/" + GetAdditionalFileName; }
        }

        public static string GetFolderPath
        {
            get { return Application.dataPath + "/../SurveyData"; }
        }

        public static string GetFileName
        {
            get
            {
                string age = PlayerPrefs.GetString(ageKey);
                string gender = PlayerPrefs.GetString(genderKey);
                string today = string.Format("{0:yyyy_MM_dd}", DateTime.Now);
                string fileCount = PlayerPrefs.GetInt(countKey).ToString();
                string underscore = "_";

                return age + underscore + gender + underscore + today + underscore + fileCount + ".json";
            }
        }

        public static string GetAdditionalFileName
        {
            get
            {
                string age = PlayerPrefs.GetString(ageKey);
                string gender = PlayerPrefs.GetString(genderKey);
                string today = string.Format("{0:yyyy_MM_dd}", DateTime.Now);
                string fileCount = PlayerPrefs.GetInt(countKey).ToString();
                string underscore = "_";

                return age + underscore + gender + underscore + today + underscore + fileCount + "_result.json";
            }
        }

    }
}