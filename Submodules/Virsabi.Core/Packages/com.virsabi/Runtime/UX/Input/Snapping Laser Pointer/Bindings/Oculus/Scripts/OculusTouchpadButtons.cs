using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Virsabi.OVR
{
    [RequireComponent(typeof(OculusPlayerEvents))]
    public class OculusTouchpadButtons : MonoBehaviour
    {
        enum TouchpadSplitLayout
        {
            Horizontal, Vertical
        }

        [SerializeField] private OculusPlayerEvents oculusPlayerEvents;
        [SerializeField] private TouchpadSplitLayout _splitLayout;

        [SerializeField] private UnityEvent onLeftSidePressed;
        [SerializeField] private UnityEvent onRightSidePressed;
        [SerializeField] private UnityEvent onLowerSidePressed;
        [SerializeField] private UnityEvent onUpperSidePressed;

        void Awake()
        {
            switch (_splitLayout)
            {
                case TouchpadSplitLayout.Horizontal:
                    oculusPlayerEvents.OnTouchPadDown2D += TouchSplitHorizontal;
                    break;
                case TouchpadSplitLayout.Vertical:
                    oculusPlayerEvents.OnTouchPadDown2D += TouchSplitVertical;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void TouchSplitHorizontal(Vector2 touchPosition)
        {
            if (touchPosition.x < 0)
            {
                onLeftSidePressed.Invoke();
            }
            else
            {
                onRightSidePressed.Invoke();
            }
        }

        void TouchSplitVertical(Vector2 touchPosition)
        {
            if (touchPosition.y < 0)
            {
                onLowerSidePressed.Invoke();
            }
            else
            {
                onUpperSidePressed.Invoke();
            }
        }
    }

}