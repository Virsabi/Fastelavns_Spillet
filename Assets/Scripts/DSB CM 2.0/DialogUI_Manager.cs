using EasyButtons;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

/// <summary>
///   <para>
///     <strong>Author:</strong> Steffen Hansen
///     </para>
///   <para>
///     <strong>Description:</strong> A small script that manages the Dialog options on the Dialog Screen. 
///     </para>
/// </summary>

public class DialogUI_Manager : MonoBehaviour
{
    #region References
    [Header("References")]
    public Timeline_Manager manager;

    [SerializeField]
    private GameObject dialogElement_Choice_UI_Prefab;
    [SerializeField]
    private GameObject dialogElement_Instruction_UI_Prefab;

    [SerializeField]
    private Transform dialogUIHolder;
    #endregion

     [ReadOnly]
    public List<GameObject> dialogUIPrefabs = new List<GameObject>();

    #region Unity Methods
    private void Start()
    {
        manager = FindObjectOfType<Timeline_Manager>();
    }
    #endregion

    /// <summary>
    /// Clears the dialog options from the <see cref="dialogUIPrefabs"/> list.
    /// </summary>
    [Button]
    public void ClearDialogOptions()
    {
        if (dialogUIPrefabs.Count > 0)
        {
            dialogUIPrefabs.ForEach(x => Destroy(x));
            dialogUIPrefabs.Clear();
        }
    }

    /// <summary>
    /// Loads the dialog options to the list and places them as children under <see cref="dialogUIHolder"/>.
    /// </summary>
    /// <param name="dialog">The dialog.</param>
    public void LoadDialogOptions(Timeline_Manager.DialogChoice dialog)
    {
        if(dialog.type == Timeline_Manager.DialogType.Choice)
        {
            var dialogObj = Instantiate(dialogElement_Choice_UI_Prefab, dialogUIHolder);
            dialogObj.GetComponent<DialogUIElement>().DialogSetup(dialog);
            dialogUIPrefabs.Add(dialogObj);
        }
        else if(dialog.type == Timeline_Manager.DialogType.Dialog)
        {
            var dialogObj = Instantiate(dialogElement_Choice_UI_Prefab, dialogUIHolder);
            dialogObj.GetComponent<DialogUIElement>().DialogSetup(dialog);
            dialogUIPrefabs.Add(dialogObj);
        }
        else if(dialog.type == Timeline_Manager.DialogType.Instruction)
        {
            var dialogObj = Instantiate(dialogElement_Instruction_UI_Prefab, dialogUIHolder);
            dialogObj.GetComponent<DialogUIElement>().DialogSetup(dialog);
            dialogUIPrefabs.Add(dialogObj);
        }


    }
}
