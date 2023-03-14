using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Virsabi.Editor_Utilities.Project_Tree_Creator.Scripts
{
    /// <summary>
    /// Author: Mikkel S. K. Mogensen
    /// <para>
    /// Folder utilities that are useful for adding, getting and sorting folders among other things.
    /// </para>
    /// </summary>
    public static class FolderUtils
    {
        public static List<Folder> GetSubfoldersRecursively(this Folder folder)
        {
            List<Folder> result = new List<Folder>();
            
            result.Add(folder);
            
            foreach (var subfolder in folder.Children)
            {
                result.AddRange(GetSubfoldersRecursively(subfolder));
            }

            return result;
        }
        
        public static List<Folder> GetAllParents(this Folder folder)
        {
            List<Folder> parentList = new List<Folder>();

            if (folder.Parent != null)
            {
                parentList.Add(folder);
                parentList.AddRange(GetAllParents(folder.Parent));
            }

            return parentList;
        }
        
        /// <summary>
        /// Add a new folder as a child of this folder.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public static Folder Add(this Folder parent, string folderName)
        {
            Folder subfolder;
            
            if (parent.ParentDirPath == "")
                subfolder = new Folder(parent, folderName, parent.FolderName);
            else
            {
                subfolder = new Folder(parent, folderName, parent.ParentDirPath + Path.DirectorySeparatorChar + parent.FolderName);
            }

            parent.Children.Add(subfolder);
            return subfolder;
        }
        
        /// <summary>
        /// Sorts folders recursively
        /// </summary>
        /// <param name="rootFolder"></param>
        public static Folder SortFolders(Folder rootFolder)
        {
            if (rootFolder.Children.Count == 0)
                return rootFolder;
            
            List<Folder> sortedList = rootFolder.Children.OrderBy(x=>x.FolderName).ToList();
            rootFolder.Children = sortedList;
            
            
            foreach (var child in rootFolder.Children)
            {
                SortFolders(child);
            }

            return rootFolder;
        }


        public static bool IsValidFolder(this Folder folder)
        {
            Debug.Log(folder.DirPath);
            return AssetDatabase.IsValidFolder(folder.DirPath);
        }
    }
}