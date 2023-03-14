using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using EasyButtons;
using Unity.XR.CoreUtils;
using System.Threading.Tasks;

/// <summary>
///   <para>
///     <strong>Author:</strong> Steffen Hansen
///     </para>
///   <para>
///     <strong>Description:</strong> This is the class for the Dialog UI elements show on the Dialog Screen. 
///     </para>
/// </summary>

public class DialogUIElement : MonoBehaviour
{
    #region References
    [Header("References")]
    public TextMeshProUGUI textField;

    [SerializeField, ReadOnly]
    private DialogUI_Manager dialogUIManager;

    [SerializeField, ReadOnly]
    private Timeline_Manager manager;

    [SerializeField]
    private Transform pokeInteractableBtn;

    [SerializeField]
    private Image BG;

    [SerializeField]
    private Image highlight;

    [SerializeField]
    private Image InstructionImg;
    #endregion

    #region Setup
    [Header("Setup")]
    [SerializeField]
    private float highlightSpeed;

    [SerializeField]
    private bool isHiglighting;

    [SerializeField]
    private bool hasTriggeredFromHighlight;
    
    // [ReadOnly]
    public string ChoiceID;
    //[ReadOnly]
    public string TimelineID;
    // [ReadOnly]
    public string TimelineAssetID;

    [SerializeField]
    public Timeline_Manager.DialogType dialogType;

    [SerializeField]
    private float originalSizeY;
    
    [SerializeField]
    private float minSizeY;
    
    [SerializeField]
    private float maxSizeY;

    #endregion

    #region Unity Methods
    private void OnEnable()
    {
        manager = FindObjectOfType<Timeline_Manager>();
        dialogUIManager = FindObjectOfType<DialogUI_Manager>();
        isHiglighting = false;
        hasTriggeredFromHighlight = false;
        StartCoroutine(DelayedPokeInteractableBtnUpdater());
    }
    private void OnDisable()
    {
        StopCoroutine(DelayedPokeInteractableBtnUpdater());
        isHiglighting = false;
    }

    private void LateUpdate()
    {
        if(isHiglighting)
        {
            if (highlight.fillAmount < 1)
            {
                highlight.fillAmount += highlightSpeed;
            }
            else if(highlight.fillAmount >= 1 && !hasTriggeredFromHighlight)
            {
                hasTriggeredFromHighlight = true;

                if(this == manager.choiceARef.choiceElement)
                {
                    PlayTimelineStep(manager.choiceARef.choice.nextTimelineID);
                    if (manager.choiceARef.choice.signalEvent != null)
                    {
                        manager.choiceARef.choice.signalEvent.Invoke();
                    }
                }
                else if(this == manager.choiceBRef.choiceElement)
                {
                    PlayTimelineStep(manager.choiceBRef.choice.nextTimelineID);
                    if (manager.choiceBRef.choice.signalEvent != null)
                    {
                        manager.choiceBRef.choice.signalEvent.Invoke();
                    }
                }
                else if(this == manager.choiceCRef.choiceElement)
                {
                    PlayTimelineStep(manager.choiceCRef.choice.nextTimelineID);
                    if (manager.choiceCRef.choice.signalEvent != null)
                    {
                        manager.choiceCRef.choice.signalEvent.Invoke();
                    }
                }
            }
        }
        else
        {
            highlight.fillAmount = 0;
        }

    }
    #endregion

    /// <summary>
    /// Setup the <see cref="DialogUIElement"/> with its assigned Data from <see cref="Timeline_Manager.DialogChoice"/>
    /// </summary>
    /// <param name="text">The text shown on the Dialog Screen.</param>
    /// <param name="choiceID">The choice identifier.</param>
    /// <param name="timelineID">The timeline identifier.</param>
    /// <param name="timelineAssetID">The timeline asset identifier.</param>
    /// <param name="keywords">The keywords that will be recognized by the <see cref="Timeline_Manager.CheckWordCorrectPercentage(string)"/>.</param>
    public void DialogSetup(Timeline_Manager.DialogChoice dialog)
    {
        ChoiceID = dialog.choiceID;
        TimelineID = dialog.timelineID;
        TimelineAssetID = dialog.timelineAssetID;
        dialogType = dialog.type;
        if(dialog.keywords.Count > 0)
        {
            var highlightedText = "";
            foreach (var word in dialog.text.Split(" "))
            {
                if (dialog.keywords.Contains(word))
                {
                    highlightedText += " <color=#fa0f>" + word + "</color> ";
                }
                else
                {
                    highlightedText += " "+word+" ";
                }
            }
            textField.text = highlightedText;
        }
        else
        {
            textField.text = dialog.text;
        }

        if(dialog.type == Timeline_Manager.DialogType.Instruction)
        {
            InstructionImg.sprite = dialog.instructionImg;
            InstructionImg.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Shows the highlight by setting the <see cref="isHiglighting"/> to true which then triggers logic in the <see cref="LateUpdate"/>.
    /// </summary>
    public void ShowHighlight()
    {
        isHiglighting = true;
    }

    //public void HideHighlight()
    //{
    //    isHiglighting = false;
    //}

    /// <summary>
    /// Updates the PokeInteractable Button that is overlayed on the <see cref="DialogUIElement"/> to scale with the UI element.
    /// </summary>
    /// <returns>Waits for 1 second before starting itself again</returns>
    private IEnumerator DelayedPokeInteractableBtnUpdater()
    {
        pokeInteractableBtn.localScale = new Vector3(textField.rectTransform.sizeDelta.x, textField.rectTransform.sizeDelta.y, 1);
        yield return new WaitForSeconds(1f);
        StartCoroutine(DelayedPokeInteractableBtnUpdater());
    }

    /// <summary>
    /// Highlights the dialog choice on the Dialog screen by expanding its size and descaling the other choices + fades the background on the descaled choices.
    /// </summary>
    [Button]
    public void HighlightDialogChoice()
    {
        textField.rectTransform.DOSizeDelta(new Vector2(textField.rectTransform.sizeDelta.x, maxSizeY), 0.5f);
        BG.DOFade(1f, 0.5f);
        foreach (var dialogChoice in dialogUIManager.dialogUIPrefabs)
        {
            if (dialogChoice.TryGetComponent(out DialogUIElement dialogElement))
            {
                if (dialogElement.ChoiceID != ChoiceID)
                {
                    dialogElement.Dehighlight();
                }
            }
        }
    }

    /// <summary>
    /// Dehighlights this instance used in <see cref="HighlightDialogChoice"/>.
    /// </summary>
    public void Dehighlight()
    {
        textField.rectTransform.DOSizeDelta(new Vector2(textField.rectTransform.sizeDelta.x, minSizeY), 0.5f);
        BG.DOFade(0.2f, 0.5f);
    }

    /// <summary>
    /// Plays the timeline step.
    /// </summary>
    /// <param name="timelineID">The timeline identifier.</param>
    public async void PlayTimelineStep(string timelineID)
    {
        if(dialogUIManager != null)
        {
            dialogUIManager.ClearDialogOptions();
        }


        if(timelineID != "")
        {

            if(manager.timelineAssetPool.Exists(x => x.name == TimelineAssetID))
            {
                manager.director.Play(manager.timelineAssetPool.Find(x => x.name == TimelineAssetID));
                Debug.Log("New Timeline: " + timelineID + " Playing timelineAsset from pool = " + TimelineAssetID);
            }
            else
            {
                Debug.Log("New Timeline: "+ timelineID + " No TimelineAsset Found defaulting to Idle..");
            }
            await Task.Delay(100);
            manager.LoadTimelineChoices(timelineID);
        }
    }


}
