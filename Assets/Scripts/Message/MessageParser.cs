using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

namespace REEL.Recorder
{
    public class ParsedMessage
    {
        public string script;
        public RobotMessage command;

        public ParsedMessage(string script, string totalCommand)
        {
            this.script = script;
            command = new RobotMessage(totalCommand);
        }
    }
}

namespace REEL.Recorder
{
    public class MessageParser : MonoBehaviour
    {
        public IEnumerator ParseMessage(string script)
        {
            if (script.Contains("No Reply"))
            {
                Debug.Log("오류: No Reply: " + script);
                yield break;
            }

            Regex rx = new Regex("(<[^>]+>)");
            MatchCollection matches = rx.Matches(script);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    GroupCollection groupCollection = match.Groups;
                    string command = groupCollection[1].ToString();
                    
                    script = script.Replace(command, "");
                    command = command.Substring(1).Substring(0, command.Length - 2);

                    yield return new ParsedMessage(script, command);
                }
            }
        }
    }
}