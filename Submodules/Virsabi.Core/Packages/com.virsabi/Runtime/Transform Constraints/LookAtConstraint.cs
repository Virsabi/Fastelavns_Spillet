using UnityEngine;
using MyBox;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Virsabi
{
    [ExecuteInEditMode]
    public class LookAtConstraint : MonoBehaviour
    {
        [SerializeField]
        public Transform lookAtTarget;

        [ConditionalField(nameof(lookAtTarget))]
        public UpdateMethod updateMethod;

        [ConditionalField(nameof(lookAtTarget))]
        public Vector3 offset;

        [ConditionalField(nameof(lookAtTarget))]
        public bool active, X = true, Y = true, Z = true, Lerp;

        //TODO: Implement lerp
        [ConditionalField(nameof(Lerp))]
        public float Damp;

        [Foldout("Debug", true)]

        [SerializeField]
        private bool activeInEditor;

        [SerializeField, ReadOnly]
        private Vector3 initRotation;

        public Quaternion resultQuaternion { get; private set; }

        [ButtonMethod]
        public void SetRest()
        {
            initRotation = transform.rotation.eulerAngles;
        }

        void Update()
        {
            if (updateMethod != UpdateMethod.update || !active)
                return;
            if (!Application.isPlaying && !activeInEditor)
                return;
            Sync();
        }

        void FixedUpdate()
        {
            if (updateMethod != UpdateMethod.fixedUpdate || !active)
                return;
            if (!Application.isPlaying && !activeInEditor)
                return;
            Sync();
        }

        void LateUpdate()
        {
            if (updateMethod != UpdateMethod.lateUpdate || !active)
                return;
            if (!Application.isPlaying && !activeInEditor)
                return;
            Sync();
        }

        private void Sync()
        {
            if (!lookAtTarget)
                return;

            Quaternion lookAtRotation = Quaternion.LookRotation(lookAtTarget.position - transform.position) * Quaternion.Euler(offset);

            if (Y && X && Z)
            {
                resultQuaternion = lookAtRotation;
                transform.rotation = resultQuaternion;
                return;
            }

            //TODO: Not working correctly yet...
            Vector3 rot = initRotation;

            if (X)
            {
                rot.x = lookAtRotation.x;
            }
            if (Y)
            {
                rot.y = lookAtRotation.y;
            }
            if (Z)
            {
                rot.z = lookAtRotation.z;
            }

            resultQuaternion = Quaternion.Euler(rot);
            transform.rotation = resultQuaternion;
        }
    }

}