using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//////////////////
// ItemSlotType

public enum EItemSlotType
{ 
    Equip,
    Backpack,
    ItemBox,
    Store,
    Quick,
    Attach,
    WareHouse, 
       
    End
}   

public class UIItemSlotBase : MonoBehaviour 
{
    [SerializeField] protected Image border;
    [SerializeField] protected Image sprite;
    [SerializeField] protected SlotDesc slotDesc;
    [SerializeField] protected EItemSlotType slotType = EItemSlotType.End;

    private int index = -1;
    private EItemID itemId = EItemID._END;  
      
    protected virtual void Awake()
    {
        border.material = new Material(border.material);
    }
    private void Start()
    {
         
    }  
     
    public virtual void ChangeEmpty()
    { 
        slotDesc.ChangeEmpty();
        RenewItemGrade(EItemGrade.Normal);
        sprite.gameObject.SetActive(false);
    }      
    public virtual void ChangePush()
    {
        slotDesc.ChangePush();  

        sprite.gameObject.SetActive(true);
        sprite.gameObject.GetComponent<UIAnimation>()?.Action_Animation();
    }     
      
    public void RenewSprite(Sprite _sprite)
    {
        sprite.sprite = _sprite;
    } 
    public void RenewItemID(EItemID _id)
    {
        itemId = _id;
    } 
    public void RenewItemGrade(EItemGrade _grade)
    {   
        Color color = GetColorByGrade(_grade);
        border.material.SetColor("_baseColor", color);
    }   
    public void RenewItemName(string _desc)
    {
        slotDesc.RenewItemName(_desc);
    }   
    public void RenewItemCnt(int _cnt)
    {
        slotDesc.RenewItemCnt(_cnt);  
    }    
    public void RenewDurability(float _ratio)
    { 
        slotDesc.RenewDurability(_ratio);
    }
    public void RenewAttachment(EAttachSlotState _muzzle, EAttachSlotState _scope, EAttachSlotState _stock)
    {
        slotDesc.RenewAttachment(_muzzle, _scope, _stock);
    }  
    public void HideAttachment()
    {
        slotDesc.HideAttachement(); 
    }
    public void HideGageAndCnt()
    {
        slotDesc.HideGageAndCnt(); 
    }

    public void SetIndex(int _index) 
    {  
        index = _index;
    } 
    public void SetShowSlotDesc(EShowSlotDesc _desc)
    {
        slotDesc.SetShowSlotDesc(_desc);
    }

    public void Selected()
    {
        var table = GameInstance.Instance.TABLE_GetItemTable();
        border.color = table.selectBorderColor;
    }
    public void NotSelected()
    {
        border.color = Color.white;
    } 

    public int GetIndex()
    {
        return index;
    }

    public EItemID GetItemID()
    {
        return itemId;
    }
    public EItemSlotType GetItemSlotType()
    {
        return slotType;
    }  
       
    private Color GetColorByGrade(EItemGrade grade)
    {
        var itemTable = GameInstance.Instance.TABLE_GetItemTable();
        if (itemTable == null)
            return new Color(60/255f, 207 / 255f, 46 / 255f, 255/255f);
            
        return grade switch
        {
            EItemGrade.Normal =>    itemTable.gradeNormalColor  , 
            EItemGrade.Rare =>      itemTable.gradeRareColor    ,
            EItemGrade.Unique =>    itemTable.gradeUniqueColor  ,  
            EItemGrade.Epic =>      itemTable.gradeEpicColor    , 
            EItemGrade.Legend =>    itemTable.gradeLegendColor  ,
            _ => Color.black  
        };
    }
}  
