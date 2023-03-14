using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using MyBox;
using System;

namespace Virsabi
{

    [RequireComponent(typeof(RawImage), typeof(VideoPlayer))]
    public class VideoSplashScreen : VirsabiSplashScreen
    {

        [SerializeField, HideInInspector]
        private VideoPlayer videoPlayer;

        [SerializeField, HideInInspector]
        private RawImage image;

        [SerializeField, HideInInspector]
        private RenderTexture renderTexture;

        [Foldout("Setup", true)]

        [SerializeField, MustBeAssigned]
        private VideoClip VideoClip;

        [SerializeField]
        private bool ManualResolution = false;

        [SerializeField, ConditionalField(nameof(ManualResolution))]
        public int width = 512, height = 512;

        [SerializeField, ReadOnly]
        private Vector2 Resolution;

        private Vector2 lastResolution;


        public override void OnValidate()
        {
            base.OnValidate();

            ///Disable validate if running in prefab
            if (!gameObject.activeInHierarchy)
                return;

            if (type != SplashscreenType.video)
                splashScreenBehavior.ReplaceSplashScreen(this, type);

            videoPlayer = GetComponent<VideoPlayer>();
            image = GetComponent<RawImage>();

            videoPlayer.renderMode = VideoRenderMode.RenderTexture;

            videoPlayer.playOnAwake = false;
            videoPlayer.waitForFirstFrame = true;
            videoPlayer.isLooping = false;
            //videoPlayer.skipOnDrop = true; - not allowed in prefab - GG unity.....
            videoPlayer.playbackSpeed = 1;

            if (VideoClip)
                videoPlayer.clip = VideoClip;


            if (ManualResolution)
            {
                Resolution.x = width;
                Resolution.y = height;
            }
            else if (videoPlayer)
            {
                Resolution.x = (int)videoPlayer.clip.width;
                Resolution.y = (int)videoPlayer.clip.width;
            }
            else
            {
                Debug.LogError("Please set video", this);
                return;
            }


            if (!renderTexture || Resolution != lastResolution)
            {
                renderTexture = new RenderTexture((int)Resolution.x, (int)Resolution.y, 16);
            }

            lastResolution = Resolution;

            videoPlayer.targetTexture = renderTexture;
            image.texture = renderTexture;
        }

        private void Awake()
        {
            videoPlayer.loopPointReached += FinishedPlaying;
        }

        private void FinishedPlaying(VideoPlayer vp)
        {
            OnSplashScreenFinished.Invoke();
        }

        public void PlayVideo()
        {
            videoPlayer.Play();
        }

        public override void Show()
        {
            image.enabled = true;
            videoPlayer.Play();
        }

        public override void Hide()
        {
            image.enabled = false;
        }

        private void OnDisable()
        {
            videoPlayer.loopPointReached -= FinishedPlaying;
        }

        private void OnDestroy()
        {
            videoPlayer.loopPointReached -= FinishedPlaying;
        }
    }

}