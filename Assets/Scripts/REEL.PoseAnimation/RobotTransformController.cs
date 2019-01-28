using REEL.Recorder;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.PoseAnimation
{
    //[ExecuteInEditMode]
    public class RobotTransformController : Singleton<RobotTransformController>
    {
        public JointSet[] jointInfo;
        public bool breath = true;
        public BehaviorRecorder behaviorRecorder;
        [SerializeField] private RobotMovement robotMovement;

        [SerializeField] private MotionData motionData;

        public string currentGesture;

        string _gesture;
        //float duration = -1f;

        bool breatheEnable = true;

        // Y, 	Z, 	  Z,   Y, 	X, Z
        float[] zeroAngle = new float[8] { 0f, -90f, 90f, 90f, 0f, 0f, 0f, 0f };
        float[] baseAngle = new float[8] { 45f, -45f, -45f, 45f, 45f, 45f, 0f, 10f };
        float[] DIRECTION = new float[8] { -1f, 1f, 1f, 1f, -1f, -1f, 0f, 1f };

        float[] OFFSET = new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };

        Dictionary<string, float[][]> motionTable;
        IEnumerator currentAnimation = null;
        private bool isPlaying = false;

        private bool isBreathActive = true;

        private readonly float playMotionDelayTime = 1f;

        // Test.
        Queue<MotionAnimInfo> animationQueue = new Queue<MotionAnimInfo>();

        private IEnumerator Start()
        {
            if (motionData == null) motionData = GetComponent<MotionData>();

            yield return StartCoroutine(SetBasePos());

            //if (breath) PlayMotion("breathing");
        }

        public void SetBreathActiveState(string message)
        {
            isBreathActive = Convert.ToBoolean(message);
        }

        public void PlayMotion(string motion)
        {
            // 흘끗보기 동작은 우선적으로 재생.
            if (motion.Contains("nod"))
            {
                if (isPlaying)
                {
                    StopAllCoroutines();
                    isPlaying = false;
                }

                StartCoroutine(PlayMotionCoroutine(motion));
                return;
            }

            if (!isBreathActive && !motion.Contains("breathing"))
            {
                StartCoroutine("DelayPlayMotion", motion);
                return;
            }

            //Debug.Log("PlayMotion: " + motion);
            SetRobotState(motion);

            animationQueue.Enqueue(new MotionAnimInfo(motion, PlayMotionCoroutine(motion)));
            if (!isPlaying && animationQueue.Count > 0)
            {
                StartCoroutine(animationQueue.Dequeue().motionCoroutine);
            }
        }

        IEnumerator DelayPlayMotion(string motion)
        {
            yield return new WaitForSeconds(playMotionDelayTime);
            PlayMotion(motion);
        }

        private void SetRobotState(string motion)
        {
            if (motion.Contains("ok") || motion.Contains("clap"))
            {
                robotMovement.SetState(RobotMovement.State.Clap);
                //Debug.Log("OK Motion");
            }

            else if (motion.Contains("no") || motion.Contains("wrong"))
            {
                robotMovement.SetState(RobotMovement.State.No);
                //Debug.Log("No Motion");
            }
        }

        IEnumerator TestAllMotion()
        {
            foreach (KeyValuePair<string, float[][]> motion in motionTable)
            {
                Debug.Log(motion.Key);
                yield return PlayMotionCoroutine(motion.Key);
            }
        }

        public void SetZeroPos()
        {
            for (int i = 0; i < 6; i++)
            {
                SetAngle(i, zeroAngle[i]);
            }
        }

        public IEnumerator SetBasePos()
        {
            float basePoseTime = 1f;

            for (int ix = 0; ix < jointInfo.Length; ++ix)
            {
                //jointInfo[ix].SetBaseAngle();
                //SetAngle(ix, baseAngle[ix]);
                SetAngleLerp(ix, baseAngle[ix], basePoseTime);
            }

            yield return new WaitForSeconds(basePoseTime);
        }

        public IEnumerator PlayMotionCoroutine(string motion)
        {
            float[][] motionFrameData = motionData.GetMotionFrameDataWithName(motion);
            if (motionFrameData != null)
            {
                currentGesture = motion;
                behaviorRecorder.RecordBehavior(new RecordEvent(0, motion));

                yield return StartCoroutine("GestureProcess", motionFrameData);
                currentAnimation = null;
            }
            else
            {
                Debug.Log("motion null, " + motion);
                currentGesture = string.Empty;
                currentAnimation = null;
            }
        }

        float GetPlayTime(float[][] motionList)
        {
            float playTime = 0f;
            foreach (float[] time in motionList)
            {
                playTime += time[0];
            }
            return playTime;
        }

        IEnumerator GestureProcess(float[][] motionInfo)
        {
            isPlaying = true;

            for (int ix = 0; ix < motionInfo.Length; ++ix)
            {
                for (int jx = 0; jx < jointInfo.Length; ++jx)
                {
                    // For Debug.
                    //if (jx == 0)
                    //{
                    //    StartCoroutine(jointInfo[jx].SetAngleLerp(motionInfo[ix][jx + 1], motionInfo[ix][0], true));
                    //    continue;
                    //}

                    StartCoroutine(jointInfo[jx].SetAngleLerp(motionInfo[ix][jx + 1], motionInfo[ix][0]));
                }

                float waitTime = motionInfo[ix][0];

                yield return new WaitForSeconds(waitTime);
            }

            yield return StartCoroutine(SetBasePos());

            isPlaying = false;

            //Debug.Log("Motion Finished, queue count: " + animationQueue.Count);
            if (animationQueue.Count > 0)
            {
                MotionAnimInfo info = animationQueue.Dequeue();
                //Debug.Log("Play Next motion: " + info.motion);
                StartCoroutine(info.motionCoroutine);
            }
            else if (animationQueue.Count == 0)
            {
                if (breath)
                {
                    //bool isActiveMode = WebSurvey.Instance.GetBehaviorMode == WebSurvey.Mode.Active;
                    string brethingName = isBreathActive ? "breathing_active" : "breathing_inactive";
                    PlayMotion(brethingName);
                }
            }

            //breatheEnable = true;
        }

        IEnumerator WaitUntilNextCoroutine(float time)
        {
            float elapsedTime = 0f;
            while (elapsedTime <= time)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        float GetAngleToGo(float[][] motionInfo, int index, int jointIndex, float duration)
        {
            // Destination angle of a series
            float angleDest = motionInfo[index][jointIndex + 1] + OFFSET[jointIndex + 1];
            // Current angle of joint
            float angleNow = GetAngle(jointIndex);
            // Different between destination and currenat angles
            float angleDiff = angleDest - angleNow;
            // Angle ratio from 
            float angleRatio = duration / motionInfo[index][0];
            // Angle of to go now
            float angleToGo = angleNow + (angleDiff * angleRatio);

            return angleToGo;
        }

        IEnumerator TestExecutor()
        {
            for (int ix = 0; ix < 5; ++ix)
            {
                yield return StartCoroutine(Test());
            }
        }

        float waitTime = 0.1f;
        IEnumerator Test()
        {
            for (int ix = 1; ix < 11; ++ix)
            {
                //SetAngle(6, -ix * 10f);
                SetAngle(7, ix * 10f);

                yield return new WaitForSeconds(waitTime);
            }

            yield return new WaitForSeconds(.5f);
            SetBasePos();
        }
        public void SetAngle(int jointId, float angle)
        {
            jointInfo[jointId].SetAngle(angle);
        }

        public void SetAngleLerp(int jointId, float angle, float duration)
        {
            StartCoroutine(jointInfo[jointId].SetAngleLerp(angle, duration));
        }

        public float GetAngle(int JointID)
        {
            return jointInfo[JointID].GetAngle();
        }

        class MotionAnimInfo
        {
            public string motion;
            public IEnumerator motionCoroutine;

            public MotionAnimInfo() { }
            public MotionAnimInfo(string motion, IEnumerator coroutine)
            {
                this.motion = motion;
                motionCoroutine = coroutine;
            }
        }
    }
}