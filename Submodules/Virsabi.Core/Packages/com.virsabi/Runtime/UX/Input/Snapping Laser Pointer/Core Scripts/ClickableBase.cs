using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.Video;

namespace Virsabi
{

    public abstract class ClickableBase : MonoBehaviour
    {
        [FormerlySerializedAs("onClicked")] [SerializeField] protected UnityEvent onClickedUE;
        [SerializeField] protected UnityEvent OnHoverEnterUE;
        [SerializeField] protected UnityEvent OnHoverExitUE;
        [SerializeField] public delegate void OnHoverEnterDelegate();
        [SerializeField] public delegate void OnHoverExitDelegate();

        public OnHoverEnterDelegate onHoverEnter;
        public OnHoverExitDelegate onHoverExit;

        [ContextMenu("Debug Hover Enter")]
        public virtual void OnHoverEnter()
        {
            OnHoverEnterUE?.Invoke();
        }

        [ContextMenu("Debug Hover Exit")]
        public virtual void OnHoverExit()
        {
            Debug.Log("Exit");
            OnHoverExitUE?.Invoke();
            onHoverEnter?.Invoke();
        }

        public virtual void Pressed()
        {
            onClickedUE?.Invoke();
            onHoverExit?.Invoke();
        }
    }
}
