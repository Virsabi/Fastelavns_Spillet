using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MyBox;

namespace Virsabi.UI
{
    [ExecuteInEditMode]
    public class ProgressUIFeedback : MonoBehaviour
    {
        [SerializeField, ReadOnly]
        private LoadingCircleController loadingCircleController;

        [SerializeField]
        private TextMeshProUGUI percentageText;

        [SerializeField, Range(0, 1)]
        private float progress;

        [SerializeField, Help("Add 10 % to the final value - Additive Loading stops at 90% per Unity Default")]
        private bool add10ToValue;

        public float Progress
        {
            get => progress;
            set
            {
                progress = value;
                UpdateProgressUI(value);
            }
        }

        private void UpdateProgressUI(float value)
        {
            float shownValue = value;

            if (add10ToValue) //adds 10 %
                shownValue +=  .1f;
            if(loadingCircleController)
                loadingCircleController.Progress = shownValue;

            percentageText?.SetText(Mathf.Clamp(Mathf.Round(shownValue * 100), 0, 100) + "%");
        }

#if UNITY_EDITOR
        private void Update()
        {
            UpdateProgressUI(progress);
        }
#endif

        private void OnValidate()
        {
            loadingCircleController = GetComponentInChildren<LoadingCircleController>(true);
        }
    }
}
