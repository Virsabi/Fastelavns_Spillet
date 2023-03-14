using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using Virsabi.UI;
using Virsabi;
using UnityEngine.SceneManagement;

namespace Virsabi
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance;


        #region Inspector Properties
        [Foldout("Setup", true)]
        [SerializeField]
        private GameObject LoadingEnvironment;

        [SerializeField]
        private CanvasGroup LoadingCanvasGroup;

        [SerializeField]
        private ProgressUIFeedback ProgressUIFeedback;

        [SerializeField, Help("Show loading screen if loading takes longer than this.")]
        private float timeToLoadingScreen = 1;

        [SerializeField, Help("The minimum time the loading screen should be shown for")]
        private float minLoadScreenLiveTime = 2;

        [Foldout("Debug", true)]

        [SerializeField]
        private bool forceLoadingScreen = false;

        [SerializeField, ReadOnly]
        private float unloadProgress;

        [SerializeField, ReadOnly]
        private float loadingProgress;

        [SerializeField]
        public bool VerboseConsole = false;
        #endregion

        #region private properties
        private enum FadeState
        {
            black,
            transitioning,
            scene,
            unknown
        }

        private FadeState fadeState = FadeState.unknown;
        
        private bool readyToLoadScene;

        #endregion

        /// <summary>
        /// Returns true if we are currently loading a scene
        /// </summary>
        public bool ReadyToLoadScene { get => readyToLoadScene; private set => readyToLoadScene = value; }

        private void Awake()
        {
            if (!Instance)
                Instance = this;
            else
                Debug.LogError("multi instances");
        }

        private void Start()
        {
            VirsabiSceneLoadingManager.Instance.OnLoadingProgressChanged.AddListener(OnLoadingProgressUpdated);
            VirsabiSceneLoadingManager.Instance.OnUnloadingProgressChanged.AddListener(OnUnLoadingProgressUpdated);

            ReadyToLoadScene = true;
        }

        private void OnDisable()
        {
            VirsabiSceneLoadingManager.Instance.OnLoadingProgressChanged.RemoveListener(OnLoadingProgressUpdated);
            VirsabiSceneLoadingManager.Instance.OnUnloadingProgressChanged.RemoveListener(OnUnLoadingProgressUpdated);
        }


        private void OnLoadingProgressUpdated(float progress)
        {
            loadingProgress = progress;
            ProgressUIFeedback.Progress = progress;
        }

        private void OnUnLoadingProgressUpdated(float progress)
        {
            unloadProgress = progress;
        }

        private void SetLoadingSceneStatus(bool status)
        {
            LoadingEnvironment.SetActive(status);
            LoadingCanvasGroup.alpha = status ? 1 : 0;
        }

        public void LoadNextSceneUsingLoadingScreen(string fromScene, string toScene)
        {
            if (!ReadyToLoadScene)
            {
                if (VerboseConsole)
                    Debug.LogWarning("Not ready to load scene - already loading something");
                return;
            }


            ReadyToLoadScene = false;
            if (VerboseConsole)
                Debug.Log("LOADING: StartedLoading", this);

            StartCoroutine(LoadNextSceneUsingLoadingScreenRoutine(fromScene, toScene));
        }

        private IEnumerator LoadNextSceneUsingLoadingScreenRoutine(string fromScene, string toScene)
        {
            if (VerboseConsole)
                Debug.Log("LOADING: Started coroutine", this);

            //Fade states not being used - don¨t know if this is a feature we have a use for yet.
            fadeState = FadeState.black;

            //wait till sound fade complete in fromScene
            yield return new WaitUntil(() => fadeState == FadeState.black);

            //Show loadinscreen environment:
            SetLoadingSceneStatus(true);

            //Unload from scene
            VirsabiSceneLoadingManager.Instance.UnloadSceneAsync(fromScene);


            yield return new WaitUntil(() => unloadProgress == 1);
            if (VerboseConsole)
                Debug.Log("LOADING: undload complete");


            VirsabiSceneLoadingManager.Instance.LoadSceneAdditiveAsync(toScene); //Starts the loading progress - needs to be done after unload to avoid stalling the load queue

            yield return new WaitForSecondsRealtime(timeToLoadingScreen);

            if (loadingProgress < 0.9f || forceLoadingScreen) //show loading screen if loading not complete
            {
                fadeState = FadeState.transitioning;
                //do loading scene fading stuff here in future
                fadeState = FadeState.scene; //remove when implementing fade


                //wait till black fade AND sound fade complete in loadingScene
                yield return new WaitUntil(() => fadeState == FadeState.scene);
                if (VerboseConsole)
                    Debug.Log("LOADING: Faded to loadingScene");

                yield return new WaitForSecondsRealtime(minLoadScreenLiveTime);

                yield return new WaitUntil(() => loadingProgress >= 0.9f);
                if (VerboseConsole)
                    Debug.Log("LOADING: new scene loading complete");

                fadeState = FadeState.transitioning;
                fadeState = FadeState.black; //remove when implementing fade

            }
            else
            {
                if (VerboseConsole)
                    Debug.Log("LOADING: Skipped Loading Screen", this);
            }

            yield return new WaitUntil(() => fadeState == FadeState.black && loadingProgress >= 0.9f);
            if (VerboseConsole)
                Debug.Log("LOADING: Faded to black");


            VirsabiSceneLoadingManager.Instance.ActivateLoadedScene();

            SetLoadingSceneStatus(false);

            if (VerboseConsole)
                Debug.Log("LOADING: loading complete");

            ReadyToLoadScene = true;

            yield return null;
        }
    }

}