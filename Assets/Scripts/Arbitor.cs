using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using REEL.Animation;
using REEL.PoseAnimation;

public class Arbitor : Singleton<Arbitor>
{
    class RobotMessage
    {   
        private string messageType = string.Empty;      // facial / motion.
        private string message = string.Empty;          //  표정 or 모션 이름.

        public RobotMessage() { }
        public RobotMessage(string[] robotMessage)
        {
            SetMessage(robotMessage);
        }

        public string GetMessageType { get { return messageType; } }
        public string GetMessage { get { return message; } }

        public void SetMessage(string[] robotMessage)
        {
            messageType = robotMessage[0];
            message = robotMessage[1];
        }

        public override string ToString()
        {
            return "messageType: " + messageType + ", message: " + message;
        }
    }

    public Toggle automaticExpression;
    public RobotFacialRenderer robotFacialRenderer;

    List<string> items = new List<string>();
    bool _isSpeaking;

    private IEnumerator coroutine;

    delegate void MessageProcessor(string message);
    Dictionary<string, MessageProcessor> messageProcessors = new Dictionary<string, MessageProcessor>();

    void Start()
    {
        SpeechRenderrer.Instance.Init();
        InitMessageProcessor();

        coroutine = Process(0.1f);
        StartCoroutine(coroutine);
    }

    // every 0.1 seconds perform
    private IEnumerator Process(float waitTime)
    {
        Debug.Log("Arbitor::Process");
        while (true)
        {
            yield return new WaitForSeconds(waitTime);

            ParseMessage();
        }
    }

    public void Insert(string item)
    {
        items.Add(item);
    }

    void InitMessageProcessor()
    {
        messageProcessors.Add("facial", robotFacialRenderer.Play);
        messageProcessors.Add("motion", RobotTransformController.Instance.PlayMotion);
        //messageProcessors.Add("mobility", BluetoothManager.Instance.Send);
    }

    bool CheckIfAnswerYes(string reply)
    {
        return reply.Contains("오") || reply.Contains("어") || reply.Contains("아") || reply.Contains("우");
    }

    bool CheckIfAnswerNo(string reply)
    {
        return reply.Contains("엑") || reply.Contains("액") || reply.Contains("악") || reply.Contains("윽");
    }

    void ParseMessage()
    {
        if (items.Count > 0)
        {
            string reply = items[0];
            Debug.Log("Arbitor::Input " + reply);
            bool isCorrect = reply.Contains("정답");
            bool isAnswer = reply.Contains("땡");

            if (SpeechRenderrer.Instance.IsSpeaking())
            {
                //Debug.Log("Speaking");
                return;
            }

            Regex rx = new Regex("(<[^>]+>)");
            MatchCollection matches = rx.Matches(reply);
            // Debug.Log("Match found " + matches.Count);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    GroupCollection groupCollection = match.Groups;
                    string command = groupCollection[1].ToString();
                    reply = reply.Replace(command, "");
                    command = command.Substring(1).Substring(0, command.Length - 2);

                    // 메시지 처리.
                    ProcessCommand(command);
                }
            }

            // Check if quiz start.
            if (WebSurvey.Instance.GetCurrentStep() == 0 && reply.Contains("시작"))
            {
                WebSurvey.Instance.StartQuiz();
                items.RemoveAt(0);
            }

            if (!SpeechRenderrer.Instance.IsSpeaking())
            {
                Debug.Log("Start Speaking: " + reply);
                SpeechRenderrer.Instance.Play(reply);
                //Debug.Log("clip length: " + SpeechRenderrer.Instance.GetComponent<AudioSource>().clip.length);
                items.RemoveAt(0);

                if (isCorrect) WebSurvey.Instance.GainScore();
            }
        }

        if (_isSpeaking && !SpeechRenderrer.Instance.IsSpeaking())
        {
            Debug.Log("Speaking finished");
            //SpeechRecognition.Instance.Enable();

            // check if on normal dialogue state.
            WebSurvey.Instance.NextStep();

            if (WebSurvey.Instance.GetCurrentScore() == 3)
            {
                WebSurvey.Instance.FinishQuiz();
            }
        }
        _isSpeaking = SpeechRenderrer.Instance.IsSpeaking();
    }

    void ProcessCommand(string command)
    {
        int index = command.IndexOf("=");
        if (index > 0)
        {
            string tag = command.Substring(0, index);
            command = command.Substring(index + 1);

            if (tag.Equals("sm"))
            {
                string[] detail = command.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (detail.Length > 0)
                {
                    RobotMessage robotMessage = new RobotMessage(detail);

                    MessageProcessor processor;
                    if (messageProcessors.TryGetValue(robotMessage.GetMessageType, out processor))
                    {
                        //Debug.Log(robotMessage);
                        processor(robotMessage.GetMessage);
                    }
                }
            }
        }
    }
}