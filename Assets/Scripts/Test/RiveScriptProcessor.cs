using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.Test
{
    public class RiveScriptProcessor : MonoBehaviour
    {
        RiveScript.RiveScript riveScript;
        [SerializeField] private TextAsset testScript;

        private void Awake()
        {
            InitRiveScript();
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

        public string GetReply(string question)
        {
            return riveScript.reply("REEL", question);
        }
    }
}