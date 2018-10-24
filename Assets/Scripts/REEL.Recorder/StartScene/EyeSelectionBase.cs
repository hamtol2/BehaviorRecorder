using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.Recorder
{
    public class EyeSelectionBase : MonoBehaviour
    {
        [SerializeField] private GameObject[] otherButtonToDisable;

        public virtual void OnGazeComplete()
        {
            if (otherButtonToDisable != null && otherButtonToDisable.Length > 0)
            {
                foreach (GameObject button in otherButtonToDisable)
                {
                    EyeTypingButton eyeTyping = button.GetComponent<EyeTypingButton>();
                    if (eyeTyping) eyeTyping.DisableEyeTypingButton();
                }
            }
        }
    }
}