using UnityEngine;
using UnityEngine.UI;
  
public class QuickConsum : MonoBehaviour
{ 
    [SerializeField] Image borderImage;
    [SerializeField] Image spriteImage;
    [SerializeField] SlotDesc slotDesc;

    private void Awake()
    {
        InitMaterial();
        RemoveItem();
    }
     
    public void RenewItemSlotRenderInfo(EItemGrade _grede, string _name, Sprite _sprite)
    {
        borderImage.gameObject.SetActive(true);
        spriteImage.gameObject.SetActive(true);
        slotDesc.ChangePush();
         
        spriteImage.sprite = _sprite;
        slotDesc.RenewItemName(_name);
        ActiveMaterialColor(_grede); 
    } 
    public void RenewConsumCnt(int _cnt)
    {
        slotDesc.RenewItemCnt(_cnt);
    } 
    public void RenewCapacity(float _ratio)
    {
        slotDesc.RenewDurability(_ratio);
    } 

    public void RemoveItem()
    {
        borderImage.gameObject.SetActive(false);
        spriteImage.gameObject.SetActive(false);
        slotDesc.ChangeEmpty();
         
        ClearMaterialColor();
    }   
    private void InitMaterial()
    {
        borderImage.material = new Material(borderImage.material);
        ClearMaterialColor();
    }
     
    private void ActiveMaterialColor(EItemGrade _grade)
    {
        var table = GameInstance.Instance.TABLE_GetItemTable();
        Color color;
        switch (_grade)
        {
            case EItemGrade.Normal:
                color = table.gradeNormalColor;
                break;

            case EItemGrade.Rare:
                color = table.gradeRareColor;
                break;

            case EItemGrade.Unique:
                color = table.gradeUniqueColor;
                break;

            case EItemGrade.Epic:
                color = table.gradeEpicColor;
                break;

            case EItemGrade.Legend:
                color = table.gradeLegendColor;
                break;
                 
            default:
                color = new Color32(255, 255, 255, 207);
                break;
        }  
        borderImage.material.SetColor("_baseColor", color);
    } 
   
    private void ClearMaterialColor()
    {
        Color color = Color.white;
        color.a = 0;
        borderImage.material.SetColor("_baseColor", color);
    } 
} 
