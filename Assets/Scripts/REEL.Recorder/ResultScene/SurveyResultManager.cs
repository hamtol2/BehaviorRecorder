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
        [SerializeField] private Questionaire[] questionaires;
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
            string quizType = PlayerPrefs.GetString(SurveyUtil.surveyTypeKey);
            SurveyType surveyType = (SurveyType)Enum.Parse(typeof(SurveyType), quizType);
            switch (surveyType)
            {
                case SurveyType.TypeGA:
                case SurveyType.TypeNA: questionaires[1].SetActiveState(false); break;
                case SurveyType.TypeDA:
                case SurveyType.TypeRA: questionaires[1].SetActiveState(true); break;
            }

            resultSaveFormat = new ResultSaveFormat();

            if (!Directory.Exists(SurveyUtil.GetFolderPath))
                Directory.CreateDirectory(SurveyUtil.GetFolderPath);

            filePath = SurveyUtil.GetSurveyAddtionalQuestionPath;
        }

        public void GoingForward()
        {
            Invoke("NextStage", nextStageWaitTime);
        }

        private void NextStage()
        {
            if (currentStage < questionaires.Length)
            {
                questionaires[currentStage].gameObject.SetActive(false);
                currentStage = GetNextStateIndex;
                questionaires[currentStage].gameObject.SetActive(true);
            }
        }

        private int GetNextStateIndex
        {
            get
            {
                int ix = currentStage + 1;
                for (; ix < questionaires.Length; ++ix)
                {
                    if (questionaires[ix].GetActiveState)
                        break;
                }

                return ix;
            }
        }

        public void ScoreQuestion(int questionNumber, int score)
        {
            ResultQuestionFormat data = new ResultQuestionFormat();
            int questionIndex = questionNumber - 1;
            data.question = questionaires[questionIndex].GetQuestionaireText;
            data.score = score;

            resultSaveFormat.AddData(data);
            GoingForward();

            if (questionNumber == questionaires.Length - 1)
                SaveToFile();
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
    }
}