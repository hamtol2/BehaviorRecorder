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

        [SerializeField] private Motion[] motions;

        public string currentGesture;

        string _gesture;
        //float duration = -1f;

        bool breatheEnable = true;

        // Y, 	Z, 	  Z,   Y, 	X, Z
        float[] zeroAngle = new float[8] { 0f, -90f, 90f, 90f, 0f, 0f, 0f, 0f };
        float[] baseAngle = new float[8] { 45f, -45f, -45f, 45f, 45f, 45f, 0f, 10f };
        float[] DIRECTION = new float[8] { -1f, 1f, 1f, 1f, -1f, -1f, 0f, 1f };

        float[] OFFSET = new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
        float[][] hiList = new float[5][] {
							// Time,	Left Arm, 			Right Arm,			Head
			new float[9] {  0.7f,       45f, -45f, -45f,    -60f, 60f, 38f,     0f, 25f    },
            new float[9] {  0.6f,       45f, -45f, -45f,    -60f, 96f, 34f,     0f, 25f   },
            new float[9] {  0.6f,       45f, -45f, -45f,    -60f, 60f, 38f,     0f, 25f   },
            new float[9] {  0.6f,       45f, -45f, -45f,    -60f, 96f, 34f,     0f, 25f   },
            new float[9] {  0.7f,       45f, -45f, -45f,    -57f, 75f, 23f,     0f, 25f    }
        };

        float[][] helloList = new float[5][] {
							// Time,	Left Arm, 			Right Arm,		    Head
			new float[9] {  0.7f,       -60f, -60f, -38f,   45f, 45f, 45f,      0f, 25f   },
            new float[9] {  0.6f,       -60f, -80f, -34f,   45f, 45f, 45f,      0f, 25f   },
            new float[9] {  0.6f,       -60f, -60f, -38f,   45f, 45f, 45f,      0f, 25f   },
            new float[9] {  0.6f,       -60f, -80f, -34f,   45f, 45f, 45f,      0f, 25f   },
            new float[9] {  0.6f,        45f, -45f, -45f,   45f, 45f, 45f,      0f, 25f   }
        };

        float[][] angryList = new float[4][] {
							// Time,	Left Arm, 		    Right Arm,			Head
			new float[9] {  0.7f,       14f, -83f, -24f,    14f, 83f, 24f,      0f, -9f    },
            new float[9] {  0.7f,       57f, -75f, -24f,    57f, 75f, 24f,      0f, -9f   },
            new float[9] {  0.7f,       14f, -83f, -24f,    14f, 83f, 24f,      0f, -99f   },
            new float[9] {  0.8f,        45f, -45f, -45f,   45f, 45f, 45f,      0f, 25f   }
        };
        float[][] sadList = new float[5][] {
                			// Time,    Left Arm,           Right Arm,          Head
			new float[9] {  0.7f,       -25f, -101f, -67f,   -43f, 101f, 67f,   0f, 20f   },
            new float[9] {  0.5f,       -43f, -101f, -67f,   -24f, 101f, 67f,   0f, 20f   },
            new float[9] {  0.5f,       -25f, -101f, -67f,   -43f, 101f, 67f,   0f, 20f   },
            new float[9] {  0.5f,       -43f, -101f, -67f,   -24f, 101f, 67f,   0f, 20f   },
            new float[9] {  0.7f,        45f, -45f, -45f,   45f, 45f, 45f,      0f, 25f   }
        };
        float[][] okList = new float[][] {
                			// Time,    Left Arm,           Right Arm,          Head
			new float[9] {  0.4f,       57f, -75f, -24f,    57f, 75f, 24f,      0f,  10f    },
            new float[9] {  0.4f,       57f, -75f, -24f,    57f, 75f, 24f,      0f, -10f   },
            new float[9] {  0.4f,       57f, -75f, -24f,    57f, 75f, 24f,      0f,  10f   },
            new float[9] {  0.4f,       57f, -75f, -24f,    57f, 75f, 24f,      0f, -10f   },
            new float[9] {  0.4f,       57f, -75f, -24f,    57f, 75f, 24f,      0f,  10f   },
            new float[9] {  0.7f,       45f, -45f, -45f,    45f, 45f, 45f,      0f, 10f   }
        };
        float[][] noList = new float[4][] {
                			// Time,    Left Arm,           Right Arm,          Head
			new float[9] {  0.4f,       57f, -75f, -24f,    57f, 75f, 24f,      -20f, -9f    },
            new float[9] {  0.5f,       57f, -75f, -24f,    57f, 75f, 24f,       20f, -9f   },
            new float[9] {  0.5f,       57f, -75f, -24f,    57f, 75f, 24f,      -20f, -9f   },
            new float[9] {  0.7f,       45f, -45f, -45f,    45f, 45f, 45f,      0f, 25f   }
        };
        float[][] happyList = new float[4][] {
                			// Time,    Left Arm,           Right Arm,          Head
			new float[9] {  0.8f,       -70f, -70f, -30f,   -70f, 70f, 30f,     0f, -25f    },
            new float[9] {  0.6f,       -70f, -70f, -30f,   -70f, 70f, 30f,     -14f, -10f   },
            new float[9] {  0.6f,       -70f, -70f, -30f,   -70f, 70f, 30f,     0f, -25f   },
            new float[9] {  0.7f,        45f, -45f, -45f,   45f, 45f, 45f,      0f, 25f   }
        };
        float[][] clapList = new float[8][] {
							// Time,	Left Arm, 			Right Arm,			Head
			new float[9] {  0.9f,       -20f, -45f, -45f,   -20f, 45f, 45f,     0f, 25f    },
            new float[9] {  0.3f,       -20f, -85f, -75f,   -20f, 85f, 75f,     0f, 25f   },
            new float[9] {  0.3f,       -20f, -45f, -45f,   -20f, 45f, 45f,     0f, 25f   },
            new float[9] {  0.3f,       -20f, -85f, -75f,   -20f, 85f, 75f,     0f, 25f   },
            new float[9] {  0.3f,       -20f, -45f, -45f,   -20f, 45f, 45f,     0f, 25f   },
            new float[9] {  0.3f,       -20f, -85f, -75f,   -20f, 85f, 75f,     0f, 25f   },
            new float[9] {  0.3f,       -20f, -45f, -45f,   -20f, 45f, 45f,     0f, 25f   },
            new float[9] {  0.9f,       45f, -45f, -45f,     45f, 45f, 45f,     0f, 25f    }
        };
        float[][] nodRightList = new float[2][] {
							// Time,	Left Arm, 			Right Arm,			Head
            new float[9] {  0.2f,       45f, -45f, -45f,   45f, 45f, 45f,     -10f, 0f    },
            new float[9] {  0.4f,       45f, -45f, -45f,   45f, 45f, 45f,     0f, 10f   }
        };
        float[][] nodLeftList = new float[2][] {
							// Time,	Left Arm, 			Right Arm,			Head
            new float[9] {  0.2f,       45f, -45f, -45f,   45f, 45f, 45f,     10f, 0f    },
            new float[9] {  0.4f,       45f, -45f, -45f,   45f, 45f, 45f,     0f, 10f   }
        };

        float[][] breathing_active = new float[][] {
            // Time,	Left Arm, 			Right Arm,			Head
            //new float[9] {  0.0f,       45f, -45f, -45f, 45f, 45f, 45f, 0f, 10f },
            new float[9] {  1.2f,       35f, -35f, -35f, 35f, 35f, 35f, 0f, 15f },
            new float[9] {  1.2f,       45f, -45f, -45f, 45f, 45f, 45f, 0f, 10f },
        };

        float[][] breathing_inactive = new float[][] {
            // Time,	Left Arm, 			Right Arm,			Head
            //new float[9] {  0.0f,       45f, -45f, -45f, 45f, 45f, 45f, 0f, 10f },
            new float[9] {  1.2f,       40f, -40f, -40f, 40f, 40f, 40f, 0f, 10f },
            new float[9] {  1.2f,       45f, -45f, -45f, 45f, 45f, 45f, 0f, 10f },
        };

        //float[] DIRECTION = new float[8] { -1f, 1f, 1f, 1f, -1f, -1f, 0f, 1f };

        Dictionary<string, float[][]> motionTable;
        IEnumerator currentAnimation = null;
        private bool isPlaying = false;

        private bool isBreathActive = false;

        private readonly float playMotionDelayTime = 1f;

        // Test.
        Queue<MotionAnimInfo> animationQueue = new Queue<MotionAnimInfo>();

        private IEnumerator Start()
        {
            InitMotionTable();
            InitMotionData();

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

        //public bool PlayMotion(string gesture)
        public IEnumerator PlayMotionCoroutine(string gesture)
        {
            float[][] gestureInfo;
            if (motionTable.TryGetValue(gesture, out gestureInfo))
            {
                currentGesture = gesture;
                behaviorRecorder.RecordBehavior(new RecordEvent(0, gesture));
                yield return StartCoroutine("GestureProcess", gestureInfo);
                currentAnimation = null;
                //return true;
            }
            else
            {
                currentGesture = string.Empty;
                currentAnimation = null;
                //return false;
            }
        }

        void InitMotionData()
        {
            motions = new Motion[motionTable.Count];
            int index = 0;
            foreach (KeyValuePair<string, float[][]> item in motionTable)
            {
                motions[index++] = new Motion(item.Key, item.Value);
            }
        }

        void InitMotionTable()
        {
            motionTable = new Dictionary<string, float[][]>();
            motionTable.Add("hi", hiList);
            motionTable.Add("hello", helloList);
            motionTable.Add("angry", angryList);
            motionTable.Add("sad", sadList);
            motionTable.Add("ok", okList);
            motionTable.Add("clap", clapList);
            motionTable.Add("no", noList);
            motionTable.Add("wrong", noList);
            motionTable.Add("happy", happyList);
            // 버튼 왼쪽과 로봇이 바라보는 방향의 왼쪽이 달라서 반대로 설정함.
            motionTable.Add("nodRight", nodRightList);
            motionTable.Add("nodLeft", nodLeftList);
            motionTable.Add("breathing_active", breathing_active);
            motionTable.Add("breathing_inactive", breathing_inactive);
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

        [Serializable]
        public struct Motion
        {
            public string motionName;
            public MotionInfo[] motionInfo;

            public Motion(string name, float[][] info)
            {
                motionName = name;
                motionInfo = new MotionInfo[info.Length];
                for (int ix = 0; ix < motionInfo.Length; ++ix)
                {
                    motionInfo[ix].time = info[ix][0];

                    motionInfo[ix].leftArm.first = info[ix][1];
                    motionInfo[ix].leftArm.second = info[ix][2];
                    motionInfo[ix].leftArm.third = info[ix][3];

                    motionInfo[ix].rightArm.first = info[ix][4];
                    motionInfo[ix].rightArm.second = info[ix][5];
                    motionInfo[ix].rightArm.third = info[ix][6];

                    motionInfo[ix].head.first = info[ix][7];
                    motionInfo[ix].head.second = info[ix][8];
                }
            }
        }

        [Serializable]
        public struct MotionInfo
        {
            public float time;
            public ArmAngleInfo leftArm;
            public ArmAngleInfo rightArm;
            public HeadAngleInfo head;
        }

        [Serializable]
        public struct ArmAngleInfo
        {
            public float first;
            public float second;
            public float third;

            public ArmAngleInfo(float first, float second, float third)
            {
                this.first = first;
                this.second = second;
                this.third = third;
            }
        }

        [Serializable]
        public struct HeadAngleInfo
        {
            public float first;
            public float second;

            public HeadAngleInfo(float first, float second)
            {
                this.first = first;
                this.second = second;
            }
        }
    }
}