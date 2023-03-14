using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Virsabi;

namespace Virsabi.OVR
{
    public class OculusSnappingLaserPointer : SnappingLaserPointer
    {
        [Header("Oculus specific parameters")]
        [SerializeField] private OculusPlayerEvents oculusPlayerEvents;
        [SerializeField] private OVRInput.Controller supportedController;
        private void OnEnable()
        {
            oculusPlayerEvents.OnControllerSource += UpdateLaserPointer;
            oculusPlayerEvents.OnPrimaryIndexTriggerDown += ProcessButtonPressed;
        }

        private void UpdateLaserPointer(OVRInput.Controller controllerSource, GameObject controllerAnchor)
        {


            if (supportedController == OVRInput.Controller.Touch)
            {
                SetLaserVisibility(false);
            }
            else
            {
                SetLaserVisibility(supportedController.HasFlag(controllerSource));
            }
            UpdateOrigin(controllerAnchor);
        }

        private void OnDisable()
        {
            oculusPlayerEvents.OnControllerSource -= UpdateLaserPointer;
            oculusPlayerEvents.OnTouchPadDown -= ProcessButtonPressed;
        }

        private void OnDestroy()
        {
            oculusPlayerEvents.OnControllerSource -= UpdateLaserPointer;
            oculusPlayerEvents.OnTouchPadDown -= ProcessButtonPressed;

        }

    }

}