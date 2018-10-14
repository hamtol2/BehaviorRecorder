using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.Recorder
{
    public class SurveyStartManager : Singleton<SurveyStartManager>
    {
        [SerializeField] private GameObject[] stages;

        private int currentState = 0;

        private void NextStage()
        {
            if (currentState < stages.Length)
            {
                stages[currentState++].SetActive(false);
                stages[currentState].SetActive(true);
            }
        }
    }
}