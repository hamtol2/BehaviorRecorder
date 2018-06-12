using REEL.Recorder;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.PoseAnimation
{
	public class RobotTransformController : MonoBehaviour
	{
		public JointSet[] jointInfo;
		public bool breath = true;
        public BehaviorRecorder behaviorRecorder;

		string _gesture;
		float duration = -1;

		bool _breathe_enable = true;
        
		// Y, 	Z, 	  Z,   Y, 	X, Z
		float[] zeroAngle = new float[8] { 0f, -90f, 90f, 90f, 0f, 0f, 0f, 0f };
		float[] baseAngle = new float[8] { 45f, -45f, -45f, -45f,  45f, 45f, 0f, 10f };
		float[] DIRECTION = new float[8] {  -1f,  1f,   1f,   1f,  -1f, -1f, 0f,  1f };

		float[] OFFSET = new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
		//float[] OFFSET = new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
        float[][] hi_list = new float[5][] {
							// Time,	Left Arm, 				Right Arm,				Head
			new float[9] {  1.4f,       89.06f, -75f,-17.87f,   60.64f,60.64f,38.96f,   0f,-9.375f    },
            new float[9] {  0.6f,       89.06f, -75f, -17.87f,  60.64f, 96.97f, 34.28f, 0f, -9.375f   },
            new float[9] {  0.6f,       89.06f, -75f, -17.87f,  60.64f, 60.64f,38.96f,  0f, -9.375f   },
            new float[9] {  0.6f,       89.06f, -75f, -17.87f,  60.64f, 96.97f, 34.28f, 0f, -9.375f   },
            new float[9] {  1.4f,       57.42f, -75.29f,-23.73f,-57.42f,75.29f,23.73f,  0f,-9.375f    }
        };



        float[][] hello_list = new float[5][] {
							// Time,	Left Arm, 				Right Arm,				Head
			new float[9] {  1.4f,       -60.64f, -60.64f, -38.96f,  60.64f, 60.64f, 38.96f,  0f, -9.375f   },
            new float[9] {  0.6f,       -60.64f, -96.97f, -34.28f,  60.64f, 96.97f, 34.28f,  0f, -9.375f   },
            new float[9] {  0.6f,       -60.64f, -60.64f, -38.96f,  60.64f, 60.64f, 38.96f,  0f, -9.375f   },
            new float[9] {  0.6f,       -60.64f, -96.97f, -34.28f,  60.64f, 96.97f, 34.28f,  0f, -9.375f   },
            new float[9] {  0.6f,        57.42f, -75f,    -23.73f,  -57.42f,75.29f, 23.73f,  0f, -9.375f   }
        };

        float[][] angry_list = new float[4][] {
							// Time,	Left Arm, 				Right Arm,				Head
			new float[9] {  0.7f,    13.77f, -82.91f, -24.02f,   -13.77f,82.91f,24.02f, 0f,-9.375f    },
            new float[9] {  0.7f,    57.42f, -75.29f, -23.73f,  -57.42f, 75.29f, 23.73f, 0f, -9.375f   },
            new float[9] {  0.7f,    13.77f, -82.91f, -24.02f,  -13.77f, 82.91f,24.02f,  0f, -9.375f   },
            new float[9] {  0.7f,    57.42f, -75.29f, -23.73f,  -57.42f, 75.29f, 23.73f, 0f, -9.375f   },
        };
        float[][] sad_list = new float[5][] {
							// Time,	Left Arm, 				Right Arm,				Head
			new float[9] {  1.2f,       -24.6f, -101.6f, -67.3f,   43.3f,101.6f,67.3f,     0f,  20f    },
            new float[9] {  0.5f,       -42.9f, -101.3f, -67f,     24f, 101.9f, 67.6f,     0f, 20f   },
            new float[9] {  0.5f,       -24.6f, -101.6f, -67.3f,   43.3f, 101.6f, 67.3f,   0f, 20f   },
            new float[9] {  0.5f,       -42.9f, -101.3f, -67f,     24f, 101.9f, 67.6f,      0f, 20f   },
            new float[9] {  1.2f,       57.42f, -75.29f, -23.73f,  -57.42f, 75.29f, 23.73f, 0f, -9.375f }
        };
        float[][] ok_list = new float[4][] {
							// Time,	Left Arm, 				Right Arm,				Head
			new float[9] {  0.5f, 57.42f, -75.29f, -23.73f, -57.42f, 75.29f, 23.73f, 0f,9.8f    },
            new float[9] {  0.5f, 57.42f, -75.29f, -23.73f, -57.42f, 75.29f, 23.73f, 0f, -9.8f   },
            new float[9] {  0.5f, 57.42f, -75.29f, -23.73f, -57.42f, 75.29f, 23.73f,  0f, 9.8f   },
            new float[9] {  0.5f, 57.42f, -75.29f, -23.73f, -57.42f, 75.29f, 23.73f, 0f, -9.375f   }

        };
        float[][] no_list = new float[4][] {
							// Time,	Left Arm, 				Right Arm,				Head
			new float[9] {  0.6f, 57.42f, -75.29f, -23.73f, -57.42f, 75.29f, 23.73f, -20f,-9.375f    },
            new float[9] {  0.8f, 57.42f, -75.29f, -23.73f, -57.42f, 75.29f, 23.73f, 20f, -9.375f   },
            new float[9] {  0.8f, 57.42f, -75.29f, -23.73f, -57.42f, 75.29f, 23.73f,  -20f, -9.375f   },
            new float[9] {  0.8f, 57.42f, -75.29f, -23.73f, -57.42f, 75.29f, 23.73f,  0f,-9.375f    }
        };
        float[][] happy_list = new float[4][] {
							// Time,	Left Arm, 				Right Arm,				Head
			new float[9] {  1.6f,       -95f, -95f, -57f,   95f,95f,57f, 0f,-25f    },
            new float[9] {  0.8f, -95f, -95f, -57f, 95f, 95f, 57f, -14.36f, -9.96f   },
            new float[9] {  0.8f, -95f, -95f, -57f, 95f, 95f, 57f,  0f, -25f   },
            new float[9] {  1.6f, 57.42f, -75.29f, -23.73f, -57.42f, 75.29f, 23.73f,  0f,-9.375f    }
        };
     


        public void SetZeroPos()
        {
            for (int i = 0; i < 6; i++)
            {
                SetAngle(i, zeroAngle[i]);
            }
        }

        public void SetBasePos()
        {
            for (int ix = 0; ix < jointInfo.Length; ++ix)
            {
                //jointInfo[ix].SetBaseAngle();
                SetAngle(ix, baseAngle[ix]);
            }
        }

        public bool PlayGesture(string gesture)
        {
            // Record robot motion.
            //behaviorRecorder.RecordBehavior(new RecordEvent(0, gesture));
            switch (gesture)
            {
                case "hi": StartCoroutine("GestureProcess", hi_list); return true;
                case "hello": StartCoroutine("GestureProcess", hello_list); return true;
                case "angry": StartCoroutine("GestureProcess", angry_list); return true;
                case "sad": StartCoroutine("GestureProcess", sad_list); return true;
                case "ok": StartCoroutine("GestureProcess", ok_list); return true;
                case "no": StartCoroutine("GestureProcess", no_list); return true;
                case "happy": StartCoroutine("GestureProcess", happy_list); return true;
                default: return false;
            }
        }

        //     public void PlayGesture(string gesture)
        //     {
        //         // Record robot motion.
        //         behaviorRecorder.RecordBehavior(new RecordEvent(0, gesture));
        //         switch (gesture) {
        //	case "hi":
        //		StartCoroutine("GestureProcess", hi_list);
        //		break;
        //	case "hello":
        //		StartCoroutine("GestureProcess", hello_list);
        //		break;
        //	case "angry":
        //		StartCoroutine("GestureProcess", angry_list);
        //		break;
        //	case "sad":
        //		StartCoroutine("GestureProcess", sad_list);
        //		break;
        //	case "ok":
        //		StartCoroutine("GestureProcess", ok_list);
        //		break;
        //	case "no":
        //		StartCoroutine("GestureProcess", no_list);
        //		break;
        //	case "happy":
        //		StartCoroutine("GestureProcess", happy_list);
        //		break;
        //	default:
        //		break;
        //}
        //     }

        private void Awake()
        {
            //StartCoroutine("TestExecutor");
            //StartCoroutine(Test());
            SetBasePos();
            StartCoroutine(Breath());

            //PlayGesture("hi");
        }

        float GetPlayTime(float[][] gesture_list)
        {
            float playTime = 0f;
            foreach (float[] time in gesture_list)
            {
                playTime += time[0];
            }
            return playTime;
        }

        IEnumerator GestureProcess(float[][] gesture_info)
        {
            const float STEP = 0.1f;
            int index = 0;  // Series index of gesture list
            float duration = 0f;
            float playTimeToGo = GetPlayTime(gesture_info);

			_breathe_enable = false;

            while (playTimeToGo > STEP)
            {

                for (int i = 0; i < 8; i++)
                {
                    // Destination angle of a series
                    float angleDest = gesture_info[index][i + 1] + OFFSET[i + 1];
                    // Current angle of joint
                    float angleNow = GetAngle(i);
                    // Different between destination and currenat angles
                    float angleDiff = angleDest - angleNow;
                    // Angle ratio from 
                    float angleRatio = duration / gesture_info[index][0];
                    // Angle of to go now
                    float angleToGo = angleNow + (angleDiff * angleRatio);

                    SetAngle(i, angleToGo);
                }

                duration += STEP;
                if (duration >= gesture_info[index][0])
                {
                    index++;
                    duration = 0f;
                }
                playTimeToGo -= STEP;

                yield return new WaitForSeconds(STEP);
            }
            SetBasePos();

			_breathe_enable = true;

            yield return null;
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

            //for (int ix = 1; ix < 11; ++ix)
            //{
            //    SetAngle(1, ix * 10f);
            //    SetAngle(4, ix * 10f);

            //    yield return new WaitForSeconds(waitTime);
            //}

            //for (int ix = 1; ix < 10; ++ix)
            //{
            //    SetAngle(2, ix * 10f);
            //    SetAngle(5, ix * 10f);

            //    yield return new WaitForSeconds(waitTime);
            //}

            SetBasePos();
        }

        int breathing_up = 1;
        IEnumerator Breath()
        {
            while (breath)
            {
				if (_breathe_enable)
				{
					if (breathing_up > 0)
					{
						for (int i = 0; i < jointInfo.Length; i++)
						{
							float angle = jointInfo[i].GetAngle();
							jointInfo[i].SetAngle(angle + 0.1f * DIRECTION[i]);
						}
						//foreach (JointSet info in jointInfo)
						//{
						//    float angle = info.GetAngle();
						//    info.SetAngle(angle + 0.1f);
						//    //Debug.Log("angle: " + angle + ", to: " + (angle + 0.1f));
						//}
						breathing_up++;
						if (breathing_up > 40)
							breathing_up = -1;
					}
					else
					{
						for (int i = 0; i < jointInfo.Length; i++)
						{
							float angle = jointInfo[i].GetAngle();
							jointInfo[i].SetAngle(angle - 0.1f * DIRECTION[i]);
						}
						//foreach (JointSet info in jointInfo)
						//{
						//    float angle = info.GetAngle();
						//    info.SetAngle(angle - 0.1f);
						//    //Debug.Log("angle: " + angle + ", to: " + (angle - 0.1f));
						//}
						breathing_up--;
						if (breathing_up < -40)
							breathing_up = 1;
					}
				}
                yield return new WaitForSeconds(0.03f);
            }
        }

        public void SetAngle(int jointId, float angle)
        {
            jointInfo[jointId].SetAngle(angle);
        }
        public float GetAngle(int JointID)
        {
            return jointInfo[JointID].GetAngle();
        }

		private static RobotTransformController _instance = null;
		public static RobotTransformController Instance
        {
            get
            {
                if (_instance == null)
                {
					_instance = FindObjectOfType(typeof(RobotTransformController)) as RobotTransformController;
                }
                return _instance;
            }
        }

    }
}