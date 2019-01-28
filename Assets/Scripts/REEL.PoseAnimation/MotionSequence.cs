using System.Collections.Generic;
using UnityEngine;
using System;

public class MotionSequence : ScriptableObject
{
    public string motionName;
    public List<MotionFrameData> sequence;

    public void AddSequenceData(float[] data)
    {
        MotionFrameData frameData = new MotionFrameData();
        frameData.deltaTime = data[0];
        frameData.leftArm = new ArmAngles() { shoulder = data[1], upperArm = data[2], lowerArm = data[3] };
        frameData.rightArm = new ArmAngles() { shoulder = data[4], upperArm = data[5], lowerArm = data[6] };
        frameData.head = new HeadAngles() { neck = data[7], headBase = data[8] };

        sequence.Add(frameData);
    }
}

[Serializable]
public class MotionFrameData
{
    public float deltaTime;
    public ArmAngles leftArm;
    public ArmAngles rightArm;
    public HeadAngles head;
}

[Serializable]
public class ArmAngles
{
    public float shoulder;
    public float upperArm;
    public float lowerArm;
}

[Serializable]
public class HeadAngles
{
    public float neck;
    public float headBase;
}