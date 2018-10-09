    using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using UnityEngine.UI;
using FrostweepGames.Plugins.GoogleCloud.SpeechRecognition;

using REEL.Recorder;

public class WebSurvey : Singleton<WebSurvey>
{
    const string FILENAME = "/survey.txt";

    public InputField speechMessage;

    public REEL.Animation.RobotFacialRenderer robotFacialRenderer;
    public Text quizNumberText;
    public Text scoreText;

    private MqttClient mqttClient;
    string requested = "";
    string received_tts = "";
    string received_facial = "";
    string received_motion = "";

    public Text TTS;

    public BehaviorRecorder behaviorRecorder;
    public BehaviorReplayer behaviorReplayer;

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

    

    public void SendSTT()
    {
        mqttClient.Publish("/input", Encoding.UTF8.GetBytes(speechMessage.text));
    }

    public void WebLogin()
    {
        string uri = "http://localhost:3000/api/v1/login/reel";
        Dictionary<string, string> postHeader = new Dictionary<string, string>();
        //Hashtable postHeader = new Hashtable();
        postHeader.Add("Content-Type", "application/json");
        var formData = System.Text.Encoding.UTF8.GetBytes("{\"password\": \"1234\"}");
        WWW www = new WWW(uri, formData, postHeader);
        StartCoroutine(Login(www));
    }

    IEnumerator Login(WWW www)
    {
        yield return www;
        Debug.Log(www.text);
    }

    public void WebUpload()
    {
        StartCoroutine("Upload");
    }

    public void StartQuiz()
    {
        Debug.Log("StartQuiz");
        behaviorRecorder.StartRecording();
    }

    public void FinishQuiz()
    {
        Debug.Log("FinishQuiz");
        behaviorRecorder.FinishRecording();
    }

    public int GetCurrentStep()
    {
        return currentQuizNumber;
    }

    public void NextStep()
    {
        ++currentQuizNumber;
        if (currentQuizNumber < 4)
            quizNumberText.text = currentQuizNumber.ToString() + " / 3";
        Debug.Log("NextStep: " + currentQuizNumber);
    }

    public void GainScore()
    {
        ++quizScore;
        scoreText.text = quizScore.ToString();
        //Debug.Log("Score: " + quizScore);
    }

    public int GetCurrentScore()
    {
        return quizScore;
    }

    public string QuizTitle {  get { return quizTitle; } }

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

    IEnumerator Upload()
    {
        string filepath = Application.dataPath + "/movie_dummy.zip";
        if (File.Exists(filepath))
        {
            byte[] data = File.ReadAllBytes(filepath);
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            formData.Add(new MultipartFormFileSection("file", data, "movie_dummy.zip", "application/zip"));

            UnityWebRequest www = UnityWebRequest.Post("http://localhost:3000/api/v1/users/reel/projects/movie_dummy", formData);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
        else
        {
            Debug.Log("File not exist");
        }
    }

    public void WebPlay()
    {
        StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        string uri = "http://localhost:3000/api/v1/users/reel/projects/movie_dummy/play";
        UnityWebRequest www = UnityWebRequest.Get(uri);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
        }
    }

    private bool speechRecognitionStart;
    public void OnClickedSpeechRec(Text text)
    {
        if (!CanClick) return;

        if (!speechRecognitionStart)
        {
            _speechRecognition.StartRecord(false);

            text.text = "Recognizing...";
            speechRecognitionStart = true;
        }
        else
        {
            //ApplySpeechContextPhrases();
            _speechRecognition.StopRecord();

            text.text = "Speech Recognition";
            speechRecognitionStart = false;
        }
    }

    bool CanClick
    {
        get { return !SpeechRenderrer.Instance.IsSpeaking; }
    }

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

            //try
            //{
            //    var r1 = _rs.reply("default", "init");
            //    Debug.Log(string.Format("{0}", r1));
            //}
            //catch (System.Exception ex)
            //{
            //    Debug.Log(string.Format("{0}", ex));
            //}
        }
        else
        {
            Debug.Log("Fail to load " + Application.persistentDataPath + FILENAME + " file");
        }
    }

    // Update is called once per frame
    void Update()
    {
//        if (received_tts.Length > 0)
//        {
//#if UNITY_EDITOR || UNITY_STANDALONE
//            //StartCoroutine(Say(received_tts));
//#elif UNITY_ANDROID
//            AndroidJNI.AttachCurrentThread();
//            SpeechRenderrer.Instance.Play(received_tts);
//            AndroidJNI.DetachCurrentThread();
//#endif
//            TTS.text = received_tts;
//            received_tts = "";
//        }
//        if (received_facial.Length > 0)
//        {
//            robotFacialRenderer.Play(received_facial);
//            received_facial = "";
//        }
//        if (received_motion.Length > 0)
//        {
//            //REEL.PoseAnimation.RobotTransformController.Instance.PlayGesture(received_motion);
//            REEL.PoseAnimation.RobotTransformController.Instance.PlayMotionCoroutine(received_motion);
//            received_motion = "";
//        }
    }

    private void OnDestroy()
    {
        _speechRecognition.RecognitionSuccessEvent -= RecognitionSuccessEventHandler;
        _speechRecognition.NetworkRequestFailedEvent -= SpeechRecognizedFailedEventHandler;
        _speechRecognition.LongRecognitionSuccessEvent -= LongRecognitionSuccessEventHandler;
    }

    void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        String topic = e.Topic;
        String message = System.Text.Encoding.UTF8.GetString(e.Message);
        Debug.Log("Topic: " + topic + ", Message: " + message);
        switch (topic)
        {
            case "/tts":
                received_tts = message;
                break;
            case "/facial":
                received_facial = message;
                break;
            case "/motion":
                received_motion = message;
                break;
        }
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

            //string other = "\nDetected alternative: ";

            //foreach (var result in obj.results)
            //{
            //    foreach (var alternative in result.alternatives)
            //    {
            //        if (obj.results[0].alternatives[0] != alternative)
            //            other += alternative.transcript + ", ";
            //    }
            //}
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