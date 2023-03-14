using UnityEditor;
using UnityEngine;
/// <summary>
/// Example for how we can create gui overlay in the scene view for future debug tools and QOL features.
/// </summary>
public class SceneGUIExample : EditorWindow
{
    [MenuItem("Virsabi Tools/Scene GUI/Enable")]
    public static void Enable()
    {
        SceneView.duringSceneGui += OnScene;
        Debug.Log("Scene GUI : Enabled");
    }

    [MenuItem("Virsabi Tools/Scene GUI/Disable")]
    public static void Disable()
    {
        SceneView.duringSceneGui -= OnScene;
        Debug.Log("Scene GUI : Disabled");
    }

    private static void OnScene(SceneView sceneview)
    {
        Handles.BeginGUI();
        if (GUILayout.Button("Press Me"))
            Debug.Log("Got it to work.");

        Handles.EndGUI();
    }
}