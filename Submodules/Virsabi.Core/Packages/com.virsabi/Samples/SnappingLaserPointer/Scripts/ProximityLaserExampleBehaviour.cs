using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Virsabi;

public class ProximityLaserExampleBehaviour : MonoBehaviour
{
    [FormerlySerializedAs("__proximityLaserPointer")] [SerializeField] private SnappingLaserPointer snappingLaserPointer;
    
    private float _rotationIncrement;
    private float _incrementCosine;
    private bool _pressed;
    
    // Update is called once per frame
    void Update()
    {
        _incrementCosine += 0.7f * Time.deltaTime;
        _rotationIncrement += Mathf.Max(0,Mathf.Cos(_incrementCosine)) * 2 * Time.deltaTime;
        transform.rotation = Quaternion.AngleAxis(Mathf.Sin(_rotationIncrement) * 45.0f, transform.up);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 20);
    }

    private void OnGUI()
    {
        GUILayout.Box("Tip: Turn on Gizmos to see more clearly what is going on.");
    }
}
