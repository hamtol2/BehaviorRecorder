using System;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.Recorder
{
    public class QuizStatusManager : MonoBehaviour
    {
        public enum AnswerType { O, X }
        public enum Mode { Active, Inactive, None }

        public QuizConfig quizConfig;

        public GameObject quizStatusWindow;
        public Text quizNumberText;
        public Text scoreText;

        [SerializeField] private int currentQuizNumber = 0;
        [SerializeField] private int quizScore = 0;
        [SerializeField] private int numOfQuiz = 0;
        [SerializeField] private AnswerType currentAnswerType;

        private readonly string answerYes = "오";
        private readonly string answerNo = "엑스";

        Mode behaviorMode = Mode.None;

        public void NextQuiz()
        {
            ++currentQuizNumber;
        }

        public void SetBehaviorMode(string message)
        {
            behaviorMode = (Mode)Enum.Parse(typeof(Mode), message);

            if (behaviorMode == Mode.Active)
                quizConfig.robotModelType = ModelType.ExpressionRobot;
            else if (behaviorMode == Mode.Inactive)
                quizConfig.robotModelType = ModelType.NonExpressionRobot;
        }

        public int GetCurrentScore()
        {
            return quizScore;
        }

        public void GainScore()
        {
            ++quizScore;
            scoreText.text = quizScore.ToString();
            quizConfig.answerState = AnswerState.Correct;
            //Debug.Log("Score: " + quizScore);
        }

        void UpdateQuizStatus()
        {
            quizNumberText.text = currentQuizNumber.ToString() + " / " + numOfQuiz.ToString();
        }

        public Mode GetBehaviorMode { get { return behaviorMode; } }
        public int GetCurrentQuizNumber { get { return currentQuizNumber; } }
        public string QuizTitle { get { return quizConfig.quizTitle; } }
        public ModelType GetModelType { get { return quizConfig.robotModelType; } }

        public ContentState GetCurrentState()
        {
            return quizConfig.quizState;
        }

        public void SetContentState(ContentState state)
        {
            quizConfig.quizState = state;
        }

        public void SetAnswerState(AnswerState state)
        {
            quizConfig.answerState = state;
        }

        public AnswerState GetAnswerState { get { return quizConfig.answerState; } }

        public int GetQuizCount { get { return numOfQuiz; } }
        public void SetQuizCount(string message)
        {
            numOfQuiz = Convert.ToInt32(message);
        }

        public void SetCurrentAnswer(string message)
        {
            //Debug.Log("SetCurrentAnswer: " + message.ToUpper());
            currentAnswerType = (AnswerType)Enum.Parse(typeof(AnswerType), message.ToUpper());
        }

        public AnswerType GetCurrentAnswer { get { return currentAnswerType; } }

        public bool IsAnswerYes { get { return currentAnswerType == AnswerType.O; } }

        public bool IsQuizFinished
        {
            get { return currentQuizNumber == (numOfQuiz + 1); }
        }

        public string GetWrongAnswer
        {
            get { return currentAnswerType == AnswerType.O ? answerNo : answerYes; }
        }
    }
}