using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using System;
using Virsabi.Utility;
using Virsabi;

namespace Virsabi.UI
{
    using Virsabi.Utility;

    [RequireComponent(typeof(Image))]
    [ExecuteInEditMode]
    public class LoadingCircleController : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f)]
        private float progress;

        [SerializeField]
        private float innerRadiusStart = 0.3f;

        [SerializeField]
        private float innerRadiusEnd = 0.1f;

        [SerializeField, ReadOnly]
        private Material loadingMaterial;

        public ShaderFloat innerRadiusName;
        public ShaderFloat radialValueName;

        public float Progress
        {
            get => progress;
            set
            {
                progress = value;
                UpdateCircle();
            }
        }
#if UNITY_EDITOR
        private void Update()
        {
            UpdateCircle();
        }

#endif

        private void UpdateCircle()
        {
            float innerRadiusProgress = Utility.Map(1, 0, innerRadiusEnd, innerRadiusStart, progress);
            loadingMaterial.SetFloat(innerRadiusName.FloatName, innerRadiusProgress);

            loadingMaterial.SetFloat(radialValueName.FloatName, progress);

        }

        private void OnValidate()
        {
            loadingMaterial = GetComponent<Image>().material;
        }
    }

}