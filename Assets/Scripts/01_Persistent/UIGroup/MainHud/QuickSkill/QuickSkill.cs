using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class QuickSkill : MonoBehaviour
{ 
    [SerializeField] private Image coolSkillImage;
    [SerializeField] private Image coolTimeImage;
    [SerializeField] private TextMeshProUGUI coolTimeText;

    private static readonly int FillID = Shader.PropertyToID("_Fill");
    private Material coolTimeMaterial;
     
    private void Awake()
    {
        coolTimeMaterial = coolTimeImage.material = new Material(coolTimeImage.material);
    }   

    /// 0~1 쿨타임 Fill 설정
    public void SetCoolTimeFill(float value)
    {
        value = Mathf.Clamp01(value);
        coolTimeMaterial.SetFloat(FillID, value);
    } 
     
    /// 남은 시간 텍스트 설정 
    public void SetCoolTimeText(float remainSeconds)
    {
        coolTimeText.text = Mathf.CeilToInt(remainSeconds).ToString();
    } 
     
    public void ActiveCoolTime(bool _isActive)
    {
        coolTimeText.gameObject.SetActive(_isActive);
        coolTimeImage.gameObject.SetActive(_isActive); 

        if (_isActive)
        {
            coolSkillImage.color = Color.black;
        } 
        else 
        {
            coolSkillImage.color = Color.white;
        } 
    }
}
