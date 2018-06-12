using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace REEL.Recorder
{
    public class BehaviorRecorder : MonoBehaviour
    {
        public float frame = 30f;
        public string fileName = "Behavior.json";
        public bool autoSave = true;

        private List<RecordFormat> records = new List<RecordFormat>();
        private float fps = 0f;
        private Timer mainTimer;
        private string filePath;
        private bool isRecording = false;
        
        private void Awake()
        {
            //InitState();
        }

        private void OnEnable()
        {
            InitState();
        }

        private void OnDisable()
        {
            ResetState();
        }

        private void Update()
        {
            if (isRecording) mainTimer.Update(Time.deltaTime);
        }

        private void OnApplicationQuit()
        {
            FinishRecording();
        }

        public void StartRecording()
        {
            isRecording = true;
        }

        public void FinishRecording()
        {
            isRecording = false;
            if (autoSave) SaveFile();
            ResetState();
        }

        public void SaveFile()
        {
            if (records == null || records.Count == 0)
                return;

            string jsonString = SimpleJson.SimpleJson.SerializeObject(records);
            //Debug.Log(jsonString);
            File.WriteAllText(filePath, jsonString);
        }

        private void RecordBehavior()
        {
            if (!isRecording) return;

            RecordFormat newRecord = new RecordFormat();
            newRecord.elapsedTime = mainTimer.GetElapsedTime;
            newRecord.markerPosition = new CustomVector2(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            records.Add(newRecord);
        }

        public void RecordBehavior(RecordEvent newEvent)
        {
            if (!isRecording) return;

            RecordFormat newRecord = new RecordFormat();
            newRecord.elapsedTime = mainTimer.GetElapsedTime;
            newRecord.markerPosition = new CustomVector2(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            newRecord.recordEvent = newEvent;

            records.Add(newRecord);
        }

        private void InitState()
        {
            fps = 1f / frame;

            mainTimer = new Timer();
            mainTimer.SetTimer(fps, RecordBehavior);

            filePath = Application.dataPath + "/" + fileName;
        }

        public void ResetState()
        {
            records = new List<RecordFormat>();
            mainTimer.Reset();
        }
    }
}