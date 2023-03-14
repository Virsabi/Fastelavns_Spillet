using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace Virsabi
{
    public class SceneChanger : MonoBehaviour
    {
        [SerializeField]
        private bool FadeAudioMixer; //not working

        [SerializeField, ConditionalField(nameof(FadeAudioMixer))]
        private float fadeTime;

        [SerializeField, ConditionalField(nameof(FadeAudioMixer))]
        private AudioMixerSnapshot AudioOnSnapshot, AudioOffSnapshot;

        [SerializeField]
        private SceneReference NextScene, LoadingScene;


        private enum FadeState
        {
            black,
            transitioning,
            scene,
            unknown
        }

        //private FadeState fadeState = FadeState.unknown; - usefull for when implementing fade

        private void Start()
        {
            if (SceneLoader.Instance == null)
                SceneManager.LoadSceneAsync(LoadingScene.SceneName, LoadSceneMode.Additive);
        }

        /// <summary>
        /// Loads the specified scene using loading screen if necessary
        /// </summary>
        /// <param name="scene"></param>
        public void GotToSceneUsingLoadingScreen(SceneReference scene)
        {
            if (!SceneLoader.Instance)
            {
                Debug.LogError("LOADING: couldn't find scene loader instance");
                return;
            }

            if (!SceneLoader.Instance.ReadyToLoadScene)
            {
                Debug.LogWarning("LOADING: Sceneloader not ready to load scene");
                return;
            }

            if (SceneLoader.Instance.VerboseConsole)
                Debug.Log("LOADING: StartedFading");

            if(FadeAudioMixer)
                AudioOffSnapshot?.TransitionTo(fadeTime);

            StartCoroutine(LoadSceneUsingFadeAndLoadingScreen(scene));
        }

        private IEnumerator LoadSceneUsingFadeAndLoadingScreen(SceneReference scene)
        {
            //if (FadeAudioMixer)
            //    yield return new WaitUntil(() => fadeState == FadeState.black);

            SceneLoader.Instance.LoadNextSceneUsingLoadingScreen(gameObject.scene.name, scene.SceneName);

            yield return null;
        }

        /// <summary>
        /// Just for debugging
        /// </summary>
        [ButtonMethod]
        public void GoToNextSceneUsingLoadingScreen()
        {
            GotToSceneUsingLoadingScreen(NextScene);
        }
    }

}