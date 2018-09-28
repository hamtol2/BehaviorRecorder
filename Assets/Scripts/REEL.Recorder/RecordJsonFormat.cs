using System;
using UnityEngine;

namespace REEL.Recorder
{
    public enum ContentState
    {
        IceBreaking, OnQuestion, Waiting, Reacting
    }

    public enum AnswerState
    {
        Wait, Correct, Wrong, Timeout
    }

    public enum ModelType
    {
        HumanEye, ExpressionRobot, NonExpressionRobot
    }

    public enum TargetRegion
    {
        Eye, Mouth, Face, Arm, Body, FingerTip, Background, OffSight
    }

    [Serializable]
	public class RecordJsonFormat
	{
        public RecordData[] recordData;

        public int Length
        {
            get { return recordData == null ? 0 : recordData.Length; }
        }

        public RecordData this[int index]
        {
            get { return recordData[index]; }
        }

        public void AddData(RecordData data)
        {
            if (recordData == null)
            {
                recordData = new RecordData[1] { data };
                return;
            }

            RecordData[] tempArray = new RecordData[recordData.Length];
            for (int ix = 0; ix < recordData.Length; ++ix)
            {
                tempArray[ix] = recordData[ix];
            }

            recordData = new RecordData[recordData.Length + 1];
            for (int ix = 0; ix < tempArray.Length; ++ix)
            {
                recordData[ix] = tempArray[ix];
            }

            recordData[recordData.Length - 1] = data;
        }
    }

    [Serializable]
    public class RecordData
    {
        public string quizTitle;
        public int quizNumber;
        public float elapsedTime;
        public ContentState contentState;
        public AnswerState answer;
        public ModelType modelType;
        public Vector2 eyePosition;
        public TargetRegion targetRegion;
        public Vector3 robotPosition;
        public string face;
        public string gesture;
        public RecordEvent recordEvent;
    }

    [Serializable]
    public class RecordEvent
    {
        // 0: motion, 1: facial.
        public int eventType = -1;
        public string eventValue = string.Empty;

        public RecordEvent() { }
        public RecordEvent(int eventType, string eventValue)
        {
            this.eventType = eventType;
            this.eventValue = eventValue;
        }
    }
}