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
    [SerializeField] private Button speechRecognitionButton;

    private SpVoiceClass voice;
    private string tryAgainScript = "잘 못알아 들었습니다. 다시 말씀해 주세요.";
    private bool isTTSStarted = false;
    [SerializeField] private SpeechInfo currentSpeech;

    [SerializeField] private Text wordEventText;

    void Awake()
    {
        voice = new SpVoiceClass();
        
        //Debug.Log(PlayerPrefs.GetString(SurveyStart.ageKey));
        //Debug.Log(PlayerPrefs.GetString(SurveyStart.genderKey));
        //Debug.Log(PlayerPrefs.GetInt(SurveyStart.countKey));
    }

    void Update()
    {
        CheckAudioPlayState();
    }

    void GetStatus(SpVoiceClass voice)
    {
        if (voice == null) return;

        SPVOICESTATUS status;
        string bookmark;
        voice.GetStatus(out status, out bookmark);
        print(status.ulInputWordPos + " / " + status.ulInputWordPos + " / " + status.ulInputSentLen);
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
        WebSurvey.Instance.TryAgain();
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

            if (currentSpeech == null)
            {
                return;
            }   

            if (currentSpeech.shouldGoNext)
            {
                WebSurvey.Instance.NextStep();
            }

            if (currentSpeech.shouldWaitForAnswer && !WebSurvey.Instance.QuizFinished)
            {
                WebSurvey.Instance.WaitForAnswer();
            }

            currentSpeech = null;
        }
    }
}