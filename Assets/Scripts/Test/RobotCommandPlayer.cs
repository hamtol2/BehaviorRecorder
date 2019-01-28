using System.Collections.Generic;
using UnityEngine;

using REEL.Recorder;
using System.Text.RegularExpressions;
using System;

namespace REEL.Test
{
    public class RobotCommandPlayer : MonoBehaviour
    {
        public SpeechController speechController;
        RiveScript.RiveScript riveScript;
        [SerializeField] private TextAsset testScript;

        public List<RobotCommand> currentCommands = new List<RobotCommand>();

        private bool shouldRun = false;

        private void Awake()
        {
            InitRiveScript();
        }

        private void Start()
        {
            string reply = riveScript.reply("REEL", "시작하자");
            SplitSpeechLines(reply);
            PlayCommand();
        }

        private void Update()
        {
            if (shouldRun && currentCommands.Count > 0)
            {
                if (speechController.IsFinished)
                {
                    PlayCommand();

                    if (currentCommands.Count == 0)
                        Debug.Log("Finished");
                }
            }
        }

        void PlayCommand()
        {
            RobotCommand command = currentCommands[0];
            speechController.Play(command.speech);
            currentCommands.RemoveAt(0);

            Debug.Log(command.motion + " : " + command.face);
        }

        private void SplitSpeechLines(string script)
        {
            string[] speeches = script.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string speech in speeches)
            {
                ParseMessage(speech);
            }

            shouldRun = true;
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
                    speech = speech.Replace(command, "");

                    int index = command.IndexOf(",");
                    if (index > 0)
                    {
                        string[] commands = command.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                        string motion = commands[0].Substring(1, commands[0].Length - 1);
                        string face = commands[1].Substring(0, commands[1].Length - 1);
                        //Debug.Log(motion + "::" + face);

                        RobotCommand robotCommand = new RobotCommand();
                        robotCommand.speech = speech;
                        robotCommand.motion = motion.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1];
                        robotCommand.face = face.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1];

                        currentCommands.Add(robotCommand);
                    }
                }
            }
        }
    }
}