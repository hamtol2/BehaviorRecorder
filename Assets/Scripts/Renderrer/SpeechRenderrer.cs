using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SpeechLib;

using REEL.Recorder;

[Serializable]
class SpeechInfo
{
    public string speechScript;
    public bool shouldGoNext;
    public bool shouldWaitForAnswer;
    //public bool isTryAgain;

    public SpeechInfo(string speechScript)
    {
        this.speechScript = speechScript;
        shouldGoNext = speechScript.Contains("정답")
            || speechScript.Contains("땡")
            || speechScript.Contains("시작")
            || speechScript.Contains("안녕");
        shouldWaitForAnswer = speechScript.Contains("문제")
            || speechScript.Contains("난이도");
        //isTryAgain = speechScript.Contains("다시 말씀해 주세요")
        //    || speechScript.Contains("대답해주세요");
    }
}

public class SpeechRenderrer : Singleton<SpeechRenderrer>, Renderrer
{
    [SerializeField] private AudioClip quizStartClip;
    [SerializeField] private AudioClip tryAgainClip;
    [SerializeField] private Button speechRecognitionButton;
    [SerializeField] private GameObject answerButton;

    private SpVoiceClass voice;
    private string tryAgainScript = "잘 못알아 들었습니다. 다시 말씀해 주세요.";
    private bool isTTSStarted = false;
    [SerializeField] private SpeechInfo currentSpeech;
    private Timer timer = null;
    [SerializeField] private float timeOutTime = 10f;
    private string timeoutReply = "timeout";

    void Awake()
    {
        voice = new SpVoiceClass();
        timer = null;

        //Debug.Log(PlayerPrefs.GetString(SurveyStart.ageKey));
        //Debug.Log(PlayerPrefs.GetString(SurveyStart.genderKey));
        //Debug.Log(PlayerPrefs.GetInt(SurveyStart.countKey));
    }

    void Start()
    {

    }

    void Update()
    {
        CheckAudioPlayState();
        CheckAnswerTimer();
    }

    public void Init()
    {

    }

    public bool IsRunning()
    {
        return false;
    }

    public void OnDisable()
    {
        if (voice != null)
            TTSStop();
    }

    public void TryAgain()
    {
        TextToSpeech(tryAgainScript);
        timer = new Timer(timeOutTime, TimeOut);
    }

    public void Play(string speech)
    {
        TextToSpeech(speech);

        currentSpeech = new SpeechInfo(speech);
    }

    public void Stop()
    {
    }

    public bool IsSpeaking
    {
        get { return isTTSStarted && voice.Status.RunningState == SpeechRunState.SRSEIsSpeaking; }
    }

    bool IsFinished
    {
        get { return isTTSStarted && voice.Status.RunningState == SpeechRunState.SRSEDone; }
    }

    void TextToSpeech(string ttsText)
    {
        voice.Volume = 100;
        voice.Rate = 1;
        //voice.Speak(speakHeader + ttsText + "</speak>", SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFIsXML);
        voice.Speak(ttsText, SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFIsXML);

        isTTSStarted = true;
        //speechRecognitionButton.interactable = false;
    }

    void TTSStop()
    {
        voice.Speak(string.Empty, SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
    }

    private void CheckAudioPlayState()
    {
        // check audio play state.
        if (IsFinished)
        {
            Debug.Log("Speak Finished");

            isTTSStarted = false;
            //speechRecognitionButton.interactable = true;

            if (currentSpeech == null)
            {
                timer = null;
                return;
            }   

            if (currentSpeech.shouldGoNext)
            {
                WebSurvey.Instance.NextStep();
                timer = null;
            }

            if (currentSpeech.shouldWaitForAnswer && !WebSurvey.Instance.QuizFinished)
            {
                timer = new Timer(timeOutTime, TimeOut);
                answerButton.SetActive(true);
            }

            //if (WebSurvey.Instance.GetCurrentScore() == 4)
            //{
            //    WebSurvey.Instance.FinishQuiz();
            //    timer = null;
            //}

            currentSpeech = null;
        }
    }

    private void CheckAnswerTimer()
    {
        if (IsSpeaking)
            return;

        if (WebSurvey.Instance.QuizFinished)
            timer = null;

        if (timer != null)
        {
            WebSurvey.Instance.SetAnswerState(AnswerState.Wait);
            Debug.Log("CheckAnswerTimer: " + timer.GetElapsedTime);
            timer.Update(Time.deltaTime);
        }
    }

    private void TimeOut()
    {
        Debug.Log(timeOutTime + "초 지남. 문제 틀림");
        timer = null;
        WebSurvey.Instance.GetReply(timeoutReply);
    }
}