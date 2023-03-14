using System.Collections.Generic;
using System.IO;

namespace Virsabi.Editor_Utilities.Project_Tree_Creator.Scripts
{
    /// <summary>
    /// Author: Mikkel S. K. Mogensen
    /// <para>
    /// Class that contains the data related to folders. Folder can reference their parent or their children.
    /// </para>
    /// </summary>
    public class Folder
    {
        /// <summary>
        /// Path pointing to the parent folder.
        /// </summary>
        public string ParentDirPath { get; private set; }

        /// <summary>
        /// Path pointing to this folder.
        /// </summary>
        public string DirPath { get; private set; }

        /// <summary>
        /// The name of this folder
        /// </summary>
        public readonly string FolderName;

        public readonly int Depth;
        public readonly Folder Parent;
        public List<Folder> Children;

        public Folder(string folderName, string parentDirPath)
        {
            FolderName = folderName;
            ParentDirPath = parentDirPath;
            DirPath = ParentDirPath + Path.DirectorySeparatorChar + FolderName;
            Children = new List<Folder>();
        }

        public Folder(Folder parent, string folderName, string parentDirPath)
        {
            FolderName = folderName;
            ParentDirPath = parentDirPath;
            DirPath = ParentDirPath + Path.DirectorySeparatorChar + FolderName;
            Children = new List<Folder>();
            Parent = parent;

            Depth = Parent.Depth + 1;
        }
    }
}