using System.Collections.Generic;
using System.IO;
using Tobii.Gaming;
using UnityEngine;
using REEL.FaceInfomation;
using REEL.PoseAnimation;
using REEL.Animation;
using System;

namespace REEL.Recorder
{
    public class BehaviorRecorder : MonoBehaviour
    {
        public float frame = 30f;
        public string fileName = "Behavior.json";
        public bool autoSave = true;
        public RobotMovement robotMovement;
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
            newData.quizTitle = WebSurvey.Instance.QuizTitle;
            newData.quizType = WebSurvey.Instance.GetQuizType;
            newData.age = WebSurvey.Instance.GetAge;
            newData.gender = WebSurvey.Instance.GetGender;
            newData.quizNumber = WebSurvey.Instance.GetCurrentStep();
            newData.elapsedTime = mainTimer.GetElapsedTime;
            newData.contentState = WebSurvey.Instance.GetCurrentState().ToString();
            newData.answer = WebSurvey.Instance.GetAnswerState.ToString();
            newData.modelType = WebSurvey.Instance.GetModelType.ToString();
            newData.eyePosition = TobbiManager.Instance.GetEyePoint;
            newData.robotPosition = robotMovement.transform.position;
            newData.robotState = robotMovement.GetRobotState.ToString();
            newData.targetRegion = GetTargetRegion.ToString();
            newData.face = facialRenderer.currentFace;
            newData.gesture = gestureController.currentGesture;

            saveData.AddData(newData);
        }

        public void RecordBehavior(RecordEvent newEvent)
        {
            if (!isRecording) return;

            RecordData newData = new RecordData();
            newData.quizTitle = WebSurvey.Instance.QuizTitle;
            newData.quizType = WebSurvey.Instance.GetQuizType;
            newData.age = WebSurvey.Instance.GetAge;
            newData.gender = WebSurvey.Instance.GetGender;
            newData.quizNumber = WebSurvey.Instance.GetCurrentStep();
            newData.elapsedTime = mainTimer.GetElapsedTime;
            newData.contentState = WebSurvey.Instance.GetCurrentState().ToString();
            newData.answer = WebSurvey.Instance.GetAnswerState.ToString();
            newData.modelType = WebSurvey.Instance.GetModelType.ToString();
            newData.eyePosition = TobbiManager.Instance.GetEyePoint;
            newData.robotPosition = robotMovement.transform.position;
            newData.robotState = robotMovement.GetRobotState.ToString();
            newData.targetRegion = GetTargetRegion.ToString();
            newData.face = facialRenderer.currentFace;
            newData.gesture = gestureController.currentGesture;
            newData.recordEvent = newEvent;

            saveData.AddData(newData);
        }

        // 마우스/토비 위치 기준으로 로봇 파츠 위치 정보 반환.
        public TargetRegion GetTargetRegion
        {
            get
            {
                // Check if O/X button is being gazed.
                if (EyeKeyboardManager.Instance.IsAnswerButtonGazed)
                {
                    if (EyeKeyboardManager.Instance.GetGazedButtonDirection.Contains("left"))
                        return TargetRegion.LeftButton;
                    else if (EyeKeyboardManager.Instance.GetGazedButtonDirection.Contains("right"))
                        return TargetRegion.RightButton;
                }

                Ray ray = Camera.main.ScreenPointToRay(TobbiManager.Instance.GetEyePoint);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100f))
                {
                    string tag = hit.transform.gameObject.tag.ToLower();

                    if (tag.StartsWith("eye"))
                        return TargetRegion.Eye;
                    else if (tag.StartsWith("mouth"))
                        return TargetRegion.Mouth;
                    else if (tag.StartsWith("face"))
                        return TargetRegion.Face;
                    else if (tag.StartsWith("arm"))
                        return TargetRegion.Arm;
                    else if (tag.StartsWith("finger"))
                        return TargetRegion.FingerTip;
                    else if (tag.StartsWith("body"))
                        return TargetRegion.Body;
                    else if (tag.StartsWith("background"))
                        return TargetRegion.Background;
                    else
                        return TargetRegion.OffSight;
                }

                return TargetRegion.OffSight;
            }
        }

        private void InitState()
        {
            fps = 1f / frame;

            mainTimer = new Timer();
            mainTimer.SetTimer(fps, RecordBehavior);

            if (!Directory.Exists(SurveyUtil.GetFolderPath))
                Directory.CreateDirectory(SurveyUtil.GetFolderPath);

            filePath = SurveyUtil.GetSurveyFilePath;
        }

        public void ResetState()
        {
            //records = new List<RecordFormat>();
            saveData = new RecordJsonFormat();
            mainTimer.Reset();
        }
    }
}