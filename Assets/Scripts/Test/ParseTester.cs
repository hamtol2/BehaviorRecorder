using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.Test
{
    class Command
    {
        public string topic;
        public string message;

        public Command(Group group)
        {
            string command = group.ToString();
            string[] splitted = command.Split(new char[] { '=', ':', '>' }, System.StringSplitOptions.RemoveEmptyEntries);
            topic = splitted[1];
            message = splitted[2];
        }

        public override string ToString()
        {
            return topic + " " + message;
        }
    }

    public class ParseTester : MonoBehaviour
    {
        public RiveScriptProcessor riveScriptProcessor;
        public SpeechController speechController;

        public Text speechText;
        public Text motionMessageText;
        public Text faceMessageText;

        Dictionary<string, Action<string>> processors = new Dictionary<string, Action<string>>();
        Queue<Command> currentCommands = new Queue<Command>();

        private void Awake()
        {
            InitProcessors();
        }

        private void Start()
        {
            string targetString = riveScriptProcessor.GetReply("시작하자");
            StartCoroutine("SplitScriptToLines", (targetString));
        }

        void InitProcessors()
        {
            processors.Add("facial", FacialPlayer);
            processors.Add("motion", MotionPlayer);
        }

        IEnumerator SplitScriptToLines(string script)
        {
            string[] lines = script.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                ParseLine(line);

                //yield return new WaitForSeconds(3f);
                yield return StartCoroutine("WaitForSpeech");
            }

            Debug.Log("<color=red>All Process Done</color>");
        }

        IEnumerator WaitForSpeech()
        {
            bool isFinished = false;
            while (!isFinished)
            {
                yield return null;

                Debug.Log("Wait..." + speechController.IsFinished);

                if (speechController.IsFinished)
                {
                    isFinished = true;
                    break;
                }   
            }
        }

        void ParseLine(string speech)
        {
            Regex rx = new Regex("(<[^>]+>)");
            MatchCollection matches = rx.Matches(speech);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    speech = speech.Replace(match.Groups[0].ToString(), "");
                    currentCommands.Enqueue(new Command(match.Groups[0]));
                }

                speechText.text = speech;
                speechController.Play(speech);
                ProcessorExecutor();
            }
        }

        void ProcessorExecutor()
        {
            Debug.Log("<color=green>Execute Start</color>");
            for (int ix = 0; ix < currentCommands.Count; ++ix)
            {
                Action<string> processor;
                Command command = currentCommands.Dequeue();
                if (processors.TryGetValue(command.topic, out processor))
                {
                    processor(command.message);
                }
                else
                {
                    ClearAllMessages();
                }
            }

            Debug.Log("<color=green>Execute Finished</color>");
            
        }

        void ProcessDone()
        {
            speechText.text = "실행 종료.";
            ClearAllMessages();
        }

        void ClearAllMessages()
        {
            faceMessageText.text = "";
            motionMessageText.text = "";
        }

        void FacialPlayer(string message)
        {
            Debug.Log("FacialPlayer: " + message);
            faceMessageText.text = message;
        }

        void MotionPlayer(string message)
        {
            Debug.Log("MotionPlayer: " + message);
            motionMessageText.text = message;
        }
    }
}