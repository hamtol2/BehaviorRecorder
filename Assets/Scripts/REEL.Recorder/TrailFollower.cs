using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.Recorder
{
    public class TrailFollower : MonoBehaviour
    {
        public TrailFollow[] fellow;

        public float smoothInitValue;
        public int smoothInterval = 1;

        public void Awake()
        {
            int index = 0;
            foreach (TrailFollow follow in fellow)
            {
                follow.smoothValue = smoothInitValue - index;
                index = index + smoothInterval;
            }
        }
    }
}