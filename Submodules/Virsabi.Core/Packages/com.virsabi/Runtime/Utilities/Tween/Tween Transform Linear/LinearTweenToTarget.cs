using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using Virsabi;

/// <summary>
/// Author: Mikkel S. K. Mogensen
/// <para>
/// Tween to the target transform using a co-routine. Position and rotation tween independently.
/// </para>
/// </summary>
/// 
namespace Virsabi.Tween
{

    public class LinearTweenToTarget : MonoBehaviour
    {
        [HelpAttribute("Hint: Go in Play mode and click the symbol in the corner. Then select \"Debug Tween\" to start tweening. LinearTweenToTarget exposes various methods to control the tween programmatically.", MessageType.Info)]
        [SerializeField] private Transform targetTransform;
        [SerializeField] private bool unparentOnTween;
        [SerializeField] private bool parentAfterTween;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float rotSpeed;

        private float _deltaDist;
        private float _deltaRot;

        delegate void TweenCompletedDel();

        private TweenCompletedDel _onTweenCompleted;

        #region Properties

        /// <summary>
        /// Whether the game object should unparent when the tween starts.
        /// </summary>
        public bool UnparentOnTween
        {
            get => unparentOnTween;
            set => unparentOnTween = value;
        }

        /// <summary>
        /// Whether the game object should parent to the target transform after tweening.
        /// </summary>
        public bool ParentAfterTween
        {
            get => parentAfterTween;
            set => parentAfterTween = value;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Start tweening.
        /// </summary>
        /// <param name="targetTransform"></param>
        public void StartTween(Transform target)
        {
            targetTransform = target;
            ForceStopTween();

            Debug.Log(targetTransform);

            StartCoroutine(MoveToTarget(targetTransform));
        }

        /// <summary>
        /// Start tweening.
        /// </summary>
        public void StartTween()
        {
            ForceStopTween();
            StartCoroutine(MoveToTarget(targetTransform));
        }

        /// <summary>
        /// Force tweening to stop.
        /// </summary>
        public void ForceStopTween()
        {
            StopAllCoroutines();
        }

        /// <summary>
        /// Set the tween target.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public LinearTweenToTarget SetTargetTransform(Transform val)
        {
            targetTransform = val;
            return this;
        }

        #endregion

        IEnumerator MoveToTarget(Transform targetTransform)
        {
            if (unparentOnTween)
                transform.parent = null;

            var fromTransform = transform;
            var toTransform = targetTransform;

            var fromPosition = fromTransform.position;
            var fromRotation = fromTransform.rotation;

            var toPosition = toTransform.position;
            var toRotation = toTransform.rotation;

            _deltaDist = Vector3.Distance(fromPosition, toPosition);
            _deltaRot = Quaternion.Angle(fromRotation, toRotation);

            while (_deltaDist > 0.05f || _deltaRot > 5f)
            {

                fromPosition += Mathf.Min(moveSpeed * Time.deltaTime, _deltaDist) * Vector3.Normalize(toPosition - fromPosition);
                fromRotation = Quaternion.RotateTowards(fromRotation, toRotation, rotSpeed * Time.deltaTime);

                fromTransform.position = fromPosition;
                fromTransform.rotation = fromRotation;
                toPosition = toTransform.position;
                toRotation = toTransform.rotation;

                _deltaDist = Vector3.Distance(fromPosition, toPosition);
                _deltaRot = Quaternion.Angle(fromRotation, toRotation);

                yield return new WaitForFixedUpdate();
            }

            yield return new WaitForFixedUpdate();

            if (parentAfterTween)
            {
                transform.parent = targetTransform;
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            }
            else
            {
                transform.position = toPosition;
                transform.rotation = toRotation;
            }

            _onTweenCompleted?.Invoke();
        }

        public void AddOnCompletedListener(Action action)
        {
            _onTweenCompleted += action.Invoke;
        }

        public void RemoveOnCompletedListener(Action action)
        {
            _onTweenCompleted -= action.Invoke;
        }

#if DEBUG

        [ContextMenu("Debug Tween")]
        void DebugTween()
        {
            StartTween(targetTransform);
        }

#endif

    }

}