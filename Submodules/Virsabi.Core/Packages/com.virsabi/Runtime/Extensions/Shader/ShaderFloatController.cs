using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Virsabi;

public class ShaderFloatController : MonoBehaviour
{
    [SerializeField]
    private ShaderFloat shaderFloat;

    [SerializeField]
    public float value;

    [SerializeField]
    private Material mat;

    private void OnValidate()
    {
        mat = GetComponent<Renderer>().sharedMaterial;
    }

    [ButtonMethod]
    private void InstantiateMaterial()
    {
        GetComponent<Renderer>().material = mat;
    }

    // Update is called once per frame
    void Update()
    {
        mat.SetFloat(shaderFloat.FloatName, value);
    }
}
