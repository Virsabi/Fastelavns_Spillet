using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

public static class FileUtility {
    /**
		 * Read all text from a file path.
		 */
    public static string ReadFile(string path) {
        if (!File.Exists(path)) {
            Debug.LogError("File path does not exist!\n" + path);
            return "";
        }

        string contents = File.ReadAllText(path);

        return contents;
    }

    /**
		 * Save some text to a file path.  Does not check that folder structure is valid!
		 */
    public static bool SaveFile(string path, string contents) {
        try {
            File.WriteAllText(path, contents);
        }
        catch (System.Exception e) {
            Debug.LogError("Failed writing to path: " + path + "\n" + e.ToString());
            return false;
        }

        return true;
    }

    public static bool IsValidPath(string path, string extension) {
        return !string.IsNullOrEmpty(path) &&
               System.Uri.IsWellFormedUriString(path, System.UriKind.RelativeOrAbsolute) &&
               path.EndsWith(extension);
    }

    /**
		 * Given a string, this function attempts to extrapolate the absolute path using current directory as the
		 * relative root.
		 */
    public static string GetFullPath(string path) {
        string full = Path.GetFullPath(path);
        return full;
    }

    /**
		 * Return the path type (file or directory)
		 */
    public static PathType GetPathType(string path) {
        return File.Exists(path) ? PathType.File : (Directory.Exists(path) ? PathType.Directory : PathType.Null);
    }

    /**
		 * Replace backslashes with forward slashes, and make sure that path is the full path.
		 */
    public static string SanitizePath(string path, string extension = null) {
        string rep = GetFullPath(path);
        // @todo On Windows this defaults to '\', but doesn't escape correctly.
        // Path.DirectorySeparatorChar.ToString());
        rep = Regex.Replace(rep, "(\\\\|\\\\\\\\){1,2}|(/)", "/");
        // white space gets the escaped symbol

        if (extension != null && !rep.EndsWith(extension)) {
            if (!extension.StartsWith("."))
                extension = "." + extension;

            rep += extension;
        }

        return rep;
    }

    public static string GetProjectPath() {
        return Application.dataPath.Substring(0, Application.dataPath.Length - 6);
    }

    public static string GetRelativePath(string absolutepath) {
        
        if (absolutepath.Contains("PackageCache")) {
            var result = absolutepath.Substring(GetProjectPath().Length + "Library/PackageCache".Length + 1);
            var packageName = String.Concat(result.TakeWhile(c => !Equals(c, '@')));
            result = result.Substring(result.IndexOf('/'));
            result = packageName + result;
            
            return "Packages/" + result;
        }

        if (absolutepath.Contains("Packages"))
        {
            var result = absolutepath.Substring(absolutepath.LastIndexOf("Packages"));
            //Debug.Log("Trimmed result: \n" + result);

            return result;
        }

        if (absolutepath.StartsWith(Application.dataPath)) { 
            return "Assets" + absolutepath.Substring(Application.dataPath.Length);
        }

        Debug.LogError("The specified path was not a data path.");
        return null;
    }

    #region UNITY_EDITOR
    #if UNITY_EDITOR
    //TODO: We have duplicate functionallity here - investigate and disect
    public static string GetMonoScriptPath(MonoBehaviour mb) {
        MonoScript ms = MonoScript.FromMonoBehaviour( mb );
        var m_ScriptFilePath = AssetDatabase.GetAssetPath( ms );
 
        FileInfo fi = new FileInfo( m_ScriptFilePath);
        var m_ScriptFolder = fi.Directory.ToString();
        m_ScriptFolder.Replace( '\\', '/');

        return m_ScriptFolder;
    }
    public static string GetMonoScriptPath(ScriptableObject so) {
        MonoScript ms = MonoScript.FromScriptableObject( so );
        var m_ScriptFilePath = AssetDatabase.GetAssetPath( ms );
 
        FileInfo fi = new FileInfo( m_ScriptFilePath);
        var m_ScriptFolder = fi.Directory.ToString();
        m_ScriptFolder.Replace( '\\', '/');

        return m_ScriptFolder;
    }

    public class ScriptPathGetter
    {
        /// <summary>
        /// Get the full path of a script. Eg; "Path/to/script/theScript.cs"
        /// </summary>
        /// <param name="script">the script to find path for</param>
        /// <returns>Eg; "Path/to/script/theScript.cs" </returns>
        public static string GetScriptPath(ScriptableObject script, string trimUpUntil)
        {
            MonoScript ms = MonoScript.FromScriptableObject(script);
            string m_ScriptFilePath = AssetDatabase.GetAssetPath(ms);

            m_ScriptFilePath = m_ScriptFilePath.Substring(m_ScriptFilePath.IndexOf(trimUpUntil));

            return m_ScriptFilePath;

        }
        /// <summary>
        /// Get the full path of a script. Eg; "Path/to/script/"
        /// </summary>
        /// <param name="script">the object to find path for</param>
        /// <param name="trimUpUntil">trims the string up until the last occurance of this. Normal usage is "Assets" or "Packages" to remove path outside of the unity project</param>
        /// <returns></returns>
        public static string GetFolderPath(ScriptableObject script, string trimUpUntil)
        {
            MonoScript ms = MonoScript.FromScriptableObject(script);
            string m_ScriptFilePath = AssetDatabase.GetAssetPath(ms);

            FileInfo fi = new FileInfo(m_ScriptFilePath);
            string m_ScriptFolder = fi.Directory.ToString();
            //Debug.Log("before replace: " + m_ScriptFolder);
            m_ScriptFolder = m_ScriptFolder.Replace('\\', '/');

            if (!m_ScriptFolder.Contains(trimUpUntil))
            {
                Debug.LogWarning("Cannot trim '" + m_ScriptFolder + "' cause the string '" + trimUpUntil + "' is not part of the path");
                return "";
            }

            m_ScriptFolder = m_ScriptFolder.Substring(m_ScriptFolder.IndexOf(trimUpUntil));
            //Debug.Log("after replace: " + m_ScriptFolder);

            return m_ScriptFolder;
        }
    }


    public static string SwapBackSlashToNormalSlash(string text)
    {
        return text.Replace('/', '\\');
    }

    public static string SwapNormalSlashToBackSlash(string text)
    {
        return text.Replace('\\', '/');
    }
    public static string TrimPathToUnityProject(string path)
    {
        return path.Substring(path.LastIndexOf("Assets"));
    }


#endif
    #endregion
}

public enum PathType
{
    Null,
    File,
    Directory
}