using UnityEngine;
using MyBox;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Virsabi.Extensions
{
    public static class TransformExtensions
    {
        /// <summary>
        /// Destroy all children
        /// </summary>
        public static Transform Clear(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            return transform;
        }

        /// <summary>
        /// Check if prefab instance
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static bool IsPrefab(this Transform This) => !This.gameObject.scene.IsValid();
    }
}
