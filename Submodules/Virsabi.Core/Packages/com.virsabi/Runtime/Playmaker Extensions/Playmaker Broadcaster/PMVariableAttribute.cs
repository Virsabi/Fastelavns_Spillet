using System;
using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Loads from global FSM variables
/// Author: mu@Virsabi.com
/// </summary>
/// <param name="isGlobal">isGlobal</param>
public class PMVariableAttribute : PropertyAttribute
{
    public bool UseDefaultTagFieldDrawer;
    public bool IsGlobal;

    /// <summary>
    /// Loads from FSM variables
    /// </summary>
    /// <param name="isGlobal">isGlobal</param>
    public PMVariableAttribute(bool isGlobal)
    {
        IsGlobal = isGlobal;
    }

    public PMVariableAttribute()
    {

    }
}