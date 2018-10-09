﻿using System;
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
    }

    public void Insert(string item)
    {
        Debug.Log("msg inserted: " + item);
        items.Add(item);
        ParseMessage(item);
    }

    void InitMessageProcessor()
    {
        messageProcessors.Add("facial", robotFacialRenderer.Play);
        messageProcessors.Add("motion", RobotTransformController.Instance.PlayMotion);
        //messageProcessors.Add("mobility", BluetoothManager.Instance.Send);
    }

    void ParseMessage(string reply)
    {
        Debug.Log("Arbitor::Input " + reply);

        if (reply.Contains("No Reply"))
        {
            Debug.Log("오류");
            items.RemoveAt(0);

            SpeechRenderrer.Instance.TryAgain();
            return;
        }

        bool isCorrect = reply.Contains("정답");
        bool isAnswer = reply.Contains("땡");

        Regex rx = new Regex("(<[^>]+>)");
        MatchCollection matches = rx.Matches(reply);
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

        if (reply.Contains("시작"))
            WebSurvey.Instance.StartQuiz();

        SpeechRenderrer.Instance.Play(reply);
        items.RemoveAt(0);
        if (isCorrect) WebSurvey.Instance.GainScore();
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