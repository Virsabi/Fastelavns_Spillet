#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.IO;
using Virsabi.Editor_Utilities.Project_Tree_Creator.Scripts;

/// <summary>
/// Author: Mikkel S. K. Mogensen
/// <para>
/// Generate folder structure automatically based on established rules.
/// </para>
/// </summary>
namespace ProjectTreeGenerator
{
    public static class CreateProjectTree
    {
        public static void CreateFolders(Folder folder)
        {
            if (!AssetDatabase.IsValidFolder(folder.DirPath))
            {
                Debug.Log("Creating: <b>" + folder.DirPath + "</b>");
                AssetDatabase.CreateFolder(folder.ParentDirPath, folder.FolderName);
                File.Create(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + folder.DirPath +
                            Path.DirectorySeparatorChar + ".keep");
            }
            else
            {
                if (Directory.GetFiles(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar +
                                       folder.DirPath).Length < 1)
                {
                    File.Create(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + folder.DirPath +
                                Path.DirectorySeparatorChar + ".keep");
                    Debug.Log("Creating '.keep' file in: <b>" + folder.DirPath + "</b>");
                }
                else
                {
                    Debug.Log("Directory <b>" + folder.DirPath + "</b> already exists");
                }
            }
        }

        public static Folder GenerateFolderStructure()
        {
            // Make sure the top-most directory is the Assets folder.
            Folder result = new Folder("Assets", "");

            // Add root folder to the asset folder. This folder contains all assets created for the current project.
            var rootFolder = result.Add("_root_");

            // Timeline assets
            var timelineAssets = rootFolder.Add("Timelines");

            // Audio assets
            var audioAssets = rootFolder.Add("Audio");
            audioAssets.Add("Mixers");
            audioAssets.Add("Music");
            audioAssets.Add("SFX");
            audioAssets.Add("Voice");

            // Graphics assets
            var graphicsAssets = rootFolder.Add("Graphics");
            var twoDimAssets = graphicsAssets.Add("2D");
            twoDimAssets.Add("Sprites");
            twoDimAssets.Add("Video");
            twoDimAssets.Add("Skybox");
            var textureAssets = twoDimAssets.Add("Textures");

            // Texture assets
            textureAssets.Add("Skybox Textures");

            // UI assets
            var uiAssets = rootFolder.Add("UI");

            // Effects assets
            var effectAssets = rootFolder.Add("Effects");
            effectAssets.Add("VFX");
            effectAssets.Add("Particles");

            var threeDimAssets = graphicsAssets.Add("3D");
            var modelTemplate = threeDimAssets.Add("ModelName");
            modelTemplate.Add("Animations");
            modelTemplate.Add("Materials");
            modelTemplate.Add("Textures");
            modelTemplate.Add("Source");

            var materialAssets = graphicsAssets.Add("Shared Materials");

            // Material assets
            materialAssets.Add("Skybox Materials");

            // Prefab assets
            rootFolder.Add("Prefabs");

            // Scene assets
            rootFolder.Add("Scenes");

            // SO assets
            var SOAssets = rootFolder.Add("Scriptable Objects");
            SOAssets.Add("SOA Vars");
            SOAssets.Add("SOA Events");

            // Script assets
            var scriptAssets = rootFolder.Add("Scripts");
            scriptAssets.Add("Generic");
            scriptAssets.Add("SOA Scripts");

            // Shader assets
            rootFolder.Add("Shaders");

            // Temporary assets
            rootFolder.Add("Temp");

            // Fonts assets
            rootFolder.Add("Fonts");

            // Post-processing assets
            rootFolder.Add("Post Processing");

            return result;
        }
    }
}

#endif