using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using Unity.XR.CoreUtils;
using EasyButtons;
using Cysharp.Threading.Tasks;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Events;
using UnityEngine.Animations;
using DG.Tweening;
using System.Linq;
using System.Collections;
using System.Threading.Tasks;

/// <summary>
///   <para>
///     <strong>Author:</strong> Steffen Hansen
///     </para>
///   <para>
///     <strong>Description:</strong> This is the overarching manager that handles the dialog choices / current locale and acts as a gateway for all other scripts.
///     </para>
/// </summary>
public class Timeline_Manager : MonoBehaviour
{
    #region Classes & Enums
    /// <summary>
    /// The DialogChoice class that holds the text, keywords, timelineID, ChoiceID, timelineAssetID, nexttimelineID to load and signal events tied to this instance of a dialog choice.
    /// </summary>
    [Serializable]
    public class DialogChoice
    {
        [HideInInspector]
        public string name;
        /// <summary>
        /// The text of the option show on the dialog UI screen.
        /// </summary>
        [Multiline(5)]
        public string text;
        /// <summary>
        /// The keywords the checker listens for when a speak have been recognized.
        /// </summary>
        public List<string> keywords = new List<string>();
        public DialogType type;
        public Sprite instructionImg;
        public string stateName;
        // [ReadOnly]
        public string timelineID;
        // [ReadOnly]
        public string choiceID;
        public string timelineAssetID;
        public string nextTimelineID;
        public UnityEvent signalEvent;

    }

    /// <summary>
    /// A convenient reference class that also holds the individual list of <see cref="DialogChoice"/>'s text.
    /// </summary>
    [Serializable]
    public class DialogRef
    {
        public List<string> choiceText = new List<string>();
        public DialogChoice choice;
        public DialogUIElement choiceElement;
    }

    [Serializable]
    public class ActorClass
    {
        public string actorName;
        public GameObject ActorObj;
        public LookAtConstraint l_eye;
        public LookAtConstraint r_eye;
        public LookAtConstraint head;
        [Range(0f, 1f)]
        public float eyeWeightTarget = 1f;
        [Range(0f, 1f)]
        public float headWeightTarget = 0.8f;
    }

    /// <summary>
    /// Just a quick enum that holds the languages this project currently supports.
    /// </summary>
    public enum Language
    {
        Danish,
        English,
    }

    public enum DialogType
    {
        Choice,
        Dialog,
        Instruction,
        Hint,
        Warning
    }
    #endregion

    #region Managers

    CognitiveSpeechManager speechManager;

    public DialogUI_Manager dialogUIManager;

    #endregion

    #region References
    [Header("References")]
    public PlayableDirector director;

    public List<TimelineAsset> timelineAssetPool = new List<TimelineAsset>();

    [SerializeField]
    private Transform playerTarget;

    [SerializeField]
    private Image micStateImg;

    #endregion

    #region Setup
    [Header("Setup")]
    /// <summary>
    /// The <see cref="DialogChoice"/> choices
    /// </summary>
    public List<DialogChoice> choices;

    [SerializeField]
    private Language Choosen_Language;

    public string currentTimelineID;

    public DialogRef choiceARef;

    public DialogRef choiceBRef;

    public DialogRef choiceCRef;
    #endregion

    #region Audio
    [Header("Audio")]
    [SerializeField]
    private AudioSource voiceInputSource;
    [SerializeField]
    private AudioClip voiceInputConfirm;
    [SerializeField]
    private AudioSource doorLeftSource;
    [SerializeField]
    private AudioClip doorOpen;
    [SerializeField]
    private AudioClip doorClose;
    [SerializeField]
    private AudioClip buttonPress;

    #endregion

    #region Animation
    [Header("Animation")]
    [SerializeField, Tooltip("Find the Actor Prefab and place that into Actor Obj and the rest will be filled out automatically. If not find the other components manually or add the missing components to the actor prefab.")]
    private List<ActorClass> actorClasses = new List<ActorClass>();


    #endregion

    #region Unity Methods
    private void OnValidate()
    {
        if (Choosen_Language == Language.Danish)
        {
            PlayerPrefs.SetString("selectedLanguage", "da-DK");
        }
        else if (Choosen_Language == Language.English)
        {
            PlayerPrefs.SetString("selectedLanguage", "en-US");
        }

        if (choices.Count > 0)
        {
            foreach (var choice in choices)
            {


                string[] split = choice.stateName.Split("_");
                if (split.Length > 0)
                {
                    if (split.Length >= 2)
                    {
                        choice.timelineID = split[0] + split[1];
                    }
                    if (split.Length >= 4)
                    {
                        choice.choiceID = split[2] + split[3];
                    }
                }

                choice.name = "Type: [" + choice.type + "] | TID: [" + choice.timelineID + "] | CID: [" + choice.choiceID + "] | NTID: [" + choice.nextTimelineID + "] | Key: [" + choice.keywords.Stringify() + "] | Txt: [" + choice.text + "]";
            }
        }


        if (actorClasses.Count > 0)
        {
            foreach (var actorClass in actorClasses)
            {
                if (actorClass.ActorObj != null)
                {
                    actorClass.actorName = actorClass.ActorObj.name;
                    actorClass.l_eye = actorClass.ActorObj.GetNamedChild("CC_Base_L_Eye").GetComponent<LookAtConstraint>();
                    actorClass.r_eye = actorClass.ActorObj.GetNamedChild("CC_Base_R_Eye").GetComponent<LookAtConstraint>();
                    actorClass.head = actorClass.ActorObj.GetNamedChild("CC_Base_Head").GetComponent<LookAtConstraint>();
                }
            }
        }

    }

    private void Start()
    {
        if (Choosen_Language == Language.Danish)
        {
            PlayerPrefs.SetString("selectedLanguage", "da-DK");
        }
        else if (Choosen_Language == Language.English)
        {
            PlayerPrefs.SetString("selectedLanguage", "en-US");
        }
        speechManager = FindObjectOfType<CognitiveSpeechManager>();
        dialogUIManager = FindObjectOfType<DialogUI_Manager>();
        if (speechManager != null)
        {
            speechManager.enabled = true;
             if (gameObject.GetComponent<Timeline_Initializor>() != null)
             {
                 dialogUIManager.ClearDialogOptions();
                 LoadTimelineChoices("Timeline0");
             }
            speechManager.StartRecording();
        }
        
    }
    public void LateUpdate()
    {
        if (micStateImg != null)
        {
             micStateImg.gameObject.SetActive(speechManager.IsRecording);
        }
    }
    #endregion

    /// <summary>
    /// Resets the scenario.
    /// </summary>
    [Button]
    public void ResetScenario()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Begins the scenario.
    /// </summary>
    public void BeginScenario()
    {
        dialogUIManager.ClearDialogOptions();
        LoadTimelineChoices("Timeline1");
        director.Play(timelineAssetPool.Find(x => x.name == "010"));
    }

    /// <summary>
    /// Loads the next timeline choices.
    /// </summary>
    public void LoadNextTimelineChoices()
    {
        int timelineIndex = int.Parse(currentTimelineID[currentTimelineID.Length - 1].ToString());
        Debug.Log(timelineIndex);
        timelineIndex++;
        dialogUIManager.ClearDialogOptions();
        LoadTimelineChoices("Timeline" + timelineIndex);
    }

    /// <summary>
    /// Loads the timeline choices.
    /// </summary>
    /// <param name="timelineID">The timeline identifier.</param>
    public void LoadTimelineChoices(string timelineID)
    {
        // Clear the choice text for each choice
        choiceARef.choiceText.Clear();
        choiceBRef.choiceText.Clear();
        choiceCRef.choiceText.Clear();
        currentTimelineID = timelineID;

        // Iterate through the choices
        foreach (var choice in choices)
        {
            // Check if the timeline ID matches the choice's timeline ID
            if (timelineID == choice.timelineID)
            {
                // Check the type of the choice
                if (choice.type == DialogType.Choice)
                {
                    // Load the dialog options
                    dialogUIManager.LoadDialogOptions(choice);
                    // Check if the choice is ChoiceA
                    if (choice.choiceID == "ChoiceA")
                    {
                        // Split the text and add it to the choice text
                        choiceARef.choiceText.AddRange(choice.text.Split(" "));
                        // Set the choice and choice element
                        choiceARef.choice = choice;
                        choiceARef.choiceElement = dialogUIManager.dialogUIPrefabs.Find(x => x.GetComponent<DialogUIElement>().ChoiceID == choice.choiceID).GetComponent<DialogUIElement>();
                    }
                    // Check if the choice is ChoiceB
                    else if (choice.choiceID == "ChoiceB")
                    {
                        // Split the text and add it to the choice text
                        choiceBRef.choiceText.AddRange(choice.text.Split(" "));
                        // Set the choice and choice element
                        choiceBRef.choice = choice;
                        choiceBRef.choiceElement = dialogUIManager.dialogUIPrefabs.Find(x => x.GetComponent<DialogUIElement>().ChoiceID == choice.choiceID).GetComponent<DialogUIElement>();
                    }
                    // Check if the choice is ChoiceC
                    else if (choice.choiceID == "ChoiceC")
                    {
                        // Split the text and add it to the choice text
                        choiceCRef.choiceText.AddRange(choice.text.Split(" "));
                        // Set the choice and choice element
                        choiceCRef.choice = choice;
                        choiceCRef.choiceElement = dialogUIManager.dialogUIPrefabs.Find(x => x.GetComponent<DialogUIElement>().ChoiceID == choice.choiceID).GetComponent<DialogUIElement>();
                    }
                    else
                    {
                        // Log an error if the choice is not ChoiceA, ChoiceB or ChoiceC
                        Debug.Log("Couldn't find choice A B or C in " + timelineID);
                    }
                }
                // Check if the type is Instruction
                else if (choice.type == DialogType.Instruction)
                {
                    // Load the dialog options
                    dialogUIManager.LoadDialogOptions(choice);
                    // Check if the choice is ChoiceA
                    if (choice.choiceID == "ChoiceA")
                    {
                        // Split the text and add it to the choice text
                        choiceARef.choiceText.AddRange(choice.text.Split(" "));
                        // Set the choice and choice element
                        choiceARef.choice = choice;
                        choiceARef.choiceElement = dialogUIManager.dialogUIPrefabs.Find(x => x.GetComponent<DialogUIElement>().ChoiceID == choice.choiceID).GetComponent<DialogUIElement>();
                    }
                }
                // Check if the type is Dialog
                else if (choice.type == DialogType.Dialog)
                {
                    // Load the dialog options
                    dialogUIManager.LoadDialogOptions(choice);
                    // Check if the choice is ChoiceA
                    if (choice.choiceID == "ChoiceA")
                    {
                        // Split the text and add it to the choice text
                        choiceARef.choiceText.AddRange(choice.text.Split(" "));
                        // Set the choice and choice element
                        choiceARef.choice = choice;
                        choiceARef.choiceElement = dialogUIManager.dialogUIPrefabs.Find(x => x.GetComponent<DialogUIElement>().ChoiceID == choice.choiceID).GetComponent<DialogUIElement>();
                    }
                }

            }
        }
    }

    /// <summary>
    /// Checks if what is spoken is good enough to be recognized using keywords and a correctness threshold.
    /// </summary>
    /// <param name="spokenResult">The spoken result to check against.</param>
    public void CheckWordCorrectPercentage(string spokenResult)
    {
        if (speechManager != null)
        {
        if (speechManager.IsRecording)
        {

            List<string> spokenWords = new List<string>();
            spokenWords.AddRange(spokenResult.Split(" "));
            int correctWordsThresholdA = 0;
            int correctWordsThresholdB = 0;
            int correctWordsThresholdC = 0;

            bool isKeywordA = false;
            bool isKeywordB = false;
            bool isKeywordC = false;


            if (choiceARef.choiceText.Count > 0)
            {
                correctWordsThresholdA = Mathf.RoundToInt(choiceARef.choiceText.Count / 4f);
            }

            if (choiceBRef.choiceText.Count > 0)
            {
                correctWordsThresholdB = Mathf.RoundToInt(choiceBRef.choiceText.Count / 4f);
            }

            if (choiceCRef.choiceText.Count > 0)
            {
                correctWordsThresholdC = Mathf.RoundToInt(choiceCRef.choiceText.Count / 4f);
            }


            int correctWordsA = 0;
            int correctWordsB = 0;
            int correctWordsC = 0;

            for (int i = 0; i < spokenWords.Count; i++)
            {
                if (choiceARef.choice.keywords.Exists(keyword => keyword.Contains(spokenWords[i], StringComparison.OrdinalIgnoreCase)))
                {
                    isKeywordA = true;
                    Debug.Log("Keyword Recognized: " + spokenWords[i] + " isKeywordA = " + isKeywordA);
                }
                else if (choiceBRef.choice.keywords.Exists(keyword => keyword.Contains(spokenWords[i], StringComparison.OrdinalIgnoreCase)))
                {
                    isKeywordB = true;
                    Debug.Log("Keyword Recognized: " + spokenWords[i] + " isKeywordB = " + isKeywordB);
                }
                else if (choiceCRef.choice.keywords.Exists(keyword => keyword.Contains(spokenWords[i], StringComparison.OrdinalIgnoreCase)))
                {
                    isKeywordC = true;
                    Debug.Log("Keyword Recognized: " + spokenWords[i] + " isKeywordC = " + isKeywordC);
                }

                if (i < choiceARef.choiceText.Count && choiceARef.choiceText.Count > 0)
                {
                    if (choiceARef.choiceText[i] == spokenWords[i])
                    {
                        correctWordsA++;
                    }
                }
                if (i < choiceBRef.choiceText.Count && choiceBRef.choiceText.Count > 0)
                {
                    if (choiceBRef.choiceText[i] == spokenWords[i])
                    {
                        correctWordsB++;
                    }
                }
                if (i < choiceCRef.choiceText.Count && choiceCRef.choiceText.Count > 0)
                {
                    if (choiceCRef.choiceText[i] == spokenWords[i])
                    {
                        correctWordsC++;
                    }
                }
            }

            if (correctWordsA >= correctWordsThresholdA && correctWordsA != 0 && !isKeywordA && !isKeywordB && !isKeywordC)
            {
                choiceARef.choiceElement.PlayTimelineStep(choiceARef.choice.nextTimelineID);
                if (choiceARef.choice.signalEvent != null)
                {
                    choiceARef.choice.signalEvent.Invoke();
                }
                if (voiceInputSource != null)
                {
                    voiceInputSource.clip = voiceInputConfirm;
                    voiceInputSource.Play();
                }
                Debug.Log("Phrase understood, Selecting choice <color=#0FFF>A</color>\n with a correctness of <color=#0FFF>" + correctWordsA + "</color> with a threshold of <color=#0FFF>" + correctWordsThresholdA + "</color>");
            }
            else if (correctWordsB >= correctWordsThresholdB && correctWordsB != 0 && !isKeywordA && !isKeywordB && !isKeywordC)
            {
                choiceBRef.choiceElement.PlayTimelineStep(choiceBRef.choice.nextTimelineID);
                if (choiceBRef.choice.signalEvent != null)
                {
                    choiceBRef.choice.signalEvent.Invoke();
                }
                if (voiceInputSource != null)
                {
                    voiceInputSource.clip = voiceInputConfirm;
                    voiceInputSource.Play();
                }
                Debug.Log("Phrase understood, Selecting choice <color=#0FFF>B</color>\n with a correctness of <color=#0FFF>" + correctWordsB + "</color> with a threshold of <color=#0FFF>" + correctWordsThresholdB + "</color>");
            }
            else if (correctWordsC >= correctWordsThresholdC && correctWordsC != 0 && !isKeywordA && !isKeywordB && !isKeywordC)
            {
                choiceCRef.choiceElement.PlayTimelineStep(choiceCRef.choice.nextTimelineID);
                if (choiceCRef.choice.signalEvent != null)
                {
                    choiceCRef.choice.signalEvent.Invoke();
                }
                if (voiceInputSource != null)
                {
                    voiceInputSource.clip = voiceInputConfirm;
                    voiceInputSource.Play();
                }
                Debug.Log("Phrase understood, Selecting choice <color=#0FFF>C</color>\n with a correctness of <color=#0FFF>" + correctWordsC + "</color> with a threshold of <color=#0FFF>" + correctWordsThresholdC + "</color>");
            }
            else if (isKeywordA)
            {
                choiceARef.choiceElement.PlayTimelineStep(choiceARef.choice.nextTimelineID);
                if (choiceARef.choice.signalEvent != null)
                {
                    choiceARef.choice.signalEvent.Invoke();
                }
                if (voiceInputSource != null)
                {
                    voiceInputSource.clip = voiceInputConfirm;
                    voiceInputSource.Play();
                }
                Debug.Log("Recognized a keyword from A");
            }
            else if (isKeywordB)
            {
                choiceBRef.choiceElement.PlayTimelineStep(choiceBRef.choice.nextTimelineID);
                if (choiceBRef.choice.signalEvent != null)
                {
                    choiceBRef.choice.signalEvent.Invoke();
                }
                if (voiceInputSource != null)
                {
                    voiceInputSource.clip = voiceInputConfirm;
                    voiceInputSource.Play();
                }
                Debug.Log("Recognized a keyword from B");
            }
            else if (isKeywordC)
            {
                choiceCRef.choiceElement.PlayTimelineStep(choiceCRef.choice.nextTimelineID);
                if (choiceCRef.choice.signalEvent != null)
                {
                    choiceCRef.choice.signalEvent.Invoke();
                }
                if (voiceInputSource != null)
                {
                    voiceInputSource.clip = voiceInputConfirm;
                    voiceInputSource.Play();
                }
                Debug.Log("Recognized a keyword from C");
            }
            else
            {
                Debug.Log("Failed to recognize any keywords or sentence thresholds\n" +
                    "[isKeywordA = " + isKeywordA + "] | " +
                    "[isKeywordB = " + isKeywordB + "] | " +
                    "[isKeywordC = " + isKeywordC + "] |" +
                    "\n<color=#0FFF>A</color> with a correctness of <color=#0FFF>" + correctWordsA + "</color> with a threshold of <color=#0FFF>" + correctWordsThresholdA + "</color>" +
                    "\n<color=#0FFF>B</color> with a correctness of <color=#0FFF>" + correctWordsB + "</color> with a threshold of <color=#0FFF>" + correctWordsThresholdB + "</color>" +
                    "\n<color=#0FFF>C</color> with a correctness of <color=#0FFF>" + correctWordsC + "</color> with a threshold of <color=#0FFF>" + correctWordsThresholdC + "</color>");
                Debug.Log("Not good enough. Try again....");
                ReinitializeRecording().Forget();

            }
        }
        }
    }
    /// <summary>
    /// Reinitializes the recording.
    /// </summary>
    /// <returns> Initializes the <see cref="CognitiveSpeechManager.StartRecording"/> again.</returns>
    private async UniTaskVoid ReinitializeRecording()
            {
                //speechManager.StopRecording();
                await UniTask.Delay(1000);
                speechManager.StartRecording();

            }

            /// <summary>
            /// Loads the index of the scene by the parsed index value.
            /// </summary>
            /// <param name="index">The index.</param>
            public void LoadSceneByIndex(int index)
            {
                SceneManager.LoadSceneAsync(index);
            }

            public void PlayDoorLeftClip(int index)
            {
                if (index == 0)
                {
                    doorLeftSource.PlayOneShot(doorOpen);
                }
                else if (index == 1)
                {
                    doorLeftSource.PlayOneShot(doorClose);
                }
            }

            #region Utility
            /// <summary>
            /// Debug selects the an option based on the integer value parsed 0=A, 1=B, 2=C.
            /// </summary>
            /// <param name="choiceVal">The choice value.</param>
            [Button]
            public void SelectOption(int choiceVal)
            {
                if (choiceVal == 0)
                {
                    if (choiceARef.choiceElement != null)
                    {
                        choiceARef.choiceElement.PlayTimelineStep(choiceARef.choice.nextTimelineID);
                        if (choiceARef.choice.signalEvent != null)
                        {
                            choiceARef.choice.signalEvent.Invoke();
                        }
                        if (voiceInputSource != null)
                        {
                            voiceInputSource.clip = voiceInputConfirm;
                            voiceInputSource.Play();
                        }
                    }
                }
                else if (choiceVal == 1)
                {
                    if (choiceBRef.choiceElement != null)
                    {
                        choiceBRef.choiceElement.PlayTimelineStep(choiceBRef.choice.nextTimelineID);
                        if (choiceBRef.choice.signalEvent != null)
                        {
                            choiceBRef.choice.signalEvent.Invoke();
                        }
                        if (voiceInputSource != null)
                        {
                            voiceInputSource.clip = voiceInputConfirm;
                            voiceInputSource.Play();
                        }
                    }
                }
                else if (choiceVal == 2)
                {
                    if (choiceCRef.choiceElement != null)
                    {
                        choiceCRef.choiceElement.PlayTimelineStep(choiceCRef.choice.nextTimelineID);
                        if (choiceCRef.choice.signalEvent != null)
                        {
                            choiceCRef.choice.signalEvent.Invoke();
                        }
                        if (voiceInputSource != null)
                        {
                            voiceInputSource.clip = voiceInputConfirm;
                            voiceInputSource.Play();
                        }
                    }
                }

            }

            /// <summary>
            /// Selects a hardcoded timeline from the <see cref="DialogChoice.stateName"/> and disregard any timeline continuety.
            /// </summary>
            /// <param name="stateName">The <see cref="DialogChoice"/> unique identifier.</param>
            [Button]
            public void SelectHardcodedTimelineChoice(string stateName)
            {
                dialogUIManager.ClearDialogOptions();
                if (stateName != "")
                {
                    foreach (var choice in choices)
                    {
                        if (choice.stateName == stateName && choice.timelineAssetID != null)
                        {
                            LoadTimelineChoices(choice.timelineID);
                            if (timelineAssetPool.Exists(x => x.name == choice.timelineAssetID))
                            {
                                director.Play(timelineAssetPool.Find(x => x.name == choice.timelineAssetID));
                                Debug.Log("New Timeline: " + choice.timelineID + " Playing timelineAsset from pool = " + choice.timelineAssetID);
                            }
                            else
                            {
                                Debug.Log("New Timeline: " + choice.timelineID + " No TimelineAsset Found defaulting to Idle..");
                            }

                            if (choice.signalEvent != null)
                            {
                                choice.signalEvent.Invoke();
                            }
                            if (voiceInputSource != null)
                            {
                                voiceInputSource.clip = voiceInputConfirm;
                                voiceInputSource.Play();
                            }
                        }
                    }

                }


            }

            /// <summary>
            /// Initializes the look at player animation using <see cref="LerpWeight(ActorClass, float, float)"/>
            /// </summary>
            /// <param name="actorName">Name of the actor.</param>
            public void AnimateLookAtPlayer(string actorName)
            {
                if (actorClasses.Count > 0)
                {
                    foreach (var actor in actorClasses)
                    {
                        if (actor.actorName == actorName)
                        {
                            StartCoroutine(LerpWeight(actor, actor.eyeWeightTarget, actor.headWeightTarget));
                            actor.r_eye.constraintActive = true;
                            actor.l_eye.constraintActive = true;
                            //actor.head.constraintActive = true;
                        }
                    }
                }
            }

            /// <summary>
            /// Initializes the stop looking at player animation using <see cref="LerpWeight(ActorClass, float, float)"/>
            /// </summary>
            /// <param name="actorName">Name of the actor.</param>
            public void AnimateStopLookAtPlayer(string actorName)
            {
                if (actorClasses.Count > 0)
                {
                    foreach (var actor in actorClasses)
                    {
                        if (actor.actorName == actorName)
                        {
                            StartCoroutine(LerpWeight(actor, 0, 0));
                            actor.r_eye.constraintActive = false;
                            actor.l_eye.constraintActive = false;
                            //actor.head.constraintActive = false;
                        }
                    }
                }
            }

            /// <summary>
            /// Animates the head & eye movement of the actors between two targets 
            /// using <see cref="DOTween.To(DG.Tweening.Core.DOGetter{float}, DG.Tweening.Core.DOSetter{float}, float, float)"/> 
            /// (used to lerp between the player and the original animated position)
            /// </summary>
            /// <param name="actor">The actor.</param>
            /// <param name="eyeTargetWeight">The eye target weight value.</param>
            /// <param name="headTargetWeight">The head target weight value.</param>
            /// <returns>A 1 second transition between the 2 positions.</returns>
            private IEnumerator LerpWeight(ActorClass actor, float eyeTargetWeight, float headTargetWeight)
            {

                //DOTween.To(() => actor.l_eye.weight, x => actor.l_eye.weight = x, eyeTargetWeight, 0.2f);
                //DOTween.To(() => actor.r_eye.weight, x => actor.r_eye.weight = x, eyeTargetWeight, 0.2f);
                DOTween.To(() => actor.head.weight, x => actor.head.weight = x, headTargetWeight, 1f);
                yield return new WaitForSeconds(1f);
            }


            #endregion
        }
