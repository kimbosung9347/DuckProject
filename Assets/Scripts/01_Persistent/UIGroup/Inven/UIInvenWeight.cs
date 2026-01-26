using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInvenWeight : MonoBehaviour
{
    [SerializeField] Image weightGuageImage;
    [SerializeField] TextMeshProUGUI weightText;
     
    [SerializeField] Color normalColor;
    [SerializeField] Color warningColor;


    private void Awake()
    {
        weightGuageImage.material = new Material(weightGuageImage.material);
    }

    public void RenewWeight(float _cur, float _max)
    { 
        weightText.text = $"{_cur:F2} / {_max}kg";

        float ratio = _cur / _max;
        weightGuageImage.material.SetFloat("_Progress", ratio);
         
        // _FillColor;
        if (ratio > DuckDefine.WARNING_WEIGHT_RATIO)
        {
            weightGuageImage.material.SetColor("_FillColor", warningColor);
        } 
        else
        {
            weightGuageImage.material.SetColor("_FillColor", normalColor);
        }
    }
}
