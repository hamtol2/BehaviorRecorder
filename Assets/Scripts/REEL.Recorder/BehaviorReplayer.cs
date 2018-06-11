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

        private void Awake()
        {
            mainTimer = new Timer();
        }

        private void OnEnable()
        {
            string filePath = Application.dataPath + "/" + fileName;
            string jsonString = File.ReadAllText(filePath);
            records = SimpleJson.SimpleJson.DeserializeObject<RecordFormat[]>(jsonString);

            marker.SetActive(true);
        }

        private void OnDisable()
        {
            ResetState();
        }

        private void Update()
        {
            if (currentIndex >= records.Length)
            {
                gameObject.SetActive(false);
                return;
            }   

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
            }
        }

        private void ResetState()
        {
            mainTimer.Reset();
            Array.Clear(records, 0, records.Length);
            records = null;
            marker.SetActive(false);
            currentIndex = 0;
        }
    }
}