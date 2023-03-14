using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Virsabi.Utility
{
    public static class Utility
    {
        /// <summary>
        /// Maps a number "s" from the range [a1, a2] to range [b1, b2].
        /// </summary>
        public static float Map(float a1, float a2, float b1, float b2, float s)
        {

            return b1 + (s - a1) * (b2 - b1) / (a2 - a1);

        }

        /// <summary>
        /// Plays a random sound on an audiosource.
        /// </summary>
        /// <param name="audioClips">The list of clips to choose from</param>
        /// <param name="audioSource">the source to play on </param>
        /// <param name="noRepeat">if true, the same clip can never be played twice in a row.</param>
        /// <param name="lastIndex">Save this parameter and pass in to tell the method which index was played last - so that noRepeat works.</param>
        /// <returns></returns>
        public static int PlayRandomSound(List<AudioClip> audioClips, AudioSource audioSource, bool noRepeat, int lastIndex)
        {
            int randomIndex;

            if (audioClips.Count == 0)
            {
                return lastIndex;
            }

            if (!noRepeat || audioClips.Count == 1)
            {
                randomIndex = 0;
            }
            else
            {
                do
                {
                    randomIndex = Random.Range(0, audioClips.Count - 1);
                } while (randomIndex == lastIndex && randomIndex != -1);

                lastIndex = randomIndex;
            }

            if (randomIndex != -1)
            {
                var clip = audioClips[randomIndex] as AudioClip;
                if (clip != null)
                {
                    audioSource.PlayOneShot(clip);
                }
            }

            return lastIndex;
        }

        public static int RandomRangeExcept(int min, int max, int except)
        {
            var number = Random.Range(min, max);
            while (number == except)
                number = Random.Range(min, max);
            return number;
        }


#if UNITY_EDITOR
        public static T LoadFirstAssetOfType<T>(string type)
        {
            //Debug.Log("Loading asset of type " + type);

            string[] guids = AssetDatabase.FindAssets("t:" + type);

            if (guids.Length == 0)
                Debug.LogError("Couldn't find any assets of type " + type);

            if (guids.Length > 1)
                Debug.LogError("Too many " + type + " assets!!!");

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);

            return (T)Convert.ChangeType(AssetDatabase.LoadAssetAtPath(path, typeof(T)), typeof(T));
        }
#endif
    }

}
