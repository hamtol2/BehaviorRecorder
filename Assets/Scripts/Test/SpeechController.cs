using UnityEngine;
using SpeechLib;
using System.Collections.Generic;
using System.Collections;
using System;

using REEL.Recorder;
using System.Text.RegularExpressions;

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

        private void Awake()
        {
            InitTTS();
        }

        private void Update()
        {
            if (isTTSStarted && IsFinished)
            {
                isTTSStarted = false;
            }
        }

        void InitTTS()
        {
            voice = new SpVoiceClass();
            voice.Volume = 100;
            voice.Rate = 1;
        }

        public void Play(string speech)
        {
            TextToSpeech(speech);
        }

        void TextToSpeech(string ttsText)
        {
            isTTSStarted = true;
            voice.Speak(ttsText, SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFIsXML);
            //string[] sentences = ttsText.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            //foreach (string sentence in sentences)
            //{
            //    voice.Speak(sentence, SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFIsXML);
            //}
        }

        void TTSStop()
        {
            voice.Speak(string.Empty, SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
        }

        public bool IsSpeaking
        {
            get { return isTTSStarted && voice.Status.RunningState == SpeechRunState.SRSEIsSpeaking; }
        }

        public bool IsFinished
        {
            get { return isTTSStarted && voice.Status.RunningState == SpeechRunState.SRSEDone; }
        }
    }
}