using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;

namespace REEL.Recorder
{
    public class TobbiTester : MonoBehaviour
    {
        public GameObject marker;

        private void Update()
        {
            GazePoint gazePoint = TobiiAPI.GetGazePoint();
            if (CanMove(gazePoint))
            {
                MoveMarker(gazePoint);
            }
        }

        void MoveMarker(GazePoint gazePoint)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(gazePoint.Screen);
            worldPos.z = -5f;

            marker.transform.position = worldPos;
        }

        bool CanMove(GazePoint gazePoint)
        {
            return !float.IsNaN(gazePoint.Screen.x) && !float.IsNaN(gazePoint.Screen.y);
        }
    }
}