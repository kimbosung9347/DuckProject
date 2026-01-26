using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class UIRoundBox : MonoBehaviour
{
    [SerializeField] float radius = 40f; 
     
    private RectTransform cachedRectTransform;
    private Material cachedMaterial;
       
    void Awake()
    {
        cachedRectTransform = GetComponent<RectTransform>();
        var image = GetComponent<Image>();
        cachedMaterial = image.material; 
        RenewRectSize();
        RenewRectRadius(); 
    }
    void OnRectTransformDimensionsChange()
    {
        if (cachedMaterial != null)
            RenewRectSize();
    }

    private void RenewRectSize()
    {
        Vector2 size = cachedRectTransform.rect.size;
        cachedMaterial.SetVector("_RectSize", size);
    }
    private void RenewRectRadius() 
    {
        cachedMaterial.SetFloat("_Radius", radius);
    } 
}
