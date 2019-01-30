using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.Recorder
{
    [CreateAssetMenu(menuName = "ScriptableObject/RiveScript/RiveScriptFileArray")]
    public class RiveScriptFileArray : ScriptableObject
    {
        public TextAsset[] riveScriptAssets;

        public TextAsset GetRiveScriptAsset(int index)
        {
            return riveScriptAssets[index];
        }
    }
}