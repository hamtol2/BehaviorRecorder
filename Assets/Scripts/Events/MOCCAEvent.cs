using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MOCCAEvent : ScriptableObject
{
    private List<MOCCAEventListener> listeners = new List<MOCCAEventListener>();

    public void Raise()
    {
        for (int ix = listeners.Count - 1; ix >= 0; --ix)
        {
            listeners[ix].OnEventRaised();
        }
    }

    public void RegisterListener(MOCCAEventListener listener)
    {
        listeners.Add(listener);
    }

    public void UnregisterListener(MOCCAEventListener listener)
    {
        listeners.Remove(listener);
    }
}