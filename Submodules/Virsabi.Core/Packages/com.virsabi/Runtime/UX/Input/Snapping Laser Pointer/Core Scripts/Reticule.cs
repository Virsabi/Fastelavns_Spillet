using System;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
///     Author: Mikkel S. K. Mogensen
///     <para>
///         Reticule used to show the location of the laser pointer.
///     </para>
/// </summary>

namespace Virsabi
{

    public class Reticule : MonoBehaviour
    {
        [FormerlySerializedAs("proximityLaserPointer")] [SerializeField] private SnappingLaserPointer snappingLaserPointer;
        [SerializeField] private SpriteRenderer circleRenderer;
        [SerializeField] private Sprite openSprite;
        [SerializeField] private Sprite closedSprite;
        [SerializeField] private Transform lookAtTransform;

        private void Awake()
        {
            snappingLaserPointer.OnPointerUpdate += UpdateSprite;
        }

        private void Update()
        {
            transform.LookAt(lookAtTransform);
        }

        private void OnDestroy()
        {
            snappingLaserPointer.OnPointerUpdate -= UpdateSprite;
        }

        private void UpdateSprite(Vector3 point, GameObject hitObject)
        {
            transform.position = point;

            // Check if hit object is null. If not null show closed sprite, otherwise show open sprite.
            if (hitObject)
                circleRenderer.sprite = closedSprite;
            else
                circleRenderer.sprite = openSprite;
        }
    }
}