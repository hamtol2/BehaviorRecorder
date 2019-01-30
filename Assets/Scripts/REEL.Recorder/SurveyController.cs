using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace REEL.Recorder
{
    public class SurveyController : MonoBehaviour
    {
        public RiveScriptProcessorSO riveScript;
        public RiveScriptFileArray riveScriptFileArray;

        public REEL.Animation.RobotFacialRenderer robotFacialRenderer;
        public REEL.PoseAnimation.RobotMotionController transformController;

        public QuizStatusManager quizStatusManager;

        public RobotMovement robotMovement;
        public BehaviorRecorder behaviorRecorder;
        public BehaviorReplayer behaviorReplayer;

        [SerializeField] private AnswerButton answerButton;

        public TimerManager timerManager;
        [SerializeField] private float timeOutTime = 10f;
        [SerializeField] private float hintTime = 0f;
        [SerializeField] private float robotMovementTime = 0f;

        private readonly float timeToResultScene = 5f;

        private bool quizFinished = false;
        public bool QuizFinished { get { return quizFinished; } }

        private bool isActiveFace = false;          // 고정된 표정 / 변화하는 표정 여부.
        private bool isCue = false;                 // Cue 줄지 여부.

        private readonly string timeoutString = "timeout";

        private readonly string answerYes = "오";
        private readonly string answerNo = "엑스";

        private readonly string faceGazeLeft = "gazeleft";
        private readonly string faceGazeRight = "gazeright";

        private readonly float answerYPosHigh = 150f;
        private readonly float answerYPosLow = 50f;

        private string quizType;
        private string age;
        private string gender;

        [SerializeField] private string resultSceneName = string.Empty;

        private void Awake()
        {
            InitRiveScript();
        }

        // Use this for initialization
        void Start()
        {
            //SpeechRenderrer.Instance.Init();
        }

        private void Update()
        {
            CheckAnswerTimer();
        }

        void InitRiveScript()
        {
            if (!PlayerPrefs.HasKey("UUID"))
            {
                PlayerPrefs.SetString("UUID", System.Guid.NewGuid().ToString());
            }

            quizType = PlayerPrefs.GetString(SurveyUtil.surveyTypeKey);
            age = PlayerPrefs.GetString(SurveyUtil.ageKey);
            gender = PlayerPrefs.GetString(SurveyUtil.genderKey);

            SurveyType surveyType = (SurveyType)Enum.Parse(typeof(SurveyType), quizType);
            switch (surveyType)
            {
                case SurveyType.TypeGA: riveScript.SetRiveScriptText(riveScriptFileArray.GetRiveScriptAsset(0)); break;
                case SurveyType.TypeNA: riveScript.SetRiveScriptText(riveScriptFileArray.GetRiveScriptAsset(1)); break;
                case SurveyType.TypeDA: riveScript.SetRiveScriptText(riveScriptFileArray.GetRiveScriptAsset(2)); break;
                case SurveyType.TypeRA: riveScript.SetRiveScriptText(riveScriptFileArray.GetRiveScriptAsset(3)); break;
                default: break;
            }

            riveScript.InitRiveScript();
        }

        public void GetReply(string message)
        {
            //string reply = riveScript.reply("default", message);
            string reply = riveScript.GetReply(message);
            if (reply.Contains("NOT_MATCHED"))
            {
                Debug.Log("Not matched");
            }
            else
            {
                Debug.Log(reply);
                Arbitor.Instance.Insert(reply);
            }
        }

        public int GetQuizCount { get { return quizStatusManager.GetQuizCount; } }
        public void SetQuizCount(string message)
        {
            quizStatusManager.SetQuizCount(message);
        }

        public void SetCurrentAnswer(string message)
        {
            //Debug.Log("SetCurrentAnswer: " + message.ToUpper());
            quizStatusManager.SetCurrentAnswer(message);
        }

        public void SetTimeoutTime(string message)
        {
            //Debug.Log("SetTimeoutTime: " + message);
            timeOutTime = Convert.ToSingle(message);
        }

        public float GetTimeoutTime { get { return timeOutTime; } }

        public void SetHintTime(string message)
        {
            hintTime = Convert.ToSingle(message);
        }

        public float GetHintTime { get { return hintTime; } }

        public void SetBehaviorMode(string message)
        {
            quizStatusManager.SetBehaviorMode(message);
        }

        public void SetCueState(string message)
        {
            isCue = Convert.ToBoolean(message);
        }
        public bool GetCueState { get { return isCue; } }

        public void SetFaceActiveState(string message)
        {
            isActiveFace = Convert.ToBoolean(message);
        }
        public bool GetFaceActiveState { get { return isActiveFace; } }

        public void StartQuiz()
        {
            Debug.Log("StartQuiz");
            //quizStatusWindow.SetActive(true);
            //UpdateQuizStatus();
            behaviorRecorder.StartRecording();
            quizFinished = false;
        }

        public void FinishQuiz()
        {
            Debug.Log("FinishQuiz");
            behaviorRecorder.FinishRecording();
            quizFinished = true;

            timerManager.SetResultSceneTimer(timeToResultScene);
        }

        public void GoToResultScene()
        {
            SceneManager.LoadScene(resultSceneName);
        }

        public int GetCurrentStep()
        {
            return quizStatusManager.GetCurrentQuizNumber;
        }

        public void NextStep()
        {
            quizStatusManager.NextQuiz();

            if (IsQuizFinished)
                FinishQuiz();

            SetTimersToNull();
        }

        bool IsQuizFinished
        {
            get { return quizStatusManager.IsQuizFinished; }
        }

        public void TryAgain()
        {
            timerManager.SetAnswerTimer(timeOutTime);
        }

        public void WaitForAnswer()
        {
            timerManager.SetAnswerTimer(timeOutTime);
            SetHintTimer();
            OpenAnswerButton();
        }

        public void RobotMovementStart(string message)
        {
            robotMovementTime = Convert.ToSingle(message);
            //Debug.Log("robotMovementTime: " + robotMovementTime);

            // Remove Random.
            //if (UnityEngine.Random.Range(0, 2) == 0)
            if (robotMovementTime == -1f)
                return;

            timerManager.SetRobotMovementTimer(robotMovementTime);
        }

        public void RobotMove()
        {
            System.Random random = new System.Random();
            int direction = random.Next(0, 10);              // result < 5 -> right / result >= 5 -> left.
            if (direction < 5) robotMovement.MoveRight();
            else if (direction >= 5) robotMovement.MoveLeft();

            timerManager.robotmovementTimer.TurnOff();
            //robotMovementTimer = null;
        }

        public string GetQuizType { get { return Enum.Parse(typeof(SurveyType), quizType).ToString(); } }
        public string GetGender { get { return gender.Equals("m") ? "남성" : "여성"; } }
        public string GetAge { get { return age + "대"; } }

        private void CheckAnswerTimer()
        {
            if (SpeechRenderrer.Instance.IsSpeaking) return;

            if (QuizFinished)
            {
                SetTimersToNull();
            }

            if (timerManager.answerTimer.IsTurnedOn)
            {
                quizStatusManager.SetAnswerState(AnswerState.Wait);
                //Debug.Log("CheckAnswerTimer: " + answerTimer.GetElapsedTime);
            }
        }

        private void SetHintTimer()
        {
            if (!isCue)
                return;

            timerManager.SetHintTimer(hintTime);
        }

        public void OpenAnswerButton()
        {
            Vector3 pos = answerButton.GetComponent<RectTransform>().anchoredPosition;
            //pos.y = pos.y == answerYPosHigh ? answerYPosLow : answerYPosHigh;
            answerButton.GetComponent<RectTransform>().anchoredPosition = pos;

            // set random pos of O/X button (Left or Right).
            int random = UnityEngine.Random.Range(0, 11);
            if (random > 5)
            {
                answerButton.SetYesButtonOnLeft();
            }
            else
            {
                answerButton.SetYesButtonOnRight();
            }

            answerButton.gameObject.SetActive(true);
        }

        public void CloseAnswerButton()
        {
            answerButton.gameObject.SetActive(false);
            timerManager.answerTimer.TurnOff();
        }

        private string GetWrongAnswer
        {
            get { return quizStatusManager.GetWrongAnswer; }
        }

        public void TimeOut()
        {
            Debug.Log(timeOutTime + "초 지남. 문제 틀림");

            SetTimersToNull();

            quizStatusManager.SetAnswerState(AnswerState.Timeout);

            //GetReply(GetWrongAnswer);
            GetReply(timeoutString);
            CloseAnswerButton();
        }

        public void GazeToButton()
        {
            //Debug.Log("GazeToButton");

            timerManager.hintTimer.TurnOff();

            ButtonPosition yesButtonPos = answerButton.GetYesButtonPosition;
            bool isAnswerYes = quizStatusManager.IsAnswerYes;

            if (isActiveFace)
            {
                //Debug.Log(GetCurrentStep() + " 문제 표정 큐");
                SetGazeFace(isAnswerYes, yesButtonPos);       // 표정 큐
            }
            else
            {
                //Debug.Log(GetCurrentStep() + " 문제 모션 큐");
                SetGazeMotion(isAnswerYes, yesButtonPos);                  // 모션 큐
            }
        }

        private void SetGazeFace(bool isAnswerYes, ButtonPosition yesButtonPos)
        {
            string faceGaze = string.Empty;

            // 정답이 O/X인지 확인하고, 
            // 현재 O버튼이 왼쪽에 있는지 오른쪽에 있는지 확인해서 쳐다보도록 설정.
            if (isAnswerYes)
            {
                if (yesButtonPos == ButtonPosition.Left) faceGaze = faceGazeLeft;
                else if (yesButtonPos == ButtonPosition.Right) faceGaze = faceGazeRight;
            }
            else
            {
                if (yesButtonPos == ButtonPosition.Left) faceGaze = faceGazeRight;
                else if (yesButtonPos == ButtonPosition.Right) faceGaze = faceGazeLeft;
            }

            robotFacialRenderer.Play(faceGaze);
        }

        private void SetGazeMotion(bool isAnswerYes, ButtonPosition yesButtonPos)
        {
            string gazeMotion = string.Empty;
            if (isAnswerYes)
            {
                if (yesButtonPos == ButtonPosition.Left) gazeMotion = "nodLeft";
                else if (yesButtonPos == ButtonPosition.Right) gazeMotion = "nodRight";
            }
            else
            {
                if (yesButtonPos == ButtonPosition.Left) gazeMotion = "nodRight";
                else if (yesButtonPos == ButtonPosition.Right) gazeMotion = "nodLeft";
            }

            //Debug.Log(GetCurrentStep() + " 문제 힌트 모션: " + gazeMotion);
            transformController.PlayMotion(gazeMotion);
        }

        private void SetTimersToNull()
        {
            timerManager.answerTimer.TurnOff();
            timerManager.hintTimer.TurnOff();
            timerManager.robotmovementTimer.TurnOff();
        }

        private void OnDestroy()
        {
        }
    }
}