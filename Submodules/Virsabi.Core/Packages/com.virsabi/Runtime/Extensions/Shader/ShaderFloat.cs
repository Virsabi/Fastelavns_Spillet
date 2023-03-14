using UnityEngine;
using System;

/// <summary>
/// This script allows serilization of any exposed floats in the attached shader for easier reference and controll.
/// </summary>
namespace Virsabi
{
    [Serializable]
    public class ShaderFloat
    {

        public string FloatName
        {
            get { return _floatName; }
        }
		public bool Assigned
		{
			get { return _assigned; }
		}

		public Material Material
		{
			get { return _linkedMaterial; }
		}

#pragma warning disable 0649
		[SerializeField] private string _floatName = String.Empty;
		[SerializeField] private bool _assigned;
		[SerializeField] private Material _linkedMaterial;
#pragma warning restore 0649


	}
}


#if UNITY_EDITOR

namespace Virsabi.Editor
{
	using MyBox.EditorTools;
	using MyBox;
	using UnityEditor;
	using UnityEditor.Animations;
	using UnityEngine;
	using UnityEngine.UI;
    using System.Linq;
	using System.Collections;

	using System.Collections.Generic;

	[CustomPropertyDrawer(typeof(ShaderFloat))]
	public class ShaderFloatDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Initialize(property);

			EditorGUI.BeginProperty(position, label, property);

			var widthWithoutRefresh = position.width - 34;

			var stateRect = position;
			stateRect.width = widthWithoutRefresh / 4 * 3;

			var animatorRect = position;
			animatorRect.width = widthWithoutRefresh / 4;
			animatorRect.x += stateRect.width + 4;

			var refreshRect = animatorRect;
			refreshRect.width = 26;
			refreshRect.x += animatorRect.width + 4;


			var state = EditorGUI.Popup(stateRect, label, CurrentIndex(), _floats.Select(s => new GUIContent(s)).ToArray());
			_floatName.stringValue = _floats[state];
			_assigned.boolValue = state > 0;

			EditorGUI.BeginChangeCheck();
			EditorGUI.ObjectField(animatorRect, _floatProperty, GUIContent.none);
			if (EditorGUI.EndChangeCheck()) UpdateFloats();

			if (GUI.Button(refreshRect, "↺")) UpdateFloats();

			EditorGUI.EndProperty();
		}

		private void Initialize(SerializedProperty property)
		{
			if (_shaderNotFound == null) _shaderNotFound = new GUIContent("Shader not found");
			if (_floatName == null) _floatName = property.FindPropertyRelative("_floatName");
			if (_assigned == null) _assigned = property.FindPropertyRelative("_assigned");

			if (_floatProperty == null)
			{
				_floatProperty = property.FindPropertyRelative("_linkedMaterial");
				if (_floatProperty.objectReferenceValue == null)
				{
					var mb = property.GetParent() as MonoBehaviour;
					if (mb != null)
					{
						Material material = null;

						if (mb.GetComponentInChildren<Image>(true) != null)
							material = mb.GetComponentInChildren<Image>(true).material;

						if (mb.GetComponentInChildren<Renderer>(true) != null)
							material = mb.GetComponentInChildren<Renderer>(true).material;

						if (material != null)
						{
							_floatProperty.objectReferenceValue = material;
							_floatProperty.serializedObject.ApplyModifiedProperties();
						}
					}
				}

				UpdateFloats();
			}
		}

		private void UpdateFloats()
		{
			_floats = _defaultState;
			if (_floatProperty.objectReferenceValue == null) return;
			var material = (Material)_floatProperty.objectReferenceValue;

			int propertyCount = ShaderUtil.GetPropertyCount(material.shader);

			List<string> onlyFloats = new List<string>();

			for (int i = 0; i < propertyCount; i++)
			{
				if (ShaderUtil.GetPropertyType(material.shader, i).Equals(ShaderUtil.ShaderPropertyType.Float))
					onlyFloats.Add(ShaderUtil.GetPropertyName(material.shader, i));
			}

			_floats = onlyFloats.ToArray();
		}

		private int CurrentIndex()
		{
			var index = _floats.IndexOfItem(_floatName.stringValue);
			if (index < 0) index = 0;
			return index;
		}

		private SerializedProperty _floatName;
		private SerializedProperty _assigned;
		private SerializedProperty _floatProperty;
		private GUIContent _shaderNotFound;

		private readonly string[] _defaultState = { "Not Assigned" };
		private string[] _floats = new string[1];
	}
}


#endif


