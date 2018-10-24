using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace REEL.Recorder
{
    [Serializable]
    class ResultQuestionFormat
    {
        public string question;
        public int score;
    }

    [Serializable]
    class ResultSaveFormat
    {
        public ResultQuestionFormat[] saveData;

        public int Length
        {
            get { return saveData == null ? 0 : saveData.Length; }
        }

        public ResultQuestionFormat this[int index]
        {
            get { return saveData[index]; }
        }

        public void AddData(ResultQuestionFormat data)
        {
            if (saveData == null)
            {
                saveData = new ResultQuestionFormat[1] { data };
                return;
            }

            ResultQuestionFormat[] tempArray = new ResultQuestionFormat[saveData.Length];
            for (int ix = 0; ix < saveData.Length; ++ix)
            {
                tempArray[ix] = saveData[ix];
            }

            saveData = new ResultQuestionFormat[saveData.Length + 1];
            for (int ix = 0; ix < tempArray.Length; ++ix)
            {
                saveData[ix] = tempArray[ix];
            }

            saveData[saveData.Length - 1] = data;
        }
    }

    public class SurveyResultManager : Singleton<SurveyResultManager>
    {
        private ResultSaveFormat resultSaveFormat;

        [SerializeField] private UnityEngine.UI.Text firstQuestion;
        [SerializeField] private UnityEngine.UI.Text secondQuestion;

        [SerializeField] private GameObject[] stages;

        [SerializeField] private string startSceneName = string.Empty;

        private string filePath = string.Empty;

        private int currentStage = 0;

        private float nextStageWaitTime = 2.0f;

        private void Awake()
        {
            Init();
        }

        void Init()
        {
            resultSaveFormat = new ResultSaveFormat();

            if (!Directory.Exists(Application.dataPath + "/SurveyData"))
                Directory.CreateDirectory(Application.dataPath + "/SurveyData");

            filePath = Application.dataPath + "/SurveyData/" + GetFileName;
        }

        public void GoingForward()
        {
            Invoke("NextStage", nextStageWaitTime);
        }

        private void NextStage()
        {
            if (currentStage < stages.Length)
            {
                stages[currentStage++].SetActive(false);
                stages[currentStage].SetActive(true);
            }
        }

        public void ScoreFirstQuestion(int score)
        {
            ResultQuestionFormat data = new ResultQuestionFormat();
            data.question = firstQuestion.text;
            data.score = score;

            resultSaveFormat.AddData(data);
            GoingForward();
        }

        public void ScoreSecondQuestion(int score)
        {
            ResultQuestionFormat data = new ResultQuestionFormat();
            data.question = secondQuestion.text;
            data.score = score;

            resultSaveFormat.AddData(data);

            SaveToFile();
            GoingForward();
        }

        public void OnCloseButtonClicked()
        {
            Application.Quit();
        }

        public void OnReturnButtonClicked()
        {
            Invoke("MoveToStartScene", 3f);
        }

        void MoveToStartScene()
        {
            SceneManager.LoadScene(startSceneName);
        }

        void SaveToFile()
        {
            string jsonString = JsonUtility.ToJson(resultSaveFormat);
            File.WriteAllText(filePath, jsonString);
        }

        string GetFileName
        {
            get
            {
                string age = PlayerPrefs.GetString(SurveyStart.ageKey);
                string gender = PlayerPrefs.GetString(SurveyStart.genderKey);
                string today = string.Format("{0:yyyy_MM_dd}", DateTime.Now);
                string fileCount = PlayerPrefs.GetInt(SurveyStart.countKey).ToString();
                string underscore = "_";

                return age + underscore + gender + underscore + today + underscore + fileCount + "_result.json";
            }
        }
    }
}