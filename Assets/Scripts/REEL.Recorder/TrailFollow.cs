using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.Recorder
{
	public class TrailFollow : MonoBehaviour
	{
        public Transform target;
        public bool isSmooth = true;
        public float smoothValue = 10f;

        private void Awake()
        {
            
        }

        private void Update()
        {
            transform.position = isSmooth ? Vector3.Lerp(transform.position, target.position, Time.deltaTime * smoothValue) : target.position;
        }
    }
}