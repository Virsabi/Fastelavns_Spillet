#if UNITY_EDITOR
using System;

namespace Virsabi.Internal
{
    [Serializable]
    public class VirsabiVersion
    {
        public string Major;
        public string Minor;
        public string Patch;

        public string AsSting;

        /// <param name="version">NUM.NUM.NUM format</param>
        public VirsabiVersion(string version)
        {
            AsSting = version;
            var v = version.Split('.');
            Major = v[0];
            Minor = v[1];
            Patch = v[2];
        }

        /// <summary>
        /// Major & Minor versions match, skip patch releases
        /// </summary>
        public bool BaseVersionMatch(VirsabiVersion version)
        {
            return Major == version.Major && Minor == version.Minor;
        }

        public bool VersionsMatch(VirsabiVersion version)
        {
            return BaseVersionMatch(version) && Patch == version.Patch;
        }
    }
}
#endif