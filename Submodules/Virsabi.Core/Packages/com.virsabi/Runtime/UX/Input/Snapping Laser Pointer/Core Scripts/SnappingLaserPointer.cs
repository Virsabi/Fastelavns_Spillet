using System.Collections;
using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

/// <summary>
///     Author: Mikkel S. K. Mogensen
///     <para>
///         Laser pointer that uses a priority queue to snap to the nearest object. Resolves overlapping colliders of
///         multiple clickables.
///     </para>
/// </summary>

namespace Virsabi
{
    public class SnappingLaserPointer : MonoBehaviour
    {
        #region Fields

        [SerializeField] private float maxDistance = 10.0f;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private LayerMask hitMask = 0;
#pragma warning disable 0414
        [SerializeField] private Collider reticuleCollider;
        [SerializeField] private int maxRayHits = 5;
#pragma warning restore 0414
        [SerializeField] private bool isLaserVisible;

        [SerializeField] private Transform _currentOrigin;

        [Header("Value only for debugging")]
        [SerializeField]
        private GameObject _currentObject;

        private ClickableBase _currentClickable;

        private readonly SimplePriorityQueue<ClickableBase> _clickablePriorityQueue =
            new SimplePriorityQueue<ClickableBase>();
#pragma warning disable 0414
        private Vector3 _cursorPosition;
#pragma warning restore 0414

        #endregion

        #region Properties

        [field: SerializeField] public UnityAction<Vector3, GameObject> OnPointerUpdate { get; set; } = null;

        #endregion

        #region Monobehaviour

        private void Start()
        {
            SetLineColor();
        }

        private void Update()
        {
            if (_currentOrigin == null)
                return;

            var lastObject = _currentObject;
            var lastClickable = _currentClickable;

            var hitPoint = UpdateLine();

            UpdatePriorityQueue();
            _currentObject = GetObjectWithHighPriority();

            if (_currentObject != lastObject)
            {
                if (_currentObject != null)
                {
                    if (AssertClickable(_currentObject, out _currentClickable))
                    {

                        ProcessButtonHoverEnter(_currentClickable);

                        if (lastClickable != null)
                        {
                            ProcessButtonHoverExit(lastClickable);
                        }
                    }
                }
                else
                {
                    if (lastClickable != null)
                    {
                        ProcessButtonHoverExit(_currentClickable);
                        _currentClickable = null;
                    }
                }
            }

            if (OnPointerUpdate != null)
                OnPointerUpdate(hitPoint, _currentObject);
        }

        #endregion

        private Vector3 UpdateLine()
        {
            // Create ray
            var hit = CreateRaycast(hitMask);

            // Default end (actual) position
            var endPosition = _currentOrigin.position + _currentOrigin.forward * maxDistance;

            // Default end (line) position
            var lineEndPosition = _currentOrigin.position + _currentOrigin.forward * maxDistance;

            if (hit.collider) endPosition = hit.point;

            // Check hit
            if (_currentObject != null)
                lineEndPosition = _currentObject.transform.position;
            else if (hit.collider) lineEndPosition = hit.point;

            // Set position
            lineRenderer.SetPosition(0, _currentOrigin.position);
            lineRenderer.SetPosition(1, lineEndPosition);

            if (hit.transform == null)
                return transform.position + transform.forward * maxDistance;

            return hit.transform.position;
        }

        protected void UpdateOrigin(GameObject controllerObject)
        {
            // Set origin of pointer
            _currentOrigin = controllerObject.transform;

            // Is laser visible?
            lineRenderer.enabled = isLaserVisible;
        }

        private GameObject GetObjectWithHighPriority()
        {
            return _clickablePriorityQueue.Count == 0 ? null : _clickablePriorityQueue.First.gameObject;
        }

        protected void SetLaserVisibility(bool value)
        {
            this.isLaserVisible = value;
        }

        private void SetLineColor()
        {
            if (!lineRenderer)
                return;

            var endColor = Color.white;
            endColor.a = 0.0f;

            lineRenderer.endColor = endColor;
        }

        private RaycastHit CreateRaycast(int layer)
        {
            var hit = new RaycastHit();
            var ray = new Ray(_currentOrigin.position, _currentOrigin.forward);
            Physics.Raycast(ray, out hit, maxDistance, layer);

            return hit;
        }

        protected void ProcessButtonHoverEnter(ClickableBase clickable)
        {
            clickable.OnHoverEnter();
        }

        protected void ProcessButtonHoverExit(ClickableBase clickable)
        {
            clickable.OnHoverExit();
        }

        protected void ProcessButtonPressed()
        {
            if (!_currentObject)
                return;

            var clickable = _currentObject.GetComponent<ClickableBase>();
            clickable.Pressed();
        }

        private bool AssertClickable(GameObject go, out ClickableBase clickableBase)
        {
            clickableBase = go.GetComponent<ClickableBase>();
            return clickableBase != null;
        }

        private void OnTriggerEnter(Collider other)
        {
            var clickable = other.GetComponent<ClickableBase>();

            if (clickable != null)
                _clickablePriorityQueue.Enqueue(clickable,
                    Vector3.Distance(transform.position, clickable.transform.position));
        }

        private void OnTriggerExit(Collider other)
        {
            var clickable = other.GetComponent<ClickableBase>();

            if (clickable != null && _clickablePriorityQueue.Contains(clickable))
            {
                _clickablePriorityQueue.Remove(clickable);
            }
        }

        private void UpdatePriorityQueue()
        {
            if (_clickablePriorityQueue.Count == 0)
                return;

            foreach (var clickable in _clickablePriorityQueue)
                //Debug.Log(clickable.gameObject + " with dist: " + Vector3.Distance(transform.position, clickable.transform.position));

                _clickablePriorityQueue.UpdatePriority(clickable,
                    Vector3.Distance(transform.position, clickable.transform.position));
        }

#if DEBUG

        public void DebugPressButton()
        {

        }

#endif
    }
}