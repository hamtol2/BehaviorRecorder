using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.EAIEditor
{
    public class EventItem : GraphItem
    {
        [SerializeField]
        private string selectedEvent = string.Empty;

        private EventPopup refEventPopup;

        public override void SetItemData(object data)
        {
            base.SetItemData(data);
            selectedEvent = data as string;
        }
    }
}