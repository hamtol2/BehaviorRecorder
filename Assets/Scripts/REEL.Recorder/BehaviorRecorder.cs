﻿using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace REEL.Recorder
{
    public class BehaviorRecorder : MonoBehaviour
    {
        public float frame = 30f;
        public string filePath = "Behavior.json";
        public List<RecordFormat> records = new List<RecordFormat>();

        private float fps = 0f;
        private Timer mainTimer;

        private void Awake()
        {
            fps = 1f / frame;
            mainTimer = new Timer(fps, RecordBehavior);
        }

        private void Update()
        {
            mainTimer.Update(Time.deltaTime);
        }

        private void RecordBehavior()
        {
            //Debug.Log("RecordBehavior Called.");
        }

        public void ResetState()
        {
            records = new List<RecordFormat>();
        }
    }
}