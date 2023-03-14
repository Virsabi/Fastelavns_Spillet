using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Virsabi.Tween;

public class TweenLinearExampleBehaviour : MonoBehaviour
{
    private LinearTweenToTarget _linearTweenToTarget;

    private void Awake()
    {
        _linearTweenToTarget = GetComponent<LinearTweenToTarget>();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Start Tween"))
        {
            _linearTweenToTarget.StartTween();
        }
    }
}
