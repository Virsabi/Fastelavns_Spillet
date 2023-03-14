#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Virsabi.Internal;

namespace Virsabi.Attributes
{
    public class HelpAttributeEditor : MonoBehaviour
    {
        
        
        public class HelpAttributeScript : Editor
        {

            [MenuItem("Virsabi Tools/Inspector/Toggle Help %h")]
            public static void ToggleHelpAttribute() {
                VirsabiSettings.ShowHelp = !VirsabiSettings.ShowHelp;
                HelpDrawer.InspectorHelpers.RepaintAllInspectors();
                
            }
        }

        [CustomPropertyDrawer(typeof(HelpAttribute))]
        public class HelpDrawer : PropertyDrawer
        {
            // Used for top and bottom padding between the text and the HelpBox border.
            const int paddingHeight = 8;

            // Used to add some margin between the the HelpBox and the property.
            const int marginHeight = 2;

            //  Global field to store the original (base) property height.
            float baseHeight = 0;

            // Custom added height for drawing text area which has the MultilineAttribute.
            float addedHeight = 0;

            /// <summary>
            /// A wrapper which returns the PropertyDrawer.attribute field as a HelpAttribute.
            /// </summary>
            HelpAttribute helpAttribute { get { return (HelpAttribute)attribute; } }

            /// <summary>
            /// A helper property to check for RangeAttribute.
            /// </summary>
            RangeAttribute rangeAttribute
            {
                get
                {
                    var attributes = fieldInfo.GetCustomAttributes(typeof(RangeAttribute), true);
                    return attributes != null && attributes.Length > 0 ? (RangeAttribute)attributes[0] : null;
                }
            }

            /// <summary>
            /// A helper property to check for MultiLineAttribute.
            /// </summary>
            MultilineAttribute multilineAttribute
            {
                get
                {
                    var attributes = fieldInfo.GetCustomAttributes(typeof(MultilineAttribute), true);
                    return attributes != null && attributes.Length > 0 ? (MultilineAttribute)attributes[0] : null;
                }
            }

            public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
            {
                if (!VirsabiSettings.ShowHelp && !helpAttribute.alwaysVisible) return base.GetPropertyHeight(prop, label);

                // We store the original property height for later use...
                baseHeight = base.GetPropertyHeight(prop, label);

                // This stops icon shrinking if text content doesn't fill out the container enough.
                float minHeight = paddingHeight * 5;

                // Calculate the height of the HelpBox using the GUIStyle on the current skin and the inspector
                // window's currentViewWidth.
                var content = new GUIContent(helpAttribute.text);
                var style = GUI.skin.GetStyle("helpbox");

                var height = style.CalcHeight(content, EditorGUIUtility.currentViewWidth);

                // We add tiny padding here to make sure the text is not overflowing the HelpBox from the top
                // and bottom.
                height += marginHeight * 2;

                // Since we draw a custom text area with the label above if our property contains the
                // MultilineAttribute, we need to add some extra height to compensate. This is stored in a
                // seperate global field so we can use it again later.
                if (multilineAttribute != null && prop.propertyType == SerializedPropertyType.String)
                {
                    addedHeight = 48f;
                }

                // If the calculated HelpBox is less than our minimum height we use this to calculate the returned
                // height instead.
                return height > minHeight ? height + baseHeight + addedHeight : minHeight + baseHeight + addedHeight;

            }
            string directory = null;
            string _bubbleTexturePath = null;
            string _warningTexturePath = null;
            string _errorTexturePath = null;
            string _bgTexturePath = null;

            public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label) {
                
                
;

                
                if (!VirsabiSettings.ShowHelp && !helpAttribute.alwaysVisible)
                {

                    if (directory == null) {
                        var stacktraceResult = new System.Diagnostics.StackTrace(true).GetFrame(0).GetFileName();
                        //Debug.Log("stacktraceResult: \n" + stacktraceResult);
                        directory = stacktraceResult.Substring(0,
                            stacktraceResult.Length - "HelpAttributeEditor.cs".Length);
                    }
                    //Debug.Log("directory: \n" + (directory + "Textures/Bubble.png"));
                    
                    //These things doesn't work because you are looking in th eassets folder with the file utility. I'm giving up fixing it at this moment - we need a coherent way of looking up files.

                    if (_bubbleTexturePath == null)
                        _bubbleTexturePath =
                            FileUtility.GetRelativePath(FileUtility.SanitizePath(directory + "Textures/Bubble.png"));
                    
                    //Debug.Log("asdasdasddadsd: " + _bubbleTexturePath);
                    
                    //Debug.Log("Bubble path not relative:\n" + FileUtility.SanitizePath(directory + "Textures/Bubble.png"));
                    //Debug.Log("Bubble path:\n" +_bubbleTexturePath);

                    if (_warningTexturePath == null)
                        _warningTexturePath =
                            FileUtility.GetRelativePath(FileUtility.SanitizePath(directory + "Textures/Bulb.png"));

                    if (_errorTexturePath == null)
                        _errorTexturePath =
                            FileUtility.GetRelativePath(
                                FileUtility.SanitizePath(directory + "Textures/Bulb_Error.png"));
                    
                    if (_bgTexturePath == null)
                        _bgTexturePath =
                            FileUtility.GetRelativePath(
                                FileUtility.SanitizePath(directory + "Textures/Help_BG.png"));
                    
                    EditorGUI.BeginProperty(position, label, prop);
                    EditorGUI.PropertyField(position, prop, label, true);
                    EditorGUI.EndProperty();

                    var textureRect = new Rect(new Vector2(position.xMin - 12f, position.yMax - 12), new Vector2(10f, 10f));
                    

                    switch (helpAttribute.type)
                    {
                        case MessageType.None:
                                
                            GUI.DrawTexture(textureRect, AssetDatabase.LoadAssetAtPath<Texture>(_bubbleTexturePath));
                            break;
                        case MessageType.Info:
                            GUI.DrawTexture(textureRect, AssetDatabase.LoadAssetAtPath<Texture>(_bubbleTexturePath));
                            break;
                        case MessageType.Warning:
                            GUI.DrawTexture(textureRect, AssetDatabase.LoadAssetAtPath<Texture>(_warningTexturePath));
                            break;
                        case MessageType.Error:
                            GUI.DrawTexture(textureRect, AssetDatabase.LoadAssetAtPath<Texture>(_errorTexturePath));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    if (textureRect.Contains(Event.current.mousePosition))
                    {

                        GUIStyle guiStyle = new GUIStyle();
                        guiStyle.wordWrap = true;
                        guiStyle.normal.textColor = Color.white;
                        guiStyle.normal.background = AssetDatabase.LoadAssetAtPath<Texture2D>(_bgTexturePath);
                        guiStyle.padding = new RectOffset(5, 5, 5, 5);

                        var content = new GUIContent(helpAttribute.text);
                        var calcHeight = guiStyle.CalcHeight(content, 200f);
                        GUI.contentColor = Color.white;
                        GUI.Label(new Rect(Event.current.mousePosition + new Vector2(0, -calcHeight), new Vector2(200, calcHeight)), helpAttribute.text, guiStyle);
                    }
                }
                else
                {
                    // We get a local reference to the MultilineAttribute as we use it twice in this method and it
                    // saves calling the logic twice for minimal optimization, etc...
                    var multiline = multilineAttribute;

                    EditorGUI.BeginProperty(position, label, prop);

                    // Copy the position out so we can calculate the position of our HelpBox without affecting the
                    // original position.
                    var helpPos = position;

                    helpPos.height -= baseHeight + marginHeight;


                    if (multiline != null)
                    {
                        helpPos.height -= addedHeight;
                    }

                    // Renders the HelpBox in the Unity inspector UI.
                    EditorGUI.HelpBox(helpPos, helpAttribute.text, helpAttribute.type);

                    position.y += helpPos.height + marginHeight;
                    position.height = baseHeight;


                    // If we have a RangeAttribute on our field, we need to handle the PropertyDrawer differently to
                    // keep the same style as Unity's default.
                    var range = rangeAttribute;

                    if (range != null)
                    {
                        if (prop.propertyType == SerializedPropertyType.Float)
                        {
                            EditorGUI.Slider(position, prop, range.min, range.max, label);
                        }
                        else if (prop.propertyType == SerializedPropertyType.Integer)
                        {
                            EditorGUI.IntSlider(position, prop, (int)range.min, (int)range.max, label);
                        }
                        else
                        {
                            // Not numeric so draw standard property field as punishment for adding RangeAttribute to
                            // a property which can not have a range :P
                            EditorGUI.PropertyField(position, prop, label);
                        }
                    }
                    else if (multiline != null)
                    {
                        // Here's where we handle the PropertyDrawer differently if we have a MultiLineAttribute, to try
                        // and keep some kind of multiline text area. This is not identical to Unity's default but is
                        // better than nothing...
                        if (prop.propertyType == SerializedPropertyType.String)
                        {
                            var style = GUI.skin.label;
                            var size = style.CalcHeight(label, EditorGUIUtility.currentViewWidth);

                            EditorGUI.LabelField(position, label);

                            position.y += size;
                            position.height += addedHeight - size;

                            // Fixed text dissappearing thanks to: http://answers.unity3d.com/questions/244043/textarea-does-not-work-text-dissapears-solution-is.html
                            prop.stringValue = EditorGUI.TextArea(position, prop.stringValue);
                        }
                        else
                        {
                            // Again with a MultilineAttribute on a non-text field deserves for the standard property field
                            // to be drawn as punishment :P
                            EditorGUI.PropertyField(position, prop, label);
                        }
                    }
                    else
                    {
                        // If we get to here it means we're drawing the default property field below the HelpBox. More custom
                        // and built in PropertyDrawers could be implemented to enable HelpBox but it could easily make for
                        // hefty else/if block which would need refactoring!
                        EditorGUI.PropertyField(position, prop, label);
                        EditorGUI.EndProperty();
                    }

                }
            }

            public static class InspectorHelpers
            {
                private static System.Reflection.MethodInfo m_RepaintInspectors = null;
                public static void RepaintAllInspectors()
                {
                    if (m_RepaintInspectors == null)
                    {
                        var inspWin = typeof(EditorApplication).Assembly.GetType("UnityEditor.InspectorWindow");
                        m_RepaintInspectors = inspWin.GetMethod("RepaintAllInspectors", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                    }
                    m_RepaintInspectors.Invoke(null, null);
                }
            }
        }
    }

}

#endif