using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Virsabi.Extensions;
using Virsabi;

/// <summary>
///     Author: Mikkel S. K. Mogensen
///     <para>
///         Counter that lerps over a specified animation curve from a sequence of floats.
///     </para>
/// </summary>
/// 
namespace Virsabi.Tween
{

    public class TweenSequencer : MonoBehaviour
    {
        public enum Direction
        {
            Positive = 1,
            Negative = -1
        }

        #region Fields

        [FormerlySerializedAs("countOnAwake")]
        [Header("These values should be initialized")]
        [SerializeField] private bool tweenOnAwake;
        [SerializeField] private bool resetOnStart;

        [SerializeField] private float tweenDuration;
        [SerializeField] private Direction tweenDirection = Direction.Positive;
        [SerializeField] private AnimationCurve tweenCurve;
        [SerializeField, Help("Set the values between 0 and 1 in ascending order.")] private float[] tweenPoints;

        [Help("Please do not set these values unless you know what you are doing.", MessageType.Warning)]
        [Header("Value only for debugging")]
        [SerializeField] private float t;

        [SerializeField] private float timeStep;
        [SerializeField] private bool isRunning;

        [SerializeField] private int tweenIdx;
        [SerializeField] private int targetIdx;
        [SerializeField] private bool _isInitialized;

        private float _tweenMin;
        private float _tweenMax;

        #endregion

        #region Events

        //[Help("Use these events to create behaviours that respond to the counter.", MessageType.Warning)]
        [Header("Events")]

        [SerializeField] UnityEvent<int> onCounterReachedPositiveUE;
        [SerializeField] UnityEvent<int> onCounterReachedNegativeUE;
        [SerializeField] UnityEvent OnDirectionSetPositiveUE;
        [SerializeField] UnityEvent OnDirectionSetNegativeUE;
        [SerializeField] UnityEvent OnCounterStarted;
        [SerializeField] UnityEvent OnCounterStoppedUE;
        [SerializeField] private TweenUpdatedUnityEvent onCounterUpdateUE;
        [SerializeField] private TweenUpdatedUnityEvent onCounterReset;

        #endregion

        #region Properties

        public float T => t;

        public bool IsCounting
        {
            get => isRunning;
            private set => isRunning = value;
        }

        public UnityEvent<int> OnCounterReachedPositiveUe
        {
            get => onCounterReachedPositiveUE;
            set => onCounterReachedPositiveUE = value;
        }

        public UnityEvent<int> OnCounterReachedNegativeUe
        {
            get => onCounterReachedNegativeUE;
            set => onCounterReachedNegativeUE = value;
        }

        public UnityEvent OnDirectionSetPositiveUe
        {
            get => OnDirectionSetPositiveUE;
            set => OnDirectionSetPositiveUE = value;
        }

        public UnityEvent OnDirectionSetNegativeUe
        {
            get => OnDirectionSetNegativeUE;
            set => OnDirectionSetNegativeUE = value;
        }

        public TweenUpdatedUnityEvent OnCounterUpdateUe
        {
            get => onCounterUpdateUE;
            set => onCounterUpdateUE = value;
        }

        #endregion

        #region Monobehaviour
        private void Awake()
        {
            SetMinMax();
            ResetTweenValue();

            if (tweenDuration == 0)
            {
                Debug.LogError("The tween duration should be bigger than zero!");
                return;
            }

            if (tweenOnAwake)
                isRunning = true;
        }

        /// <summary>
        /// Set tween to the current index.
        /// </summary>
        private void ResetTweenValue()
        {
            t = tweenPoints[tweenIdx];
        }

        private void Start()
        {
            if (!resetOnStart)
                return;
            // This might have to be removed and replaced by a new event called OnCounterAwake
            onCounterReset?.Invoke(tweenCurve.Evaluate(t));
        }

        private void FixedUpdate()
        {
            if (!isRunning || !_isInitialized) return;

            UpdateLerpValue();
            onCounterUpdateUE?.Invoke(tweenCurve.Evaluate(t));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Set the minimum and maximum values from the tween points. Used to calculated the tween interval.
        /// </summary>
        private void SetMinMax()
        {
            // TODO: Assert sorted values 0 (lowest) -> 1 (highest). For now specify points in the correct order.
            _tweenMin = tweenPoints[0];
            _tweenMax = tweenPoints[tweenPoints.Length - 1];
        }

        private void InitializeTween()
        {
            tweenDirection = targetIdx > tweenIdx ? Direction.Positive : Direction.Negative;

            // Check if tween is valid before continuing.
            if (!AssertIsTweenValid())
            {
                isRunning = false;
                return;
            }

            float tweenInterval = tweenPoints[tweenIdx + (int)tweenDirection] - tweenPoints[tweenIdx];

            // Set the time increment value.
            // The tween duration is a fraction of the total duration determined by a fraction of the tweenMax times duration [tweenInterval / tweenMax * tweenDuration].
            // [TweenDuration / DeltaTime] gives us the total number of frames we have to step.
            // TODO: This calculation can be simplified as the timestep is the same no matter the current interval.
            timeStep = tweenInterval / (tweenInterval / _tweenMax * tweenDuration / Time.fixedDeltaTime) * (int)tweenDirection;

            // Mark initialized "true".
            _isInitialized = true;
        }

        private bool AssertIsTweenValid()
        {
            if (tweenDuration == 0)
                throw new Exception("Tween duration should not be zero.");

            switch (tweenDirection)
            {
                case Direction.Positive:
                    return tweenIdx + 1 < tweenPoints.Length;
                case Direction.Negative:
                    return tweenIdx - 1 >= 0;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        bool GetTweenCondition(float stepResult)
        {
            switch (tweenDirection)
            {
                case Direction.Positive:
                    return stepResult > tweenPoints[tweenIdx] && stepResult < tweenPoints[tweenIdx + 1];
                case Direction.Negative:
                    return stepResult < tweenPoints[tweenIdx] && stepResult > tweenPoints[tweenIdx - 1];
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Interpolates a value between the current and next value using the computed time step.
        /// </summary>
        private void UpdateLerpValue()
        {
            var stepResult = t + timeStep;

            // Allow the time step to be added if it is within the bounds.
            if (GetTweenCondition(stepResult))
            {
                t += timeStep;
            }
            // Otherwise check the direction of the time and set the time value to the boundary.
            else if (tweenDirection == Direction.Negative)
            {
                // Set t to the target value just in case.
                tweenIdx = tweenIdx - 1;
                t = tweenPoints[tweenIdx];

                // Invoke events
                onCounterReachedNegativeUE?.Invoke(tweenIdx);

                // Set initialized to false, so the tween can be initialized again.
                _isInitialized = false;

                // If the tween index is not equal to the target, do another step.
                if (tweenIdx != targetIdx)
                {
                    InitializeTween();
                }
                else
                {
                    isRunning = false;
                    OnCounterStoppedUE?.Invoke();
                }
            }
            else
            {
                // Set t to the target value just in case.
                tweenIdx = tweenIdx + 1;
                t = tweenPoints[tweenIdx];

                // Set initialized to false, so the tween can be initialized again.
                _isInitialized = false;

                // Invoke events
                onCounterReachedPositiveUE?.Invoke(tweenIdx);

                // If the tween index is not equal to the target, do another step.
                if (tweenIdx != targetIdx)
                {
                    InitializeTween();
                }
                else
                {
                    isRunning = false;
                    OnCounterStoppedUE?.Invoke();
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the direction of the tween.
        /// </summary>
        /// <param name="direction"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void SetDirection(Direction direction)
        {
            this.tweenDirection = direction;
            _isInitialized = false;

            switch (direction)
            {
                case Direction.Positive:
                    OnDirectionSetPositiveUE?.Invoke();
                    break;
                case Direction.Negative:
                    OnDirectionSetNegativeUE?.Invoke();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        /// <summary>
        /// Sets the direction of the tween to positive.
        /// </summary>
        public void SetPositiveDirection()
        {
            this.tweenDirection = Direction.Positive;
            _isInitialized = false;
            StartTween();
            OnDirectionSetPositiveUE?.Invoke();
        }

        /// <summary>
        /// Sets the direction of the tween to negative.
        /// </summary>
        public void SetNegativeDirection()
        {
            this.tweenDirection = Direction.Negative;
            _isInitialized = false;
            StartTween();
            OnDirectionSetNegativeUE?.Invoke();
        }

        [ContextMenu("Start Tween")]
        public void StartTween()
        {
            if (isRunning || targetIdx == tweenIdx)
                return;

            isRunning = true;

            InitializeTween();

            OnCounterStarted?.Invoke();
        }

        [ContextMenu("Stop Tween")]
        public void StopTween()
        {
            isRunning = false;
            _isInitialized = false;
            OnCounterStoppedUE?.Invoke();
        }

        [ContextMenu("Tween To Next")]
        public void TweenToNextPoint()
        {
            if (!isRunning)
            {
                if (tweenIdx + 1 > tweenPoints.Length - 1)
                {
                    return;
                }

                targetIdx = tweenIdx + 1;
                StartTween();
            }
            else
            {
                IncrementTarget();
            }
        }

        [ContextMenu("Tween To Previous")]
        public void TweenToPreviousPoint()
        {
            if (!isRunning)
            {
                if (tweenIdx - 1 < 0)
                {
                    return;
                }

                targetIdx = tweenIdx - 1;
                StartTween();
            }
            else
            {
                DecrementTarget();
            }
        }

        private void IncrementTarget()
        {
            if (targetIdx + 1 > tweenPoints.Length - 1)
            {
                return;
            }

            targetIdx = targetIdx + 1;
            InitializeTween();
        }

        private void DecrementTarget()
        {
            if (targetIdx - 1 < 0)
            {
                return;
            }

            targetIdx = targetIdx - 1;
            InitializeTween();
        }

        public void SetTargetIndex(int val)
        {
            if (targetIdx < 0 || targetIdx > tweenPoints.Length - 1)
                return;

            targetIdx = val;
        }

        public int GetTargetIndex()
        {
            return targetIdx;
        }

        public void ForceOverwriteCounterState(float t, Direction direction)
        {
            this.t = t;
            this.tweenDirection = direction;
        }

        #endregion

#if DEBUG && UNITY_EDITOR

        [ContextMenu("Debug Switch Direction")]
        private void DebugSwitchDirection()
        {
            switch (tweenDirection)
            {
                case Direction.Positive:
                    tweenDirection = Direction.Negative;
                    break;
                case Direction.Negative:
                    tweenDirection = Direction.Positive;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _isInitialized = false;
        }
#endif
    }

    [System.Serializable]
    public class TweenUpdatedUnityEvent : UnityEvent<float>
    {
    }
}
