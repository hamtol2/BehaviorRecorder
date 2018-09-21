using System.Collections.Generic;
using System.IO;
using Tobii.Gaming;
using UnityEngine;
using REEL.FaceInfomation;
using REEL.PoseAnimation;
using REEL.Animation;

namespace REEL.Recorder
{
    public class BehaviorRecorder : MonoBehaviour
    {
        public float frame = 30f;
        public string fileName = "Behavior.json";
        public bool autoSave = true;
        public Transform robotTransform;
        public RobotFacialRenderer facialRenderer;
        public RobotTransformController gestureController;

        //private List<RecordFormat> records = new List<RecordFormat>();
        private RecordJsonFormat saveData = new RecordJsonFormat();
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
            if (saveData == null || saveData.Length == 0) return;

            string jsonString = JsonUtility.ToJson(saveData);
            File.WriteAllText(filePath, jsonString);
        }

        public void RecordBehavior()
        {
            if (!isRecording) return;

            RecordData newData = new RecordData();
            newData.quizTitle = QuizManager.Instance.quizTitle;
            newData.quizNumber = QuizManager.Instance.quizNumber;
            newData.elapsedTime = mainTimer.GetElapsedTime;
            newData.contentState = QuizManager.Instance.quizState;
            newData.answer = QuizManager.Instance.answerState;
            newData.modelType = QuizManager.Instance.robotModelType;
            newData.eyePosition = EyePoint;
            newData.robotPosition = robotTransform.position;
            newData.targetRegion = GetTargetRegion;
            newData.face = facialRenderer.currentFace;
            newData.gesture = gestureController.currentGesture;

            saveData.AddData(newData);
        }

        public Vector2 EyePoint
        {
            get
            {
                GazePoint gazePoint = TobiiAPI.GetGazePoint();
                return TobbiTester.Instance.isMouseTracking ? new Vector2(Input.mousePosition.x, Input.mousePosition.y) : gazePoint.Screen;
            }
        }

        // todo : 마우스 위치 기준으로 로봇의 위치 정보 반환하도록 기능 추가해야 함.
        public TargetRegion GetTargetRegion
        {
            get
            {
                return TargetRegion.Eye;
            }
        }

        //public void RecordBehavior(RecordEvent newEvent)
        //{
        //    if (!isRecording) return;

        //    RecordFormat newRecord = new RecordFormat();
        //    newRecord.elapsedTime = mainTimer.GetElapsedTime;
        //    newRecord.markerPosition = new CustomVector2(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        //    newRecord.recordEvent = newEvent;

        //    records.Add(newRecord);
        //}

        private void InitState()
        {
            fps = 1f / frame;

            mainTimer = new Timer();
            mainTimer.SetTimer(fps, RecordBehavior);

            filePath = Application.dataPath + "/" + fileName;
        }

        public void ResetState()
        {
            //records = new List<RecordFormat>();
            saveData = new RecordJsonFormat();
            mainTimer.Reset();
        }
    }
}