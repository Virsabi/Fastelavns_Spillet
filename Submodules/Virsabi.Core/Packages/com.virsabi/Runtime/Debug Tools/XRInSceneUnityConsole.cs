using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MyBox;

namespace Virsabi.DebugTools
{
    public class XRInSceneUnityConsole : MonoBehaviour
    {
        //It prints out the console message inside the scene for debugging

        [ReadOnly]
        public TextMeshPro textMesh;
        public bool excludeNormalLogs;
        public bool showStackTrace;

        public Color warningColor = Color.yellow;
        public Color errorColor = Color.red;

        // Use this for initialization
        void OnValidate()
        {
            textMesh = gameObject.GetComponent<TextMeshPro>();
        }

        void OnEnable()
        {
            Application.logMessageReceived += LogMessage;
        }

        void OnDisable()
        {
            Application.logMessageReceived -= LogMessage;
        }

        public void LogMessage(string message, string stackTrace, LogType type)
        {
            if (showStackTrace)
            {
                message = message + "\n" + stackTrace;
            }


            switch (type)
            {
                case LogType.Error:
                    message = "<color=#" + ColorUtility.ToHtmlStringRGB(errorColor) + ">" + message + "</color>";
                    break;
                case LogType.Assert:
                    break;
                case LogType.Warning:
                    message = "<color=#" + ColorUtility.ToHtmlStringRGB(warningColor) + ">" + message + "</color>";
                    break;
                case LogType.Log:
                    break;
                case LogType.Exception:
                    break;
                default:
                    break;
            }


            if (type == LogType.Log && excludeNormalLogs)
            {

            }
            else
            {
                if (textMesh?.text.Length > 400)
                {
                    textMesh.text = message + "\n";
                }
                else if (textMesh)
                {
                    textMesh.text += message + "\n";
                }
            }

        }
    }
}