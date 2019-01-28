using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionData : MonoBehaviour
{
    [SerializeField] private MotionSequence[] motionSequences;

    public float[][] GetMotionFrameDataWithName(string motionName)
    {
        foreach (MotionSequence motionSequence in motionSequences)
        {
            List<float[]> motionFrameData = new List<float[]>();
            if (motionSequence.motionName.Equals(motionName))
            {
                foreach (MotionFrameData data in motionSequence.sequence)
                {
                    float[] frameData = new float[]
                    {
                        data.deltaTime,
                        data.leftArm.shoulder, data.leftArm.upperArm, data.leftArm.lowerArm,
                        data.rightArm.shoulder, data.rightArm.upperArm, data.rightArm.lowerArm,
                        data.head.neck, data.head.headBase
                    };

                    motionFrameData.Add(frameData);
                }

                return motionFrameData.ToArray();
            }   
        }

        return null;
    }

    public MotionSequence GetMotionWithName(string motionName)
    {
        foreach (MotionSequence sequence in motionSequences)
        {
            if (sequence.motionName.Equals(motionName))
                return sequence;
        }

        return null;
    }

    public MotionSequence[] GetAllMotionSequence
    {
        get { return motionSequences; }
    }
}