﻿#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace TheEditorToolboxProject
{
    /*
     * Small script for adding QOL buttons on the transform component
     * 
     * 
     * 
     * Modified by Simon Tysland
     * 
     * Originally written by Peter Schraut
     *     http://www.console-dev.de
     *     (source) https://bitbucket.org/snippets/pschraut/
     *
     * Save this file as
     *     Assets/Editor/TransformEditor.cs
     */

    [CanEditMultipleObjects]
    [CustomEditor(typeof(Transform))]
    class TransformEditor : Editor
    {
        SerializedProperty m_LocalPosition;
        SerializedProperty m_LocalRotation;
        SerializedProperty m_LocalScale;
        object m_TransformRotationGUI;

        bool lockedScale;

        [SerializeField]
        private Vector3 lastValidatedScale;

        [SerializeField]
        private Vector3 localScale;

        /// <summary>
        /// This is kinda broken when dragging the scaling handle - dunno why - can only drag the first axis (x)
        /// For some reason is m_localscale not applied as fast as the update...
        /// </summary>
        private void CheckForLockedScale()
        {
            if (lockedScale)
            {
                serializedObject.Update();

                localScale = serializedObject.FindProperty("m_LocalScale").vector3Value;

                if (localScale != lastValidatedScale)
                    Debug.Log("\nLocalScale: " + m_LocalScale.vector3Value + " \nLastValid: " + lastValidatedScale);
                
                
                if (lastValidatedScale.x != localScale.x)
                {
                    Debug.Log("changing x");

                    localScale = new Vector3(localScale.x, localScale.x, localScale.x);

                    lastValidatedScale = new Vector3(localScale.x, localScale.x, localScale.x);

                    serializedObject.FindProperty("m_LocalScale").vector3Value = localScale;

                    serializedObject.ApplyModifiedProperties();
                    Debug.Log("x: " + (localScale == lastValidatedScale));
                    return;
                }


                if (lastValidatedScale.y != localScale.y)
                {
                    Debug.Log("Changing Y: \nLocalScale: " + localScale + " \nLastValid: " + lastValidatedScale);

                    localScale = new Vector3(localScale.y, localScale.y, localScale.y);
                    Debug.Log("changing y");
                    lastValidatedScale = new Vector3(localScale.y, localScale.y, localScale.y);

                    serializedObject.FindProperty("m_LocalScale").vector3Value = localScale;


                    serializedObject.ApplyModifiedProperties();

                    Debug.Log("changed Y: \nLocalScale: " + localScale + " \nLastValid: " + lastValidatedScale);
                    return;
                }

                if (lastValidatedScale.z != m_LocalScale.vector3Value.z)
                {
                    m_LocalScale.vector3Value = new Vector3(m_LocalScale.vector3Value.z, m_LocalScale.vector3Value.z, m_LocalScale.vector3Value.z);
                    Debug.Log("changing z");
                    lastValidatedScale = m_LocalScale.vector3Value;

                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        private void OnSceneGUI()
        {
            //CheckForLockedScale();
        }

        void OnEnable()
        {
            m_LocalPosition = serializedObject.FindProperty("m_LocalPosition");
            m_LocalRotation = serializedObject.FindProperty("m_LocalRotation");
            m_LocalScale = serializedObject.FindProperty("m_LocalScale");

            if (m_TransformRotationGUI == null)
            {
                m_TransformRotationGUI = System.Activator.CreateInstance(typeof(SerializedProperty).Assembly.GetType("UnityEditor.TransformRotationGUI", false, false));
            }

            m_TransformRotationGUI.GetType().GetMethod("OnEnable")?.Invoke(m_TransformRotationGUI, new object[] { m_LocalRotation, new GUIContent("Rotation") });

            EditorApplication.update += CheckForLockedScale;
        }

        private void OnDisable()
        {
            EditorApplication.update -= CheckForLockedScale;
        }

        public override void OnInspectorGUI()
        {
            SerializedObject serObj = serializedObject;


            serObj.Update();

            //CheckForLockedScale();

            DrawLocalPosition();
            DrawLocalRotation();
            DrawLocalScale();

            DrawPropertiesExcluding(serObj, "m_LocalPosition", "m_LocalRotation", "m_LocalScale");

            Verify();

            serObj.ApplyModifiedProperties();
        }

        void DrawLocalPosition()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PropertyField(m_LocalPosition, new GUIContent("Position"));

                if (GUILayout.Button(new GUIContent("C", "Set parent position to center of children"), EditorStyles.miniButton, GUILayout.Width(21)))
                {
                    CenterParentToChildren();
                }

                if (GUILayout.Button(new GUIContent("P", "Reset Position"), EditorStyles.miniButton, GUILayout.Width(21)))
                {
                    m_LocalPosition.vector3Value = Vector3.zero;
                }
            }
        }

        void DrawLocalRotation()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                m_TransformRotationGUI.GetType().GetMethod("RotationField", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { typeof(bool) }, null)
                                      .Invoke(m_TransformRotationGUI, new object[] { false });

                GUILayout.Space(22);

                if (GUILayout.Button(new GUIContent("R", "Reset Rotation"), EditorStyles.miniButton, GUILayout.Width(21)))
                    m_LocalRotation.quaternionValue = Quaternion.identity;
            }
        }

        void DrawLocalScale()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PropertyField(m_LocalScale, new GUIContent("Scale"));

                //GUILayout.Space(22);
                //GUI.color = Color.white;
                //if (GUILayout.Button(new GUIContent("L", "Lock Scale"), EditorStyles.miniButton, GUILayout.Width(21)))
                //  lockedScale = !lockedScale;

                lockedScale = GUILayout.Toggle(lockedScale, new GUIContent("L", "Lock Aspect"), EditorStyles.miniButton, GUILayout.Width(22));

                if (GUILayout.Button(new GUIContent("S", "Reset Scale"), EditorStyles.miniButton, GUILayout.Width(21)))
                    m_LocalScale.vector3Value = Vector3.one;
            }
        }

        void Verify()
        {
            var transform = target as Transform;
            var position = transform.position;
            if (Mathf.Abs(position.x) > 100000f || Mathf.Abs(position.y) > 100000f || Mathf.Abs(position.z) > 100000f)
                EditorGUILayout.HelpBox("Due to floating-point precision limitations, it is recommended to bring the world coordinates of the GameObject within a smaller range.", MessageType.Warning);
        }

        private void CenterParentToChildren()
        {
            foreach (Object targetObject in targets)
            {
                if (targetObject == null)
                {
                    continue;
                }

                var thisTransform = targetObject as Transform;
                if (thisTransform == null || thisTransform.childCount < 1)
                {
                    continue;
                }

                SerializedObject[] children = new SerializedObject[thisTransform.childCount];

                for (int i = 0; i < thisTransform.childCount; i++)
                {
                    children[i] = new SerializedObject(thisTransform.GetChild(i));
                }

                Vector3 average = Vector3.zero;

                for (int i = 0; i < children.Length; i++)
                {
                    Vector3 pos = children[i].FindProperty("m_LocalPosition").vector3Value;
                    average += pos;
                }

                average /= children.Length;
                var thisTransformObject = new SerializedObject(thisTransform);
                thisTransformObject.FindProperty("m_LocalPosition").vector3Value += average;
                thisTransformObject.ApplyModifiedProperties();

                for (int i = 0; i < children.Length; i++)
                {
                    children[i].FindProperty("m_LocalPosition").vector3Value = (average - children[i].FindProperty("m_LocalPosition").vector3Value) * -1;
                    children[i].ApplyModifiedProperties();
                }
            }
        }
    }
}
#endif