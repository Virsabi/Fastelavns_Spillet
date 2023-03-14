using UnityEngine;
using UnityEditor;
using Virsabi.Internal;
using System;

using SyntaxTree.VisualStudio.Unity.Bridge;

namespace Virsabi.Internal 
{ 
	public class VirsabiConsoleSupressor : MonoBehaviour
	{
		[InitializeOnLoad]
		public class VirsabiConsoleSupressorWindow : EditorWindow
		{
			private static EditorWindow _windowInstance;

			static VirsabiConsoleSupressorWindow()
			{
				
			}

			/* old method
			private static void RefreshSupressions()
			{
				//rebuilds the .csproj files
				SyntaxTree.VisualStudio.Unity.Bridge.ProjectFilesGenerator.GenerateProject();
				AssetDatabase.ImportAsset("Packages/com.virsabi/Editor/Virsabi.Editor.asmdef");
				AssetDatabase.Refresh();
				Debug.Log("Refreshed supressions");
			}*/

			[MenuItem("Virsabi Tools/Utilities/Warning Supressor")]
			private static void VirsabiConsoleSupressorMenuItem()
			{
				_windowInstance = GetWindow<VirsabiConsoleSupressorWindow>();
				_windowInstance.maxSize = new Vector2(250, 60 * VirsabiSettings.WarningSupresisons.Count + 275);
				_windowInstance.minSize = _windowInstance.maxSize;
				_windowInstance.titleContent = new GUIContent("Console Supressor");
			}


			private void OnEnable()
			{
				_windowInstance = this;
			}

			private void OnGUI()
			{
				EditorGUILayout.HelpBox("Annoyed by a ton of insignificant warnings in the console? Here you can suppress them. " +
					"\n\nThis will only suppress in the console - on builds these warnings will still appear. " +
					"\n\nYou can add your own custom supressions to the csc.rsp file in the Asset folder.", MessageType.None);
				EditorGUILayout.Separator();

				foreach (VirsabiSettings.SupressDefinition item in VirsabiSettings.WarningSupresisons)
				{
					SupressElement temp = new SupressElement(item);
				}
				
				
				EditorGUILayout.Separator();
				EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
				EditorGUILayout.Separator();

				if (GUILayout.Button("Update Supressions"))
					CSCIntepreter.WriteSettings();

				if (EditorApplication.isCompiling)
					EditorGUILayout.HelpBox("Recompiling... Please Wait!", MessageType.Warning);
			}

			private class SupressElement : GUILayout
			{
				public SupressElement(VirsabiSettings.SupressDefinition supressDefinition)
				{
					EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
					EditorGUILayout.Separator();
					supressDefinition.Enabled = EditorGUILayout.Toggle("Supress CS" + supressDefinition.WarningCode, supressDefinition.Enabled);
					EditorGUILayout.Space();
					EditorGUILayout.HelpBox(supressDefinition.Description, MessageType.None);
					EditorGUILayout.Separator();
				}
			}
		}
	}

}
