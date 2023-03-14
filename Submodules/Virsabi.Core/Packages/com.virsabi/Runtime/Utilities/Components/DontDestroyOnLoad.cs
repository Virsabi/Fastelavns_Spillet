using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Virsabi.Utility
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
    }
}
