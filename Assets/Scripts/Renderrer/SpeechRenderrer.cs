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
    [SerializeField] private ContentState contentState;
    public ContentState GetContentState { get { return contentState; } }

    public SpeechInfo(string speechScript)
    {
        this.speechScript = speechScript;
        shouldGoNext = CheckShouldGoNextStep(speechScript);
        shouldWaitForAnswer = CheckShouldWaitForAnswer(speechScript);
        contentState = CheckContentState(speechScript);
    }

    bool CheckShouldGoNextStep(string speechScript)
    {
        return speechScript.Contains("정답")
            || speechScript.Contains("땡")
            || speechScript.Contains("초과")
            || speechScript.Contains("시작");
    }

    bool CheckShouldWaitForAnswer(string sppechScript)
    {
        return speechScript.Contains("문제")
            || speechScript.Contains("난이도");
    }

    ContentState CheckContentState(string speechScript)
    {
        if (WebSurvey.Instance.GetCurrentStep().Equals(0))
            return ContentState.IceBreaking;

        else if (speechScript.Contains("문제입니다")
            || speechScript.Contains("맞으면 오"))
            return ContentState.Asking;

        else if (speechScript.Contains("맞았습니다")
            || speechScript.Contains("틀리셨네요")
            || speechScript.Contains("지나버렸네요")
            || speechScript.Contains("정답")
            || speechScript.Contains("땡")
            || speechScript.Contains("초과"))
            return ContentState.Answering;

        return ContentState.Waiting;
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
        InitTTS();
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
        return IsSpeaking;
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

    private readonly string speakFaceName = "speak";
    public void Play(string speech)
    {
        //WebSurvey.Instance.robotFacialRenderer.Play(speakFaceName);
        currentSpeech = new SpeechInfo(speech);
        WebSurvey.Instance.SetContentState(currentSpeech.GetContentState);
        TextToSpeech(speech);
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

    void InitTTS()
    {
        voice = new SpVoiceClass();
        voice.Volume = 100;
        voice.Rate = 1;
    }

    private readonly string inSecondsString = "초안에";
    private readonly string inQuizCountString = "개에";
    void TextToSpeech(string ttsText)
    {
        isTTSStarted = true;

        string[] sentences = ttsText.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string sentence in sentences)
        {
            if (sentence.Contains(inSecondsString))
            {
                string targetString = ((int)WebSurvey.Instance.GetTimeoutTime).ToString() + inSecondsString;
                voice.Speak(
                    sentence.Replace(inSecondsString, targetString),
                    SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFIsXML
                );
                continue;
            }
            else if (sentence.Contains(inQuizCountString))
            {
                string targetString = ((int)WebSurvey.Instance.GetQuizCount).ToString() + inQuizCountString;
                voice.Speak(
                    sentence.Replace(inQuizCountString, targetString),
                    SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFIsXML
                );
                continue;
            }

            voice.Speak(sentence, SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFIsXML);
        }
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
            //Debug.LogWarning("Speak Finished");

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
                //Debug.Log(WebSurvey.Instance.GetCurrentStep() + " 문제 답변 대기");
                WebSurvey.Instance.WaitForAnswer();
            }

            currentSpeech = null;
        }
    }
}