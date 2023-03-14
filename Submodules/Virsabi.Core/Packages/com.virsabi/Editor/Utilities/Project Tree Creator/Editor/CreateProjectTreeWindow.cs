#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using ProjectTreeGenerator;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Virsabi.Editor_Utilities.Project_Tree_Creator.Scripts;

namespace Virsabi.Editor_Utilities.Project_Tree_Creator.Editor
{
    /// <summary>
    /// Author: Mikkel S. K. Mogensen
    /// <para>
    /// Custom editor for project tree creator.
    /// </para>
    /// </summary>
    public class CreateProjectTreeWindow : EditorWindow
    {
        private Folder folderStructure;
        private List<Folder> folders;
        private Dictionary<Folder, Label> _labelDictionary = new Dictionary<Folder, Label>();
        private Dictionary<Folder, Toggle> _folderToggleDictionary = new Dictionary<Folder, Toggle>();

        private void OnProjectChange()
        {
            foreach (var folder in folders)
            {
                if (!folder.IsValidFolder())
                    _labelDictionary[folder].RemoveFromClassList("existing");
            }
        }

        [MenuItem("Virsabi Tools/Utilities/Project Tree Creator")]
        private static void ShowWindow()
        {
            var window = GetWindow(typeof(CreateProjectTreeWindow), true, "Project Tree Creator");
            window.maxSize = new Vector2(500f, 10000f);
            window.minSize = new Vector2(500f, 200f);
        }

        private void OnEnable()
        {
            var root = rootVisualElement;
            
            //Debug.Log("internal path: " + VirsabiInternalPath.VirsabiDirectory);

            // Import stylesheet

            string monoDir = FileUtility.GetMonoScriptPath(this);
            string styleSheetPath = FileUtility.GetRelativePath(FileUtility.SanitizePath(monoDir + "/ProjectTreeCreator.uss"));
            string projectTreeUxmlPath = FileUtility.GetRelativePath(FileUtility.SanitizePath(monoDir + "/ProjectTreeCreator.uxml"));
            string treeItemUxmlPath = FileUtility.GetRelativePath(FileUtility.SanitizePath(monoDir + "/TreeItem.uxml"));

            Debug.Log(styleSheetPath);
            
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(styleSheetPath);

            // Import ProjectTree UXML
            var projectTreeCreatorUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(projectTreeUxmlPath);
            VisualElement projectTreeInstance = projectTreeCreatorUXML.CloneTree();
            root.Add(projectTreeInstance);
            projectTreeInstance.styleSheets.Add(styleSheet);
        
            // Import TreeItem UXML
            var treeItemUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(treeItemUxmlPath);
            VisualElement treeItemInstance = treeItemUXML.CloneTree();
            treeItemInstance.styleSheets.Add(styleSheet);
        
            // Add entries to the UXML list view
            var listView = projectTreeInstance.Q<ListView>("FolderList");
        
            // Generate structure from predefined project tree and select the _root_ folder (first child).
            folderStructure = FolderUtils.SortFolders(CreateProjectTree.GenerateFolderStructure().Children[0]);
            folders = folderStructure.GetSubfoldersRecursively();
        
            // Bind buttons
            projectTreeInstance.Q<Button>("Generate Button").RegisterCallback<MouseUpEvent>(evt => GenerateFolders());
            projectTreeInstance.Q<Button>("Select Button").RegisterCallback<MouseUpEvent>(evt => SelectAll());
            projectTreeInstance.Q<Button>("Deselect Button").RegisterCallback<MouseUpEvent>(evt => DeselectAll());

            // Generate visual representation of folders and register callbacks for toggles
            for (int i = 0; i < folders.Count; i++)
            {
                var treeItemContainer = treeItemUXML.CloneTree();
                var indentationElement = treeItemContainer.Q<Label>(null, "indentation");
                var labelElement = treeItemContainer.Q<Label>(null, "itemLabel");
                var lookupIdx = i;

                _labelDictionary.Add(folders[i], labelElement);
            
                // Add chars for indentation
                indentationElement.text = new String('_', (folders[i].Depth) * 3);
            
                // Set labels for folders
                labelElement.text = folders[i].FolderName;
            
                // Check if folder exists and set the style
                if (folders[i].IsValidFolder())
                {
                    labelElement.AddToClassList("existing");

                    labelElement.RegisterCallback<MouseUpEvent>(evt =>
                    {
                        // Ping the folder if it exists
                        UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(folders[lookupIdx].DirPath, typeof(UnityEngine.Object));
                        Selection.activeObject = obj;
                        EditorGUIUtility.PingObject(obj);
                    });
                }
            
                // Add toggle to dictionary
                _folderToggleDictionary.Add(folders[i], treeItemContainer.Q<Toggle>()); 
            
                //Register callbacks for toggles
                treeItemContainer.Q<Toggle>().RegisterValueChangedCallback((evt) =>
                {
                    CheckDependencies(folders[lookupIdx], evt.newValue);
                });
            
                // Add the tree item to the list view
                listView.Add(treeItemContainer);
            }
        }

        private void CheckDependencies(Folder folder, bool toggleVal)
        {
            if (folder.Parent.FolderName != "Assets" && toggleVal)
                _folderToggleDictionary[folder.Parent].value = toggleVal;
            else if (!toggleVal && folder.Children.Count > 0) {
                foreach (var child in folder.Children)
                {
                    _folderToggleDictionary[child].value = toggleVal;
                }
            }
        }

        private void GenerateFolders()
        {
            foreach (var keyValuePair in _folderToggleDictionary.Where(keyValuePair => keyValuePair.Value.value))
            {
                CreateProjectTree.CreateFolders(keyValuePair.Key);
            
                if (_labelDictionary.ContainsKey(keyValuePair.Key))
                    _labelDictionary[keyValuePair.Key].AddToClassList("existing");
                
                
                // Register callback so we can ping the generated folder.
                _labelDictionary[keyValuePair.Key].RegisterCallback<MouseUpEvent>(evt =>
                {
                    // Ping the folder if it exists
                    UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(keyValuePair.Key.DirPath, typeof(UnityEngine.Object));
                    Selection.activeObject = obj;
                    EditorGUIUtility.PingObject(obj);
                });
            }
        }

        private void SelectAll()
        {
            foreach (var keyValuePair in _folderToggleDictionary)
            {
                keyValuePair.Value.value = true;
            }
        }

        private void DeselectAll()
        {
            foreach (var keyValuePair in _folderToggleDictionary)
            {
                keyValuePair.Value.value = false;

            }
        }
    }
}
#endif