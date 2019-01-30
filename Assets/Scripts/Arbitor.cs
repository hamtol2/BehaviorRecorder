using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using REEL.Animation;
using REEL.PoseAnimation;

namespace REEL.Recorder
{
    public class Arbitor : Singleton<Arbitor>
    {
        public RobotMotionController robotMotionController;
        public RobotFacialRenderer robotFacialRenderer;
        public SurveyController surveyController;
        public QuizStatusManager quizStatusManager;

        [SerializeField] private float quizStartTime = 1f;

        Dictionary<string, Action<string>> messageProcessors = new Dictionary<string, Action<string>>();

        bool isStarted = false;

        void Start()
        {
            SpeechRenderrer.Instance.Init();
            InitMessageProcessor();

            Invoke("QuizStart", quizStartTime);
        }

        void QuizStart()
        {
            surveyController.GetReply("시작하자");
            isStarted = true;
        }

        public void Insert(string item)
        {
            ParseMessage(item);
        }

        void InitMessageProcessor()
        {
            messageProcessors.Add("facial", robotFacialRenderer.Play);
            messageProcessors.Add("motion", robotMotionController.PlayMotion);
            messageProcessors.Add("qcount", surveyController.SetQuizCount);
            messageProcessors.Add("answer", surveyController.SetCurrentAnswer);
            messageProcessors.Add("answertime", surveyController.SetTimeoutTime);
            messageProcessors.Add("hinttime", surveyController.SetHintTime);
            messageProcessors.Add("robotmove", surveyController.RobotMovementStart);
            messageProcessors.Add("mode", surveyController.SetBehaviorMode);
            messageProcessors.Add("facetype", surveyController.SetFaceActiveState);
            messageProcessors.Add("iscue", surveyController.SetCueState);
            messageProcessors.Add("activebreath", robotMotionController.SetBreathActiveState);
        }

        void ParseMessage(string reply)
        {
            //Debug.Log("Arbitor::Input " + reply);

            if (reply.Contains("No Reply"))
            {
                Debug.Log("오류: No Reply: " + reply);

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
                surveyController.StartQuiz();

            SpeechRenderrer.Instance.Play(reply);

            // check answer is correct or wrong.
            if (isCorrect) quizStatusManager.GainScore();
            else if (isWrong) quizStatusManager.SetAnswerState(REEL.Recorder.AnswerState.Wrong);
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
}