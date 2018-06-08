using UnityEngine;
using UnityEngine.UI;

namespace REEL.EAIEditor
{
    public class EventNameItem : MonoBehaviour
    {
        [SerializeField]
        private Image eventNameBG;

        [SerializeField]
        private Text eventNameText;

        private RectTransform _rectTransform;

        private EventPopup targetEventPopup;

        private Color selectedColor = Color.black;

        private void OnDisable()
        {
            SetItemSelected(false);
        }

        public void OnOKClicked()
        {
            if (targetEventPopup == null) targetEventPopup = GetComponentInParent<EventPopup>();
            targetEventPopup.SetAllItemNoSelected();
            targetEventPopup.SetSelectedData(eventNameText.text);
            //targetEventPopup.SetTargetItemData(eventNameText.text);
        }

        public void SetEventPopup(EventPopup eventPopup)
        {
            targetEventPopup = eventPopup;
        }

        public void SetItemSelected(bool isSelected)
        {
            if (selectedColor == Color.black)
                selectedColor = GetComponent<Button>().colors.highlightedColor;

            eventNameBG.color = isSelected ? selectedColor : Color.white;
        }

        public void SetEventName(string eventName)
        {
            //this.eventName = eventName;
            eventNameText.text = eventName;
        }

        public string GetEventName()
        {
            return eventNameText.text;
        }

        public RectTransform rectTransform
        {
            get
            {
                if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();

                return _rectTransform;
            }
        }
    }
}