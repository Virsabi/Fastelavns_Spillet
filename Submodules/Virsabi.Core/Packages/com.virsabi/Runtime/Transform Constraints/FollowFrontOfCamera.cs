using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

namespace Virsabi
{
    /// <summary>
    /// Attatches the object to float in front of the camera smoothly. 
    /// </summary>
    [ExecuteInEditMode]
    public class FollowFrontOfCamera : MonoBehaviour
    {
        [SerializeField]
        private bool runInEditor = false;

        [SerializeField]
        private bool UseMainCamera = false;

        [ConditionalField(nameof(UseMainCamera), true), SerializeField]
        private Camera Camera = null;

        [SerializeField]
        [Tooltip("In meters")]
        private float distanceFromCamera = 1;

        [SerializeField]
        [Tooltip("1 for hard attachtment")]
        private float followSpeed = 0.1f;

        [SerializeField]
        private bool fasterFollowBasedOnDistance = true;

        [SerializeField] //make conditional field
        private float distanceFactor = 1;

        [SerializeField]
        private bool aimAtCamera = true;

        private float slerpSpeed;
        
        [SerializeField]
        private bool lockYToCameraHeight;

        private void OnValidate()
        {
            if(UseMainCamera)
                Camera = Camera.main;
        }

        private void LateUpdate()
        {
            AttacthToCamera();
        }

        private void Start()
        {
            if ((!Application.isPlaying && !runInEditor))
                return;

            if (Camera == null)
            {
                Debug.LogError("Camera not set or found!");
                return;
            }
            Vector3 attachPoint = Camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, distanceFromCamera));
            if (lockYToCameraHeight)
                attachPoint.y = Camera.transform.position.y;

            transform.position = attachPoint;
        }

        public void SetCameraToFollow(Camera _camera)
        {
            Camera = _camera;
        }

        private void AttacthToCamera()
        {
#if UNITY_EDITOR
            if ((!Application.isPlaying && !runInEditor) || Camera == null)
                return;
#endif
            Vector3 attachPoint = Camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, distanceFromCamera));

            if (fasterFollowBasedOnDistance)
                slerpSpeed = followSpeed * Mathf.Clamp(Vector3.Distance(attachPoint, transform.position), .5f, 50) * distanceFactor;
            else
                slerpSpeed = followSpeed;

            if (lockYToCameraHeight)
                attachPoint.y = Camera.transform.position.y;

            transform.position = Vector3.Slerp(transform.position, attachPoint, slerpSpeed);

            if (aimAtCamera)
                transform.LookAt(Camera.transform.position);
        }


        private void OnDrawGizmos()
        {
            if (Camera == null)
                return;

            Gizmos.color = Color.red;

            Vector3 attacthPoint = Camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, distanceFromCamera));

            Gizmos.DrawLine(Camera.transform.position, attacthPoint);
        }
    }

}