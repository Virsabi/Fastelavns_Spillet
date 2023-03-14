using System;
using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PMEventAttribute))]
public class EventSelectorPropertyDrawer  : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.String)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            var attrib = this.attribute as PMEventAttribute;

            if (attrib.UseDefaultTagFieldDrawer)
            {
                property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
            }
            else
            {
                //generate the taglist + custom tags
                List<string> tagList = new List<string>();
                
                if (property.stringValue != "")
                    tagList.Add("<Current value: " + property.stringValue + ">");
                
                tagList.Add("<No Event>");
                List<FsmEvent> fsmEventList;

                if (attrib.IsGlobal)
                    fsmEventList = FsmEvent.EventList.Where<FsmEvent>((Func<FsmEvent, bool>) (p => !p.IsSystemEvent && p.IsGlobal)).ToList<FsmEvent>();
                else
                {
                    fsmEventList = FsmEvent.EventList.Where<FsmEvent>((Func<FsmEvent, bool>) (p => !p.IsSystemEvent && !p.IsGlobal)).ToList<FsmEvent>();
                }

                foreach (var e in fsmEventList)
                {
                    tagList.Add(e.Name);
                }
                
                //tagList.AddRange(FsmEvent.globalEvents);
                string propertyString = property.stringValue;
                int index = -1;
                if (propertyString == "")
                {
                    //The tag is empty
                    index = 0; //first index is the special <notag> entry
                }
                else
                {
                    //check if there is an entry that matches the entry and get the index
                    //we skip index 0 as that is a special custom case
                    for (int i = 1; i < tagList.Count; i++)
                    {
                        if (tagList[i] == propertyString)
                        {
                            index = i;
                            break;
                        }
                    }
                }
                
                //attrib.testEnum = (PMEventGlobal.TestEnum) EditorGUI.EnumPopup(position, attrib.testEnum);
                
                // Calculate new position
                //Rect newpos = position;
                //newpos.y += EditorGUIUtility.singleLineHeight + 2;
                Rect newpos = position;
                newpos.y -= EditorGUIUtility.singleLineHeight + 2;
                
                //Draw the popup box with the current selected index
                index = EditorGUI.Popup(position, label.text, index, tagList.ToArray());

                //Adjust the actual string value of the property based on the selection
                if (index == 0)
                {
                    if (tagList[0] == "")
                    {
                        property.stringValue = "";
                    }
                    else
                    {
                        property.stringValue = property.stringValue;
                        //EditorGUI.HelpBox(newpos, "Cannot locate event", MessageType.Error);
                    }
                }
                else if (index >= 1)
                {
                    property.stringValue = tagList[index];
                }
                else
                {
                    if (tagList[0] == "")
                    {
                        property.stringValue = "";
                    }
                    else
                    {
                        property.stringValue = property.stringValue;
                        //EditorGUI.HelpBox(newpos, "Cannot locate event called \"" + property.stringValue + "\". If you want to still use the event regardless ignore the message.", MessageType.Error);
                    }
                }
            }

            EditorGUI.EndProperty();
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }
}