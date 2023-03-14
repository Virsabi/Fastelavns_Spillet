using System;
using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Loads from FSM events
/// </summary>
/// <param name="isGlobal">isGlobal</param>
public class PMEventAttribute : PropertyAttribute
{
    public bool UseDefaultTagFieldDrawer;
    public bool IsGlobal; 
    
    /// <summary>
    /// Loads from FSM events
    /// </summary>
    /// <param name="isGlobal">isGlobal</param>
    public PMEventAttribute(bool isGlobal)
    {
        IsGlobal = isGlobal;
    }
    
    public PMEventAttribute()
    {
        
    }
}