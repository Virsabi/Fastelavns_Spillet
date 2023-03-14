using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Virsabi;

public class HelpExampleScript : MonoBehaviour
{
#pragma warning disable 0414
    [SerializeField] [Help("This help message is always visible. Can't get rid of me. Toggle help messages from the toolbar \"Virsabi Tools / Inspector / Toggle Help\" or by pressing CTRL + H", true)]
    private string alwaysVisible;
    
    [SerializeField] [Help("This is a test message. Toggle help messages from the toolbar \"Virsabi Tools / Inspector / Toggle Help\" or by pressing CTRL + H")]
    private int integerField;
    
    [SerializeField]
    private int integerField2;
    
    [SerializeField, Help("This is an error message. Look out for these!", MessageType.Error)]
    private bool boolField;

    [SerializeField, Help("Don't touch this string! I'm warning you!", MessageType.Warning)]
    private string str = "I'm sensitive to touch";
#pragma warning restore 0414
}
