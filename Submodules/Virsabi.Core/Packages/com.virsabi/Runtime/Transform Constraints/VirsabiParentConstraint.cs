using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System;

/// <summary>
/// Very simple behavior for synchronizing position, scale and rotation of this object relative to another like it was childed 
/// - sometimes better than changing in hierarchy during runtime and helps with flattening the scene structure.
/// </summary>
/// 
namespace Virsabi
{
    [ExecuteInEditMode]
    public class VirsabiParentConstraint : MonoBehaviour
    {
        public UpdateMethod updateMethod;

        public bool active, useMainCamera;

        public Transform Parent;

        [Foldout("Debug Values", true)]
        [SerializeField]
        private Vector3 childLocalPos;

        [SerializeField]
        private Quaternion _childRotOffset;



        void OnEnable()
        {
            //SetRest();   
        }

        private void Start()
        {
            if (!Application.isPlaying)
                return;

            if (useMainCamera && Camera.main != null)
                Parent = Camera.main.transform;
            else if (useMainCamera)
                Debug.LogError("Couldn't find main camera");
        }

        [ButtonMethod]
        public void SetRest()
        {
            childLocalPos = Parent.InverseTransformPoint(transform.position);
            _childRotOffset = Quaternion.FromToRotation(Parent.forward, transform.forward);
        }

        void Update()
        {
            if (useMainCamera && Parent == null)
                Parent = Camera.main.transform;

            if (updateMethod != UpdateMethod.update)
                return;
            if (Parent && active)
                Sync();
        }

        private void FixedUpdate()
        {
            if (updateMethod != UpdateMethod.fixedUpdate)
                return;
            if (Parent && active)
                Sync();
        }
        public void Sync()
        {
            transform.position = Parent.TransformPoint(childLocalPos);
            transform.rotation = Parent.rotation * _childRotOffset;

        }
        void LateUpdate()
        {
            if (updateMethod != UpdateMethod.lateUpdate)
                return;
            if (Parent && active)
                Sync();
        }
    }

}