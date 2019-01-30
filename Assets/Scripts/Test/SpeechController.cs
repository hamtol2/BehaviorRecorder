using UnityEngine;
using SpeechLib;

namespace REEL.Test
{
    public class SpeechController : MonoBehaviour
    {
        public GameEvent OnSpeechFinished;

        private SpVoiceClass voice;
        private bool isTTSStarted = false;


        private void Awake()
        {
            InitTTS();
        }

        private void Update()
        {
            if (IsFinished)
            {
                isTTSStarted = false;
                OnSpeechFinished.Raise();
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