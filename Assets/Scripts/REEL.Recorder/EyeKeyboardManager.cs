using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace REEL.Recorder
{
    public class EyeKeyboardManager : MonoBehaviour
    {
        [SerializeField] private GraphicRaycaster raycaster;
        [SerializeField] private EventSystem eventSystem;
        [SerializeField] private Timer timer = new Timer();

        private PointerEventData data;
        private List<RaycastResult> results;

        private void Awake()
        {
            data = new PointerEventData(eventSystem);
        }

        private void Update()
        {
            RaycastByMouseOrTobii();
        }

        void RaycastByMouseOrTobii()
        {
            data.position = TobbiManager.Instance.GetEyePoint;

            results = new List<RaycastResult>();
            raycaster.Raycast(data, results);

            foreach (RaycastResult result in results)
            {
                TimerButton button = result.gameObject.GetComponent<TimerButton>();
                if (button == null) continue;

                //button.UpdateTimer(Time.deltaTime);
                button.UpdateTimer();
            }
        }
    }
}