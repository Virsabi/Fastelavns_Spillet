using UnityEngine;

public class FSMBroadcast : MonoBehaviour
{
    [PMEvent(true)]
    public string pmEvent;

    [ContextMenu("Test Broadcast")]
    public void BroadCastEvent()
    {
        PlayMakerFSM.BroadcastEvent(pmEvent);
    }
}
