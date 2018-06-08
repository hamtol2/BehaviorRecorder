using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace REEL.EAIEditor
{
    public class JsonParser : MonoBehaviour
    {
        public EventPopup.EventCategory[] category;
        string jsonPath = "Assets/Data/EventList.json";

        void Awake()
        {
            string jsonString = File.ReadAllText(jsonPath);
            Debug.Log(jsonString);
            category = SimpleJson.SimpleJson.DeserializeObject<EventPopup.EventCategory[]>(jsonString);
        }
    }
}