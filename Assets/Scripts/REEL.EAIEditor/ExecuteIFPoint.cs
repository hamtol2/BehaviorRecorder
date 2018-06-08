using UnityEngine;

namespace REEL.EAIEditor
{
    public class ExecuteIFPoint : ExecutePoint
    {
        public enum IFOutputType : int
        {
            False, True
        }

        [SerializeField]
        private IFOutputType outputType = IFOutputType.False;
    }
}