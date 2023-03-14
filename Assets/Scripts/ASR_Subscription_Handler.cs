using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

/// <summary>
///   <para>
///     <strong>Author:</strong> Alexander Larsen & Steffen Hansen.
///     </para>
///   <para>
///     <strong>Description:</strong> This is a Singleton class that will persist throughout the app session
///     It holds the <see cref="subscriptionKey"/> & <see cref="serviceRegion"/> + the <see cref="SpeechConfig"/> & <see cref="SpeechRecognizer"/>
///     </para>
/// </summary>

public class ASR_Subscription_Handler : MonoBehaviour
{
    private readonly string subscriptionKey = "075f9b459c31416ab9b50f5516fe0d3f";
    private readonly string serviceRegion = "northeurope";

    ///This line of code creates a variable called speechConfig of type <see cref="SpeechConfig"/>.
    ///SpeechConfig is a class used to configure the Speech SDK.
    public SpeechConfig speechConfig;

    ///This line of code creates a SpeechRecognizer object called "recognizer".
    ///This object can be used to recognize speech and convert it into text.
    public SpeechRecognizer recognizer;

    void Start()
    {
        // Get the language from PlayerPrefs, or set it to "da-DK" if it doesn't exist
        string language = PlayerPrefs.GetString("selectedLanguage") ?? "da-DK";
        Debug.Log("Current Language: " + language);

        // Create a SpeechConfig object using the subscription key and service region
        speechConfig = SpeechConfig.FromSubscription(subscriptionKey, serviceRegion);
        // Set the SpeechConfig object's language to the language from PlayerPrefs
        speechConfig.SpeechRecognitionLanguage = language;
        // Set the Speech Silence Timeout to 300 miliseconds instead of the default 500 miliseconds
        //speechConfig.SetProperty(PropertyId.Speech_SegmentationSilenceTimeoutMs, "300");
        // Create a SpeechRecognizer object using the SpeechConfig and AudioConfig objects
        recognizer = new SpeechRecognizer(speechConfig, AudioConfig.FromDefaultMicrophoneInput());
        // Don't destroy this gameObject when a new scene is loaded
        DontDestroyOnLoad(gameObject);
    }




}
