using UnityEngine;
using UnityEngine.UI;

public class ItemBillboard : MonoBehaviour
{
    [SerializeField] private GameObject itemCanvas;

    private DetectionTarget cachedDetectionTarget;

    private void Awake() 
    {
        cachedDetectionTarget = GetComponent<DetectionTarget>(); 
    }

    public DetectionTarget GetDetectionTarget() {  return cachedDetectionTarget; }
    public void SetSprite(Sprite _sprite) 
    {
        var image = itemCanvas.GetComponentInChildren<Image>();
        image.sprite = _sprite;  
    }  
      
}
