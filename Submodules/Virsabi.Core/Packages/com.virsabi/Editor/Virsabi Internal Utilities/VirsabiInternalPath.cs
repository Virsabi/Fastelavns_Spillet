#if UNITY_EDITOR
using MyBox.EditorTools;
using UnityEngine;
using System.IO;

namespace Virsabi.Internal
{
    /// <summary>
    /// SO is needed to determine the path to this script.
    /// Thereby it's used to get relative path to Virsabi
    /// </summary>
    public class VirsabiInternalPath : ScriptableObject
    {
        /// <summary>
        /// Absolute path to Virsabi folder
        /// </summary>
        public static DirectoryInfo VirsabiDirectory
        {
            get
            {
                if (_directoryChecked) return _virsabiDirectory;

                var internalPath = MyEditor.GetScriptAssetPath(Instance);
                var scriptDirectory = new DirectoryInfo(internalPath);

                // Script is in Virsabi/Tools/Internal so we need to get dir two steps up in hierarchy
                if (scriptDirectory.Parent == null || scriptDirectory.Parent.Parent == null)
                {
                    _directoryChecked = true;
                    return null;
                }

                _virsabiDirectory = scriptDirectory.Parent.Parent;
                _directoryChecked = true;
                return _virsabiDirectory;
            }
        }

        private static DirectoryInfo _virsabiDirectory;
        private static bool _directoryChecked;

        private static VirsabiInternalPath Instance
        {
            get
            {
                if (_instance != null) return _instance;
                return _instance = CreateInstance<VirsabiInternalPath>();
            }
        }

        private static VirsabiInternalPath _instance;
    }
}
#endif