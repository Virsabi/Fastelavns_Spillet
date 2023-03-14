using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inherit from this class to get instance unique icons for your Scriptable Objects in the Project view - Icons are rendered in GizmoIconUtility
/// </summary>
public abstract class IconScriptableObject : ScriptableObject
{
    [SerializeField, HideInInspector]
    public int guid;

    [SerializeField]
    public Sprite icon;

    private void OnValidate()
    {
        guid = this.GetInstanceID();
    }
}
