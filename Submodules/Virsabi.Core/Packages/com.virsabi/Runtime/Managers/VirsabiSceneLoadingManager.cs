using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;

/// <summary>
/// This class should be platform agnostic - simply takes in requests for loading scenes and manages the loading screen and progress.
/// </summary>
/// 
namespace Virsabi
{
    public class VirsabiSceneLoadingManager : MonoBehaviour
    {
        public static VirsabiSceneLoadingManager Instance;

        public UEFloatEvent OnLoadingProgressChanged = new UEFloatEvent();

        public UEFloatEvent OnUnloadingProgressChanged = new UEFloatEvent();

        [SerializeField]
        private bool VerboseConsole = false;

        //TODO: add these types to virsabi repo? - test the unityevent visualization tool from bookmarks!
        public class UEFloatEvent : UnityEvent<float> { }



        private float loadingProgress;


        private float unloadingProgress;
        private bool allowSceneActivation;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
            else
                Debug.LogError("multiple singleton instances!", Instance);
        }

        /// <summary>
        /// loadingprogress form 0-1;
        /// </summary>
        public float LoadingProgress
        {
            get => loadingProgress;

            private set
            {
                loadingProgress = value;
                OnLoadingProgressChanged.Invoke(loadingProgress);
            }
        }
        /// <summary>
        /// Unloadingprogress form 0-1;
        /// </summary>
        public float UnloadingProgress
        {
            get => unloadingProgress;

            private set
            {
                unloadingProgress = value;
                OnUnloadingProgressChanged.Invoke(unloadingProgress);
            }
        }


        public void LoadSceneAdditiveAsync(string scene)
        {
            //Start loading the Scene asynchronously and output the progress bar
            StartCoroutine(LoadSceneAdditiveAsyncWithProgress(scene, LoadSceneMode.Additive));
        }

        public void UnloadSceneAsync(string scene)
        {
            //Start unloading the Scene asynchronously and output the progress bar
            StartCoroutine(UnloadSceneAsyncWithProgress(scene));
        }

        public void ActivateLoadedScene()
        {
            allowSceneActivation = true;
        }

        IEnumerator LoadSceneAdditiveAsyncWithProgress(string scene, LoadSceneMode mode)
        {
            yield return null;

            if(VerboseConsole)
                Debug.Log("Started loading Coroutine");

            //Begin to load the Scene you specify
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene, mode);
            //Don't let the Scene activate until you allow it to
            asyncOperation.allowSceneActivation = false;
            if (VerboseConsole)
                Debug.Log("Pro :" + asyncOperation.progress);

            //When the load is still in progress, output the Text and progress bar
            while (!asyncOperation.isDone)
            {
                //Output the current progress
                //m_Text.text = "Loading progress: " + (asyncOperation.progress * 100) + "%";
                LoadingProgress = asyncOperation.progress;
                if (VerboseConsole)
                    Debug.Log("load Progress :" + asyncOperation.progress);

                // Check if the load has finished
                if (asyncOperation.progress >= 0.9f)
                {
                    if (allowSceneActivation)
                    {
                        //Activate the Scene
                        asyncOperation.allowSceneActivation = true;
                    }
                }

                yield return null;
            }

            LoadingProgress = 1;

            allowSceneActivation = false;

            //set the scene as main scene (this scene will spawn new gameobjects and use this scene's lighting settings)
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene));

            //Recalculate lightprobes - NEEDED after additively loading a scene.
            LightProbes.TetrahedralizeAsync();

            yield return null;
        }

        IEnumerator UnloadSceneAsyncWithProgress(string scene)
        {
            yield return null;
            if (VerboseConsole)
                Debug.Log("Started Unloading Coroutine");

            //Begin to unload the Scene you specify
            AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(scene);
            if (VerboseConsole)
                Debug.Log("Pro :" + asyncOperation.progress);


            //When the load is still in progress, output the Text and progress bar
            while (!asyncOperation.isDone)
            {
                UnloadingProgress = asyncOperation.progress;
                if (VerboseConsole)
                    Debug.Log("unload Progress :" + asyncOperation.progress);
                yield return null;
            }

            yield return Resources.UnloadUnusedAssets();

            UnloadingProgress = 1;

            yield return null;
        }

    }

}