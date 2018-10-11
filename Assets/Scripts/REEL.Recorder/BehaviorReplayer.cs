using System;
using System.Collections;
using UnityEngine;
using REEL.Animation;
using REEL.PoseAnimation;

namespace REEL.Recorder
{
    public class BehaviorReplayer : MonoBehaviour
    {
        public string fileName = "Behavior.json";
        public RobotFacialRenderer facialRenderer;
        public RobotTransformController transformController;
        public GameObject marker;
        public RobotMovement robotMovement;

        public RecordJsonFormat recordData;
        private Timer mainTimer;
        private int currentIndex = 0;
        private bool isReplaying = false;

        private float moveLerpSpeed = 10f;

        private void Awake()
        {
            mainTimer = new Timer();
        }

        private void OnEnable()
        {
            //InitState();
        }

        private void OnDisable()
        {
            //ResetState();
        }

        private void Update()
        {
            if (!isReplaying) return;

            PlayJsonData();

            //mainTimer.Update(Time.deltaTime);

            //if (mainTimer.GetElapsedTime >= records[currentIndex].elapsedTime)
            //{
            //    Vector3 markerPos = records[currentIndex].markerPosition.ToVector2();
            //    markerPos.z = -5f;
            //    marker.transform.position = markerPos;

            //    if (records[currentIndex].recordEvent != null && records[currentIndex].recordEvent.eventType != -1)
            //    {
            //        // -1 : none / 0 : motion / 1 : facial.
            //        int eventType = records[currentIndex].recordEvent.eventType;

            //        if (eventType == 0) transformController.PlayGesture(records[currentIndex].recordEvent.eventValue);
            //        else facialRenderer.Play(records[currentIndex].recordEvent.eventValue);
            //    }

            //    ++currentIndex;

            //    if (currentIndex >= records.Length)
            //    {
            //        StopReplay();
            //        return;
            //    }
            //}
        }

        public void PlayReplay()
        {
            StartCoroutine("InitState");
            //isReplaying = true;
            //marker.SetActive(true);
        }

        public void StopReplay()
        {
            isReplaying = false;
            ResetState();
        }

        private IEnumerator InitState()
        {
            // Set file path.
            string filePath = Application.dataPath + "/" + fileName;

            // Load json text file.
            WWW www = new WWW("file://" + filePath);
            yield return www;

            // Read json data and convert it to certain format.
            string jsonString = www.text;
            //records = SimpleJson.SimpleJson.DeserializeObject<RecordFormat[]>(jsonString);
            recordData = JsonUtility.FromJson<RecordJsonFormat>(jsonString);

            // Set active state.
            isReplaying = true;
            marker.SetActive(true);

            // Disable EyePoint tracking.
            TobbiManager.Instance.SetSimulationMode(false);

            // Disable Robot movement.
            robotMovement.SetSimulation(false);

            // Set base robot face.
            facialRenderer.Play(facialRenderer.baseFace);
        }

        private void PlayJsonData()
        {
            mainTimer.Update(Time.deltaTime);

            //if (mainTimer.GetElapsedTime >= records[currentIndex].elapsedTime)
            if (mainTimer.GetElapsedTime >= recordData[currentIndex].elapsedTime)
            {
                Vector3 markerPos = Camera.main.ScreenToWorldPoint(recordData[currentIndex].eyePosition);
                markerPos.z = -5f;
                //marker.transform.position = markerPos;
                marker.transform.position = Vector3.Lerp(marker.transform.position, markerPos, Time.deltaTime * moveLerpSpeed);

                robotMovement.transform.position
                    = Vector3.Lerp(robotMovement.transform.position, recordData[currentIndex].robotPosition, Time.deltaTime * moveLerpSpeed);
                //robotMovement.transform.position = recordData[currentIndex].robotPosition;

                if (HasRecordEvent(recordData[currentIndex]))
                {
                    // -1 : none / 0 : motion / 1 : facial.
                    int eventType = recordData[currentIndex].recordEvent.eventType;

                    if (eventType == 0) transformController.PlayMotion(recordData[currentIndex].recordEvent.eventValue);
                    else facialRenderer.Play(recordData[currentIndex].recordEvent.eventValue);
                }

                ++currentIndex;

                if (currentIndex >= recordData.Length)
                {
                    StopReplay();
                    return;
                }
            }
        }

        bool HasRecordEvent(RecordData record)
        {
            return record.recordEvent != null && record.recordEvent.eventType != -1;
        }

        private void ResetState()
        {
            mainTimer.Reset();
            if (recordData != null && recordData.Length > 0)
            {
                recordData = new RecordJsonFormat();
            }

            marker.SetActive(false);
            currentIndex = 0;

            // Enable EyePoint tracking.
            TobbiManager.Instance.SetSimulationMode(true);

            // Move robot to origin.
            robotMovement.MoveToOrigin();

            // Set base robot face.
            facialRenderer.Play(facialRenderer.baseFace);
        }
    }
}