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

        [SerializeField] private Motion[] motions;

        public string currentGesture;

        string _gesture;
        //float duration = -1f;

        bool breatheEnable = true;

        // Y, 	Z, 	  Z,   Y, 	X, Z
        float[] zeroAngle = new float[8] { 0f, -90f, 90f, 90f, 0f, 0f, 0f, 0f };
        float[] baseAngle = new float[8] { 45f, -45f, -45f, -45f, 45f, 45f, 0f, 10f };
        float[] DIRECTION = new float[8] { -1f, 1f, 1f, 1f, -1f, -1f, 0f, 1f };

        float[] OFFSET = new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
        float[][] hiList = new float[5][] {
							// Time,	Left Arm, 				Right Arm,				Head
			new float[9] {  1.4f,       89.06f, -75f,-17.87f,   60.64f,60.64f,38.96f,   0f,-9.375f    },
            new float[9] {  0.6f,       89.06f, -75f, -17.87f,  60.64f, 96.97f, 34.28f, 0f, -9.375f   },
            new float[9] {  0.6f,       89.06f, -75f, -17.87f,  60.64f, 60.64f,38.96f,  0f, -9.375f   },
            new float[9] {  0.6f,       89.06f, -75f, -17.87f,  60.64f, 96.97f, 34.28f, 0f, -9.375f   },
            new float[9] {  1.4f,       57.42f, -75.29f,-23.73f,-57.42f,75.29f,23.73f,  0f,-9.375f    }
        };

        float[][] helloList = new float[5][] {
							// Time,	Left Arm, 				Right Arm,				Head
			new float[9] {  1.4f,       -60.64f, -60.64f, -38.96f,  60.64f, 60.64f, 38.96f,  0f, -9.375f   },
            new float[9] {  0.6f,       -60.64f, -96.97f, -34.28f,  60.64f, 96.97f, 34.28f,  0f, -9.375f   },
            new float[9] {  0.6f,       -60.64f, -60.64f, -38.96f,  60.64f, 60.64f, 38.96f,  0f, -9.375f   },
            new float[9] {  0.6f,       -60.64f, -96.97f, -34.28f,  60.64f, 96.97f, 34.28f,  0f, -9.375f   },
            new float[9] {  0.6f,        57.42f, -75f,    -23.73f,  -57.42f,75.29f, 23.73f,  0f, -9.375f   }
        };

        float[][] angryList = new float[4][] {
							// Time,	Left Arm, 				Right Arm,				Head
			new float[9] {  0.7f,    13.77f, -82.91f, -24.02f,   -13.77f,82.91f,24.02f, 0f,-9.375f    },
            new float[9] {  0.7f,    57.42f, -75.29f, -23.73f,  -57.42f, 75.29f, 23.73f, 0f, -9.375f   },
            new float[9] {  0.7f,    13.77f, -82.91f, -24.02f,  -13.77f, 82.91f,24.02f,  0f, -9.375f   },
            new float[9] {  0.7f,    57.42f, -75.29f, -23.73f,  -57.42f, 75.29f, 23.73f, 0f, -9.375f   },
        };
        float[][] sadList = new float[5][] {
							// Time,	Left Arm, 				Right Arm,				Head
			new float[9] {  1.2f,       -24.6f, -101.6f, -67.3f,   43.3f,101.6f,67.3f,     0f,  20f    },
            new float[9] {  0.5f,       -42.9f, -101.3f, -67f,     24f, 101.9f, 67.6f,     0f, 20f   },
            new float[9] {  0.5f,       -24.6f, -101.6f, -67.3f,   43.3f, 101.6f, 67.3f,   0f, 20f   },
            new float[9] {  0.5f,       -42.9f, -101.3f, -67f,     24f, 101.9f, 67.6f,      0f, 20f   },
            new float[9] {  1.2f,       57.42f, -75.29f, -23.73f,  -57.42f, 75.29f, 23.73f, 0f, -9.375f }
        };
        float[][] okList = new float[4][] {
							// Time,	Left Arm, 				Right Arm,				Head
			new float[9] {  0.5f, 57.42f, -75.29f, -23.73f, -57.42f, 75.29f, 23.73f, 0f,9.8f    },
            new float[9] {  0.5f, 57.42f, -75.29f, -23.73f, -57.42f, 75.29f, 23.73f, 0f, -9.8f   },
            new float[9] {  0.5f, 57.42f, -75.29f, -23.73f, -57.42f, 75.29f, 23.73f,  0f, 9.8f   },
            new float[9] {  0.5f, 57.42f, -75.29f, -23.73f, -57.42f, 75.29f, 23.73f, 0f, -9.375f   }

        };
        float[][] noList = new float[4][] {
							// Time,	Left Arm, 				Right Arm,				Head
			new float[9] {  0.6f, 57.42f, -75.29f, -23.73f, -57.42f, 75.29f, 23.73f, -20f,-9.375f    },
            new float[9] {  0.8f, 57.42f, -75.29f, -23.73f, -57.42f, 75.29f, 23.73f, 20f, -9.375f   },
            new float[9] {  0.8f, 57.42f, -75.29f, -23.73f, -57.42f, 75.29f, 23.73f,  -20f, -9.375f   },
            new float[9] {  0.8f, 57.42f, -75.29f, -23.73f, -57.42f, 75.29f, 23.73f,  0f,-9.375f    }
        };
        float[][] happyList = new float[4][] {
							// Time,	Left Arm, 				Right Arm,				Head
			//new float[9] {  1.6f,       -95f, -95f, -57f,   95f,95f,57f, 0f,-25f    },
   //         new float[9] {  0.8f, -95f, -95f, -57f, 95f, 95f, 57f, -14.36f, -9.96f   },
   //         new float[9] {  0.8f, -95f, -95f, -57f, 95f, 95f, 57f,  0f, -25f   },
   //         new float[9] {  1.6f, 57.42f, -75.29f, -23.73f, -57.42f, 75.29f, 23.73f,  0f,-9.375f    }
            new float[9] {  1.6f,       -40f, -95f, -57f,   40f,95f,57f, 0f,-25f    },
            new float[9] {  0.8f,       -40f, -95f, -57f,   40f, 95f, 57f, -14.36f, -9.96f   },
            new float[9] {  0.8f,       -40f, -95f, -57f,   40f, 95f, 57f,  0f, -25f   },
            new float[9] {  1.6f,       57.42f, -75.29f,-23.73f, -57.42f, 75.29f, 23.73f,  0f,-9.375f    }
        };

        Dictionary<string, float[][]> motionTable;
        IEnumerator currentAnimation = null;

        private IEnumerator Start()
        {
            //StartCoroutine("TestExecutor");
            //StartCoroutine(Test());

            InitMotionTable();
            InitMotionData();

            yield return StartCoroutine(SetBasePos());
            //StartCoroutine(Breath());

            //StartCoroutine(PlayMotion("happy"));

            //AnimationRunner(TestAllMotion());
            //AnimationRunner(Breath());

            //yield return StartCoroutine("TestAllMotion");
            //StartCoroutine(Breath());
        }

        public void AnimationRunner(string motion)
        {
            if (currentAnimation != null)
            {
                StopCoroutine(currentAnimation);
                currentAnimation = null;
                return;
            }

            currentAnimation = PlayMotion(motion);
            StartCoroutine(currentAnimation);
        }

        IEnumerator TestAllMotion()
        {
            foreach (KeyValuePair<string, float[][]> motion in motionTable)
            {
                yield return PlayMotion(motion.Key);
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
            for (int ix = 0; ix < jointInfo.Length; ++ix)
            {
                //jointInfo[ix].SetBaseAngle();
                //SetAngle(ix, baseAngle[ix]);
                SetAngleLerp(ix, baseAngle[ix], 2f);
            }

            yield return new WaitForSeconds(2f);
        }

        //public void SetBasePos()
        //{
        //    for (int ix = 0; ix < jointInfo.Length; ++ix)
        //    {
        //        //jointInfo[ix].SetBaseAngle();
        //        SetAngle(ix, baseAngle[ix]);
        //    }
        //}

        //public bool PlayMotion(string gesture)
        public IEnumerator PlayMotion(string gesture)
        {
            float[][] gestureInfo;
            if (motionTable.TryGetValue(gesture, out gestureInfo))
            {
                currentGesture = gesture;
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
            motionTable.Add("no", noList);
            motionTable.Add("happy", happyList);
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
            for (int ix = 0; ix < motionInfo.Length; ++ix)
            {
                for (int jx = 0; jx < jointInfo.Length; ++jx)
                {
                    StartCoroutine(jointInfo[jx].SetAngleLerp(motionInfo[ix][jx + 1], motionInfo[ix][0]));
                }

                float waitTime = motionInfo[ix][0];
                yield return new WaitForSeconds(waitTime);
            }

            yield return StartCoroutine(SetBasePos());

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

        int breathingUp = 1;
        IEnumerator Breath()
        {
            while (breath)
            {
                if (breatheEnable)
                {
                    if (breathingUp > 0)
                    {
                        for (int i = 0; i < jointInfo.Length; i++)
                        {
                            float angle = jointInfo[i].GetAngle();
                            jointInfo[i].SetAngle(angle + 0.1f * DIRECTION[i]);
                        }

                        breathingUp++;
                        if (breathingUp > 40)
                            breathingUp = -1;
                    }
                    else
                    {
                        for (int i = 0; i < jointInfo.Length; i++)
                        {
                            float angle = jointInfo[i].GetAngle();
                            jointInfo[i].SetAngle(angle - 0.1f * DIRECTION[i]);
                        }

                        breathingUp--;
                        if (breathingUp < -40)
                            breathingUp = 1;
                    }
                }
                yield return new WaitForSeconds(0.03f);
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

        public float GetAngle(int JointID)
        {
            return jointInfo[JointID].GetAngle();
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