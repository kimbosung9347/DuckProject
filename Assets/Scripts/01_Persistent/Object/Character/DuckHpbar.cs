using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DuckHpbar : MonoBehaviour
{
    [SerializeField] private GameObject hpBarObject;
    [SerializeField] private Image hpBar;
    [SerializeField] private TextMeshProUGUI duckName;
        
    private Material hpMaterial;
    private RectTransform hpBarRectTransform;

    private static readonly int CurHP = Shader.PropertyToID("_CurHP");
    private static readonly int LossStart = Shader.PropertyToID("_LossStart");
    private static readonly int LossEnd = Shader.PropertyToID("_LossEnd");
    private static readonly int ShowLoss = Shader.PropertyToID("_ShowLoss");
    private static readonly int HPBarHeight = Shader.PropertyToID("_HPBarHeight");
    private static readonly int LossBarHeight = Shader.PropertyToID("_LossBarHeight");
    private static readonly int HPColor = Shader.PropertyToID("_HPColor");
    private static readonly int LossColor = Shader.PropertyToID("_LossColor");
     
    private void Awake()
    {
        hpMaterial = hpBar.material = new Material(hpBar.material);
        hpBarRectTransform = hpBar.gameObject.GetComponent<RectTransform>();
    }   
     

    public void Active()
    {
        hpBarObject.gameObject.SetActive(true);
        duckName.gameObject.SetActive(true); 
    }
    public void Disable()
    {
        hpBarObject.gameObject.SetActive(false);
        duckName.gameObject.SetActive(false);
    } 


    public void SetName(string _name)
    {
        duckName.gameObject.SetActive(true);
        duckName.text = _name; 
    }
    public void SetHpBarSize(float _size)
    {
        hpBarRectTransform.localScale = new Vector2(
            _size, 
            _size
        ); 
    }
    public void SetHP(float value)
    {
        hpMaterial.SetFloat(CurHP, Mathf.Clamp01(value));
    }
    public void SetLossRange(float start, float end)
    {
        hpMaterial.SetFloat(LossStart, Mathf.Clamp01(start));
        hpMaterial.SetFloat(LossEnd, Mathf.Clamp01(end));
    }
    public void ShowLossBar(bool show)
    {
        hpMaterial.SetFloat(ShowLoss, show ? 1f : 0f);
    }
    public void SetHPBarHeight(float height)
    {
        hpMaterial.SetFloat(HPBarHeight, Mathf.Clamp01(height));
    }
    public void SetLossBarHeight(float height)
    {
        hpMaterial.SetFloat(LossBarHeight, Mathf.Clamp01(height));
    }
    public void SetHPColor(Color color)
    {
        hpMaterial.SetColor(HPColor, color);
    }
    public void SetLossColor(Color color)
    {
        hpMaterial.SetColor(LossColor, color);
    }
}
 