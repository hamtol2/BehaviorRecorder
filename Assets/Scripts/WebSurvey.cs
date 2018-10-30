using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using FrostweepGames.Plugins.GoogleCloud.SpeechRecognition;

using REEL.Recorder;
using UnityEngine.SceneManagement;

public class WebSurvey : Singleton<WebSurvey>
{
    public enum AnswerType { O, X }
    public enum Mode { Active, Inactive, None }

    [SerializeField] private TextAsset surveyTypeGA;
    [SerializeField] private TextAsset surveyTypeNA;
    [SerializeField] private TextAsset surveyTypeDA;
    [SerializeField] private TextAsset surveyTypeRA;

    public REEL.Animation.RobotFacialRenderer robotFacialRenderer;
    public REEL.PoseAnimation.RobotTransformController transformController;
    public GameObject quizStatusWindow;
    public Text quizNumberText;
    public Text scoreText;

    public RobotMovement robotMovement;
    public BehaviorRecorder behaviorRecorder;
    public BehaviorReplayer behaviorReplayer;

    [SerializeField] private AnswerButton answerButton;

    RiveScript.RiveScript riveScript;

    private AudioSource audioSource;
    [SerializeField] private int currentQuizNumber = 0;
    [SerializeField] private int quizScore = 0;
    [SerializeField] private string quizTitle = "OX-Quiz";
    [SerializeField] private ContentState contentState = ContentState.IceBreaking;
    [SerializeField] private AnswerState answerState = AnswerState.Wait;
    [SerializeField] private ModelType robotModelType = ModelType.ExpressionRobot;

    private Timer answerTimer = null;
    private Timer hintTimer = null;
    private Timer robotMovementTimer = null;
    private Timer resultSceneTimer = null;
    [SerializeField] private float timeOutTime = 10f;
    [SerializeField] private float hintTime = 0f;
    [SerializeField] private float robotMovementTime = 0f;

    private readonly float timeToResultScene = 5f;

    private bool quizFinished = false;
    public bool QuizFinished { get { return quizFinished; } }

    [SerializeField] private int numOfQuiz = 0;
    private AnswerType currentAnswerType;

    private bool isActiveFace = false;          // 고정된 표정 / 변화하는 표정 여부.
    private bool isCue = false;                 // Cue 줄지 여부.

    private readonly string timeoutString = "timeout";

    private readonly string answerYes = "오";
    private readonly string answerNo = "엑스";

    private readonly string faceGazeLeft = "gazeleft";
    private readonly string faceGazeRight = "gazeright";

    private readonly float answerYPosHigh = 150f;
    private readonly float answerYPosLow = 50f;

    private Mode behaviorMode = Mode.None;

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
        SpeechRenderrer.Instance.Init();

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

        riveScript = new RiveScript.RiveScript(utf8: true, debug: true);

        quizType = PlayerPrefs.GetString(SurveyUtil.surveyTypeKey);
        age = PlayerPrefs.GetString(SurveyUtil.ageKey);
        gender = PlayerPrefs.GetString(SurveyUtil.genderKey);

        SurveyType surveyType = (SurveyType)Enum.Parse(typeof(SurveyType), quizType);
        TextAsset riveScriptTextAsset = null;
        switch (surveyType)
        {
            case SurveyType.TypeGA: riveScriptTextAsset = surveyTypeGA; break;
            case SurveyType.TypeNA: riveScriptTextAsset = surveyTypeNA; break;
            case SurveyType.TypeDA: riveScriptTextAsset = surveyTypeDA; break;
            case SurveyType.TypeRA: riveScriptTextAsset = surveyTypeRA; break;
            default: riveScriptTextAsset = null; break;
        }

        if (riveScript.LoadTextAsset(riveScriptTextAsset))
        {
            riveScript.sortReplies();
            Debug.Log("Successfully load file: " + riveScriptTextAsset.name);
        }
        else
        {
            Debug.Log("Fail to load RiveScript, SurveyType: " + quizType);
        }
    }

    public void GetReply(string message)
    {
        //string reply = riveScript.reply("default", message);
        string reply = riveScript.reply("REEL", message);
        if (reply.Contains("NOT_MATCHED"))
        {
            Debug.Log("Not matched");
        }
        else
        {
            Arbitor.Instance.Insert(reply);
        }
   }

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
        behaviorMode = (Mode)Enum.Parse(typeof(Mode), message);

        if (behaviorMode == Mode.Active)
            robotModelType = ModelType.ExpressionRobot;
        else if (behaviorMode == Mode.Inactive)
            robotModelType = ModelType.NonExpressionRobot;
    }

    public Mode GetBehaviorMode { get { return behaviorMode; } }

    public void SetCueState(string message)
    {
        isCue = Convert.ToBoolean(message);
    }
    public bool GetCueState { get { return isCue; } }

    public void SetFaceActiveState(string message)
    {
        isActiveFace = Convert.ToBoolean(message);
    }
    public bool GetFaceActiveState {  get { return isActiveFace; } }

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

        resultSceneTimer = new Timer(timeToResultScene, GoToResultScene);
    }

    void GoToResultScene()
    {
        SceneManager.LoadScene(resultSceneName);
    }

    public int GetCurrentStep()
    {
        return currentQuizNumber;
    }

    public void NextStep()
    {
        ++currentQuizNumber;

        if (IsQuizFinished)
            FinishQuiz();

        SetTimersToNull();
    }

    bool IsQuizFinished
    {
        get { return currentQuizNumber == (numOfQuiz + 1); }
    }

    public void TryAgain()
    {
        answerTimer = new Timer(timeOutTime, TimeOut);
    }

    public void WaitForAnswer()
    {
        answerTimer = new Timer(timeOutTime, TimeOut);
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

        robotMovementTimer = new Timer(robotMovementTime, RobotMove);
    }

    void RobotMove()
    {
        System.Random random = new System.Random();
        int direction = random.Next(0, 10);              // result < 5 -> right / result >= 5 -> left.
        if (direction < 5) robotMovement.MoveRight();
        else if (direction >= 5) robotMovement.MoveLeft();

        robotMovementTimer = null;
    }

    void UpdateQuizStatus()
    {
        quizNumberText.text = currentQuizNumber.ToString() + " / " + numOfQuiz.ToString();
    }

    public void GainScore()
    {
        ++quizScore;
        scoreText.text = quizScore.ToString();
        answerState = AnswerState.Correct;
        //Debug.Log("Score: " + quizScore);
    }

    public int GetCurrentScore()
    {
        return quizScore;
    }

    public string QuizTitle { get { return quizTitle; } }
    public string GetQuizType { get { return Enum.Parse(typeof(SurveyType), quizType).ToString(); } }
    public string GetGender { get { return gender.Equals("m") ? "남성" : "여성"; } }
    public string GetAge { get { return age + "대"; } }

    public ContentState GetCurrentState()
    {
        if (currentQuizNumber.Equals(0))
            contentState = ContentState.IceBreaking;
        else
        {
            if (SpeechRenderrer.Instance.IsRunning()) contentState = ContentState.OnQuestion;
            else contentState = ContentState.Waiting;
        }

        return contentState;
    }

    public ModelType GetModelType { get { return robotModelType; } }

    public void SetAnswerState(AnswerState state)
    {
        answerState = state;
    }
    public AnswerState GetAnswerState { get { return answerState; } }

    private void CheckAnswerTimer()
    {
        if (robotMovementTimer != null)
        {
            robotMovementTimer.Update(Time.deltaTime);
        }

        if (SpeechRenderrer.Instance.IsSpeaking) return;

        if (QuizFinished)
        {
            SetTimersToNull();
        }

        if (hintTimer != null)
        {
            hintTimer.Update(Time.deltaTime);
        }

        if (answerTimer != null)
        {
            SetAnswerState(AnswerState.Wait);
            //Debug.Log("CheckAnswerTimer: " + answerTimer.GetElapsedTime);
            answerTimer.Update(Time.deltaTime);
        }

        if (quizFinished && resultSceneTimer != null)
        {
            resultSceneTimer.Update(Time.deltaTime);
        }
    }

    private void SetHintTimer()
    {
        if (!isCue)
            return;

        hintTimer = new Timer(hintTime, GazeToButton);
    }

    public void OpenAnswerButton()
    {
        Vector3 pos = answerButton.GetComponent<RectTransform>().anchoredPosition;
        pos.y = pos.y == answerYPosHigh ? answerYPosLow : answerYPosHigh;
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
    }

    private string GetWrongAnswer
    {
        get { return currentAnswerType == AnswerType.O ? answerNo : answerYes; }
    }
    
    private void TimeOut()
    {
        Debug.Log(timeOutTime + "초 지남. 문제 틀림");

        SetTimersToNull();

        answerState = AnswerState.Timeout;

        //GetReply(GetWrongAnswer);
        GetReply(timeoutString);
        CloseAnswerButton();
    }

    private void GazeToButton()
    {
        //Debug.Log("GazeToButton");

        hintTimer = null;

        ButtonPosition yesButtonPos = answerButton.GetYesButtonPosition;
        bool isAnswerYes = currentAnswerType == AnswerType.O;

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
        answerTimer = null;
        hintTimer = null;
        robotMovementTimer = null;
    }

    private void OnDestroy()
    {
    }
}