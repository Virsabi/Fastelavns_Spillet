using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Virsabi.Internal
{
    public class VirsabiDefineSymbolsEditor : EditorWindow
    {

        private static EditorWindow _windowInstance;

        static VirsabiDefineSymbolsEditor()
        {

        }

        [MenuItem("Virsabi Tools/Import Settings", false, 10000 - 1)]
        private static void VirsabiUpdateMenuItem()
        {
            _windowInstance = GetWindow<VirsabiDefineSymbolsEditor>();


            if (VirsabiSettings.VirsabiSymbols == null)
                return;

            _windowInstance.maxSize = new Vector2(250, 60 * VirsabiSettings.VirsabiSymbols.Count + 275);
            _windowInstance.minSize = _windowInstance.maxSize;
            _windowInstance.titleContent = new GUIContent("Set Script Define Symbols");
        }

        private void OnEnable()
        {
            //VirsabiSettings.SavedSymbols = VirsabiSymbols;
            _windowInstance = this;
        }

        private void OnGUI()
        {
            EditorGUILayout.HelpBox("Here you can quickly set which libraries of the Virsabi Toolset you want to import. " +
                "\nSome are imported by default. " +
                "\n\nIf certain libraries creates bugs that doesn't have apparent solutions, you can disable them here. " +
                "\n\nThis is the equivelant of manually setting the Script Define Symbol in the Player Settings. " +
                "\n\nIf you are looking for samples, these are not the settings you are looking for... " +
                "\nOpen the Unity Package Manager instead.",
                MessageType.None);

            EditorGUI.BeginDisabledGroup(EditorApplication.isCompiling);

            foreach (VirsabiSettings.VirsabiSymbol item in VirsabiSettings.VirsabiSymbols)
            {
                EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
                EditorGUILayout.Separator();
                item.Enabled = EditorGUILayout.Toggle(item.Symbol, item.Enabled);
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox(item.Description, MessageType.None);
                EditorGUILayout.Separator();

            }



            if (GUILayout.Button("Update Symbols"))
                RefreshSymbols();
            EditorGUI.EndDisabledGroup();

            if (EditorApplication.isCompiling)
                EditorGUILayout.HelpBox("Recompiling... Please Wait!", MessageType.Warning);
        }
        private void RefreshSymbols()
        {
            AssetDatabase.ImportAsset("Packages/com.virsabi/Editor/Virsabi.Editor.asmdef");
            AssetDatabase.Refresh();
            Debug.Log("Refreshing Script Define Symbols - Please wait for full recompilation!");
        }
    }


}