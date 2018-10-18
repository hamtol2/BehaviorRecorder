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

    //public Toggle automaticExpression;
    public RobotFacialRenderer robotFacialRenderer;

    [SerializeField] private float quizStartTime = 2f;

    Dictionary<string, Action<string>> messageProcessors = new Dictionary<string, Action<string>>();

    REEL.Recorder.Timer timer = new REEL.Recorder.Timer();
    bool isStarted = false;

    void Start()
    {
        SpeechRenderrer.Instance.Init();
        InitMessageProcessor();

        timer = new REEL.Recorder.Timer(quizStartTime, QuizStart);
    }

    private void Update()
    {
        if (!isStarted)
            timer.Update(Time.deltaTime);
    }

    void QuizStart()
    {
        WebSurvey.Instance.GetReply("시작하자");
        isStarted = true;
    }

    public void Insert(string item)
    {
        ParseMessage(item);
    }

    void InitMessageProcessor()
    {
        messageProcessors.Add("facial", robotFacialRenderer.Play);
        messageProcessors.Add("motion", RobotTransformController.Instance.PlayMotion);
        messageProcessors.Add("qcount", WebSurvey.Instance.SetQuizCount);
        messageProcessors.Add("answer", WebSurvey.Instance.SetCurrentAnswer);
        messageProcessors.Add("answertime", WebSurvey.Instance.SetTimeoutTime);
        messageProcessors.Add("hinttime", WebSurvey.Instance.SetHintTime);
        messageProcessors.Add("movetime", WebSurvey.Instance.SetRobotMovementTime);
    }

    void ParseMessage(string reply)
    {
        Debug.Log("Arbitor::Input " + reply);

        if (reply.Contains("No Reply"))
        {
            Debug.Log("오류: No Reply");

            SpeechRenderrer.Instance.TryAgain();
            return;
        }

        bool isCorrect = reply.Contains("정답");
        bool isWrong = reply.Contains("땡");

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

        // check answer is correct or wrong.
        if (isCorrect) WebSurvey.Instance.GainScore();
        else if (isWrong) WebSurvey.Instance.SetAnswerState(REEL.Recorder.AnswerState.Wrong);
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

                    Action<string> processor;
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