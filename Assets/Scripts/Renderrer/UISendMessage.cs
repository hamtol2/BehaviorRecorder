using REEL.PoseAnimation;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.Animation
{
    public class UISendMessage : MonoBehaviour
    {
        public InputField inputField;
        public RobotFacialRenderer robotFacialRenderer;
        public RobotMotionController robotTransformController;
        public MessageTranslator messageTranslator;

        private StringBuilder sb = new StringBuilder();

        public void SendInputMesage()
        {
            sb.Clear();
            sb.Append("<sm=facial:");
            sb.Append(inputField.text);
            sb.Append(">");
            messageTranslator.SetMessage(sb.ToString());

            sb.Clear();
            sb.Append("<sm=motion:");
            sb.Append(inputField.text);
            sb.Append(">");
            messageTranslator.SetMessage(sb.ToString());
        }
    }
}