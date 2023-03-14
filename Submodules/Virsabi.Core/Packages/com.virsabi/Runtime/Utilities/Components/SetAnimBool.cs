using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple script for controlling animation bool
/// </summary>
namespace Virsabi.Misc
{
    public class SetAnimBool : MonoBehaviour
    {
        public string boolString = "Active";

        private Animator anim;

        private void Start()
        {
            anim = GetComponent<Animator>();
        }

        public void SetStatus(bool status)
        {
            anim.SetBool(boolString, status);
        }
    }
}
