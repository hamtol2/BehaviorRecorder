using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using REEL.Recorder;
using REEL.PoseAnimation;

namespace REEL.Animation
{
    public class MessageTranslator : MonoBehaviour
    {
        public TextAsset riveScriptFile;
        public RobotFacialRenderer robotFacialRenderer;
        public RobotTransformController robotTransformController;
        public BehaviorRecorder behaviorRecorder;
        public Text debugText;

        string rsFile = "/begin.txt";

        RiveScript.RiveScript riveScript = new RiveScript.RiveScript(utf8: true, debug: true);

        public void Awake()
        {
            Init();
        }

        void Init()
        {
            string[] lines = riveScriptFile.text.Split(new char[] { '\n', '\r' });
            if (!riveScript.parse(rsFile, lines))
                ShowDebugTest("RS File Not Loaded");

            riveScript.sortReplies();
        }

        void ShowDebugTest(string text)
        {
            debugText.text = text;
        }

        public void SetMessage(string input)
        {   
            //string reply = riveScript.reply("default", input);
            //Process(reply);
            Process(input);
        }

        private void RecordBehavior(int eventType, string eventValue)
        {
            behaviorRecorder.RecordBehavior(new RecordEvent(eventType, eventValue));
        }

        void Process(string reply)
        {
            Regex rx = new Regex("(<[^>]+>)");
            MatchCollection matches = rx.Matches(reply);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    GroupCollection groupCollection = match.Groups;
                    String command = groupCollection[1].ToString();
                    reply = reply.Replace(command, "");
                    command = command.Substring(1).Substring(0, command.Length - 2);

                    int index = command.IndexOf("=");
                    if (index > 0)
                    {
                        String tag = command.Substring(0, index);
                        command = command.Substring(index + 1);

                        switch (tag)
                        {
                            case "sm":
                                String[] detail = Regex.Split(command, ":");
                                if (detail.Length > 0)
                                {
                                    switch (detail[0])
                                    {
                                        case "motion":
                                            {
                                                ShowDebugTest("Sub command motion with " + detail[1]);
                                                if (robotTransformController.PlayGesture(detail[1]))
                                                {
                                                    // Record motion gesture.
                                                    RecordBehavior(0, detail[1]);
                                                }
                                            }
                                            break;

                                        case "facial":
                                            {
                                                ShowDebugTest("Sub command facial with " + detail[1]);
                                                if (robotFacialRenderer.Play(detail[1]))
                                                {
                                                    // Record facial animation.
                                                    RecordBehavior(1, detail[1]);
                                                }
                                            }
                                            break;

                                        default:
                                            break;
                                    }
                                }
                                break;
                            default:
                                ShowDebugTest("No matched command with " + tag);
                                //Debug.Log("No matched command with " + tag);
                                break;
                        }
                    }
                }
            }
        }
    }
}