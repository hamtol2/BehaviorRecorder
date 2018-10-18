using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using FrostweepGames.Plugins.GoogleCloud.SpeechRecognition;

using REEL.Recorder;

public class WebSurvey : Singleton<WebSurvey>
{
    public enum AnswerType
    {
        O, X
    }

    [SerializeField] private TextAsset surveyScript;
    public REEL.Animation.RobotFacialRenderer robotFacialRenderer;
    public GameObject quizStatusWindow;
    public Text quizNumberText;
    public Text scoreText;

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
    [SerializeField] private float timeOutTime = 10f;
    [SerializeField] private float hintTime = 0f;

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

        if (riveScript.LoadTextAsset(surveyScript))
        {
            riveScript.sortReplies();
            Debug.Log("Successfully load file");
        }
        else
        {
            Debug.Log("Fail to load " + surveyScript.name + " file");
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

        //if (currentQuizNumber < (numOfQuiz + 1))
        //    UpdateQuizStatus();

        answerTimer = null;
        //Debug.Log("NextStep: " + currentQuizNumber);
    }

    public void TryAgain()
    {
        answerTimer = new Timer(timeOutTime, TimeOut);
    }

    private readonly string normalFaceName = "normal";
    public void WaitForAnswer()
    {
        answerTimer = new Timer(timeOutTime, TimeOut);
        hintTimer = new Timer(hintTime, GazeToButton);
        OpenAnswerButton();
        robotFacialRenderer.Play(normalFaceName);
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
        if (SpeechRenderrer.Instance.IsSpeaking)
            return;

        if (QuizFinished)
        {
            answerTimer = null;
            hintTimer = null;
        }
        
        if (hintTimer != null)
        {
            hintTimer.Update(Time.deltaTime);
        }

        if (answerTimer != null)
        {
            SetAnswerState(AnswerState.Wait);
            Debug.Log("CheckAnswerTimer: " + answerTimer.GetElapsedTime);
            answerTimer.Update(Time.deltaTime);
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
        answerTimer = null;
        GetReply(GetWrongAnswer);
        CloseAnswerButton();
    }
    
    private void GazeToButton()
    {
        robotFacialRenderer.Play(currentAnswerType == AnswerType.O ? faceGazeO : faceGazeX);
    }

    private void OnDestroy()
    {
    }
}