using REEL.FaceInfomation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using REEL.Recorder;

namespace REEL.Animation
{
    [Serializable]
    class RobotFacialInfo
    {
        public string faceName;
        public float animPeriod;
        public RobotFacialInfo(string faceName, float animPeriod)
        {
            this.faceName = faceName;
            this.animPeriod = animPeriod;
        }
    }

    public class RobotFacialRenderer : MonoBehaviour
    {
        public RobotFacialData robotFacialData;
        public RobotFacialAnimator robotFacialAnimator;
        public SurveyController surveyController;
        public Text debugText;
        public bool isDebug = true;
        public string baseFace = "normal";

        public string currentFace = "";

        //Queue<RobotFacialInfo> facialQueue = new Queue<RobotFacialInfo>();
        [SerializeField] private List<RobotFacialInfo> facialQueue = new List<RobotFacialInfo>();
        private float defaultAnimPeriod = 1f;
        private RobotFacialInfo currentFacialInfo;

        private readonly string speakFaceName = "speak";
        private readonly string gazeFaceName = "gaze";

        public int queueCount = 0;

        void Awake()
        {
            if (robotFacialData == null) GetComponent<RobotFacialData>();
            if (robotFacialAnimator == null) GetComponent<RobotFacialRenderer>();

            Play(baseFace);

            //StartCoroutine("PlayAllForTest");
        }

        private void Update()
        {
            if (facialQueue != null)
                queueCount = facialQueue.Count;
        }

        public void Init()
        {

        }

        // 테스트용. 표정 정보 전체를 설정해가면서 이상 없는지 확인할 때 사용.
        IEnumerator PlayAllForTest()
        {
            foreach (RobotFacePartSO data in robotFacialData.partData)
            {
                yield return new WaitForSeconds(2f);

                Play(data.faceName);
            }
        }

        private void Play(RobotFacialInfo info)
        {
            //Debug.Log("Added to facial queue: " + info.faceName);

            // Test.
            if (currentFacialInfo != null)
            {
                //facialQueue.Enqueue(info);
                facialQueue.Add(info);
                return;
            }

            currentFacialInfo = info;
            SetFacialModel(info.faceName);
            robotFacialAnimator.PlayFacialAnim(info.faceName, info.animPeriod);

            StartCoroutine(CheckFacialAnimFinished(info.animPeriod));

            // 현재 표정 저장.
            currentFace = info.faceName;

            surveyController.behaviorRecorder.RecordBehavior(new Recorder.RecordEvent(1, info.faceName));
        }

        public void Play(string name)
        {
            float animTime = 1f;
            if (name.Contains(speakFaceName)) animTime = 0.8f;
            else if (name.Contains(gazeFaceName)) animTime = 0.4f;

            Play(new RobotFacialInfo(name, animTime));
        }

        IEnumerator CheckFacialAnimFinished(float animPeriod)
        {
            float elapsedTime = 0f;

            while (elapsedTime <= animPeriod)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            currentFacialInfo = null;
            if (facialQueue.Count > 0)
            {
                //Play(facialQueue.Dequeue());
                RobotFacialInfo info = facialQueue[0];
                facialQueue.RemoveAt(0);
                Play(info);
            }
            else
            {
                bool isSpeaking = SpeechRenderrer.Instance.IsSpeaking;
                //bool isActive = webSurvey.GetBehaviorMode == WebSurvey.Mode.Active;

                string faceName = string.Empty;
                if (isSpeaking && surveyController.GetFaceActiveState)
                {
                    faceName = "speak";
                }

                else if (!isSpeaking && surveyController.GetFaceActiveState)
                {
                    faceName = "normal_active";
                    //Debug.Log("Set Normal Active");
                }

                else if (!surveyController.GetFaceActiveState)
                    faceName = "normal_inactive";

                RobotFacialInfo info = new RobotFacialInfo(faceName, 1.0f);
                Play(info);
            }
        }

        public void Stop()
        {

        }

        public bool IsRunning()
        {
            return false;
        }

        // 표정 이름으로 캔버스에 있는 표정 설정하는 함수.
        //private void SetFacialModel(string faceName)
        private bool SetFacialModel(string faceName)
        {
            if (isDebug) debugText.text = faceName;

            for (int ix = 0; ix < robotFacialData.partData.Count; ++ix)
            {
                if (CompareTwoStrings(faceName, robotFacialData.partData[ix].faceName))
                {
                    SetFacePart(robotFacialData.partData[ix]);
                    return true;
                }
            }

            SetFacePart(robotFacialData.partData[4]);
            return false;
        }

        // 대문자, 소문자 관계없이 두 문자열을 비교하는 함수.
        bool CompareTwoStrings(string str1, string str2)
        {
            return str1.Equals(str2, StringComparison.CurrentCultureIgnoreCase);
        }

        // 캔버스에 있는 각 표정 이미지에 표정 정보 파일에서 불러온 정보를 설정하는 함수.
        void SetFacePart(RobotFacePartSO model)
        {
            TurnOffAllFacialPart();

            for (int ix = 0; ix < model.faceParts.Count; ++ix)
            {
                SpriteRenderer partRenderer = robotFacialData.partDictionary[model.faceParts[ix].facialPartEnum];
                partRenderer.gameObject.SetActive(true);
                SetSprite(partRenderer, model.faceParts[ix]);
            }
        }

        // 캔버스 이미지에 스프라이트 설정하는 함수.
        void SetSprite(SpriteRenderer partRenderer, RobotFacePart facePart)
        {
            partRenderer.sprite = facePart.partSprite;
            SetSpriteSize(partRenderer, facePart.partScale);
            SetSpritePosition(partRenderer, facePart.partPosition);
        }

        // 캔버스 이미지 위치 설정 함수.
        void SetSpritePosition(SpriteRenderer partRenderer, Vector2 partPosition)
        {
            partRenderer.transform.localPosition = partPosition;
        }

        // 캔버스 이미지 크기 설정 함수.
        void SetSpriteSize(SpriteRenderer partRenderer, Vector2 partScale)
        {
            partRenderer.transform.localScale = partScale;
        }

        // 스프라이트가 속한 텍스처에서 크기 정보 구하는 함수.
        Vector2 GetSpriteSize(Sprite sprite)
        {
            return sprite.textureRect.size;
        }

        // 표정 파트 전체를 끄는 함수.
        // 표정 변경할 때 일단 파트 전체를 끈 다음 필요한 게임 오브젝트를 활성화해 설정함.
        void TurnOffAllFacialPart()
        {
            for (int ix = 0; ix < robotFacialData.partSprites.Length; ++ix)
            {
                robotFacialData.partSprites[ix].gameObject.SetActive(false);
                robotFacialData.partSprites[ix].transform.localScale = Vector3.one;
            }
        }
    }
}