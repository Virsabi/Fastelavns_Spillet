using UnityEngine;
using MyBox;

public class PMLocalBroadcast : MonoBehaviour
{
    [SerializeField] [PMEvent(false)] private string pmEvent;
    [SerializeField] private PlayMakerFSM _playMakerFsm;
    
    [ButtonMethod]
    public void Broadcast()
    {
        if (!_playMakerFsm)
            Debug.LogWarning("no FSM", this);

        _playMakerFsm.SendEvent(pmEvent);
    }
}
