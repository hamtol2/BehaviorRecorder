using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.Recorder
{
    public enum ButtonPosition
    {
        Left, Right, None
    }

    public class AnswerButton : MonoBehaviour
    {
        public RectTransform yesButton;
        public RectTransform noButton;

        private readonly float answerXLeft = -200f;
        private readonly float answerXRight = 200f;

        public void SetYesButtonOnRight()
        {
            SetButtonPosition(answerXRight);
        }

        public void SetYesButtonOnLeft()
        {
            SetButtonPosition(answerXLeft);
        }

        private void SetButtonPosition(float xPos)
        {
            Vector3 buttonPos = yesButton.anchoredPosition;
            buttonPos.x = xPos;
            yesButton.anchoredPosition = buttonPos;

            buttonPos.x = -xPos;
            noButton.anchoredPosition = buttonPos;
        }

        /// <summary>
        /// Get the position of Yes Button (left or right).
        /// </summary>
        public ButtonPosition GetYesButtonPosition
        {
            get { return GetButtonPosition(yesButton); }
        }

        /// <summary>
        /// Get the position of No Button (left or right).
        /// </summary>
        public ButtonPosition GetNoButtonPosition
        {
            get { return GetButtonPosition(noButton); }
        }

        private ButtonPosition GetButtonPosition(RectTransform button)
        {
            Vector3 buttonPos = button.anchoredPosition;
            if (buttonPos.x <= -150f) return ButtonPosition.Left;
            else if (buttonPos.x >= 150f) return ButtonPosition.Right;
            else return ButtonPosition.None;
        }
    }
}