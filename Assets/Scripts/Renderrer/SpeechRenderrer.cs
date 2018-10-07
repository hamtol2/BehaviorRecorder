using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using REEL.Recorder;

public class SpeechRenderrer : Singleton<SpeechRenderrer>, Renderrer
{
    [SerializeField] private AudioClip quizStartClip;
    [SerializeField] private AudioClip tryAgainClip;
    [SerializeField] private Button speechRecognitionButton;

    private bool isPlaying = false;
    AndroidJavaClass ttsPlugin;
    List<string> _speeches = new List<string>();

    private AudioSource audioSource;
    private Queue<SpeechInfo> speechQueue = new Queue<SpeechInfo>();
    private SpeechInfo currentSpeechInfo;

    // Test.
    int fileIndex = 1;

    void Awake()
    {
        if (!audioSource)
            audioSource = GetComponent<AudioSource>();

        //Debug.Log(PlayerPrefs.GetString(SurveyStart.ageKey));
        //Debug.Log(PlayerPrefs.GetString(SurveyStart.genderKey));
        //Debug.Log(PlayerPrefs.GetInt(SurveyStart.countKey));
    }

    public void Init()
    {

    }

    public void QuizStart()
    {
        currentSpeechInfo = new SpeechInfo("시작", quizStartClip);
        ImmediatePlay(currentSpeechInfo);
    }

    public void TryAgain()
    {
        currentSpeechInfo = new SpeechInfo("다시 말씀해주세요.", tryAgainClip);
        ImmediatePlay(currentSpeechInfo);
    }

    void ImmediatePlay(SpeechInfo info)
    {
        audioSource.clip = info.speechClip;
        audioSource.Play();

        isPlaying = true;
        speechRecognitionButton.interactable = false;
    }

    public void Play(string speech)
    {
#if UNITY_EDITOR
        StartCoroutine(Say(speech));
#elif UNITY_ANDROID
        ttsPlugin.CallStatic ("speak", name);
#endif
    }

    public void Stop()
    {
#if UNITY_EDITOR
#elif UNITY_ANDROID
        ttsPlugin.CallStatic ("stopTTS");
#endif
    }

    public bool IsRunning()
    {
#if UNITY_EDITOR
        return false;
#elif UNITY_ANDROID
        return ttsPlugin.CallStatic<bool> ("isSpeaking");
#else
        return false;
#endif
    }

    public bool IsSpeaking()
    {
        //Debug.Log("clip length: " + audioSource.clip.length);
        return audioSource.isPlaying;
    }

    void Start()
    {
#if UNITY_EDITOR
#elif UNITY_ANDROID
        AndroidJavaClass androidPlugins = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject mainActivity = androidPlugins.GetStatic<AndroidJavaObject>("currentActivity");

        ttsPlugin = new AndroidJavaClass ("kr.ac.hansung.eai.TTSPlugin");
        ttsPlugin.CallStatic ("initTTS", mainActivity);
#endif
    }

    IEnumerator Say(string speech)
    {
        File.Delete(Application.dataPath + "/AudioClip/tts.mp3");

        string url = "https://naveropenapi.apigw.ntruss.com/voice/v1/tts";
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Headers.Add("X-NCP-APIGW-API-KEY-ID", "4lk8cmcq67");
        request.Headers.Add("X-NCP-APIGW-API-KEY", "Dnv1bksb2Trwh7DIbahih3QxFR9FOtAEdN1fPZz2");
        request.Method = "POST";
        byte[] byteDataParams = Encoding.UTF8.GetBytes("speaker=jinho&speed=0&text=" + speech);
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = byteDataParams.Length;
        Stream st = request.GetRequestStream();
        st.Write(byteDataParams, 0, byteDataParams.Length);
        st.Close();
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        string status = response.StatusCode.ToString();
        //Debug.Log("status=" + status);

        string filePath = Application.dataPath + "/AudioClip/tts" + fileIndex + ".mp3";
        ++fileIndex;
        using (Stream output = File.OpenWrite(filePath))
        using (Stream input = response.GetResponseStream())
        {
            input.CopyTo(output);
        }

        //Debug.Log(filePath + " was created");

        WWW www = new WWW("file://" + filePath);
        while (www.isDone)
        {
            yield return null;
        }

        byte[] mp3bytes = File.ReadAllBytes(filePath);
        //audioSource.clip = GetAudioClipFromMP3ByteArray(mp3bytes);
        //audioSource.Play();

        //SpeechInfo newSpeech = new SpeechInfo()
        AudioClip mp3Clip = GetAudioClipFromMP3ByteArray(mp3bytes);
        SpeechInfo newSpeech = new SpeechInfo(speech, mp3Clip);
        AddToAudioQueue(newSpeech);

        yield return null;
    }

    void Update()
    {
        // check audio play state.
        if (isPlaying && !audioSource.isPlaying)
        {
            isPlaying = audioSource.isPlaying;
            speechRecognitionButton.interactable = true;

            if (currentSpeechInfo.shouldGoNext)
                WebSurvey.Instance.NextStep();

            if (WebSurvey.Instance.GetCurrentStep() == 4)
                WebSurvey.Instance.FinishQuiz();

            if (speechQueue.Count > 0)
            {
                currentSpeechInfo = speechQueue.Dequeue();
                ImmediatePlay(currentSpeechInfo);
                //audioSource.clip = currentSpeechInfo.speechClip;
                //audioSource.Play();

                //isPlaying = true;
            }
        }
    }

    private void AddToAudioQueue(SpeechInfo newClip)
    {
        Debug.Log("AddToAudioQueue: " + newClip.speechScript);
        speechQueue.Enqueue(newClip);

        if (isPlaying)
        {   
            return;
        }

        currentSpeechInfo = speechQueue.Dequeue();
        audioSource.clip = currentSpeechInfo.speechClip;
        audioSource.Play();

        isPlaying = true;
    }

    private AudioClip GetAudioClipFromMP3ByteArray(byte[] in_aMP3Data)
    {
        AudioClip l_oAudioClip = null;
        Stream l_oByteStream = new MemoryStream(in_aMP3Data);
        MP3Sharp.MP3Stream l_oMP3Stream = new MP3Sharp.MP3Stream(l_oByteStream);

        //Get the converted stream data
        MemoryStream l_oConvertedAudioData = new MemoryStream();
        byte[] l_aBuffer = new byte[4096];
        int l_nBytesReturned = -1;
        int l_nTotalBytesReturned = 0;

        while (l_nBytesReturned != 0)
        {
            l_nBytesReturned = l_oMP3Stream.Read(l_aBuffer, 0, l_aBuffer.Length);
            l_oConvertedAudioData.Write(l_aBuffer, 0, l_nBytesReturned);
            l_nTotalBytesReturned += l_nBytesReturned;
        }

        //Debug.Log("MP3 file has " + l_oMP3Stream.ChannelCount + " channels with a frequency of " + l_oMP3Stream.Frequency);

        byte[] l_aConvertedAudioData = l_oConvertedAudioData.ToArray();
        //Debug.Log("Converted Data has " + l_aConvertedAudioData.Length + " bytes of data");

        //Convert the byte converted byte data into float form in the range of 0.0-1.0
        float[] l_aFloatArray = new float[l_aConvertedAudioData.Length / 2];

        for (int i = 0; i < l_aFloatArray.Length; i++)
        {
            //if (BitConverter.IsLittleEndian)
            //{
            //    //Evaluate earlier when pulling from server and/or local filesystem - not needed here
            //    Array.Reverse( l_aConvertedAudioData, i * 2, 2 );
            //}

            //Yikes, remember that it is SIGNED Int16, not unsigned (spent a bit of time before realizing I screwed this up...)
            l_aFloatArray[i] = (float)(BitConverter.ToInt16(l_aConvertedAudioData, i * 2) / 32768.0f);
        }

        //For some reason the MP3 header is readin as single channel despite it containing 2 channels of data (investigate later)
        //l_oAudioClip = AudioClip.Create("MySound", (int)(l_aFloatArray.Length), 2, l_oMP3Stream.Frequency, false);
        l_oAudioClip = AudioClip.Create("MySound" + fileIndex, (int)(l_aFloatArray.Length * 0.52f), 2, l_oMP3Stream.Frequency, false);
        l_oAudioClip.SetData(l_aFloatArray, 0);

        return l_oAudioClip;
    }

    class SpeechInfo
    {
        public string speechScript;
        public bool shouldGoNext;
        public AudioClip speechClip;

        public SpeechInfo(string speechScript, AudioClip speechClip)
        {
            this.speechScript = speechScript;
            shouldGoNext = speechScript.Contains("정답") 
                || speechScript.Contains("땡") 
                || speechScript.Contains("시작")
                || speechScript.Contains("안녕");
            this.speechClip = speechClip;
        }
    }
}