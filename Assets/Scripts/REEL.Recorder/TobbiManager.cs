using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;
using System.IO;

namespace REEL.Recorder
{
    public class TobbiManager : Singleton<TobbiManager>
    {
        public GameObject marker;

        [SerializeField] private TextAsset settingText;

        [SerializeField] private bool isShowMarker = true;
        public bool isMouseTracking = false;

        // for mevement smoothing.
        [SerializeField] private float smoothTime = 15f;

        [SerializeField] private bool isSimulation = true;

        private readonly string useMouseKey = "UseMouse";

        private void Awake()
        {
            TobbiSettingFormat setting = JsonUtility.FromJson<TobbiSettingFormat>(settingText.text);
            isMouseTracking = setting.isUsingMouse;
            if (setting.isUsingMarker) marker.SetActive(true);
        }

        private void Update()
        {
            if (!isShowMarker) return;
            if (!isSimulation) return;

            if (isMouseTracking)
            {
                MoveMarkerWithMouse();
                RaycastCheck(Input.mousePosition);
            }
            else
            {
                MoveMarkerWithTobii();
                GazePoint gazePoint = TobiiAPI.GetGazePoint();
                RaycastCheck(gazePoint.Screen);
            }
        }

        public void SetSimulationMode(bool isSimulation)
        {
            this.isSimulation = isSimulation;
        }

        void RaycastCheck(Vector2 screenPos)
        {
            //Ray ray = Camera.main.ScreenPointToRay(screenPos);
            //RaycastHit hit;

            //if (Physics.Raycast(ray, out hit, 100f))
            //{
            //    Debug.Log(hit.transform.gameObject.tag);
            //}
        }

        void MoveMarkerWithMouse()
        {
            if (CanMove(Input.mousePosition))
            {
                MoveMarker(Input.mousePosition);
            }
        }

        void MoveMarkerWithTobii()
        {
            GazePoint gazePoint = TobiiAPI.GetGazePoint();
            if (CanMove(gazePoint.Screen))
            {
                MoveMarker(gazePoint);
            }
        }

        void MoveMarker(GazePoint gazePoint)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(gazePoint.Screen);
            worldPos.z = -5f;

            marker.transform.position = Vector3.Lerp(marker.transform.position, worldPos, Time.deltaTime * smoothTime);
        }

        void MoveMarker(Vector3 mousePoint)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePoint);
            worldPos.z = -5f;

            marker.transform.position = Vector3.Lerp(marker.transform.position, worldPos, Time.deltaTime * smoothTime);
        }

        bool CanMove(Vector3 screenPos)
        {
            return !float.IsNaN(screenPos.x) && !float.IsNaN(screenPos.y);
        }

        public Vector2 GetEyePoint
        {
            get
            {
                GazePoint point = TobiiAPI.GetGazePoint();
                return isMouseTracking ? new Vector2(Input.mousePosition.x, Input.mousePosition.y) : point.Screen;
            }
        }
    }
}