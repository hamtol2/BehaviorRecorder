using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.Recorder
{
    public class SurveyStartManager : Singleton<SurveyStartManager>
    {
        [SerializeField] private GameObject[] stages;

        private int currentStage = 0;

        private int stage1CompleteCount = 0;
        private int gazeTargetCount = 3;

        private float waitTime = 2f;

        // Stage1.
        public void AddHitCount()
        {
            ++stage1CompleteCount;
            if (stage1CompleteCount == gazeTargetCount)
            {
                Invoke("NextStage", waitTime);
            }
        }

        public void GoingForward()
        {
            Invoke("NextStage", 0.5f);
        }

        private void NextStage()
        {
            if (currentStage < stages.Length)
            {
                stages[currentStage++].SetActive(false);
                stages[currentStage].SetActive(true);
            }
        }
    }
}