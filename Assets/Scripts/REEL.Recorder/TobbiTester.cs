using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;

namespace REEL.Recorder
{
    public class TobbiTester : Singleton<TobbiTester>
    {
        public GameObject marker;

        [SerializeField] private bool isShowMarker = true;
        public bool isMouseTracking = false;

        // for mevement smoothing.
        [SerializeField] private float smoothTime = 15f;

        [SerializeField] private bool isSimulation = true;

        private void Awake()
        {

        }

        private void Update()
        {
            if (!isShowMarker) return;
            if (!isSimulation) return;

            if (isMouseTracking)
                MoveMarkerWithMouse();

            else
                MoveMarkerWithTobii();
        }

        public void SetSimulationMode(bool isSimulation)
        {
            this.isSimulation = isSimulation;
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
    }
}