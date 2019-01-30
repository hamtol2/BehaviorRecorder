using System.Text.RegularExpressions;

namespace REEL.Test
{
    public class Command
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
}