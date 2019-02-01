using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.Recorder
{
    [CreateAssetMenu(menuName = "ScriptableObject/Event/MOCCA<string>Event")]
    public class MOCCAStringEvent : ScriptableObject
    {
        private List<MOCCAStringEventListener> listeners = new List<MOCCAStringEventListener>();

        public void Raise(string message)
        {
            for (int ix = listeners.Count - 1; ix >= 0; --ix)
            {
                listeners[ix].OnEventRaised(message);
            }
        }

        public void RegisterListener(MOCCAStringEventListener listener)
        {
            listeners.Add(listener);
        }

        public void UnregisterListener(MOCCAStringEventListener listener)
        {
            listeners.Remove(listener);
        }
    }
}