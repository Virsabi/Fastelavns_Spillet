using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timeline_Initializor : MonoBehaviour
{
    Timeline_Manager manager;
    [SerializeField] string initialTimelineName;
    [SerializeField] string firstTimelineToPlay;
    // Start is called before the first frame update
    void Start()
    {
        manager = this.GetComponent<Timeline_Manager>();
        manager.dialogUIManager.ClearDialogOptions();
        manager.LoadTimelineChoices(initialTimelineName);
        if (firstTimelineToPlay != "")
        {
           manager.director.Play(manager.timelineAssetPool.Find(x => x.name == firstTimelineToPlay));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
