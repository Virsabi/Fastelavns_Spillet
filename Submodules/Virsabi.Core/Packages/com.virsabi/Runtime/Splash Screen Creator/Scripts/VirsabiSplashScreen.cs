using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MyBox;


namespace Virsabi
{

    public abstract class VirsabiSplashScreen : MonoBehaviour
    {
        public abstract void Show();

        public abstract void Hide();

        protected SplashScreenBehavior splashScreenBehavior;

        [HideInInspector]
        public UnityEvent OnSplashScreenFinished;

        public event EventHandler SplashScreenFinished;

        public SplashscreenType type;

        /// <summary>
        /// If Set True this will be deleted
        /// </summary>
        public bool deleteMe = false;

        public enum SplashscreenType
        {
            logo,
            video,
            custom
        }

        public virtual void OnValidate()
        {
            //Do not run any validation in prefabs
            if (!gameObject.activeInHierarchy)
                return;

            splashScreenBehavior = transform.GetComponentInParent<SplashScreenBehavior>();

            if (deleteMe)
                splashScreenBehavior.RemoveSplashScreen(this);


        }
    }

}