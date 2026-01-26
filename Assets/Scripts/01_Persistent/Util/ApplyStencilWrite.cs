using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer))]
public class ApplyStencilWrite : MonoBehaviour
{
    [SerializeField] Material stencilWriteMaterial;
    [SerializeField] int stencilRef = 4;
    [SerializeField] int renderQueue = 1999;

    static readonly int StencilID = Shader.PropertyToID("_StencilID");

    void Awake() 
    {
        if (!stencilWriteMaterial)
            return;
        var mat = new Material(stencilWriteMaterial);
        mat.SetInt(StencilID, stencilRef);
        mat.renderQueue = renderQueue;
        GetComponent<MeshRenderer>().material = mat;
    } 
}
