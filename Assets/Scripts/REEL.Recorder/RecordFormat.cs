using UnityEngine;
using System;

namespace REEL.Recorder
{
    [Serializable]
    public class RecordFormat
    {
        public float elapsedTime;
        public CustomVector2 markerPosition;
        public RecordEvent recordEvent;
    }

    [Serializable]
    public class RecordEvent
    {
        // 0: motion, 1: facial.
        public int eventType;
        public string eventValue;

        public RecordEvent() { }
        public RecordEvent(int eventType, string eventValue)
        {
            this.eventType = eventType;
            this.eventValue = eventValue;
        }
    }

    [Serializable]
    public class CustomVector3
    {
        public float x;
        public float y;
        public float z;

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }

    [Serializable]
    public class CustomVector2
    {
        public float x;
        public float y;

        public Vector2 ToVector2()
        {
            return new Vector2(x, y);
        }
    }
}