using UnityEngine;

namespace REEL.Recorder
{
    [CreateAssetMenu(menuName = "ScriptableObject/RiveScript/RiveScript Processor")]
    public class RiveScriptProcessorSO : ScriptableObject
    {
        RiveScript.RiveScript riveScript;
        [SerializeField] private TextAsset riveScriptText;

        public void SetRiveScriptText(TextAsset textAsset)
        {
            riveScriptText = textAsset;
        }

        public void InitRiveScript()
        {
            riveScript = new RiveScript.RiveScript(utf8: true, debug: true);

            if (riveScript.LoadTextAsset(riveScriptText))
            {
                riveScript.sortReplies();
                Debug.Log("Successfully load file: " + riveScriptText.name);
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