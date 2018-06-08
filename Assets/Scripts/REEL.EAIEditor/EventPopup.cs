using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace REEL.EAIEditor
{
    public class EventPopup : Popup
    {
        [Serializable]
        public class EventCategory
        {
            public string categoryName;
            public string[] eventList;
        }

        public enum EventType
        {
            FrontHeadTouched, LeftBumperTouched, RightBumperTouched
        }

        [SerializeField]
        private TextAsset eventListJson;

        [SerializeField]
        private RectTransform content;

        [SerializeField]
        private RectTransform topListPane;

        [SerializeField]
        private Text categoryTextPrefab;

        [SerializeField]
        private EventNameItem eventNamePrefab;

        private float blockHeight = -25f;
        private float leftMargin = 30f;
        private Vector2 anchorMin = new Vector2(0f, 1f);
        private Vector2 anchorMax = new Vector2(1f, 1f);
        private Vector2 pivot = new Vector2(0.5f, 1f);

        public EventCategory[] eventCategories;

        private EventType selectedEventType;

        [SerializeField]
        private string selectedEvent = string.Empty;

        private List<EventNameItem> eventListItems = new List<EventNameItem>();

        public override void OnOKClicked()
        {
            if (targetItem) targetItem.SetItemData(selectedEvent);
            base.OnOKClicked();
        }

        public override void OnCancelClicked()
        {
            base.OnCancelClicked();
        }

        public override void ShowPopup(GraphItem targetItem = null)
        {
            base.ShowPopup(targetItem);

            // Init scroll position.
            content.localPosition = Vector2.zero;

            // Set event list.
            if (eventCategories == null || eventCategories.Length == 0)
            {
                ReadEventList();
                SetEventCategories();
            }

            // Set item selected status.
            if (targetItem != null && targetItem.GetItemData() != null)
            {
                for (int ix = 0; ix < eventListItems.Count; ++ix)
                {
                    if (Util.CompareTwoStrings(eventListItems[ix].GetEventName(), targetItem.GetItemData().ToString()))
                    {
                        eventListItems[ix].SetItemSelected(true);
                        break;
                    }
                }
            }
        }

        public void SetSelectedData(string selectedEvent)
        {
            this.selectedEvent = selectedEvent;
        }

        private void SetEventCategories()
        {
            float lastHeight = 0f;
            List<EventNameItem> eventListTexts = new List<EventNameItem>();
            for (int ix = 0; ix < eventCategories.Length; ++ix)
            {
                eventListTexts.Clear();
                Text category = CreateCategory(ix, ref lastHeight);

                // Create Event Name Item(Prefab).
                for (int jx = 0; jx < eventCategories[ix].eventList.Length; ++jx)
                {
                    EventNameItem eventNameItem = CreateEventList(ix, jx, category);
                    eventListItems.Add(eventNameItem);
                    eventListTexts.Add(eventNameItem);
                }

                // Set Text.text with event name from json.
                // has to be set with this way.
                for (int jx = 0; jx < eventListTexts.Count; ++jx)
                {
                    eventListTexts[jx].SetEventName(eventCategories[ix].eventList[jx]);
                }
            }

            SetScrollAreaHeight(lastHeight);
        }

        // Disable selected status.
        public void SetAllItemNoSelected()
        {
            for (int ix = 0; ix < eventListItems.Count; ++ix)
            {
                eventListItems[ix].SetItemSelected(false);
            }
        }

        // set total scroll area.
        private void SetScrollAreaHeight(float lastHeight)
        {
            int numOfevent = eventCategories[eventCategories.Length - 1].eventList.Length;
            lastHeight += blockHeight + blockHeight * numOfevent;
            content.sizeDelta = new Vector2(0f, Mathf.Abs(lastHeight));
        }

        private EventNameItem CreateEventList(int ix, int jx, Text category)
        {
            EventNameItem eventList = Instantiate<EventNameItem>(eventNamePrefab);
            eventList.rectTransform.SetParent(category.rectTransform);
            SetAnchorAndPivot(eventList.rectTransform, eventNamePrefab.rectTransform);
            eventList.rectTransform.localPosition = new Vector3(0f, blockHeight + blockHeight * jx, 0f);
            eventNamePrefab.SetEventName(eventCategories[ix].eventList[jx]);

            return eventList;
        }

        private Text CreateCategory(int ix, ref float lastHeight)
        {
            Text category = Instantiate<Text>(categoryTextPrefab);
            category.rectTransform.SetParent(topListPane);
            SetAnchorAndPivot(category.rectTransform, categoryTextPrefab.rectTransform);
            category.rectTransform.localPosition = new Vector3(0f, GetBlockYPos(ix, lastHeight), 0f);
            category.text = eventCategories[ix].categoryName;

            lastHeight = category.rectTransform.localPosition.y;

            return category;
        }

        private void SetAnchorAndPivot(RectTransform rect, RectTransform prefabRect)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.sizeDelta = prefabRect.sizeDelta;
        }

        private float GetBlockYPos(int ix, float lastHeight)
        {
            if (ix == 0) return blockHeight;

            return lastHeight + blockHeight + blockHeight * eventCategories[ix - 1].eventList.Length;
        }

        private void ReadEventList()
        {
            eventCategories = Util.ReadFromJson<EventCategory[]>(eventListJson);
        }
    }
}