using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.Test
{
    public class CheckActiveInHierachy : MonoBehaviour
    {
        [SerializeField] private Child checkObject;

        void Update()
        {
            if (checkObject.gameObject.activeInHierarchy)
                checkObject.UpdateFunction();
        }
    }
}