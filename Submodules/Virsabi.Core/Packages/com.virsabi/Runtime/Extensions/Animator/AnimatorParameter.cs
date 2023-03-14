using System;
using System.Linq.Expressions;
using UnityEngine;

/// <summary>
/// This script allows serilization of any exposed floats in the attached shader for easier reference and controll.
/// </summary>
namespace Virsabi
{
	[Serializable]
	public class AnimatorParameter
	{

#pragma warning disable 0649
		[SerializeField] private string _animParameterName = String.Empty;
		[SerializeField] private AnimatorControllerParameterType _animParamType;
		[SerializeField] private bool _autoMap;
		[SerializeField] private bool _assigned;
		[SerializeField] private Animator _linkedAnimator;
#pragma warning restore 0649

		public string AnimParameterName
		{
			get { return _animParameterName; }
		}
		public bool Assigned
		{
			get { return _assigned; }
		}

		public Animator Animator
		{
			get { return _linkedAnimator; }
		}

		public bool AutoMap
		{
			get { return _autoMap; }
		}
			   
		public AnimatorControllerParameterType AnimParamType { get => _animParamType; }

		public void Trigger()
		{
			//I cannot figure out how to correctly serialize this enum. 
			if (AnimParamType != AnimatorControllerParameterType.Trigger)
			{
				//Debug.LogWarning("Trying to set trigger on non-trigger animation parameter. Type is: " + AnimParamType.ToString());
			}
				
			_linkedAnimator.SetTrigger(_animParameterName);
		}

		public void ResetTrigger()
		{
			if (AnimParamType != AnimatorControllerParameterType.Trigger)
			{
				//Debug.LogError("Trying to reset trigger on non-trigger animation parameter");
				//return;
			}
			_linkedAnimator.ResetTrigger(_animParameterName);
		}

		public void SetBool(bool status) => _linkedAnimator.SetBool(_animParameterName, status);

		public bool GetBool() => _linkedAnimator.GetBool(_animParameterName);

		public void SetInt(int value) => _linkedAnimator.SetInteger(_animParameterName, value);

		public int GetInt() => _linkedAnimator.GetInteger(_animParameterName);

		public void SetFloat(float value) => _linkedAnimator.SetFloat(_animParameterName, value);

		public float GetFloat() => _linkedAnimator.GetFloat(_animParameterName);
	}
}


#if UNITY_EDITOR

namespace Virsabi.Editor
{
	using MyBox;
	using MyBox.EditorTools;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEditor.Animations;
	using UnityEngine;
	using Virsabi.Extensions;

	[CustomPropertyDrawer(typeof(AnimatorParameter)), RequireComponent(typeof(Animator))]
	public class AnimatorParameterDrawer : PropertyDrawer
	{
		private SerializedProperty _animParameterName;
		private SerializedProperty _animParameterType;
		private SerializedProperty _assigned;
		[SerializeField]
		private SerializedProperty _autoMap;
		private SerializedProperty _linkedAnimator;

		private readonly List<string> _defaultState = new string[] { "Not Assigned" }.ToList();
		private readonly List<AnimatorControllerParameterType> _defaultType = new AnimatorControllerParameterType[] { AnimatorControllerParameterType.Bool }.ToList();

		private List<string> _parameters = new List<string>();
		private List<AnimatorControllerParameterType> _parameterTypes = new List<AnimatorControllerParameterType>();

		#region Gui
		private int verticalFields = 2;
		private float fieldSize = 16;
		private float padding = 2;
		#endregion



		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			
			//set the height of the drawer by the field size and padding
			return (fieldSize * verticalFields) + (padding * verticalFields);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.GetParent() == null)
				return;

			if ((property.GetParent() as MonoBehaviour).transform.IsPrefab()) //dont run if this is a prefab instance not in a scene
			{
				EditorGUI.BeginProperty(position, label, property);

				EditorGUI.LabelField(position, "Cannot edit animation parameters on prefabs");

				EditorGUI.EndProperty();
				return;
			}

			Initialize(property);

			EditorGUI.BeginProperty(position, label, property);

			Rect objRect = position;
			//objRect.width = (position.width - EditorGUIUtility.labelWidth - padding) / 2;
			//objRect.x = position.x + EditorGUIUtility.labelWidth + padding;
			objRect.height = position.height / verticalFields;

			Rect dropdownRect = position;
			dropdownRect.width = (position.width - EditorGUIUtility.labelWidth - padding) / 1.5f;
			dropdownRect.x = position.x + EditorGUIUtility.labelWidth + padding;
			dropdownRect.height = position.height / verticalFields;
			dropdownRect.y = position.y + fieldSize + padding * 2;

			Rect ToggleRect = position;
			ToggleRect.width = position.width / 2;
			ToggleRect.height = position.height / verticalFields;
			ToggleRect.x = dropdownRect.x + dropdownRect.width + 10;
			ToggleRect.y = dropdownRect.y;
			


			bool autoMapPossible = _parameters.Exists(s => s.Equals(property.name, StringComparison.InvariantCultureIgnoreCase));



			EditorGUI.BeginChangeCheck();


			EditorGUI.BeginDisabledGroup(!autoMapPossible);
			_autoMap.boolValue = EditorGUI.ToggleLeft(ToggleRect, new GUIContent("AutoMap", "Use the variable name to set parameter"), _autoMap.boolValue);
			EditorGUI.EndDisabledGroup();

			int index;

			if (!autoMapPossible)
				_autoMap.boolValue = false;
	
			if (autoMapPossible && _autoMap.boolValue)
			{
				index = GetIndexOf(_parameters.Find(x => x.Equals(property.name, StringComparison.InvariantCultureIgnoreCase)));
			}
			else
			{
				index = CurrentIndex();
			}

			if (_linkedAnimator.objectReferenceValue == null)
				index = 0;

			int state = 0;

			if (_parameters.Count < 1)
			{
				_parameters = _defaultState;
				_parameterTypes = new AnimatorControllerParameterType[] { AnimatorControllerParameterType.Bool }.ToList();
				index = 0;
				_animParameterName.stringValue = _parameters[state];
				_assigned.boolValue = state > 0;
				//Debug.Log("count 0");
			}

			EditorGUI.ObjectField(objRect, _linkedAnimator, label);

			EditorGUI.BeginDisabledGroup(_autoMap.boolValue && autoMapPossible);
			state = EditorGUI.Popup(dropdownRect, new GUIContent("", "Anim Parameter"), index, _parameters.Select(s => new GUIContent(s + " (" + _parameterTypes[_parameters.IndexOf(s)] + ")")).ToArray());
			EditorGUI.EndDisabledGroup();

			_animParameterName.stringValue = _parameters[state];
			_assigned.boolValue = state > 0;

			

			

			if (EditorGUI.EndChangeCheck())
			{
				UpdateParameters(property);
			}

			_assigned.serializedObject.Update();

			_animParameterName.serializedObject.ApplyModifiedProperties();
			_assigned.serializedObject.ApplyModifiedProperties();
			//_autoMap.serializedObject.ApplyModifiedProperties();
			
			EditorGUI.EndProperty();
		}

		//Runs every gui update
		private void Initialize(SerializedProperty property)
		{


			if (_animParameterName == null) _animParameterName = property.FindPropertyRelative("_animParameterName");
			if (_animParameterType == null) _animParameterType = property.FindPropertyRelative("_animParamType");
			if (_assigned == null)			_assigned = property.FindPropertyRelative("_assigned");
			if (_autoMap == null)			_autoMap = property.FindPropertyRelative("_autoMap");
			if (_linkedAnimator == null)	_linkedAnimator = property.FindPropertyRelative("_linkedAnimator");

			//if not assigned, try to get component from child
			if (_linkedAnimator.objectReferenceValue == null)
			{
				var mb = property.GetParent() as MonoBehaviour;
				if (mb != null)
				{
					Animator anim = null;

					if (mb.GetComponentInChildren<Animator>(true) != null)
						anim = mb.GetComponentInChildren<Animator>(true);
						

					if (anim != null)
					{
						//WHAT THE FUCK UNITY - WHY IS THIS HACK NEEDED IN ORDER TO UPDATE THE PARAMETERS OF AN ANIMATOR!
						anim.enabled = false;
						anim.enabled = true;

						_linkedAnimator.objectReferenceValue = anim;
						_linkedAnimator.serializedObject.ApplyModifiedProperties();
					}
				}
			}



			if (_linkedAnimator.objectReferenceValue != null)
				CheckIfAnimatorParametersChanged(property);
			else
				UpdateParameters(property);
		}

		private void CheckIfAnimatorParametersChanged(SerializedProperty property)
		{
			Animator anim = _linkedAnimator.objectReferenceValue as Animator;

			if (!anim.runtimeAnimatorController)
				return;

			List<string> paramStrings = new List<string>(anim.parameters.Select(x => x.name).ToList());

			UpdateParameters(property);

			if (paramStrings == _parameters)
				return;
			//Reloads the animator controller into the component - otherwise parameters wont be updated
			bool currentStatus = anim.enabled;
			anim.enabled = !currentStatus;
			anim.enabled = currentStatus;


			_linkedAnimator.objectReferenceValue = anim;
		}

		/// <summary>
		/// Update properties
		/// </summary>
		private void UpdateParameters(SerializedProperty property)
		{
			_parameters.Clear();
			_parameters = new string[] { "Not Assigned" }.ToList();
			_parameterTypes = _defaultType;

			if (_linkedAnimator.objectReferenceValue == null)
			{
				return;
			}

			Animator animator = (Animator)_linkedAnimator.objectReferenceValue;

			if (!animator.runtimeAnimatorController)
			{
				Debug.Log("No AnimatorController on Animator Component.");
				_parameters = new string[] { "Missing AnimatorController" }.ToList();
				return;
			}

			//if the animator doesn't have any parameters
			if(animator.parameters.Length != 0)
			{
				_parameters.Clear();
				_parameterTypes.Clear();
			}

			//Add the animator parameters to list of available parameters
			foreach (AnimatorControllerParameter param in animator.parameters)
			{
				_parameters.Add(param.name);
				_parameterTypes.Add(param.type);
			}

			property.serializedObject.ApplyModifiedProperties();
		}

		private int CurrentIndex()
		{
			if (_parameters == null || _animParameterName == null || _parameters.Count < 1)
				return 0;

			//Debug.Log("index: " + _parameters.IndexOfItem(_animParameterName.stringValue));

			var index = _parameters.IndexOfItem(_animParameterName.stringValue);
			if (index < 0) index = 0;
			return index;
		}

		private int GetIndexOf(string animParameterName)
		{
			if (_parameters == null || animParameterName == null || _parameters.Count < 1)
				return 0;

			//Debug.Log("index: " + _parameters.IndexOfItem(_animParameterName.stringValue));

			var index = _parameters.IndexOfItem(animParameterName);
			if (index < 0) index = 0;
			return index;
		}
	}
}
#endif

