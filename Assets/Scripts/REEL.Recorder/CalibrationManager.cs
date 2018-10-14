using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace REEL.Recorder
{
    public class CalibrationManager : Singleton<CalibrationManager>
    {
        [SerializeField] private int hitCount = 0;
        [SerializeField] private int targetHitCount;

        public void AddHitCount()
        {
            ++hitCount;
            if (hitCount >= targetHitCount)
                Debug.Log("done");
        }
    }
}