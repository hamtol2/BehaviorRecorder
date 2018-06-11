using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using REEL.Animation;
using System.IO;

namespace REEL.Recorder
{
    public class BehaviorReplayer : MonoBehaviour
    {
        public string fileName = "Behavior.json";
        public RobotFacialRenderer facialRenderer;
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
            InitState();
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

                if (records[currentIndex].recordEvent != null)
                {
                    facialRenderer.Play(records[currentIndex].recordEvent.eventValue);
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
            InitState();
            isReplaying = true;
            marker.SetActive(true);
        }

        public void StopReplay()
        {
            isReplaying = false;
            ResetState();
        }

        private void InitState()
        {   
            string filePath = Application.dataPath + "/" + fileName;
            string jsonString = File.ReadAllText(filePath);
            records = SimpleJson.SimpleJson.DeserializeObject<RecordFormat[]>(jsonString);
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
        }
    }
}