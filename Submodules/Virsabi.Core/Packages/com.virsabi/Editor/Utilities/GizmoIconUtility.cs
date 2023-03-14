using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.Callbacks;

/// <summary>
/// This editor script renders instance unique icons on top of scriptable objects which inherit from IconScriptableObjects
/// </summary>
public class GizmoIconUtility
{
    [DidReloadScripts]
    static GizmoIconUtility()
    {
        EditorApplication.projectWindowItemOnGUI = ItemOnGUI;
    }

    static void ItemOnGUI(string guid, Rect rect)
    {
        string assetPath = AssetDatabase.GUIDToAssetPath(guid);

        IconScriptableObject obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(IconScriptableObject)) as IconScriptableObject;

        if (obj != null && obj.icon != null)
        {
            if (rect.height > rect.width)
                rect.height = rect.width;
            else
                rect.width = rect.height;

            Texture iconTexture = obj.icon.texture;

            GUI.DrawTexture(rect, iconTexture);
        }
    }
}