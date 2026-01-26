using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CursorRelad : MonoBehaviour
{
    [SerializeField] Image gaugeImage;
    [SerializeField] TextMeshProUGUI gaugeText;

    private void Awake()
    {
        gaugeImage.material = new Material(gaugeImage.material);
        gameObject.SetActive(false);
    } 

    public void RenewGauge(float _ratio)
    {
        gaugeImage.material.SetFloat("_Gauge", _ratio);
        gaugeText.text = Mathf.RoundToInt(_ratio * 100f).ToString();
    } 
} 
 