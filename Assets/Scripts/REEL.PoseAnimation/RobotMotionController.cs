using REEL.Recorder;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.PoseAnimation
{
    public class RobotMotionController : MonoBehaviour
    {
        public JointSet[] jointInfo;
        public bool breath = true;
        public BehaviorRecorder behaviorRecorder;
        [SerializeField] private RobotMovement robotMovement;
        [SerializeField] private MotionData motionData;

        public MOCCAEvent OnRobotReady;

        public string currentGesture;

        bool breatheEnable = true;

        private float rotSpeed = 360f;
        private float rotSpeedPercentage = 0.8f;

        // Y, 	Z, 	  Z,   Y, 	X, Z
        float[] zeroAngle = new float[8] { 0f, -90f, 90f, 90f, 0f, 0f, 0f, 0f };
        float[] baseAngle = new float[8] { 45f, -45f, -45f, 45f, 45f, 45f, 0f, 10f };

        private bool isPlaying = false;
        private bool isBreathActive = true;
        private readonly float playMotionDelayTime = 1f;

        private IEnumerator Start()
        {
            if (motionData == null) motionData = GetComponent<MotionData>();

            yield return StartCoroutine(SetBasePos());

            OnRobotReady.Raise();

            //if (breath) PlayMotion("breathing");
        }

        public void SetBreathActiveState(string message)
        {
            isBreathActive = Convert.ToBoolean(message);
        }

        public void PlayMotion(string motion)
        {
            if (!isBreathActive && !motion.Contains("breathing"))
            {
                StartCoroutine("DelayPlayMotion", motion);
                return;
            }
            else
            {
                if (isPlaying)
                {
                    StopCoroutine("GestureProcess");
                    StopCoroutine("DelayPlayMotion");
                    isPlaying = false;
                }

                StartCoroutine(PlayMotionCoroutine(motion));
                return;
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
            foreach (MotionSequence motionSequence in motionData.GetAllMotionSequence)
            {
                Debug.Log(motionSequence.motionName);
                yield return PlayMotionCoroutine(motionSequence.motionName);
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
                if (behaviorRecorder) behaviorRecorder.RecordBehavior(new RecordEvent(0, motion));

                yield return StartCoroutine("GestureProcess", motionFrameData);
            }
            else
            {
                Debug.Log("motion null, " + motion);
                currentGesture = string.Empty;
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

        float GetDurationToFirstAngle(float[] motionInfo)
        {
            float maxDegree = 0f;
            for (int ix = 0; ix < jointInfo.Length; ++ix)
            {
                float degree = jointInfo[ix].GetTargetAngleAxis(motionInfo[ix + 1]);
                maxDegree = Mathf.Max(maxDegree, degree);
            }

            return maxDegree;
        }

        IEnumerator GestureProcess(float[][] motionInfo)
        {
            isPlaying = true;

            for (int ix = 0; ix < motionInfo.Length; ++ix)
            {
                // 첫 번째를 제외한 나머지 각도 설정인 경우, 각도 데이터에 있는 시간 값 사용.
                float rotDuration = motionInfo[ix][0];

                // 첫 번째 각도 설정인 경우, 최대 회전 각도를 기반으로 회전 시간 계산.
                if (ix == 0)
                {
                    float maxDegree = GetDurationToFirstAngle(motionInfo[0]);
                    rotDuration = maxDegree / (rotSpeed * rotSpeedPercentage);
                    //Debug.Log("Max Degree: " + maxDegree + " , Rot Duration: " + rotDuration);
                }

                for (int jx = 0; jx < jointInfo.Length; ++jx)
                {
                    // For Debug.
                    //if (jx == 0)
                    //{
                    //    StartCoroutine(jointInfo[jx].SetAngleLerp(motionInfo[ix][jx + 1], motionInfo[ix][0], true));
                    //    continue;
                    //}

                    StartCoroutine(jointInfo[jx].SetAngleLerp(motionInfo[ix][jx + 1], rotDuration));
                }

                yield return new WaitForSeconds(rotDuration);
            }

            yield return StartCoroutine(SetBasePos());

            isPlaying = false;

            if (breath)
            {
                string brethingName = isBreathActive ? "breathing_active" : "breathing_inactive";
                PlayMotion(brethingName);
            }
        }

        public void SetAngle(int jointId, float angle)
        {
            jointInfo[jointId].SetAngle(angle);
        }

        public void SetAngleLerp(int jointId, float angle, float duration)
        {
            StartCoroutine(jointInfo[jointId].SetAngleLerp(angle, duration));
        }
    }
}