using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Virsabi.Internal
{
    /// <summary>
    /// Adds the given define symbols to PlayerSettings define symbols.
    /// Just add your own define symbols to the Symbols property at the below.
    /// </summary>
    [InitializeOnLoad]
    public class UpdateDefineSymbols : Editor
    {
        /// <summary>
        /// Add define symbols as soon as Unity gets done compiling.
        /// </summary>
        static UpdateDefineSymbols()
        {

            string[] ourSymbols = VirsabiSettings.VirsabiSymbols.Select(symbol => symbol.Symbol).ToArray();


            string currentDefinesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            List<string> allDefines = currentDefinesString.Split(';').ToList();

            allDefines.AddRange(ourSymbols.Except(allDefines)); //adds our symbols except if they are already added

            //now remove those sysmbols that are disabled:
            foreach (VirsabiSettings.VirsabiSymbol virsabiSymbol in VirsabiSettings.VirsabiSymbols)
            {
                if (!virsabiSymbol.Enabled)
                    allDefines.Remove(virsabiSymbol.Symbol);
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup,
                string.Join(";", allDefines.ToArray()));


            //fix for null reference
            if (!EditorApplication.isUpdating || !EditorApplication.isCompiling) //stop the compile if we are entering playmode
            {
                //Debug.LogWarning("exited");
                return;
            }

            try
            {
                AssetDatabase.Refresh(); //used to have parameter: ImportAssetOptions.ForceUpdate - dunno if necessary
            }
            catch (NullReferenceException e)
            {
                Debug.LogError("NullReferenceException: " + e);
            }
                
        }
    }
}
