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

    const string FILENAME = "/survey.txt";

    public REEL.Animation.RobotFacialRenderer robotFacialRenderer;
    public GameObject quizStatusWindow;
    public Text quizNumberText;
    public Text scoreText;

    public BehaviorRecorder behaviorRecorder;
    public BehaviorReplayer behaviorReplayer;

    [SerializeField] private GameObject answerButton;

    private Hashtable session_ident = new Hashtable();

    //RiveScript.RiveScript _rs = new RiveScript.RiveScript(utf8: true, debug: true);
    RiveScript.RiveScript _rs;

    private GCSpeechRecognition _speechRecognition;

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

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("UUID"))
        {
            PlayerPrefs.SetString("UUID", System.Guid.NewGuid().ToString());
        }

        _rs = new RiveScript.RiveScript(utf8: true, debug: true);
    }

    // Use this for initialization
    void Start()
    {
        SpeechRenderrer.Instance.Init();

        // Google Cloud Speech Recognition
        _speechRecognition = GCSpeechRecognition.Instance;
        _speechRecognition.SetLanguage(Enumerators.LanguageCode.ko_KR);
        _speechRecognition.RecognitionSuccessEvent += RecognitionSuccessEventHandler;
        _speechRecognition.NetworkRequestFailedEvent += SpeechRecognizedFailedEventHandler;
        _speechRecognition.LongRecognitionSuccessEvent += LongRecognitionSuccessEventHandler;

        // rivescript
#if UNITY_EDITOR || UNITY_WEBGL || UNITY_STANDALONE
        Debug.Log("UNITY_EDITOR");
        string filepath = Application.dataPath + FILENAME;
#elif UNITY_ANDROID
        Debug.Log("UNITY_ANDROID");
        string filepath = Application.persistentDataPath + FILENAME;
#elif UNITY_IOS
        Debug.Log("UNITY_IOS");
        string filepath = Application.persistentDataPath + FILENAME;
#endif
        //Debug.Log("filepath: " + filepath);
        if (_rs.loadFile(filepath))
        {
            _rs.sortReplies();
            Debug.Log("Successfully load file");

        }
        else
        {
            Debug.Log("Fail to load " + Application.persistentDataPath + FILENAME + " file");
        }
    }

    private void Update()
    {
        CheckAnswerTimer();
    }

    public void GetReply(string message)
    {
        var reply = _rs.reply("default", message);
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
        quizStatusWindow.SetActive(true);
        UpdateQuizStatus();
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

        if (currentQuizNumber < (numOfQuiz + 1))
            UpdateQuizStatus();

        answerTimer = null;
        //Debug.Log("NextStep: " + currentQuizNumber);
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

    private bool speechRecognitionStart;

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
        WebSurvey.Instance.GetReply(GetWrongAnswer);
        CloseAnswerButton();
    }

    private string faceGazeO = "gazeo";
    private string faceGazeX = "gazex";
    private void GazeToButton()
    {
        robotFacialRenderer.Play(currentAnswerType == AnswerType.O ? faceGazeO : faceGazeX);
    }

    private void OnDestroy()
    {
        _speechRecognition.RecognitionSuccessEvent -= RecognitionSuccessEventHandler;
        _speechRecognition.NetworkRequestFailedEvent -= SpeechRecognizedFailedEventHandler;
        _speechRecognition.LongRecognitionSuccessEvent -= LongRecognitionSuccessEventHandler;
    }

    private void SpeechRecognizedFailedEventHandler(string obj, long requestIndex)
    {
        Debug.Log("SpeechRecognizedFailedEventHandler: " + obj);
        SpeechRenderrer.Instance.TryAgain();
    }

    private void RecognitionSuccessEventHandler(RecognitionResponse obj, long requestIndex)
    {
        Debug.Log("RecognitionSuccessEventHandler: " + obj);
        if (obj != null && obj.results.Length > 0)
        {
            Debug.Log("Speech Recognition succeeded! Detected Most useful: " + obj.results[0].alternatives[0].transcript);

            var words = obj.results[0].alternatives[0].words;

            if (words != null)
            {
                string times = string.Empty;

                foreach (var item in obj.results[0].alternatives[0].words)
                    times += "<color=green>" + item.word + "</color> -  start: " + item.startTime + "; end: " + item.endTime + "\n";

                Debug.Log(times);

                var reply = _rs.reply("default", words[0].word);
                if (reply.Contains("NOT_MATCHED"))
                {
                    Debug.Log("Not matched");
                }
                else
                {
                    Arbitor.Instance.Insert(reply);
                }
            }
        }
        else
        {
            Debug.Log("Speech Recognition succeeded! Words are no detected.");
        }
    }

    private void LongRecognitionSuccessEventHandler(OperationResponse operation, long index)
    {
        Debug.Log("LongRecognitionSuccessEventHandler: " + operation);
        if (operation != null && operation.response.results.Length > 0)
        {
            Debug.Log("Long Speech Recognition succeeded! Detected Most useful: " + operation.response.results[0].alternatives[0].transcript);

            string other = "\nDetected alternative: ";

            foreach (var result in operation.response.results)
            {
                foreach (var alternative in result.alternatives)
                {
                    if (operation.response.results[0].alternatives[0] != alternative)
                        other += alternative.transcript + ", ";
                }
            }

            Debug.Log(other);
        }
        else
        {
            Debug.Log("Speech Recognition succeeded! Words are no detected.");
        }
    }
}