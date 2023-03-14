using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

namespace Virsabi.Misc
{
    /// <summary>
    /// Generic material changer script - currently only works for single material renderers
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    [ExecuteInEditMode]
    public class MaterialChanger : MonoBehaviour
    {

        public Material newMaterial;

        [SerializeField]
        [ReadOnly]
        private Material originalMaterial;

        [SerializeField]
        [ReadOnly]
        private Renderer _renderer;

        private void OnValidate()
        {
            _renderer = GetComponent<Renderer>();
            originalMaterial = _renderer.sharedMaterial;
        }

        [ButtonMethod]
        public void ChangeToNewMaterial()
        {
            _renderer.material = newMaterial;
        }
        [ButtonMethod]
        public void ChangeToOriginalMaterial()
        {
            _renderer.material = originalMaterial;
        }

        public void ChangeMaterialTo(Material mat)
        {
            _renderer.material = mat;
        }
    }

}