#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using UnityEditor;
using MyBox;

/// <summary>
/// Rewritten from mybox - still uses some methods from mybox
/// </summary>
namespace Virsabi.Internal
{
    public enum VirsabiInstallMethod
    {
        masterUpm,
        upmDev,
        fromDisk,
        broken
    }

    public static class VirsabiInternalUtilities
    {
        private static readonly string ReleasesURL = "https://github.com/Virsabi/Virsabi.Core/releases";
        private static readonly string VirsabiPackageInfoURL = "https://raw.githubusercontent.com/Virsabi/VirsabiPublicFiles/master/Virsabi.Core/package.json";

        private static readonly string VirsabiPackageTag = "com.virsabi";
        //public static readonly string MyBoxRepoLink = "https://github.com/Deadcows/MyBox.git";

        public static void OpenVirsabiGitInBrowser()
        {
            Application.OpenURL(ReleasesURL);
        }


        #region Get Current / Latest Versions

        public static async void GetVirsabiLatestVersionAsync(Action<VirsabiVersion> onVersionRetrieved)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var packageJson = await client.GetStringAsync(VirsabiPackageInfoURL);

                    var versionRaw = RetrievePackageVersionOutOfJson(packageJson);
                    if (versionRaw == null)
                    {
                        Debug.LogWarning("VirsabiVersion was unable to parse package.json :(");
                        return;
                    }

                    var version = new VirsabiVersion(versionRaw);
                    if (onVersionRetrieved != null) onVersionRetrieved(version);
                }
            }
            catch (HttpRequestException requestException)
            {
                Debug.LogWarning("VirsabiVersion is unable to check version online :(. Exception is: " + requestException.Message);
            }
        }

        public static VirsabiVersion GetVirsabiInstalledVersion()
        {
            var packageJsonPath = PackageJsonPath;
            if (packageJsonPath == null)
            {
                Debug.LogWarning("VirsabiVersion is unable to check installed version :(");
                return null;
            }

            var packageJsonContents = File.ReadAllText(packageJsonPath);
            var versionRaw = RetrievePackageVersionOutOfJson(packageJsonContents);
            if (versionRaw == null)
            {
                Debug.LogWarning("VirsabiVersion was unable to parse package.json :(");
                return null;
            }

            var version = new VirsabiVersion(versionRaw);
            return version;
        }

        private static string RetrievePackageVersionOutOfJson(string json)
        {
            var versionLine = json.Split('\r', '\n').SingleOrDefault(l => l.Contains("version"));
            if (versionLine == null) return null;

            var matches = Regex.Matches(versionLine, "\"(.*?)\"");
            if (matches.Count <= 1 || matches[1].Value.IsNullOrEmpty()) return null;

            return matches[1].Value.Trim('"');
        }

        #endregion


        #region Update Git Packages

        /// <summary>
        /// Remove lock {} section out of manifest.json
        /// </summary>
        /// <returns>is there were any git packages</returns>
        public static bool UpdateGitPackages()
        {
            var manifestFilePath = ManifestJsonPath;
            var manifest = File.ReadAllLines(manifestFilePath).ToList();

            var cutFrom = -1;
            for (int i = 0; i < manifest.Count; i++)
            {
                if (!manifest[i].Contains("\"lock\": {")) continue;

                cutFrom = i;
                break;
            }

            if (cutFrom <= 0) return false;

            manifest[cutFrom - 1] = "}";
            var removeLinesCount = manifest.Count - cutFrom - 1;
            manifest.RemoveRange(cutFrom, removeLinesCount);

            try
            {
                using (StreamWriter sr = new StreamWriter(manifestFilePath))
                {
                    for (int i = 0; i < manifest.Count; i++)
                        sr.WriteLine(manifest[i]);
                }

                AssetDatabase.Refresh(ImportAssetOptions.Default);
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Virsabi.Internal is unable to rewrite packages.json to update git packages :(. Exception is: " + ex.Message);
            }

            return true;
        }

        #endregion


        #region Installed Via UPM

        public static VirsabiInstallMethod _InstallMethod
        {
            get
            {
                if (_installedViaUPMChecked) return _packageInstalledFrom;

                if (ManifestJsonPath == null)
                {
                    Debug.LogWarning("Virsabi is unable to find manifest.json file :(");
                    return VirsabiInstallMethod.broken;
                }

                var manifest = File.ReadAllLines(ManifestJsonPath);

                if (manifest.Any(l => l.Contains(VirsabiPackageTag + "\": \"https://github.com/Virsabi/VirsabiRepoDevelopment.git#upmDev")))
                    _packageInstalledFrom = VirsabiInstallMethod.upmDev;
                else if (manifest.Any(l => l.Contains(VirsabiPackageTag + "\": \"https://github.com/")))
                    _packageInstalledFrom = VirsabiInstallMethod.masterUpm;
                else
                    _packageInstalledFrom = VirsabiInstallMethod.fromDisk;


                _installedViaUPMChecked = true;
                return _packageInstalledFrom;
            }
        }

        private static VirsabiInstallMethod _packageInstalledFrom;
        private static bool _installedViaUPMChecked;

        #endregion


        #region Package Json Path

        private static string PackageJsonPath
        {
            get
            {
                if (_packageJsonPathChecked) return _packageJsonPath;

                var virsabiDirectory = VirsabiInternalPath.VirsabiDirectory;
                if (virsabiDirectory == null)
                {
                    Debug.LogWarning("Virsabi is unable to find the path of the package :(");
                    _packageJsonPathChecked = true;
                    return null;
                }

                var packageJson = virsabiDirectory.GetFiles().SingleOrDefault(f => f.Name == "package.json");
                if (packageJson == null)
                {
                    Debug.LogWarning("Virsabi is unable to find package.json file :(");
                    _packageJsonPathChecked = true;
                    return null;
                }

                _packageJsonPath = packageJson.FullName;
                _packageJsonPathChecked = true;
                return _packageJsonPath;
            }
        }

        public static string _packageJsonPath;
        private static bool _packageJsonPathChecked;

        #endregion


        #region Manifest JSON Path

        private static string ManifestJsonPath
        {
            get
            {
                if (_manifestJsonPathChecked) return _manifestJsonPath;

                var packageDir = Application.dataPath.Replace("Assets", "Packages");
                _manifestJsonPath = Directory.GetFiles(packageDir).SingleOrDefault(f => f.EndsWith("manifest.json"));
                _manifestJsonPathChecked = true;
                return _manifestJsonPath;
            }
        }

        private static string _manifestJsonPath;
        private static bool _manifestJsonPathChecked;

        #endregion
    }
}

#endif
