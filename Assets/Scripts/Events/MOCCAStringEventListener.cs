using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace REEL.Recorder
{
    [System.Serializable]
    public class UnityStringEvent : UnityEvent<string>
    {
    }

    public class MOCCAStringEventListener : MonoBehaviour
    {
        public MOCCAStringEvent Event;
        public UnityStringEvent response;

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised(string message)
        {
            response.Invoke(message);
        }
    }
}