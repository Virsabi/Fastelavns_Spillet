using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using UnityEngine.UI;
using System;

namespace Virsabi
{
    [RequireComponent(typeof(Image))]
    public class ImageSplashScreen : VirsabiSplashScreen
    {
        [Foldout("Setup", true)]

        [SerializeField, MustBeAssigned]
        private Sprite Logo;

        [SerializeField]
        private float fadeTime = 1;

        [SerializeField]
        private float ShowTime = 2f;

        [SerializeField, HideInInspector]
        private Image image;

        private Color ImageColor;

        [SerializeField, HideInInspector]
        private bool showLogo;

        [SerializeField, HideInInspector]
        private float t;

        [SerializeField, HideInInspector]
        private float timeSinceStart;

        public override void OnValidate()
        {
            base.OnValidate();

            if (!gameObject.activeInHierarchy)
                return;

            if (type != SplashscreenType.logo)
                splashScreenBehavior.ReplaceSplashScreen(this, type);


            image = GetComponent<Image>();


            image.preserveAspect = true;
            image.type = Image.Type.Simple;
            image.raycastTarget = false;


        }

        private void Awake()
        {
            ImageColor = image.color;
            ImageColor.a = 0;
            image.color = ImageColor;
            image.sprite = Logo;
        }

        private void ShowLogo()
        {
            showLogo = true;
            timeSinceStart = 0;
        }


        private void HideLogo()
        {
            showLogo = false;
        }


        private void Update()
        {
            UpdateLogoAlpha();
            if (showLogo)
            {
                timeSinceStart += Time.deltaTime;
                if (timeSinceStart > ShowTime)
                    HideLogo();
            }
        }

        private void UpdateLogoAlpha()
        {
            if (showLogo && ImageColor.a < 1)
            {
                t += fadeTime * Time.deltaTime;
                if (t > 1)
                    t = 1;

                ImageColor.a = Mathf.Lerp(0, 1, t);
                image.color = ImageColor;
            }
            else if (!showLogo && ImageColor.a > 0)
            {
                t -= fadeTime * Time.deltaTime;
                if (t < 0)
                {
                    t = 0;
                    OnSplashScreenFinished.Invoke();
                }


                ImageColor.a = Mathf.Lerp(0, 1, t);
                image.color = ImageColor;
            }
        }

        public override void Show()
        {
            ShowLogo();
        }

        public override void Hide()
        {
            HideLogo();
        }
    }
}
