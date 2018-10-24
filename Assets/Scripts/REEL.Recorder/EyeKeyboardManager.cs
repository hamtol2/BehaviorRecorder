﻿using System.Collections;
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

            // position IsNaN 확인.
            if (IsNaN(data.position)) return;

            raycaster.Raycast(data, results);

            foreach (RaycastResult result in results)
            {
                EyeTypingButton button = result.gameObject.GetComponent<EyeTypingButton>();
                if (button == null) continue;

                // 활성화인 경우에만 타이머 갱신.
                if (button.gameObject.activeInHierarchy)
                    button.UpdateTimer();
            }
        }

        bool IsNaN(Vector2 position)
        {
            return float.IsNaN(position.x) || float.IsNaN(position.y);
        }
    }
}