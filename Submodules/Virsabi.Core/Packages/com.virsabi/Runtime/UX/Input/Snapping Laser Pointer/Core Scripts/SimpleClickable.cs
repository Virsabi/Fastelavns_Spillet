using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Virsabi
{
    public class SimpleClickable : ClickableBase
    {
        [ContextMenu("Debug Pressed")]
        public override void Pressed()
        {
            Debug.Log("Pressed");
            base.Pressed();
        }
    }
}

