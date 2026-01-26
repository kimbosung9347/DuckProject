using UnityEngine;
using UnityEngine.UI;

public class CircleGauge : MonoBehaviour
{
    [SerializeField] Texture2D mainTexture;
    private Image cachedImage; 
      
    private void Awake() 
    {
        cachedImage = GetComponent<Image>();
        cachedImage.material = new Material(cachedImage.material);
        cachedImage.material.SetTexture("_MainTex", mainTexture);
    } 

    public void RenewGauge(float _gauge)
    {
        cachedImage.material.SetFloat("_Gauge", _gauge);
    }

}
