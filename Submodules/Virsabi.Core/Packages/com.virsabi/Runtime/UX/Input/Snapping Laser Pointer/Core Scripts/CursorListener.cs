using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Virsabi
{
    public class CursorListener : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Listener message");
        }
    }
}
