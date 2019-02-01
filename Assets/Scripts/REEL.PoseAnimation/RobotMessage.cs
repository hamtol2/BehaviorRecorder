using System;

namespace REEL.Recorder
{
    public class RobotMessage
    {
        private string topic = string.Empty;      // facial / motion.
        private string value = string.Empty;          //  표정 or 모션 이름.

        public RobotMessage() { }
        public RobotMessage(string[] robotMessage)
        {
            SetMessage(robotMessage);
        }

        public RobotMessage(string totalMessage)
        {
            SetMessage(totalMessage);
        }

        public string GetMessageType { get { return topic; } }
        public string GetMessage { get { return value; } }

        public void SetMessage(string totalMessage)
        {
            SetMessage(ProcessCommand(totalMessage));
        }

        public void SetMessage(string[] robotMessage)
        {
            topic = robotMessage[0];
            value = robotMessage[1];
        }

        string[] ProcessCommand(string command)
        {
            int index = command.IndexOf("=");
            if (index > 0)
            {
                string tag = command.Substring(0, index);
                command = command.Substring(index + 1);

                if (tag.Equals("sm"))
                {
                    return command.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                }
            }

            return null;
        }

        public override string ToString()
        {
            return "messageType: " + topic + ", message: " + value;
        }
    }
}