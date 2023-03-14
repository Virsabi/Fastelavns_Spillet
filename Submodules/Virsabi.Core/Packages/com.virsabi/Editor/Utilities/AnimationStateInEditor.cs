
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;


/// <summary>
/// Script to change animation states in editor to preview states in scene
/// </summary>
namespace Virsabi.Editor_Utilities
{
    [RequireComponent(typeof(Animator))]
    [ExecuteInEditMode]
    public class AnimationStateInEditor : MonoBehaviour
    {
        [AutoProperty, ReadOnly, SerializeField]
        private Animator anim;

        public AnimationStateReference state;

        private void OnValidate()
        {
            anim = GetComponent<Animator>();
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                //forces to play in edit mode
                anim.Update(Time.deltaTime);


            }

        }

        [ButtonMethod]
        void PlayAnimation()
        {
            anim.Play(state);
        }
    }
}

#endif