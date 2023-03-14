using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using TMPro;
using System;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using EasyButtons;
// using Unity.XR.CoreUtils;
using System.Threading.Tasks;

/// <summary>
///   <para>
///     <strong>Author:</strong> Alexander Larsen & Steffen Hansen.
///     </para>
///   <para>
///     <strong>Description:</strong> A class that implements and handles Azure Speech-to-text service.
/// Azure Speech documentation: <see href="https://docs.microsoft.com/en-gb/azure/cognitive-services/speech-service/get-started-speech-to-text?tabs=script%2Cwindowsinstall&pivots=programming-language-csharp"/> 
///     </para>
/// </summary>

public class CognitiveSpeechManager : MonoBehaviour
{
    #region References
    public TMP_InputField tmpInputField;

    [SerializeField]
    private AudioSource microphoneSource;

    [SerializeField]
    private ASR_Subscription_Handler subscription_Handler;

    #endregion

    #region Setup
    public bool IsRecording { get; private set; }

    [Header("Setup")]
    [SerializeField, Tooltip("Single shot stops recording after the first sentence, while Continuous needs to be stopped manually.")]
    private Modes mode = Modes.SingleShot;

    [SerializeField, Tooltip("Dictation mode allows the user to speak out symbol. E.g., \"Question mark\" becomes \"?\".")]
    private bool enableDictationMode = false;

    [SerializeField]
    private bool recordOnEnable = false;


    [SerializeField, Tooltip("Text displayed if speech input couldn't be matched.")]
    private string noMatchText = "Speech not recognized";
    #endregion

    #region Events
    [Header("Events")]
    [SerializeField]
    public UnityEvent<string> OnDictationResult;

    public UnityEvent OnDictationComplete;

    [SerializeField]
    public UnityEvent<string> OnDictationNoMatch;
    #endregion

    #region Private Hidden

    private string tempResult = "";
    private enum Modes { SingleShot, Continuous }

    #endregion

    #region Unity Methods

    private void OnEnable()
    {
        subscription_Handler = FindObjectOfType<ASR_Subscription_Handler>();

        if (enableDictationMode && subscription_Handler != null)
        {
            subscription_Handler.speechConfig.EnableDictation();
        }

        subscription_Handler.recognizer.Recognizing += ContinuousRunningEvent();
        subscription_Handler.recognizer.Recognized += ContinuousCompleteEvent();

        if (microphoneSource != null && subscription_Handler != null)
        {
            StartCoroutine(PrepareMicrophone());
        }

        if (recordOnEnable && subscription_Handler != null)
        {
            StartRecording();
        }
    }

    private void OnDisable()
    {
        subscription_Handler.recognizer.Recognizing -= ContinuousRunningEvent();
        subscription_Handler.recognizer.Recognized -= ContinuousCompleteEvent();
        if (microphoneSource != null)
        {

            microphoneSource.Stop();
            Microphone.End(null);
        }
        StopRecording();
    }

    private void LateUpdate()
    {
        if (microphoneSource != null)
        {
            if (!microphoneSource.isPlaying)
            {
                if (Microphone.GetPosition(Microphone.devices[0]) > 0)
                {
                    microphoneSource.Play();
                }
            }
        }


    }
    #endregion

    /// <summary>
    /// Clears the current spoken text.
    /// </summary>
    public void ClearCurrentText()
    {
        tempResult = "";
        OnDictationResult.Invoke("");
    }

    /// <summary>
    /// Starts listening asyncronously based of which mode is currently selected from <see cref="Modes"/>
    /// </summary>
    [Button]
    public void StartRecording()
    {
        //If a temporary input field is present, store the text in a temporary result.
        if (tmpInputField != null)
        {
            tempResult = tmpInputField.text;
        }
        //If a microphone source is present, unmute it and log the mute status.
        if (microphoneSource != null && subscription_Handler != null)
        {
            microphoneSource.mute = false;
            Debug.Log("Microphone muted = " + microphoneSource.mute);
        }

        //If the temporary result is not empty, add a space to it.
        if (!string.IsNullOrEmpty(tempResult))
        {
            tempResult += " ";
        }

        //Depending on the mode, begin either a single shot or continuous recording.
        switch (mode)
        {
            case Modes.SingleShot: BeginSingleShotAsync().Forget(); break;
            case Modes.Continuous: BeginContinuousAsync().Forget(); break;
        }

        //Set the recording status to true.
        IsRecording = true;
    }

    /// <summary>
    /// Stops an ongoing asyncronously listener based on if <see cref="IsRecording"/> is true.
    /// </summary>
    [Button]
    public void StopRecording()
    {
        //If the recording is not active, return
        if (!IsRecording) return;

        //If the microphone source is not null, mute the microphone
        if (microphoneSource != null)
        {
            microphoneSource.mute = true;
            Debug.Log("Microphone muted = " + microphoneSource.mute);
        }

        //Stop the continuous dictation and forget the result
        StopContinuousDictationAsync().Forget();

        //Invoke the OnDictationComplete event
        OnDictationComplete.Invoke();

        //Set IsRecording to false
        IsRecording = false;
        Debug.Log("IsRecording: " + IsRecording);
    }

    /// <summary>
    /// Toggles the listener.
    /// </summary>
    [Button]
    public void ToggleRecording()
    {
        if (!IsRecording) StartRecording();
        else StopRecording();

        Debug.Log($"is recording = {IsRecording}");
    }

    /// <summary>
    /// Begins a single shot asyncronously. This listens to recognized words until a fixed amount of time where no words have been recognized then stops itself automatically.
    /// </summary>
    /// <returns>the result of what was spoken during the listening session. </returns>
    private async UniTaskVoid BeginSingleShotAsync()
    {
        SpeechRecognitionResult result = await subscription_Handler.recognizer.RecognizeOnceAsync();

        switch (result.Reason)
        {
            case ResultReason.RecognizedSpeech:
                {
                    OnDictationResult.Invoke(result.Text);
                    if (tmpInputField != null)
                    {
                        tmpInputField.text = result.Text;
                    }

                    Debug.Log($"Regonized: Text = <color=#0FFF>{result.Text}</color>");
                    break;
                }
            case ResultReason.NoMatch:
                {
                    OnDictationNoMatch.Invoke(noMatchText);
                    if (tmpInputField != null)
                    {
                        tmpInputField.text = noMatchText;
                    }

                    break;
                }
        }

        OnDictationComplete.Invoke();
        IsRecording = false;
        Debug.Log("IsRecording: " + IsRecording);
        if (microphoneSource != null && subscription_Handler != null)
        {
            microphoneSource.mute = true;
            Debug.Log("Microphone muted = " + microphoneSource.mute);
        }
    }

    /// <summary>
    /// Begins the continuous listening asynchronous. Just starts a stream that has to be shut down manually.
    /// </summary>
    private async UniTaskVoid BeginContinuousAsync()
    {
        await subscription_Handler.recognizer.StartContinuousRecognitionAsync();
        Debug.Log("Started Continuous Speech Recognition");
    }

    /// <summary>
    /// Stops the continuous listening asynchronous. Stops the on going stream.
    /// </summary>
    private async UniTaskVoid StopContinuousDictationAsync()
    {
        await subscription_Handler.recognizer.StopContinuousRecognitionAsync();
        Debug.Log("Stopped Continuous Speech Recognition");
    }

    /// <summary>
    /// Gives the final result back from a continuous stream.
    /// </summary>
    /// <returns>final result of the spoken words recognized.</returns>
    private EventHandler<SpeechRecognitionEventArgs> ContinuousCompleteEvent()
    {
        return (s, e) =>
        {
            if (e.Result.Reason == ResultReason.RecognizedSpeech)
            {
                OnDictationResult.Invoke(tempResult + e.Result.Text);
                tempResult += e.Result.Text + " ";
                Debug.Log(tempResult + e.Result.Text);
            }
            else if (e.Result.Reason == ResultReason.NoMatch)
            {
                OnDictationNoMatch.Invoke(noMatchText);
            }
        };
    }

    /// <summary>
    /// Gives Interim results back from a continuous stream.
    /// </summary>
    /// <returns>Interim results of the spoken words recognized.</returns>
    private EventHandler<SpeechRecognitionEventArgs> ContinuousRunningEvent()
    {
        return (s, e) =>
        {
            OnDictationResult.Invoke(tempResult + e.Result.Text);
            Debug.Log(tempResult + e.Result.Text);
        };
    }

    /// <summary>
    /// Prepares the microphone by unmuting, looping, enabling effects, setting the spatial blend, starting the microphone, and playing the microphone source.
    /// </summary>
    /// <returns>
    /// An IEnumerator that waits until the microphone position is greater than 0.
    /// </returns>
    private IEnumerator PrepareMicrophone()
    {
        // Unmute the microphone source
        microphoneSource.mute = false;

        // Set the microphone source to loop
        microphoneSource.loop = true;

        // Enable effects on the microphone source
        microphoneSource.bypassEffects = false;

        // Enable listener effects on the microphone source
        microphoneSource.bypassListenerEffects = false;

        // Set the spatial blend of the microphone source to 0
        microphoneSource.spatialBlend = 0;

        // Start the microphone and set the clip to the microphone source
        microphoneSource.clip = Microphone.Start(null, true, 1, AudioSettings.outputSampleRate);

        // Wait until the microphone position is greater than 0
        yield return new WaitUntil(() => Microphone.GetPosition(null) > 0);

        // Play the microphone source
        microphoneSource.Play();
    }

}
