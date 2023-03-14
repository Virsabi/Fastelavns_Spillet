using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
//using Unity.XR.CoreUtils;
using UnityEngine;

public class ASR_Initializer : MonoBehaviour
{
    [SerializeField]
    private GameObject ASR_Subscription_Prefab;

    // [ReadOnly]
    public GameObject ASR_Subscription_Reference;

    private bool isASR_Subscription_Handler_Present;

    private async void Start()
    {
        if(FindObjectOfType<ASR_Subscription_Handler>() != null)
        {
            isASR_Subscription_Handler_Present = true;
        }
        else
        {
            isASR_Subscription_Handler_Present = false;
        }

        await Task.Delay(500);

        if(!isASR_Subscription_Handler_Present)
        {
            GameObject subscriptionObj = Instantiate(ASR_Subscription_Prefab);
            ASR_Subscription_Reference = subscriptionObj;
            isASR_Subscription_Handler_Present = true;
        }
        else
        {
            Debug.Log("ASR_Subscription_Handler already exists.");
        }

    }
}
