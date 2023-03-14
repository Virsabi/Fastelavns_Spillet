using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;


/// <summary>
/// This is the most basic splashscreen for the virsabi splashscreen system. Only implements the necessary. Usefull for making custom splashscreens (like multilogo).
/// </summary>
/// 

namespace Virsabi
{
    [RequireComponent(typeof(CanvasGroup))]
    public class SimpleSplashScreen : VirsabiSplashScreen
    {
        [SerializeField, ReadOnly, AutoProperty]
        private CanvasGroup canvasGroupForFading;

        [SerializeField]
        private GameObject Prefab;

        #region Public
        public float fadeTime = 1;

        public float ShowTime = 2;
        #endregion

        #region Private
        private bool showCanvas;

        private float timeSinceStart;

        private float t;

        #endregion

        private void HideLogo()
        {
            showCanvas = false;
        }

        public override void Hide()
        {
            showCanvas = false;
        }

        private void Awake()
        {
            canvasGroupForFading.alpha = 0;
        }

        public override void OnValidate()
        {
            base.OnValidate();

            if (!gameObject.activeInHierarchy)
                return;

            if (type != SplashscreenType.custom)
                splashScreenBehavior.ReplaceSplashScreen(this, type);

            if (Prefab != null)
            {
                //Clear children
                foreach (var item in transform.GetComponentsInChildren<RectTransform>())
                {
                    if (item != this.GetComponent<RectTransform>())
                        StartCoroutine(SplashScreenBehavior.DestroyDelayed(item.gameObject));
                }

                //instantiate new prefab instead
                Instantiate(Prefab, transform);
            }


            canvasGroupForFading = GetComponent<CanvasGroup>();

            canvasGroupForFading.interactable = false;
            canvasGroupForFading.blocksRaycasts = false;
        }


        public override void Show()
        {
            showCanvas = true;
            timeSinceStart = 0;
        }


        private void Update()
        {
            UpdateLogoAlpha();
            if (showCanvas)
            {
                timeSinceStart += Time.deltaTime;
                if (timeSinceStart > ShowTime)
                    HideLogo();
            }
        }

        private void UpdateLogoAlpha()
        {
            if (showCanvas && canvasGroupForFading.alpha < 1)
            {
                t += fadeTime * Time.deltaTime;
                if (t > 1)
                    t = 1;

                canvasGroupForFading.alpha = Mathf.Lerp(0, 1, t);
            }
            else if (!showCanvas && canvasGroupForFading.alpha > 0)
            {
                t -= fadeTime * Time.deltaTime;
                if (t < 0)
                {
                    t = 0;
                    OnSplashScreenFinished.Invoke();
                }


                canvasGroupForFading.alpha = Mathf.Lerp(0, 1, t);
            }
        }

    }

}
