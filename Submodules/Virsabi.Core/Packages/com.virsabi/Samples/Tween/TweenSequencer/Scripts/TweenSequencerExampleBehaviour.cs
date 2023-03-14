using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Virsabi;
using Virsabi.Tween;

public class TweenSequencerExampleBehaviour : MonoBehaviour
{
    [Help("Press the buttons to tween between the points on the animation curve. There is an issue at " + 
          "the moment when tweening in one direction and then changing the target index in the opposite direction.")]
    public Material material;
    private TweenSequencer _tweenSequencer;
    private static readonly int StepValue = Shader.PropertyToID("_StepVal");

    private void Awake()
    {
        _tweenSequencer = GetComponent<TweenSequencer>();
    }

    public void SetStepValue(float t)
    {
    material.SetFloat(StepValue, t);
    }

    private void OnGUI()
    {
        GUILayout.Box("Output value: " + _tweenSequencer.T);
        GUILayout.Box("Target index: " + _tweenSequencer.GetTargetIndex());

        if (GUILayout.Button("Previous Tween Point"))
            _tweenSequencer.TweenToPreviousPoint();
        
        if (GUILayout.Button("Next Tween Point"))
            _tweenSequencer.TweenToNextPoint();
    }
}
