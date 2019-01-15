using UnityEngine;
using SpeechLib;
using System.Collections.Generic;
using System.Collections;
using System;

namespace REEL.Test
{
    public class Paragraph
    {
        public List<Sentence> sentences = new List<Sentence>();

        public void InitSentences(string script)
        {

        }

        public void Clear()
        {
            sentences = new List<Sentence>();
        }
    }

    public class Sentence
    {
        public int sentenceID = -1;
        public string speech;
    }

    public class SpeechController : MonoBehaviour
    {
        private SpVoiceClass voice;
        private bool isTTSStarted = false;
        RiveScript.RiveScript riveScript;
        [SerializeField] private TextAsset testScript;

        private void Awake()
        {
            InitTTS();
            InitRiveScript();
        }

        private void Start()
        {
            string reply = riveScript.reply("REEL", "시작하자");
            Play(reply);
        }

        void InitTTS()
        {
            voice = new SpVoiceClass();
            voice.Volume = 100;
            voice.Rate = 1;
        }

        void InitRiveScript()
        {
            riveScript = new RiveScript.RiveScript(utf8: true, debug: true);
            
            if (riveScript.LoadTextAsset(testScript))
            {
                riveScript.sortReplies();
                Debug.Log("Successfully load file: " + testScript.name);
            }
            else
            {
                Debug.Log("Fail to load RiveScript, SurveyType");
            }
        }

        void Play(string script)
        {
            string[] speechs = script.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            IEnumerator enumerator = speechs.GetEnumerator();
            while (enumerator.MoveNext())
            {
                
            }
            //foreach (string speech in speechs)
            //{
            //    TextToSpeech(speech);
            //}
        }

        void TextToSpeech(string ttsText)
        {
            isTTSStarted = true;
            string[] sentences = ttsText.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string sentence in sentences)
            {   
                voice.Speak(sentence, SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFIsXML);
            }
        }

        bool isFinished = true;
        IEnumerator CheckSpeechFinished()
        {
            while (!IsFinished)
            {
                yield return null;
            }
        }

        void TTSStop()
        {
            voice.Speak(string.Empty, SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
        }

        public bool IsSpeaking
        {
            get { return isTTSStarted && voice.Status.RunningState == SpeechRunState.SRSEIsSpeaking; }
        }

        bool IsFinished
        {
            get { return isTTSStarted && voice.Status.RunningState == SpeechRunState.SRSEDone; }
        }
    }
}