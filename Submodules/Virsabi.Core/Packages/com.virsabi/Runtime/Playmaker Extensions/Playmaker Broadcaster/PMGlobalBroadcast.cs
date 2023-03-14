using UnityEngine;
using HutongGames.PlayMaker;
using MyBox;

public class PMGlobalBroadcast : MonoBehaviour
{
    [SerializeField] [PMEvent(true)] private string pmEvent;

    [ButtonMethod]
    public void Broadcast()
    {
        PlayMakerFSM.BroadcastEvent(pmEvent);
    }
}
