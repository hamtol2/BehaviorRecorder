﻿using System;
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

        public RecordFormat[] records;
        private Timer mainTimer;
        private int currentIndex = 0;
        private bool isReplaying = false;

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

            mainTimer.Update(Time.deltaTime);

            if (mainTimer.GetElapsedTime >= records[currentIndex].elapsedTime)
            {
                Vector3 markerPos = records[currentIndex].markerPosition.ToVector2();
                markerPos.z = -5f;
                marker.transform.position = markerPos;

                if (records[currentIndex].recordEvent != null && records[currentIndex].recordEvent.eventType != -1)
                {
                    // -1 : none / 0 : motion / 1 : facial.
                    int eventType = records[currentIndex].recordEvent.eventType;

                    if (eventType == 0) transformController.PlayGesture(records[currentIndex].recordEvent.eventValue);
                    else facialRenderer.Play(records[currentIndex].recordEvent.eventValue);
                }

                ++currentIndex;

                if (currentIndex >= records.Length)
                {
                    StopReplay();
                    return;
                }
            }
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
            records = SimpleJson.SimpleJson.DeserializeObject<RecordFormat[]>(jsonString);

            // Set active state.
            isReplaying = true;
            marker.SetActive(true);

            // Set base robot face.
            facialRenderer.Play(facialRenderer.baseFace);
        }

        private void ResetState()
        {
            mainTimer.Reset();
            if (records != null && records.Length > 0)
            {
                Array.Clear(records, 0, records.Length);
                //records = null;
            }

            marker.SetActive(false);
            currentIndex = 0;

            // Set base robot face.
            facialRenderer.Play(facialRenderer.baseFace);
        }
    }
}