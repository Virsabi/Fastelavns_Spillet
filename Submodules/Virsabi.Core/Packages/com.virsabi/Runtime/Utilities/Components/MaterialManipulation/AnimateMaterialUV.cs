using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;


/// <summary>
/// Animates this renderers material uv
/// </summary>
/// 

// TODO: Add funcitonality to scroll y and/or X

namespace Virsabi.Misc
{
    //[ExecuteInEditMode]
    public class AnimateMaterialUV : MonoBehaviour
    {
        /// <summary>
        /// Multiplied with deltaTime
        /// </summary>
        public float Input;

        public string textureString = "_BaseMap";

        public bool useMaterialPropertyBlock;

        [Separator("Debug Values")]

        public float debugOffset;

        [ReadOnly, SerializeField]
        private float speed;

        [ReadOnly, SerializeField]
        private Material _mat;

        [ReadOnly, SerializeField]
        private float current = 0;

        [ReadOnly, SerializeField]
        private MaterialPropertyBlock _propBlock;

        [ReadOnly, SerializeField]
        private Renderer _renderer;

        [ReadOnly, SerializeField]
        private float originalXTile;

        [SerializeField]
        private bool instantiateMaterial;

        private void OnValidate()
        {
            GetMaterial();

        }

        [ButtonMethod]
        public void GetMaterial()
        {
            if (useMaterialPropertyBlock)
            {
                _propBlock = new MaterialPropertyBlock();
                _renderer = GetComponent<Renderer>();
                _mat = _renderer.sharedMaterials[0];
                originalXTile = _mat.GetTextureOffset(textureString).x;
            }
            else
            {
                _renderer = GetComponent<Renderer>();
                _mat = _renderer.sharedMaterials[0];
                originalXTile = _mat.GetTextureOffset(textureString).x;
            }
        }

        private void Awake()
        {
            if (useMaterialPropertyBlock)
            {
                _propBlock = new MaterialPropertyBlock();
                _renderer = GetComponent<Renderer>();
                _mat = _renderer.sharedMaterial;
                originalXTile = _mat.GetTextureOffset(textureString).x;
            }
            else if (instantiateMaterial)
            {
                _renderer = GetComponent<Renderer>();
                _mat = _renderer.materials[0];
                originalXTile = _mat.GetTextureOffset(textureString).x;
            }
            else
            {
                _renderer = GetComponent<Renderer>();
                _mat = _renderer.sharedMaterials[0];
                originalXTile = _mat.GetTextureOffset(textureString).x;
            }
        }

        // Update is called once per frame
        void Update()
        {
            speed = Input;


            //originalXTile = _mat.GetTextureOffset(textureString).x;

            if ((speed <= 0.1 && speed > 0) || (speed >= -0.101 && speed < 0)) //clamping small values
                speed = 0;


            if (useMaterialPropertyBlock)
            {
                _renderer.GetPropertyBlock(_propBlock, 0);
                _propBlock.SetVector(textureString, new Vector4(0.5f, 0.5f, originalXTile, current));
                _renderer.SetPropertyBlock(_propBlock, 0);
            }
            else
            {
                _renderer.materials[0].SetTextureOffset(textureString, new Vector2(originalXTile, current));
            }

            current = current + speed * Time.deltaTime;

        }
    }

}