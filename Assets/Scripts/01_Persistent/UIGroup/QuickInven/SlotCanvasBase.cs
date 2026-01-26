using UnityEngine;
 
public class SlotCanvasBase : MonoBehaviour
{
    protected static void RenewSlotRenderInfo(UIItemSlotBase _slot, EItemID _id, EItemGrade _grade, string _name, Sprite _sprite)
    {
        _slot.RenewItemID(_id);
        _slot.RenewItemGrade(_grade);
        _slot.RenewItemName(_name);
        _slot.RenewSprite(_sprite);
        _slot.SetShowSlotDesc(EShowSlotDesc.None);
    }  
    protected static void RenewSlotCnt(UIItemSlotBase _slot, int _cnt)
    {
        _slot.RenewItemCnt(_cnt);
    }
    protected static void RenewSlotDurability(UIItemSlotBase _slot, float _ratio)
    {
        _slot.RenewDurability(_ratio);
    } 
    protected static void RenewSlotAttachment(UIItemSlotBase _slot, EAttachSlotState _muzzle, EAttachSlotState _scope, EAttachSlotState _stock)
    {
        _slot.RenewAttachment(_muzzle, _scope, _stock);
    }
    protected static void RenewSlotHideAttach(UIItemSlotBase _slot)
    {
        _slot.HideAttachment(); 
    }
    protected static void RenewHideGuageAndCnt(UIItemSlotBase _slot)
    {
        _slot.HideGageAndCnt();
    } 
    protected static void RenewChangeEmpty(UIItemSlotBase _slot)
    {  
        _slot.ChangeEmpty();
    }
     
}
