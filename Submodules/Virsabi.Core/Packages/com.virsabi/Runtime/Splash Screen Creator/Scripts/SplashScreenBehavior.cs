using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MyBox;
using UnityEngine.SceneManagement;
using System;

namespace Virsabi
{
    public class SplashScreenBehavior : MonoBehaviour
    {
        [Foldout("SetUp", true)]
        [SerializeField]
        private RectTransform Screens;

        [SerializeField]
        private GameObject VideoSplashScreen, LogoSplashScreen, CustomSplashScreen;

        [Foldout("Splash Screens")]
        [DisplayInspector]
        public List<VirsabiSplashScreen> splashScreens = new List<VirsabiSplashScreen>();

        private int splashScreenNumber;

        private void OnValidate()
        {
            splashScreens = new List<VirsabiSplashScreen>(Screens.gameObject.GetComponentsInChildren<VirsabiSplashScreen>());
        }

        private void OnEnable()
        {


            foreach (var item in splashScreens)
            {
                item.OnSplashScreenFinished.AddListener(OnSplashDone);
            }
        }

        private void OnDisable()
        {
            foreach (var item in splashScreens)
            {
                item.OnSplashScreenFinished.RemoveListener(OnSplashDone);
            }
        }


        private void OnDestroy()
        {
            foreach (var item in splashScreens)
            {
                item.OnSplashScreenFinished.RemoveListener(OnSplashDone);
            }
        }

        public void OnSplashDone()
        {
            //if last splashscreen was shown
            if (splashScreenNumber == splashScreens.Count)
            {
                Debug.Log("Loading scene");
                SceneManager.LoadScene(1);
            }
            else
            {
                ShowNextSplashScreen();
                Debug.Log("Done");
            }

        }

        private void Start()
        {
            splashScreenNumber = 0;
            ShowNextSplashScreen();
        }

        private void ShowNextSplashScreen()
        {
            splashScreens[splashScreenNumber].Show();
            splashScreenNumber++;
        }

        [ButtonMethod]
        public void AddSplashScreen()
        {
            GameObject temp = Instantiate(VideoSplashScreen, Screens);
            splashScreens.Add(temp.GetComponent<VirsabiSplashScreen>());

            OnValidate();
        }

        [ButtonMethod]
        public void RemoveLastSplashScreen()
        {
            StartCoroutine(DestroyDelayed(splashScreens[splashScreens.Count - 1].gameObject));

            splashScreens.RemoveAt(splashScreens.Count - 1);
            OnValidate();
        }

        public void RemoveSplashScreen(VirsabiSplashScreen screen)
        {
            splashScreens.Remove(screen);
            StartCoroutine(DestroyDelayed(screen.gameObject));

            OnValidate();
        }

        public GameObject GenerateSplashScreenObject(VirsabiSplashScreen.SplashscreenType type)
        {
            GameObject temp = null;

            switch (type)
            {
                case VirsabiSplashScreen.SplashscreenType.logo:
                    temp = Instantiate(LogoSplashScreen, Screens);
                    break;
                case VirsabiSplashScreen.SplashscreenType.video:
                    temp = Instantiate(VideoSplashScreen, Screens);
                    break;
                case VirsabiSplashScreen.SplashscreenType.custom:
                    temp = Instantiate(CustomSplashScreen, Screens);
                    break;
                default:
                    break;
            }

            return temp;
        }

        public void ReplaceSplashScreen(VirsabiSplashScreen oldScreen, VirsabiSplashScreen.SplashscreenType newScreenType)
        {
            int index = splashScreens.IndexOf(oldScreen);
            StartCoroutine(DestroyDelayed(oldScreen.gameObject));
            GameObject newSplashGO = GenerateSplashScreenObject(newScreenType);
            splashScreens[index] = newSplashGO.GetComponent<VirsabiSplashScreen>();

            //OnValidate();
        }

        public static IEnumerator DestroyDelayed(GameObject go)
        {
            yield return new WaitForEndOfFrame();
            DestroyImmediate(go);
        }

        IEnumerator DestroyDelayed(MonoBehaviour mb)
        {
            yield return new WaitForEndOfFrame();
            DestroyImmediate(mb);
        }

    }

}