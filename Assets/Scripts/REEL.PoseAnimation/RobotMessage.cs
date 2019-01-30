namespace REEL.Recorder
{
    public class RobotMessage
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
}