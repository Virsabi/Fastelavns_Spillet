using UnityEngine;

namespace Virsabi.Extensions
{
    /// <summary>
    /// Author: Rosetta Code
    /// <para>
    /// A function/subroutine that takes two ranges and a real number, and returns the mapping of the real number from the first to the second range. Use this function to map values from the range [0, 10] to the range [-1, 0].
    /// </para>
    /// </summary>
    public class MapRange
    {
        public static float Map(float a1, float a2, float b1, float b2, float s)
        {
            return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
        }
    }
}