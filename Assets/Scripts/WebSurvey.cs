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
    public REEL.Animation.RobotFacialRenderer robotFacialRenderer;
    public REEL.PoseAnimation.RobotTransformController transformController;
    public GameObject quizStatusWindow;
    public Text quizNumberText;
    public Text scoreText;

    public RobotMovement robotMovement;
    public BehaviorRecorder behaviorRecorder;
    public BehaviorReplayer behaviorReplayer;

    [SerializeField] private GameObject answerButton;

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

    private bool quizFinished = false;
    public bool QuizFinished { get { return quizFinished; } }

    [SerializeField] private int numOfQuiz = 0;
    private AnswerType currentAnswerType;

    private readonly string answerYes = "오";
    private readonly string answerNo = "엑스";

    private readonly string faceGazeO = "gazeo";
    private readonly string faceGazeX = "gazex";

    private readonly float answerYPosHigh = 150f;
    private readonly float answerYPosLow = 50f;

    private Mode behaviorMode = Mode.None;

    private readonly string surveyTypeKey = "surveyType";

    [SerializeField] private string resultSceneName = string.Empty;

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("UUID"))
        {
            PlayerPrefs.SetString("UUID", System.Guid.NewGuid().ToString());
        }

        riveScript = new RiveScript.RiveScript(utf8: true, debug: true);
    }

    // Use this for initialization
    void Start()
    {
        SpeechRenderrer.Instance.Init();

        Debug.Log(PlayerPrefs.GetString(surveyTypeKey));

        TextAsset riveScriptTextAsset 
            = PlayerPrefs.GetString(surveyTypeKey) 
            == SurveyTypeSelectButton.SurveyType.TypeGA.ToString() ? surveyTypeGA : surveyTypeNA;

        if (riveScript.LoadTextAsset(riveScriptTextAsset))
        {
            riveScript.sortReplies();
            Debug.Log("Successfully load file: " + riveScriptTextAsset.name);
        }
        else
        {
            Debug.Log("Fail to load " + riveScriptTextAsset.name + " file");
        }
    }

    private void Update()
    {
        CheckAnswerTimer();
    }

    public void GetReply(string message)
    {
        var reply = riveScript.reply("default", message);
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

    public void SetHintTime(string message)
    {
        hintTime = Convert.ToSingle(message);
    }

    public void SetBehaviorMode(string message)
    {
        behaviorMode = (Mode)Enum.Parse(typeof(Mode), message);
    }

    public Mode GetBehaviorMode { get { return behaviorMode; } }

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

        resultSceneTimer = new Timer(10f, GoToResultScene);
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

        if (currentQuizNumber == (numOfQuiz + 1))
           FinishQuiz();

        SetTimersToNull();
    }

    public void TryAgain()
    {
        answerTimer = new Timer(timeOutTime, TimeOut);
    }

    public void WaitForAnswer()
    {
        answerTimer = new Timer(timeOutTime, TimeOut);
        hintTimer = new Timer(hintTime, GazeToButton);
        OpenAnswerButton();
    }

    public void RobotMovementStart(string message)
    {
        robotMovementTime = Convert.ToSingle(message);

        //Debug.LogWarning("RobotMovementStart: " + robotMovementTime);
        
        if (UnityEngine.Random.Range(0, 2) == 0)
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

    public void OpenAnswerButton()
    {
        Vector3 pos = answerButton.GetComponent<RectTransform>().anchoredPosition;
        pos.y = pos.y == answerYPosHigh ? answerYPosLow : answerYPosHigh;
        answerButton.GetComponent<RectTransform>().anchoredPosition = pos;

        answerButton.SetActive(true);
    }

    public void CloseAnswerButton()
    {
        answerButton.SetActive(false);
    }

    private string GetWrongAnswer
    {
        get { return currentAnswerType == AnswerType.O ? answerNo : answerYes; }
    }

    private void TimeOut()
    {
        Debug.Log(timeOutTime + "초 지남. 문제 틀림");

        SetTimersToNull();

        GetReply(GetWrongAnswer);
        CloseAnswerButton();
    }
    
    private void GazeToButton()
    {
        Debug.Log("GazeToButton");

        hintTimer = null;

        robotFacialRenderer.Play(currentAnswerType == AnswerType.O ? faceGazeO : faceGazeX);

        if (behaviorMode == Mode.Active)
            transformController.PlayMotion(currentAnswerType == AnswerType.O ? "nodRight" : "nodLeft");
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