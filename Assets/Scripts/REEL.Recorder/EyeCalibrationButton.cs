using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace REEL.Recorder
{
    public class EyeCalibrationButton : MonoBehaviour
    {
        [SerializeField] private Image gaugeImage;
        [SerializeField] protected Text text;
        [SerializeField] protected GameObject scatterParticle;

        public void OnEyeClicked()
        {
            scatterParticle.SetActive(true);
            this.GetComponent<Image>().enabled = false;
            gaugeImage.enabled = false;
            text.enabled = false;
        }
    }
}