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
            int index = 0;
            foreach (string speech in speechs)
            {
                Debug.Log(index++);
                TextToSpeech(speech);
                ParseMessage(speech);
            }
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

        void ParseMessage(string speech)
        {
            Regex rx = new Regex("(<[^>]+>)");
            MatchCollection matches = rx.Matches(speech);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    GroupCollection groupCollection = match.Groups;
                    string command = groupCollection[1].ToString();

                    int index = command.IndexOf(",");
                    if (index > 0)
                    {
                        string[] commands = command.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                        string motion = commands[0].Substring(1, commands[0].Length - 1);
                        string face = commands[1].Substring(1, commands[1].Length - 1);
                        Debug.Log(motion + "::" + face);
                    }
                }
            }
        }
    }
}