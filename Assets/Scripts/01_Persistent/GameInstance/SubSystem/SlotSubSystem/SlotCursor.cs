using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotCursor
{
    private CursorInUI cachedUICursor;

    public void Init()
    {
        var instance = GameInstance.Instance;
        var uiGroup = instance.UI_GetPersistentUIGroup();
        cachedUICursor = uiGroup.GetCursorCanvas().GetUICursor();
    }
     
    // ActiveTooltipInfo(_itemId, _type);
    // 
    // cachedUICursor.ActiveTooltipInfo(_itemId, _type);
    //     slotCursor
    // 
    // 
    //     ////////////////////////////////////
    //     // 여기서 아이템 타입에 따라서
    //     // 어떤 u,x,v가 사용될지를 설정해줘야함
    //     cachedUICursor.ClearUseTip();
     
    public void RenewHoverSlot(EItemSlotType _type, EItemID _itemId, bool _isActiveStore)
    {
        cachedUICursor.ActiveTooltipInfo(_itemId, _type);
        ////////////////////////////////////
        // 여기서 아이템 타입에 따라서
        // 어떤 u,x,v가 사용될지를 설정해줘야함   
        cachedUICursor.ClearUseTip();
          
        // Hover정보 
        EItemType itemType = DuckUtill.GetItemTypeByItemID(_itemId);
        switch (_type)
        {
            case EItemSlotType.Equip:
                {
                    RenewEquipUseTip(_itemId);
                }
                break;
            case EItemSlotType.Backpack:
                {
                    RenewBackpackUseTip(_itemId, itemType);
                }
                break;

            case EItemSlotType.ItemBox:
                {
                    RenewItemBoxUseTip(_itemId, itemType);
                }
                break;

            case EItemSlotType.Store:
                {
                    RenewStoreUseTip(_itemId, itemType);
                }
                break;

            case EItemSlotType.Quick:
                {
                    RenewQuickUseTip();
                }
                break;

            case EItemSlotType.Attach:
                {
                    RenewAttachUseTip();
                }
                break;

            case EItemSlotType.WareHouse:
                {
                    RenewWarehouseUseTip(); 
                }
                break;
        } 

        cachedUICursor.ActiveHowToUse();
    }
    public void ChangePlay()
    {
        cachedUICursor.DisableItemSlotSelectList();
    }
    public void ClearHoverInfo()
    {
        cachedUICursor.DisableItemTooltip();
    } 
    public void ClearSelectInfo()
    {
        cachedUICursor.DisableItemSlotSelectList();
    }
    public void ActiveDrageItem(EItemID _itemId)
    {
        cachedUICursor.ActiveDrageItem(_itemId);
    } 
    public void DisableDragItem()
    {
        cachedUICursor.DisableDragItem();
    }

    public void RSelect(RectTransform _rectTransform)
    {
        cachedUICursor.ClearSlotSelectList();
        List<(string key, string desc)> listTips = cachedUICursor.GetAllTips();
        foreach (var item in listTips)
        {
            cachedUICursor.InsertItemSlotSelect(item.key, item.desc);
        }

        // 위치를 세팅해줘야함, 해당 아이템 슬롯의 RectTransform을 받아와야함
        cachedUICursor.ActiveItemSlotSelect(_rectTransform);

        cachedUICursor.GetRaycaster();
    }
    public GraphicRaycaster GetRaycaster()
    {
        return cachedUICursor.GetRaycaster();
    } 

    private void RenewEquipUseTip(EItemID _itemId)
    {
        cachedUICursor.InseretUseTip("F", "해제");
        EEquipSlot equipType = DuckUtill.GetEquipTypeByItemID(_itemId);
        if (equipType == EEquipSlot.Weapon)
        {
            cachedUICursor.InseretUseTip("U", "총알 빼기");
        } 
    }  
     
    private void RenewBackpackUseTip(EItemID _itemId, EItemType _itemType)
    {
        switch (_itemType)
        {
            case EItemType.Equipment:
                {
                    EEquipSlot equipType = DuckUtill.GetEquipTypeByItemID(_itemId);
                     
                    cachedUICursor.InseretUseTip("F", "장착");
                    if (equipType == EEquipSlot.Weapon)
                    {
                        cachedUICursor.InseretUseTip("U", "총알 빼기");
                    }
                    cachedUICursor.InseretUseTip("X", "버리기");
                }
                break;

            case EItemType.Attachment:
                {
                    cachedUICursor.InseretUseTip("X", "버리기");
                    cachedUICursor.InseretUseTip("U", "부착");
                }
                break;

            case EItemType.Consumable:
                {
                    if (DuckUtill.GetConsumTypeByItemID(_itemId) != EConsumableType.Bullet)
                    {
                        cachedUICursor.InseretUseTip("F", "등록");
                        cachedUICursor.InseretUseTip("U", "사용");
                    }

                    cachedUICursor.InseretUseTip("X", "버리기");
                }
                break;

            case EItemType.Material:
                {
                    cachedUICursor.InseretUseTip("X", "버리기");
                }
                break;
        }
    }
    private void RenewItemBoxUseTip(EItemID _itemId, EItemType _itemType)
    {
        switch (_itemType)
        {
            case EItemType.Attachment:
                {
                    cachedUICursor.InseretUseTip("F", "획득");
                    cachedUICursor.InseretUseTip("U", "부착");
                }
                break;
                 
            case EItemType.Equipment:
                {
                    cachedUICursor.InseretUseTip("F", "획득");
                    EEquipSlot equipType = DuckUtill.GetEquipTypeByItemID(_itemId);
                    if (equipType == EEquipSlot.Weapon)
                    {
                        cachedUICursor.InseretUseTip("U", "총알 빼기");
                    }
                }
                break;

            case EItemType.Consumable:
                {
                    cachedUICursor.InseretUseTip("F", "획득");

                    if (DuckUtill.GetConsumTypeByItemID(_itemId) != EConsumableType.Bullet)
                    {
                        cachedUICursor.InseretUseTip("U", "사용");
                    }
                }
                break;

            case EItemType.Material:
                {
                    cachedUICursor.InseretUseTip("F", "획득");
                }
                break;
        }
    }
    private void RenewStoreUseTip(EItemID _itemId, EItemType _itemType)
    {
        cachedUICursor.InseretUseTip("F", "구매");
    }
    private void RenewQuickUseTip()
    {
        cachedUICursor.InseretUseTip("F", "해제");
    }
    private void RenewAttachUseTip()
    {
        cachedUICursor.InseretUseTip("F", "해제");
    }
    private void RenewWarehouseUseTip()
    {
        cachedUICursor.InseretUseTip("F", "빼기");
    }  
} 
 