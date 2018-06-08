using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.EAIEditor
{
    public abstract class MenuPopup : MonoBehaviour
    {
        public abstract void ShowPopup(GraphItem targetItem = null);
        public abstract void HidePopup();
    }
}