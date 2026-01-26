using UnityEngine;
using UnityEngine.UI;

public class StoreSellBoundary : MonoBehaviour
{
    [SerializeField] private Image boundrayImage;
     
    public bool IsMouseInside(Vector2 screenPos)
    {
        if (!boundrayImage || !gameObject.activeInHierarchy)
            return false;
         
        return RectTransformUtility.RectangleContainsScreenPoint(
            boundrayImage.rectTransform,
            screenPos,
            null
        );
    } 
}
