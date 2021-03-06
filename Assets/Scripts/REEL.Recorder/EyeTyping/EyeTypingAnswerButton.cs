﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.Recorder
{
    public class EyeTypingAnswerButton : EyeTypingButton
    {
        private AnswerListener answerListener;

        protected override void Awake()
        {
            base.Awake();
            answerListener = GetComponent<AnswerListener>();
        }

        public override void UpdateTimer()
        {
            base.UpdateTimer();

            // 활성화인 경우에만.
            if (EyeKeyboardManager.Instance && gameObject.activeInHierarchy)
                EyeKeyboardManager.Instance.SetAnswerButtonGazed(true, GetComponent<RectTransform>());
        }

        protected override void ResetButton()
        {
            base.ResetButton();

            if (EyeKeyboardManager.Instance)
                EyeKeyboardManager.Instance.SetAnswerButtonGazed(false, null);
        }
    }
}