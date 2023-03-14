using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

namespace Virsabi.Misc
{

    /// <summary>
    /// Example script for controlling and changing material property blocks. Needs more work to be "generic", but is a good start. 
    /// !Remember! the shader needs to implement property blocks or it wont work - and the CustomMaterialValues class needs to be changed and map these values depending on what should be changed in the shader.
    /// Author: Mathias Munk Ulrich
    /// </summary>

    [ExecuteInEditMode, RequireComponent(typeof(Renderer))]
    public class MaterialPropBlockController : MonoBehaviour
    {
        /// <summary>
        /// List of custom classes for assigning saving and assinging property values to each material in a renderer. 
        /// </summary>
        [SerializeField]
        public List<CustomMaterialValues> matVals = new List<CustomMaterialValues>();


        [SerializeField]
        [ReadOnly]
        private Renderer _renderer;


        [SerializeField]
        [ReadOnly]
        private MaterialPropertyBlock _propBlock;

        private void OnValidate()
        {
            _propBlock = new MaterialPropertyBlock();
            _renderer = GetComponent<Renderer>();
        }

        [ButtonMethod] //Generates Material-ValueSets From Materials On Renderer
        private void GenerateMaterialValueSets()
        {
            matVals.Clear();

            foreach (Material material in _renderer.sharedMaterials)
            {
                matVals.Add(new CustomMaterialValues(new Color(), new float(), new float()));
            }
        }

        // updates the material(s) on the renderer thourgh property blocks.
        void Update()
        {
            int i = 0;
            if (_renderer.sharedMaterials.Length > 0 && matVals.Count > 0)
            {

                foreach (Material material in _renderer.sharedMaterials)
                {
                    _renderer.GetPropertyBlock(_propBlock, i);

                    _propBlock.SetColor("_Color", matVals[i].color);
                    _propBlock.SetFloat("_Glossiness", matVals[i].Smoothness);
                    _propBlock.SetFloat("_Metallic", matVals[i].Metallic);



                    _renderer.SetPropertyBlock(_propBlock, i);

                    i++;
                }
            }
        }

        /// <summary>
        /// Custom class to hold values for property blocks and expose to inspector for easy editing
        /// </summary>
        [System.Serializable]
        public class CustomMaterialValues
        {
            public Color color;
            public float Smoothness;
            public float Metallic;

            public CustomMaterialValues(Color _color, float _smooth, float _metal)
            {
                color = _color;
                Smoothness = _smooth;
                Metallic = _metal;
            }
        }
    }

}
