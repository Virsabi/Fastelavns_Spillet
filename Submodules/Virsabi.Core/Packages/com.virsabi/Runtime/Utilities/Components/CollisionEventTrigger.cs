using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// Generic system for triggering unity events with collision triggers.
/// </summary>
/// 
namespace VirsabiRepoDevelopment.Packages.com.virsabi.Runtime._IN_REVIEW.Unsorted
{
    [RequireComponent(typeof(Collider))]
    public class CollisionEventTrigger : MonoBehaviour
    {
        /// <summary>
        /// Should we check for tags?
        /// </summary>
        public bool checkTag;
        [ConditionalField(nameof(checkTag)), Tag]
        public string CollideWithTag = "Untagged";

        /// <summary>
        /// Should we check for layers?
        /// </summary>
        public bool checkLayer;
        [ConditionalField(nameof(checkLayer)), Layer]
        public int CollideWithLayer;

        [Header("Colliders To Trigger with - empty for triggering with layer and/or tag")]
        public List<Collider> OtherCollider = new List<Collider>();

        [Tooltip("0 for infinite - timer for avoid repeting enter triggers")]
        [PositiveValueOnly]
        public float enterRetriggerDelay = 0;

        /// <summary>
        /// resets the enterRetriggerDelay on exit
        /// </summary>
        public bool resetEnterOnExit = false;

        [Separator("Events")]
        public UnityEvent OnEnter;
        public UnityEvent OnStay;
        public UnityEvent OnExit;

        private bool enterTriggered = false;

        [SerializeField]
        [HideInInspector]
        private Collider Collider;

        private void OnValidate()
        {
            Collider = GetComponent<Collider>();
            Collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((other.tag == CollideWithTag && checkTag) || (other.gameObject.layer == CollideWithLayer && checkLayer) || OtherCollider.Contains(other))
            {
                if (!enterTriggered)
                {
                    enterTriggered = true;
                    OnEnter.Invoke();
                    if (enterRetriggerDelay != 0)
                        Invoke("ResetEnterTrigger", enterRetriggerDelay);
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if ((other.tag == CollideWithTag && checkTag) || (other.gameObject.layer == CollideWithLayer && checkLayer) || OtherCollider.Contains(other))
                OnStay.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            if ((other.tag == CollideWithTag && checkTag) || (other.gameObject.layer == CollideWithLayer && checkLayer) || OtherCollider.Contains(other))
            {
                OnExit.Invoke();
                if (resetEnterOnExit)
                    enterTriggered = false;
            }
        }

        private void ResetEnterTrigger()
        {
            enterTriggered = false;
        }


    }
}
