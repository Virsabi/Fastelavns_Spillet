using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
///     Author: Mikkel S. K. Mogensen
///     <para>
///         Responsible for sending input events from the Oculus controller to objects that are listening.
///     </para>
/// </summary>

namespace Virsabi.OVR
{
    public class OculusPlayerEvents : MonoBehaviour
    {
        #region Events

        public UnityAction OnTouchPadUp = null;
        public UnityAction OnTouchPadDown = null;
        public UnityAction OnPrimaryIndexTriggerDown = null;
        public UnityAction OnPrimaryIndexTriggerUp = null;

        public UnityAction<Vector2> OnTouchPadDown2D = null;
        public UnityAction<OVRInput.Controller, GameObject> OnControllerSource = null;

        public bool UseHeadsetAsController;

        #endregion

        #region Anchors

        [SerializeField] private GameObject leftAnchor;
        [SerializeField] private GameObject rightAnchor;
        [SerializeField] private GameObject headAnchor;

        #endregion

        #region Input

        private Dictionary<OVRInput.Controller, GameObject> _controllerSets;
        private OVRInput.Controller _inputSource = OVRInput.Controller.None;
        private OVRInput.Controller _controller = OVRInput.Controller.None;
        private bool _inputActive = true;

        #endregion

        #region Monobehaviour

        private void Awake()
        {
            OVRManager.HMDMounted += PlayerFound;
            OVRManager.HMDUnmounted += PlayerLost;

            _controllerSets = CreateControllerSets();
        }

        private void OnDestroy()
        {
            OVRManager.HMDMounted -= PlayerFound;
            OVRManager.HMDUnmounted -= PlayerLost;
        }

        private void Update()
        {
            if (!_inputActive)
                return;

            // Check if controller exists
            CheckForController();

            // Check for input source
            CheckInputSource();

            // Check for actual input
            Input();
        }

        #endregion

        private void CheckForController()
        {
            var controllerCheck = _controller;

            // Right remote 
            if (OVRInput.IsControllerConnected(OVRInput.Controller.RTouch))
                controllerCheck = OVRInput.Controller.RTouch;

            // Left remote 
            if (OVRInput.IsControllerConnected(OVRInput.Controller.LTouch))
                controllerCheck = OVRInput.Controller.LTouch;

            // If no controllers, headset
            if (!OVRInput.IsControllerConnected(OVRInput.Controller.LTouch) &&
                !OVRInput.IsControllerConnected(OVRInput.Controller.RTouch))
                controllerCheck = OVRInput.Controller.Touch;

            // Update
            _controller = UpdateSource(controllerCheck, _controller);
        }

        private void CheckInputSource()
        {
            // Update
            _inputSource = UpdateSource(OVRInput.GetActiveController(), _inputSource);
        }

        private void Input()
        {
            // Primary Touchpad Input
            if (OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad))
            {
                OnTouchPadDown?.Invoke();
                OnTouchPadDown2D?.Invoke(OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad));
            }

            if (OVRInput.GetUp(OVRInput.Button.PrimaryTouchpad)) OnTouchPadUp?.Invoke();

            // Primary Index Trigger Input
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)) OnPrimaryIndexTriggerDown?.Invoke();
            if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger)) OnPrimaryIndexTriggerUp?.Invoke();

        }

        private OVRInput.Controller UpdateSource(OVRInput.Controller check, OVRInput.Controller previous)
        {
            // Check if values are the same, return
            if (check == previous)
                return previous;

            // Get controller object
            GameObject controllerObject;
            _controllerSets.TryGetValue(check, out controllerObject);
            Debug.Log("TryGet " + controllerObject);

            // If no object set to the head
            if (controllerObject == null) controllerObject = headAnchor;

            // Send out event

            if (!UseHeadsetAsController)
            {
                if (OnControllerSource != null)
                    OnControllerSource(check, controllerObject);
            }
            else
            {
                OnControllerSource(check, headAnchor);
            }

            // Return new value
            return check;
        }

        private void PlayerFound()
        {
            _inputActive = true;
        }

        private void PlayerLost()
        {
            _inputActive = false;
        }

        private Dictionary<OVRInput.Controller, GameObject> CreateControllerSets()
        {
            var newSets = new Dictionary<OVRInput.Controller, GameObject>
        {
            //{OVRInput.Controller.LTrackedRemote, leftAnchor},
            //{OVRInput.Controller.RTrackedRemote, rightAnchor},
            //{OVRInput.Controller.Touchpad, headAnchor},
            {OVRInput.Controller.Touch, rightAnchor},
            {OVRInput.Controller.RTouch, rightAnchor},
            {OVRInput.Controller.LTouch, leftAnchor}
        };
            return newSets;
        }
    }
}
