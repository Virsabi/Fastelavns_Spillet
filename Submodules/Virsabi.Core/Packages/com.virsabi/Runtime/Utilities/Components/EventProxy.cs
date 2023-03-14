using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Simply for triggering events from forinstance an animation clip
/// </summary>

namespace Virsabi.Misc
{
    public class EventProxy : MonoBehaviour
    {
        public UnityEvent OnEventTrigger;

        public void TriggerEvent()
        {
            OnEventTrigger.Invoke();
        }
    }

}
