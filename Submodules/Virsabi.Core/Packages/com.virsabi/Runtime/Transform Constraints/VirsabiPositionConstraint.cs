using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;


/// <summary>
/// Very simple behavior for synchronizing position of this object relative to another.
/// </summary>
/// 
namespace Virsabi
{

    [ExecuteInEditMode]
    public class VirsabiPositionConstraint : MonoBehaviour
    {

        public Transform targetTransform; // Current target transform to constrain to, can be left null for use of provided Vector3

        [ConditionalField(nameof(targetTransform)), ReadOnly]
        public Vector3 targetPosition; // The current position to constraint to, can be set manually if the target Transform is null

        [ConditionalField(nameof(targetTransform))]
        public bool
            X = true,
            Y = true,
            Z = true,
            startOffset,
            localOffset;

        [ConditionalField(nameof(localOffset))]
        public Vector3 offset;

        [ConditionalField(nameof(targetTransform))]
        public bool Lerp;

        [ConditionalField(nameof(Lerp))]
        public float LerpVal;

        [ConditionalField(nameof(targetTransform))]
        public UpdateMethod updateMethod;

        Vector3 cachedoffset;
        private Transform _tr;

        public Transform tr { get { if (!_tr) _tr = GetComponent<Transform>(); return _tr; } }

        void OnEnable()
        {
            cachedoffset = targetTransform.position - tr.position;
        }

        void Update()
        {
            if (updateMethod != UpdateMethod.update)
                return;
            Sync();
        }

        void FixedUpdate()
        {
            if (updateMethod != UpdateMethod.fixedUpdate)
                return;
            Sync();
        }

        public void Sync()
        {
            if (targetTransform != null)
            {
                targetPosition = startOffset ? targetTransform.position - cachedoffset : targetTransform.position;
            }

            Vector3 pos = tr.position;
            Vector3 localoffsetpos = Vector3.zero;
            if (localOffset)
            {
                localoffsetpos = targetTransform.TransformPoint(offset);
            }

            if (X)
            {
                pos.x = localOffset ? localoffsetpos.x : targetPosition.x;
            }
            if (Y)
            {
                pos.y = localOffset ? localoffsetpos.y : targetPosition.y;
            }
            if (Z)
            {
                pos.z = localOffset ? localoffsetpos.z : targetPosition.z;
            }

            tr.position = Lerp ? Vector3.Lerp(tr.position, pos, LerpVal) : pos;
            if (!localOffset)
                tr.position += offset;

        }

        void LateUpdate()
        {
            if (updateMethod != UpdateMethod.lateUpdate)
                return;
            Sync();

        }
    }
}