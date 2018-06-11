using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace REEL.Recorder
{
    public class BehaviorRecorder : MonoBehaviour
    {
        public float frame = 30f;
        public string fileName = "Behavior.json";

        private List<RecordFormat> records = new List<RecordFormat>();
        private float fps = 0f;
        private Timer mainTimer;
        private string filePath;

        private void Awake()
        {
            fps = 1f / frame;
            mainTimer = new Timer(fps, RecordBehavior);
            filePath = Application.dataPath + "/" + fileName;
        }

        private void OnDisable()
        {
            ResetState();
        }

        private void OnApplicationQuit()
        {
            string jsonString = SimpleJson.SimpleJson.SerializeObject(records);
            File.WriteAllText(filePath, jsonString);
        }

        private void Update()
        {
            mainTimer.Update(Time.deltaTime);
        }

        private void RecordBehavior()
        {
            RecordFormat newRecord = new RecordFormat();
            newRecord.elapsedTime = mainTimer.GetElapsedTime;
            newRecord.markerPosition = new CustomVector2(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            records.Add(newRecord);
        }

        public void RecordBehavior(RecordEvent newEvent)
        {
            RecordFormat newRecord = new RecordFormat();
            newRecord.elapsedTime = mainTimer.GetElapsedTime;
            newRecord.markerPosition = new CustomVector2(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            newRecord.recordEvent = newEvent;

            records.Add(newRecord);
        }

        public void ResetState()
        {
            records = new List<RecordFormat>();
        }
    }
}