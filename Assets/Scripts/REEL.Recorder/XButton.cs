using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.Recorder
{
    public class XButton : MonoBehaviour
    {
        public void OnXButtonClicked()
        {
            Invoke("QuitApplication", 1.5f);
        }

        void QuitApplication()
        {
            Application.Quit();
        }
    }
}