using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.Recorder
{
    public class XButton : MonoBehaviour
    {
        public void OnButtonClicked()
        {
            QuitApplication();
        }

        void QuitApplication()
        {
            Application.Quit();
        }
    }
}