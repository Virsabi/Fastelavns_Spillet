System for creating and running splashscreens.

The testscene attatches the canvas to a OVR cameraRig, but this can just be changed for AR/Normal applications - OVR is not a dependency!

The system is just a series of VirsabiSplashScreen objects being sequentially shown and hidden. Once all has been shown, scene index 1 (or a specific scene) will be loaded.

The system currently contains three types of splashscreens;

-LogoSplashScreen
	Simply fade in a logo and calls OnSplashScreenFinished after a user defined amount of time.

-VideoSplashScreen
	Play a video with direct audio and calls OnSplashScreenFinished once video has finished playing.

-CustomSplashScreen
	Holds a custom Recttrans prefab - example case is a grid with 4 logos and some text. Calls OnSplashScreenFinished after userdefined amount of time.