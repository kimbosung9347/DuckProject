using System;
using System.Collections.Generic;

enum EBetwwenSlotType
{
    Equip_Equip,
    Equip_Backpack,
    Equip_ItemBox,
    Equip_Store,
    Equip_Quick,
    Equip_Attach,
    Equip_WareHouse,

    Backpack_Backpack,
    Backpack_ItemBox,
    Backpack_Store,
    Backpack_Quick,
    Backpack_Attach,
    Backpack_WareHouse,

    ItemBox_ItemBox,
    ItemBox_Store,
    ItemBox_Quick,
    ItemBox_Attach,
    ItemBox_WareHouse,

    Store_Store,
    Store_Quick,
    Store_Attach,
    Store_WareHouse,

    Quick_Quick,
    Quick_Attach,
    Quick_WareHouse,

    Attach_Attach,
    Attach_WareHouse,

    WareHouse_WareHouse
}
 
public class SlotHandle 
{ 
    // 캐싱 
    private ItemInfoCanvas cachedItemInfoCanvas;
    private ItemBoxCanvas cachedItemBoxCanvas;
    private StoreCanvas cachedStoreCanvas;
    private WareHouseCanvas cachedWareHouseCanvas;

    private PlayerStorage cachedPlayerStorage;
    private PlayerEquip cachedPlayerEquip; 
    private PlayerQuickSlot cachedPlayerQuickSlot;

    private SlotCheck cachedSlotCheck;
    private UIItemSlotBase lSlectedSlot;
     
    public void Init(SlotCheck slotCheck)
    {
        var instance = GameInstance.Instance;
        var uiGroup = instance.UI_GetPersistentUIGroup();

        cachedItemInfoCanvas = uiGroup.GetItemInfoCanvas();
        cachedItemBoxCanvas = uiGroup.GetItemBoxCanvas();
        cachedStoreCanvas   = uiGroup.GetStoreCanvas();
        cachedWareHouseCanvas = uiGroup.GetWareHouseCanvas();

        cachedPlayerStorage = instance.PLAYER_GetPlayerStorage();
        cachedPlayerEquip = instance.PLAYER_GetPlayerEquip();
        cachedPlayerQuickSlot = instance.PLAYER_GetPlayerQuickSlot();
         
        cachedSlotCheck = slotCheck;
    }

    public bool IsExistItem(EItemSlotType _slotType, int _index)
    {
        switch (_slotType)
        {
            case EItemSlotType.Equip:
                {
                    if (!cachedPlayerEquip.CanEquipInput(_index))
                        return false;
                }
                break;

            case EItemSlotType.Backpack:
                {
                    if (!cachedPlayerStorage.IsEmpty(_index))
                        return false;
                }
                break;

            case EItemSlotType.ItemBox:
                {
                    var itemBox = cachedItemBoxCanvas.GetItemBox();
                    if (!itemBox)
                        return false;

                    if (!itemBox.CanInput(_index))
                        return false;
                }
                break;

            case EItemSlotType.Quick:
                {
                    if (!cachedPlayerQuickSlot.CanInput(_index))
                        return false;
                }
                break;

            case EItemSlotType.Store:
                {
                    var store = cachedStoreCanvas.GetStore();
                    if (!store)
                        return false;

                    if (!store.IsExistItem(_index))
                        return false;
                }
                break;

            case EItemSlotType.Attach:
                {
                    EAttachSlotState attachState = cachedItemInfoCanvas.GetAttachState((EAttachmentType)_index);
                    if (attachState != EAttachSlotState.Exist)
                        return false;
                }
                break;

            case EItemSlotType.WareHouse:
                {
                    var wareHouse = cachedWareHouseCanvas.GetWareHouse();
                    if (!wareHouse)
                        return false;
                     
                    if (!wareHouse.IsExistItem(_index))
                        return false;
                }
                break;
        }

        return true;
    }
    public void DisableLSelectInfo()
    {
        cachedItemInfoCanvas.Disable();
        lSlectedSlot?.NotSelected();
        lSlectedSlot = null;
    }
    public void RenewLClickSlot(ref FItemSlotInfo _slotInfo, UIItemSlotBase _selectedSlot)
    {
        // 창고일 경우 껍데기 데이터름 이용해서 Active해줘야함 
        ItemBase item = null;
        switch (_slotInfo.slotType)
        {
            case EItemSlotType.Equip:
                {
                    item = cachedPlayerEquip.GetItem((EEquipSlot)_slotInfo.slotIndex);
                }
                break;

            case EItemSlotType.Backpack:
                {
                    item = cachedPlayerStorage.GetItem(_slotInfo.slotIndex);
                }
                break;

            case EItemSlotType.ItemBox:
                {
                    item = cachedItemBoxCanvas.GetItemBox().GetItem(_slotInfo.slotIndex);
                }
                break;

            case EItemSlotType.Store:
                {
                    item = cachedStoreCanvas.GetStore().GetItem(_slotInfo.slotIndex);
                }
                break;

            case EItemSlotType.WareHouse:
                {
                    item = cachedWareHouseCanvas.GetWareHouse().GetItem(_slotInfo.slotIndex);
                } 
                break;

            default:
                return;
        }

        // 얘는 왜 아이템으로 전달하나? - 아이템에 따라서 수치값이 달라질 수 있어서
        // 예를 들어 총같은 경우, 총의 기본정보와 부착된 정보가 다르기 떄문임 

        ETradeType tradeType = ETradeType.End;
        var store = cachedStoreCanvas.GetStore();
        if (store)
        {
            if (_slotInfo.slotType == EItemSlotType.Store)
            {
                tradeType = ETradeType.StoreSell;
            }

            else if (_slotInfo.slotType == EItemSlotType.Backpack)
            {
                tradeType = ETradeType.PlayerSell;
            }
        }

        cachedItemInfoCanvas.Active(item, _slotInfo.slotType, tradeType, _slotInfo.slotIndex, store);

         
        lSlectedSlot?.NotSelected();
        lSlectedSlot = _selectedSlot;
        lSlectedSlot.Selected();
    }
    public bool TryBuy(ref FItemSlotInfo _selectSlot)
    {
        if (_selectSlot.slotType == EItemSlotType.Store)
        {
            // 구매 가능체크
            if (!cachedSlotCheck.CanBuy(_selectSlot.slotIndex))
                return false;
             
            var backpackItemInfo = new FItemSlotInfo();
            backpackItemInfo.slotType = EItemSlotType.Backpack;
            backpackItemInfo.slotIndex = cachedPlayerStorage.GetEmptyIndex();
                
            TransferStore_ToBackpack(ref _selectSlot, ref backpackItemInfo);
        } 

        else if (_selectSlot.slotType == EItemSlotType.Backpack)
        {

        }

        return true;
    }
 
    public void InteractionBetwwenSlot(ref FItemSlotInfo _drag, ref FItemSlotInfo _ray)
    {
        // 해당 슬롯과 슬롯관의 관계를 계산하고 
        EBetwwenSlotType betweenSlotType = GetBetweenSlotType(_drag.slotType, _ray.slotType);

        // 그 관계간의 조합들에 맞는 기능들을 호출함, 드래그나, 즉시 이동 모두 이 함수를 통해서 계산 됩니다
        switch (betweenSlotType)
        {
            case EBetwwenSlotType.Equip_Backpack:
                {
                    // 넣을 때, 장착아이템인지 검사
                    if (_drag.slotType == EItemSlotType.Backpack)
                    {
                        if (DuckUtill.GetEquipTypeByItemID(_drag.itemId) != (EEquipSlot)_ray.slotIndex)
                            break;
                    }  

                    if (!CanDropBackpack(ref _drag, ref _ray))
                        break;

                    TransferItemBetweenSlots(ref _drag, ref _ray);
                }
                break;
            case EBetwwenSlotType.Equip_ItemBox:
                {
                    var dragItem = GetItemFromSlot(_drag.slotType, _drag.slotIndex);
                    var rayItem = GetItemFromSlot(_ray.slotType, _ray.slotIndex);
                    
                    EEquipSlot targetEquipSlot = EEquipSlot.End;
                     
                    if (_drag.slotType == EItemSlotType.Equip)
                    {
                        targetEquipSlot = (EEquipSlot)_drag.slotIndex;
                        if (rayItem && targetEquipSlot != DuckUtill.GetEquipTypeByItemID(rayItem.GetItemData().itemID))    
                            break; 
                    }  
                    else if (_ray.slotType == EItemSlotType.Equip)
                    {
                        targetEquipSlot = (EEquipSlot)_ray.slotIndex;
                        if (dragItem && targetEquipSlot != DuckUtill.GetEquipTypeByItemID(dragItem.GetItemData().itemID))
                            break;  
                    } 
                     
                    TransferItemBetweenSlots(ref _drag, ref _ray);
                } 
                break;

            case EBetwwenSlotType.Backpack_ItemBox:
            case EBetwwenSlotType.Backpack_WareHouse: 
                {
                    if (_drag.slotType == EItemSlotType.Backpack)
                    {
                        TransferBackpack_To(ref _drag, ref _ray);
                    } 

                    else if (_ray.slotType == EItemSlotType.Backpack)
                    {
                        Transfer_ToBackpack(ref _drag, ref _ray);
                    }   
                }
                break;
            case EBetwwenSlotType.Backpack_Backpack:
                {
                    TransferBackpack_ToBackpack(ref _drag, ref _ray);
                } 
                break; 
            case EBetwwenSlotType.Backpack_Store:
                { 
                    // 상점에서 가방
                    if (_drag.slotType == EItemSlotType.Store)
                    {
                        // 돈이 가능한지 체크,
                        if (!cachedSlotCheck.CanBuy(_drag.slotIndex))
                            return;    
                          
                        TransferStore_ToBackpack(ref _drag, ref _ray); 
                    } 
                     
                    // 가방에서 상점은 -> 상점 영역 바운더리로 체킹함
                }
                break;
            case EBetwwenSlotType.Backpack_Quick:
                {
                    if (_drag.slotType == EItemSlotType.Backpack)
                    {
                        TransferBackpack_ToQuickSlot(ref _drag, ref _ray);
                    }

                    else if (_drag.slotType == EItemSlotType.Quick)
                    { 
                        TransferQuickSlot_ToBackpack(_drag.slotIndex);
                    } 
                } 
                break;
                 
            case EBetwwenSlotType.Backpack_Attach:
            case EBetwwenSlotType.ItemBox_Attach:
            case EBetwwenSlotType.Attach_WareHouse:
            {
                if (_drag.slotType != EItemSlotType.Attach)
                {
                    Transfer_ToAttach(ref _drag, ref _ray);
                }

                else if (_drag.slotType == EItemSlotType.Attach)
                {
                    Transfer_FromAttach(ref _ray, ref _drag);
                } 
            }
            break;

            case EBetwwenSlotType.WareHouse_WareHouse:
            case EBetwwenSlotType.ItemBox_ItemBox:
            {
                TransferItemBetweenSlots(ref _drag, ref _ray);
            }
            break;
                  
            case EBetwwenSlotType.Quick_Quick:
                {
                    cachedPlayerQuickSlot.SwitchBtweenQuickSlot(_drag.slotIndex, _ray.slotIndex);
                }  
                break; 
                 
            // 불가능항목들 
            case EBetwwenSlotType.Attach_Attach:
            case EBetwwenSlotType.Quick_Attach:
            case EBetwwenSlotType.Quick_WareHouse:
            case EBetwwenSlotType.Store_WareHouse:
            case EBetwwenSlotType.Store_Attach:
            case EBetwwenSlotType.Store_Quick:
            case EBetwwenSlotType.Store_Store:
            case EBetwwenSlotType.ItemBox_Quick:
            case EBetwwenSlotType.ItemBox_Store:
            case EBetwwenSlotType.Equip_Store:
            case EBetwwenSlotType.Equip_WareHouse:
            case EBetwwenSlotType.Equip_Attach:
            case EBetwwenSlotType.Equip_Quick:
            case EBetwwenSlotType.Equip_Equip:
                break;
        }
    }
    public void InteractionBySitulation(ref FItemSlotInfo _slotInfo, EItemSlotSelectType _type)
    {
        // 즉시 획득
        switch (_slotInfo.slotType)
        {
            case EItemSlotType.Equip:
                {
                    SelectEquipSlot(_type, ref _slotInfo);
                }
                break;

            case EItemSlotType.Backpack:
                {
                    SelectBackpackSlot(_type, ref _slotInfo);
                }
                break;

            case EItemSlotType.ItemBox:
                {
                    SelectItemBoxSlot(_type, ref _slotInfo);
                }
                break;

            case EItemSlotType.Store:
                {
                    SelectStoreSlot(_type, ref _slotInfo);
                }
                break;

            case EItemSlotType.Quick:
                {
                    SelectQuickSlot(_type, ref _slotInfo);
                } 
                break;

            case EItemSlotType.Attach:
                {
                    SelectAttachSlot(_type, ref _slotInfo);
                }
                break;

            case EItemSlotType.WareHouse:
                {
                    SelectWarehouseSlot(_type, ref _slotInfo);
                }
                break;
        } 
    }
    public void SortPlayerInventroy(SortedSet<int> _empty, SortedSet<int> _fill)
    {
        // 앞쪽 빈 슬롯부터 채우기
        while (_empty.Count > 0 && _fill.Count > 0)
        { 
            int emptyIndex = _empty.Min;
            int fillIndex = _fill.Min;
             
            // 채워진 슬롯이 빈 슬롯보다 뒤에 있을 때만 이동
            if (fillIndex > emptyIndex)
            {
                _empty.Remove(emptyIndex);
                _fill.Remove(fillIndex);

                TransferBackpack_ToEmptyBackpack(fillIndex, emptyIndex);

                _fill.Add(emptyIndex);
                _empty.Add(fillIndex);
            }
            else
            {
                // 이 fillIndex는 이미 제자리
                _fill.Remove(fillIndex);
            }
        }
    }
    public void PushBackpackToWare()
    {
        int curItemCnt = cachedPlayerStorage.GetCurItemCnt();
        var wareHouse = cachedWareHouseCanvas.GetWareHouse();
        
        int canStoreCnt = wareHouse.GetEmptySlotCount();
        if (canStoreCnt == 0)
            return; 
         
        List<int> emptyWareIndices = wareHouse.GetEmptySlotIndices(canStoreCnt);
        if (emptyWareIndices.Count == 0)
            return;
        
        SortedSet<int> backpackFillSet = cachedPlayerStorage.GetFillSlotSet();
        if (backpackFillSet.Count == 0)
            return;
         
        var backpackInfo = new FItemSlotInfo();
        backpackInfo.slotType = EItemSlotType.Backpack;

        var wareHouseInfo = new FItemSlotInfo();
        wareHouseInfo.slotType = EItemSlotType.WareHouse;
         
        using var wareEnum = emptyWareIndices.GetEnumerator();
        foreach (int backpackIndex in backpackFillSet)
        {
            // 창고가 다 찬 상태
            if (!wareEnum.MoveNext())
                break;
             
            backpackInfo.slotIndex = backpackIndex;
            wareHouseInfo.slotIndex = wareEnum.Current;
            TransferBackpack_ToEmptyWareHouse(ref backpackInfo, ref wareHouseInfo);
        } 
      
        GameInstance.Instance.SOUND_PlaySoundSfx(ESoundSfxType.PickUp, new UnityEngine.Vector3());
    }

    public void SellItem(ref FItemSlotInfo _sellInfo)
    { 
        if (!(_sellInfo.slotType == EItemSlotType.Equip || _sellInfo.slotType == EItemSlotType.Backpack))
        {
            return;
        }
         
        var store = cachedStoreCanvas.GetStore();
        if (!store)
            return;

        // 가방인 경우 퀵슬롯을 체크해야함 
        var removedSellItem = RemoveItemFromSlot(ref _sellInfo);
        cachedPlayerStorage.AcquireMoney(store.GetPlayerSellPrice(removedSellItem.GetPrice()));
        cachedStoreCanvas.ComplateBuyOrSell(false);
         
        // 만약에 가방에 있던 인덱스가 퀵슬롯이라면, 해당 퀵슬롯에 다른 아이템 아이디로 Track아이디를 변경해주거나 없애줘야함
        if (_sellInfo.slotType == EItemSlotType.Backpack)
        {
            bool isTrackBackpack = cachedPlayerQuickSlot.HasSlotByTrackIndex(_sellInfo.slotIndex);
            if (isTrackBackpack)
            {
                int slotIndex = cachedPlayerQuickSlot.GetSlotByTrackId(_sellInfo.slotIndex);
                cachedPlayerQuickSlot.DisableOrChangeTrack(slotIndex);
            }
        }
    } 

    private EBetwwenSlotType GetBetweenSlotType(EItemSlotType a, EItemSlotType b)
    {
        // 순서 고정
        if (a > b)
            (a, b) = (b, a);
         
        return (a, b) switch
        {
            (EItemSlotType.Equip, EItemSlotType.Equip) => EBetwwenSlotType.Equip_Equip,
            (EItemSlotType.Equip, EItemSlotType.Backpack) => EBetwwenSlotType.Equip_Backpack,
            (EItemSlotType.Equip, EItemSlotType.ItemBox) => EBetwwenSlotType.Equip_ItemBox,
            (EItemSlotType.Equip, EItemSlotType.Store) => EBetwwenSlotType.Equip_Store,
            (EItemSlotType.Equip, EItemSlotType.Quick) => EBetwwenSlotType.Equip_Quick,
            (EItemSlotType.Equip, EItemSlotType.Attach) => EBetwwenSlotType.Equip_Attach,
            (EItemSlotType.Equip, EItemSlotType.WareHouse) => EBetwwenSlotType.Equip_WareHouse,

            (EItemSlotType.Backpack, EItemSlotType.Backpack) => EBetwwenSlotType.Backpack_Backpack,
            (EItemSlotType.Backpack, EItemSlotType.ItemBox) => EBetwwenSlotType.Backpack_ItemBox,
            (EItemSlotType.Backpack, EItemSlotType.Store) => EBetwwenSlotType.Backpack_Store,
            (EItemSlotType.Backpack, EItemSlotType.Quick) => EBetwwenSlotType.Backpack_Quick,
            (EItemSlotType.Backpack, EItemSlotType.Attach) => EBetwwenSlotType.Backpack_Attach,
            (EItemSlotType.Backpack, EItemSlotType.WareHouse) => EBetwwenSlotType.Backpack_WareHouse,

            (EItemSlotType.ItemBox, EItemSlotType.ItemBox) => EBetwwenSlotType.ItemBox_ItemBox,
            (EItemSlotType.ItemBox, EItemSlotType.Store) => EBetwwenSlotType.ItemBox_Store,
            (EItemSlotType.ItemBox, EItemSlotType.Quick) => EBetwwenSlotType.ItemBox_Quick,
            (EItemSlotType.ItemBox, EItemSlotType.Attach) => EBetwwenSlotType.ItemBox_Attach,
            (EItemSlotType.ItemBox, EItemSlotType.WareHouse) => EBetwwenSlotType.ItemBox_WareHouse,

            (EItemSlotType.Store, EItemSlotType.Store) => EBetwwenSlotType.Store_Store,
            (EItemSlotType.Store, EItemSlotType.Quick) => EBetwwenSlotType.Store_Quick,
            (EItemSlotType.Store, EItemSlotType.Attach) => EBetwwenSlotType.Store_Attach,
            (EItemSlotType.Store, EItemSlotType.WareHouse) => EBetwwenSlotType.Store_WareHouse,

            (EItemSlotType.Quick, EItemSlotType.Quick) => EBetwwenSlotType.Quick_Quick,
            (EItemSlotType.Quick, EItemSlotType.Attach) => EBetwwenSlotType.Quick_Attach,
            (EItemSlotType.Quick, EItemSlotType.WareHouse) => EBetwwenSlotType.Quick_WareHouse,

            (EItemSlotType.Attach, EItemSlotType.Attach) => EBetwwenSlotType.Attach_Attach,
            (EItemSlotType.Attach, EItemSlotType.WareHouse) => EBetwwenSlotType.Attach_WareHouse,

            (EItemSlotType.WareHouse, EItemSlotType.WareHouse) => EBetwwenSlotType.WareHouse_WareHouse,

            _ => throw new ArgumentOutOfRangeException(
                $"Invalid BetweenSlotType : {a} <-> {b}")
        };
    }
    private ItemBase GetItemFromSlot(ref FItemSlotInfo _slotInfo)
    {
        return GetItemFromSlot(_slotInfo.slotType, _slotInfo.slotIndex);
    }
    private ItemBase GetItemFromSlot(EItemSlotType _type, int _index)
    {
        switch (_type)
        {
            case EItemSlotType.ItemBox:
                {
                    ItemBoxBase box = cachedItemBoxCanvas.GetItemBox();
                    return box.GetItem(_index);
                }

            case EItemSlotType.Backpack:
                {
                    return cachedPlayerStorage.GetItem(_index);
                }

            case EItemSlotType.Equip:
                {
                    return cachedPlayerEquip.GetItem((EEquipSlot)_index);
                } 

            case EItemSlotType.Store:
                {
                    return cachedStoreCanvas.GetStore().GetItem(_index);
                }

            case EItemSlotType.Attach:
                {
                    return cachedItemInfoCanvas.GetAttach((EAttachmentType)_index);
                }
                 
            case EItemSlotType.WareHouse:
                {
                    return cachedWareHouseCanvas.GetWareHouse().GetItem(_index);
                }
        }
        return null;
    }

    private ItemBase RemoveItemFromSlot(ref FItemSlotInfo _slotInfo)
    {
        return RemoveItemFromSlot(_slotInfo.slotType, _slotInfo.slotIndex);
    } 
    private ItemBase RemoveItemFromSlot(EItemSlotType _type, int _index)
    {
        switch (_type)
        {
            case EItemSlotType.ItemBox:
                {
                    ItemBoxBase box = cachedItemBoxCanvas.GetItemBox();
                    return box.RemoveItem(_index);
                }

            case EItemSlotType.Backpack:
                {
                    return cachedPlayerStorage.RemoveItem(_index, false);
                }

            case EItemSlotType.Equip:
                {
                    return cachedPlayerEquip.DetachEquip((EEquipSlot)_index);
                }

            case EItemSlotType.Store:
                {
                    return cachedStoreCanvas.GetStore().SellItem(_index);
                }

            case EItemSlotType.Attach:
                {
                    return cachedItemInfoCanvas.RemoveAttach((EAttachmentType)_index);
                }

            case EItemSlotType.WareHouse:
                {
                    return cachedWareHouseCanvas.GetWareHouse().RemoveItem(_index);
                }
        }
        return null;
    }

    private void InsertItem(EItemSlotType _type, int _index, ItemBase _item)
    {
        switch (_type)
        {
            case EItemSlotType.ItemBox:
                {
                    ItemBoxBase box = cachedItemBoxCanvas.GetItemBox();
                    box.InsertItem(_index, _item);
                }
                break;

            case EItemSlotType.Backpack:
                {
                    cachedPlayerStorage.InsertItem(_index, _item);
                }
                break;

            case EItemSlotType.Equip:
                {
                    cachedPlayerEquip.EquipItem((EEquipSlot)_index, _item);
                }
                break;

            case EItemSlotType.Attach:
                {
                    cachedItemInfoCanvas.CheckAndInsertAttachment(_index, _item);
                }
                break;

            case EItemSlotType.WareHouse:
                {
                    cachedWareHouseCanvas.GetWareHouse()?.InsertItem(_index, _item);
                }
                break; 
        }
    }
      
    private void TransferItemBetweenSlots(ref FItemSlotInfo _from, ref FItemSlotInfo _to)
    {
        ItemBase fromItem = GetItemFromSlot(_from.slotType, _from.slotIndex);
        ItemBase toItem = GetItemFromSlot(_to.slotType, _to.slotIndex);
        
        EItemID fromItemId = fromItem ? fromItem.GetItemData().itemID : EItemID._END;
        EItemID toItemId = toItem ? toItem.GetItemData().itemID : EItemID._END;
   
        // from만 존재 → 단순 이동
        if (fromItem && !toItem)
        {
            RemoveItemFromSlot(_from.slotType, _from.slotIndex);
            InsertItem(_to.slotType, _to.slotIndex, fromItem);
            return;
        }  
         
        // 둘 다 존재
        if (fromItem && toItem)
        {
            RemoveItemFromSlot(_from.slotType, _from.slotIndex);
            RemoveItemFromSlot(_to.slotType, _to.slotIndex);
        
            // 교환
            InsertItem(_to.slotType, _to.slotIndex, fromItem);
            InsertItem(_from.slotType, _from.slotIndex, toItem);
            return;
        }
    }
    private void Transfer_ToBackpack(ref FItemSlotInfo _itemBox, ref FItemSlotInfo _backpack)
    {
        var itemBoxItem = GetItemFromSlot(ref _itemBox);
        var backpackItem = GetItemFromSlot(ref _backpack);

        if (!itemBoxItem)
            return; 
         
        bool isSwitch = (backpackItem != null); 
        if (isSwitch)
        {
            var removedItemBoxItem = RemoveItemFromSlot(ref _itemBox);
            var removedBackpackItem = RemoveItemFromSlot(ref _backpack);
            InsertItem(_itemBox.slotType, _itemBox.slotIndex, removedBackpackItem);
            InsertItem(_backpack.slotType, _backpack.slotIndex, removedItemBoxItem);
        }   
           
        else
        {
            // 단순 옮기기
            var removedItemBoxItem  = RemoveItemFromSlot(ref _itemBox);
            InsertItem(_backpack.slotType, _backpack.slotIndex, removedItemBoxItem);
        }
    } 
    private void TransferBackpack_To(ref FItemSlotInfo _backpack, ref FItemSlotInfo _itemBox)
    {
        var itemBoxItem = GetItemFromSlot(ref _itemBox);
        var backpackItem = GetItemFromSlot(ref _backpack);

        if (!backpackItem)
            return;

        bool isSwitch = (itemBoxItem != null);
        if (isSwitch)
        {
            var removedItemBoxItem = RemoveItemFromSlot(ref _itemBox);
            var removedBackpackItem = RemoveItemFromSlot(ref _backpack);
            InsertItem(_itemBox.slotType, _itemBox.slotIndex, removedBackpackItem);
            InsertItem(_backpack.slotType, _backpack.slotIndex, removedItemBoxItem);
        } 

        else
        {
            // 단순 옮기기
            var removedBackpackItem = RemoveItemFromSlot(ref _backpack);
            InsertItem(_itemBox.slotType, _itemBox.slotIndex, removedBackpackItem);
        } 
    } 
    private void TransferBackpack_ToEmptyWareHouse(ref FItemSlotInfo _backpack, ref FItemSlotInfo _ware)
    {
        var wareHouse = cachedWareHouseCanvas.GetWareHouse();
        if (!wareHouse)
            return;

        var wareHouseItem = GetItemFromSlot(ref _ware);
        var backpackItem = GetItemFromSlot(ref _backpack);

        if (!backpackItem)
            return;

        if (wareHouseItem)
            return;
         
        var removedBackpackItem = RemoveItemFromSlot(ref _backpack);
        wareHouse.InsertItemByOriginIndex(_ware.slotIndex, removedBackpackItem);
    }
    private void TransferBackpack_ToBackpack(ref FItemSlotInfo _from, ref FItemSlotInfo _to)
    {
        ItemBase fromItem = GetItemFromSlot(_from.slotType, _from.slotIndex);
        ItemBase toItem = GetItemFromSlot(_to.slotType, _to.slotIndex);

        // 단순 이동
        if (fromItem && !toItem)
        {
            TransferBackpack_ToEmptyBackpack(_from.slotIndex, _to.slotIndex);
        }
        // 병합 or 교환
        else if (fromItem && toItem)
        {
            bool isTrackFrom = cachedPlayerQuickSlot.HasSlotByTrackIndex(_from.slotIndex);
            bool isTrackTo = cachedPlayerQuickSlot.HasSlotByTrackIndex(_to.slotIndex);

            int fromQuickSlotIndex = -1;
            int toQuickSlotIndex = -1;

            if (isTrackFrom)
                fromQuickSlotIndex = cachedPlayerQuickSlot.GetSlotByTrackId(_from.slotIndex);
            if (isTrackTo)
                toQuickSlotIndex = cachedPlayerQuickSlot.GetSlotByTrackId(_to.slotIndex);

            var fromData = fromItem.GetItemData();
            var toData = toItem.GetItemData();

            bool sameId = (fromData.itemID == toData.itemID);
            bool bothConsum = (fromData is ConsumData) && (toData is ConsumData);
            bool canStack = sameId && bothConsum && !((ConsumData)fromData).isCapacity;

            var removedFrom = RemoveItemFromSlot(_from.slotType, _from.slotIndex);
            var removedTo = RemoveItemFromSlot(_to.slotType, _to.slotIndex);

            if (canStack)
            {
                var fromConsum = (ConsumBase)removedFrom;
                var toConsum = (ConsumBase)removedTo;

                int remain = toConsum.PushAndCheckRemain(fromConsum.GetCurCnt());

                InsertItem(_to.slotType, _to.slotIndex, removedTo);

                if (remain > 0)
                {
                    fromConsum.SetCurCnt(remain);
                    InsertItem(_from.slotType, _from.slotIndex, removedFrom);
                }
                else
                {
                    UnityEngine.Object.Destroy(removedFrom.gameObject);
                }
            }
            else
            {
                InsertItem(_to.slotType, _to.slotIndex, removedFrom);
                InsertItem(_from.slotType, _from.slotIndex, removedTo);
            }

            // 퀵슬롯 재-Push
            if (isTrackFrom)
            {
                cachedPlayerQuickSlot.TryPushQuick(
                    fromQuickSlotIndex,
                    _to.slotIndex,
                    GetItemFromSlot(_to.slotType, _to.slotIndex)
                );
            }
            else if (isTrackTo)
            {
                cachedPlayerQuickSlot.TryPushQuick(
                    toQuickSlotIndex,
                    _from.slotIndex,
                    GetItemFromSlot(_from.slotType, _from.slotIndex)
                );
            }
        }
    }
    private void TransferBackpack_ToEmptyBackpack(int _fillIndex, int _emptyIndex)
    {
        // 퀵슬롯 정보 이전에 캐싱 
        bool isTrack = cachedPlayerQuickSlot.HasSlotByTrackIndex(_fillIndex);
        int slotIndex = -1;
        if (isTrack)
        {
            slotIndex = cachedPlayerQuickSlot.GetSlotByTrackId(_fillIndex);
        }

        // 백팩 내부 이동 
        ItemBase fillItem = RemoveItemFromSlot(EItemSlotType.Backpack, _fillIndex);
        InsertItem(EItemSlotType.Backpack, _emptyIndex, fillItem);
         
        // 퀵슬롯이 기존 trackIndex를 추적 중이면 다시 Push
        if (isTrack)
        {
            cachedPlayerQuickSlot.TryPushQuick(
                slotIndex,
                _emptyIndex,
                fillItem
            );
        } 
    }
    private void TransferBackpack_ToQuickSlot(ref FItemSlotInfo _backpack, ref FItemSlotInfo _quick)
    {
        var item = GetItemFromSlot(ref _backpack);
        if (!item)
            return;

        cachedPlayerQuickSlot.TryPushQuick(
            _quick.slotIndex,
            _backpack.slotIndex,
            item
        ); 
    }
    private void TransferQuickSlot_ToBackpack(int removecQuckIndex)
    {
        cachedPlayerQuickSlot.DisableSlot(removecQuckIndex);
    }  
     
    private void Transfer_ToAttach(ref FItemSlotInfo _toAttach, ref FItemSlotInfo _beAttached)
    { 
        // 무기인지 체크 -> 무기만 부착가능함
        if (!cachedItemInfoCanvas.IsActiveWeapon())
            return;

        var notAttachItem = GetItemFromSlot(_toAttach.slotType, _toAttach.slotIndex);
        if (notAttachItem is Attachment attach)
        {
            EAttachmentType attachType = attach.GetAttachType();

            // 같은 부위가 아니다
            if (_beAttached.slotIndex != (int)attachType)
            {
                return;
            }
             
            // 비활성화된 부위다  
            EAttachSlotState state = cachedItemInfoCanvas.GetAttachState(attachType);
            if (state == EAttachSlotState.Disable)
            {
                return;
            }

            // 드래그 아이템을 빼주고
            var removedDragItem = RemoveItemFromSlot(_toAttach.slotType, _toAttach.slotIndex);

            // 만약 장착되고 있는 부착물이 존재한다 -> 바꿔주기
            if (state == EAttachSlotState.Exist)
            {
                var removedAttachItem = RemoveItemFromSlot(_beAttached.slotType, _beAttached.slotIndex);
                InsertItem(_toAttach.slotType, _toAttach.slotIndex, removedAttachItem);
            }

            // 부착물을 넣어준다
            if (removedDragItem is Attachment removeAttachment)
            {
                InsertItem(_beAttached.slotType, _beAttached.slotIndex, removedDragItem);
            } 
        }
    }
    private void Transfer_FromAttach(ref FItemSlotInfo _notAttach, ref FItemSlotInfo _attach)
    {
        // 무기인지 체크 -> 무기만 부착가능함
        if (!cachedItemInfoCanvas.IsActiveWeapon())
            return;
          
        bool isNotAttachWare = (_notAttach.slotType == EItemSlotType.WareHouse);
        bool isAttachWare = (_attach.slotType == EItemSlotType.WareHouse);

        var setectedItem = cachedItemInfoCanvas.GetItem();
        if (setectedItem is not Weapon selectedWeapon)
        {
            return;
        }

        var notAttachItem = GetItemFromSlot(_notAttach.slotType, _notAttach.slotIndex);
        if (notAttachItem)
        {
            // 같은 부착용 템이 아니라면
            if (DuckUtill.GetAttachTypeByItemID(_notAttach.itemId) != (EAttachmentType)_notAttach.slotIndex)
                return;

            var removedAttachItem = RemoveItemFromSlot(_attach.slotType, _attach.slotIndex);
            var removedNotAttachItem = RemoveItemFromSlot(_notAttach.slotType, _notAttach.slotIndex);
             
            InsertItem(_attach.slotType, _attach.slotIndex, removedNotAttachItem);
            InsertItem(_notAttach.slotType, _notAttach.slotIndex, removedAttachItem);
        }
         
        else
        {
            // 단순하게 넣어주기 
            var removedAttachItem = RemoveItemFromSlot(_attach.slotType, _attach.slotIndex);
            InsertItem(_notAttach.slotType, _notAttach.slotIndex, removedAttachItem);
        } 
    }  
     
    private void TransferStore_ToBackpack(ref FItemSlotInfo _store, ref FItemSlotInfo _backpack)
    {
        var store = cachedStoreCanvas.GetStore();
        if (!store)
            return;

        ItemBase backpackItem = GetItemFromSlot(ref _backpack); 
        ItemBase storeItem = GetItemFromSlot(ref _store);
         
        if (backpackItem)
            return;

        if (!storeItem) 
            return; 
        
        var removedStoreItem = RemoveItemFromSlot(_store.slotType, _store.slotIndex);
        InsertItem(_backpack.slotType, _backpack.slotIndex, removedStoreItem);
         
        cachedPlayerStorage.ReduceMoney(store.GetStoreSellPrice(removedStoreItem.GetPrice()));
        cachedStoreCanvas.ComplateBuyOrSell(true); 
    }
    private void SelectEquipSlot(EItemSlotSelectType _itemSlotSelectType, ref FItemSlotInfo _itemSlotInfo)
    {
        // F 해제임 -> 가방으로 들어감
        if (_itemSlotSelectType == EItemSlotSelectType.Pick)
        {
            if (!TryGetAutoBackpackSlot(out var autoItemSlotInfo))
                return;

            if (!CanDropBackpack(ref _itemSlotInfo, ref autoItemSlotInfo))
                return;

            TransferItemBetweenSlots(ref _itemSlotInfo, ref autoItemSlotInfo);
        }

        else if (_itemSlotSelectType == EItemSlotSelectType.Use)
        {
            UnloadAmmoToInventory(ref _itemSlotInfo);
        }
    } 
    private void SelectBackpackSlot(EItemSlotSelectType _itemSlotSelectType, ref FItemSlotInfo _itemSlotInfo)
    {
        EItemType itemType = DuckUtill.GetItemTypeByItemID(_itemSlotInfo.itemId);
        if (itemType == EItemType.None)
            return;

        // 가방에서 이동
        if (_itemSlotSelectType == EItemSlotSelectType.Pick)
        {
            // 장비인 경우 - 해당 아이템을 장착가능한지 체크
            if (itemType == EItemType.Equipment)
            {
                EEquipSlot equipType = DuckUtill.GetEquipTypeByItemID(_itemSlotInfo.itemId);
                if (equipType == EEquipSlot.End)
                    return;
                 
                FItemSlotInfo equipItemSlotInfo = new();
                equipItemSlotInfo.slotType = EItemSlotType.Equip;
                equipItemSlotInfo.slotIndex = (int)equipType;
                 
                TransferItemBetweenSlots(ref _itemSlotInfo, ref equipItemSlotInfo);
            }

            // 소비템인 경우 
            else if (itemType == EItemType.Consumable)
            { 
                // 해당 아이템이 현재 등록되어있다면 -> 추적아이템을 변경 
                if (cachedPlayerQuickSlot.HasSlotByItemId(_itemSlotInfo.itemId))
                {
                    int slotIdx = cachedPlayerQuickSlot.GetSlotByItemId(_itemSlotInfo.itemId);
                    cachedPlayerQuickSlot.ChangeTrackID(slotIdx, _itemSlotInfo.slotIndex);
                }  

                // 아니라면 자동으로 넣어주기 
                else
                {
                    int emptyIndex = cachedPlayerQuickSlot.GetEmptyIndex();
                    if (emptyIndex == -1)
                        return;

                    FItemSlotInfo quickSlotInfo = new();
                    quickSlotInfo.slotType = EItemSlotType.Quick;
                    quickSlotInfo.slotIndex = emptyIndex;
                    TransferBackpack_ToQuickSlot(ref _itemSlotInfo, ref quickSlotInfo);
                }
            } 
        }

        // 사용 소비 아이템 
        else if (_itemSlotSelectType == EItemSlotSelectType.Use)
        {
            if (itemType == EItemType.Equipment && EEquipSlot.Weapon == DuckUtill.GetEquipTypeByItemID(_itemSlotInfo.itemId))
            {
                UnloadAmmoToInventory(ref _itemSlotInfo);
            } 

            else if (itemType == EItemType.Consumable)
            {
                // 아이템 사용
                cachedPlayerStorage.TryUseConsum(_itemSlotInfo.slotIndex);
            }
              
            else if (itemType == EItemType.Attachment)
            {
                AttachToItemInTtemInfo(ref _itemSlotInfo);
            } 
        }
         
        // 버리기 
        else if (_itemSlotSelectType == EItemSlotSelectType.Throw)
        {
            var Item = cachedPlayerStorage.RemoveItem(_itemSlotInfo.slotIndex, true);
        }
    } 
    private void SelectItemBoxSlot(EItemSlotSelectType _itemSlotSelectType, ref FItemSlotInfo _itemSlotInfo)
    {
        EItemType itemType = DuckUtill.GetItemTypeByItemID(_itemSlotInfo.itemId);
        if (itemType == EItemType.None)
            return;
         
        // 아이템 박스에서 F -> 가방의 빈곳에 넣어주기 
        if (_itemSlotSelectType == EItemSlotSelectType.Pick)
        {
            if (!TryGetAutoBackpackSlot(out var autoItemSlotInfo))
                return;
 
            TransferItemBetweenSlots(ref _itemSlotInfo, ref autoItemSlotInfo);
        } 

        // 퀵슬롯 사용 
        else if (_itemSlotSelectType == EItemSlotSelectType.Use)
        {
            // 아이템 사용할건데
            if (itemType == EItemType.Consumable)
            {
                cachedPlayerStorage.TryUseConsum(cachedItemBoxCanvas.GetItemBox(), _itemSlotInfo.slotIndex);
            }
            
            // 부착물인 경우
            else if (itemType == EItemType.Attachment)
            {
                AttachToItemInTtemInfo(ref _itemSlotInfo);
            } 
        }

        else if (_itemSlotSelectType == EItemSlotSelectType.Throw)
        {
            // ItemBox에서는 Throw없을듯
        }
    }
    private void SelectStoreSlot(EItemSlotSelectType _itemSlotSelectType, ref FItemSlotInfo _itemSlotInfo)
    {
        if (_itemSlotInfo.slotType == EItemSlotType.Store)
        {
            // 구매임 
            if (_itemSlotSelectType == EItemSlotSelectType.Pick)
            {
                // 돈이 가능한지 체크,
                if (!cachedSlotCheck.CanBuy(_itemSlotInfo.slotIndex))
                    return;
                
                var backpackItemInfo = new FItemSlotInfo();
                backpackItemInfo.slotType = EItemSlotType.Backpack;
                backpackItemInfo.slotIndex = cachedPlayerStorage.GetEmptyIndex();
                 
                TransferStore_ToBackpack(ref _itemSlotInfo, ref backpackItemInfo);
            }
        }
    }
    private void SelectQuickSlot(EItemSlotSelectType _itemSlotSelectType, ref FItemSlotInfo _itemSlotInfo)
    {
        // 퀵슬롯 해제임
        if (_itemSlotSelectType == EItemSlotSelectType.Pick)
        {
            TransferQuickSlot_ToBackpack(_itemSlotInfo.slotIndex);
        }   
    } 
    private void SelectAttachSlot(EItemSlotSelectType _itemSlotSelectType, ref FItemSlotInfo _itemSlotInfo)
    {
        // 부착품 해제 -> 인벤토리에 다시 넣어주기 
        if (_itemSlotSelectType == EItemSlotSelectType.Pick)
        {
            var item = cachedItemInfoCanvas.GetItem();
            if (item is not Weapon weapon)
                return;
             
            if (!cachedPlayerStorage.IsEmptySlotSet())
                return;

            var backpackSlotInfo = new FItemSlotInfo();
            backpackSlotInfo.slotType = EItemSlotType.Backpack;
            backpackSlotInfo.slotIndex = cachedPlayerStorage.GetEmptyIndex();
            Transfer_FromAttach(ref backpackSlotInfo, ref _itemSlotInfo);
        }   
    }
    private void SelectWarehouseSlot(EItemSlotSelectType _itemSlotSelectType, ref FItemSlotInfo _itemSlotInfo)
    {
        if (_itemSlotSelectType == EItemSlotSelectType.Pick)
        {
            if (!cachedSlotCheck.CanInsertInventory())
                return;
             
            int emptyIndex = cachedPlayerStorage.GetEmptyIndex();
            FItemSlotInfo backpackInfo = new FItemSlotInfo();
            backpackInfo.slotType = EItemSlotType.Backpack;
            backpackInfo.slotIndex = emptyIndex;
            Transfer_ToBackpack(ref _itemSlotInfo, ref backpackInfo);
        } 
    } 

    private bool CanDropBackpack(ref FItemSlotInfo _backpack, ref FItemSlotInfo _ray)
    {
        // 장비 슬롯이 아니면 통과
        if (_backpack.slotType != EItemSlotType.Equip)
            return true;

        // 가방 슬롯이 아니면 통과
        if (_backpack.slotIndex != (int)EEquipSlot.Backpack)
            return true;

        var item = GetItemFromSlot(ref _backpack);
        if (item is not Backpack backpack)
            return false;

        int addSize = backpack.GetBackpackData().addStorgeSize;
        int maxCnt = cachedPlayerStorage.GetItemMaxCnt();

        // 가방 제거 후 허용 슬롯 수
        int newMaxCnt = maxCnt - addSize;

        // 가방 제거 후 범위 밖이면 드롭 불가
        return _ray.slotIndex < newMaxCnt;
    }
    private bool TryGetAutoBackpackSlot(out FItemSlotInfo slotInfo)
    {
        slotInfo = default;

        int emptyIndex = cachedPlayerStorage.GetEmptyIndex();
        if (emptyIndex == -1)
            return false;

        slotInfo.slotType = EItemSlotType.Backpack;
        slotInfo.slotIndex = emptyIndex;
        slotInfo.itemId = EItemID._END;

        return true;
    }
    private void UnloadAmmoToInventory(ref FItemSlotInfo _weaponInfo)
    {
        EItemType itemType = DuckUtill.GetItemTypeByItemID(_weaponInfo.itemId);
        if (itemType == EItemType.None)
            return;

        if (itemType == EItemType.Equipment && EEquipSlot.Weapon == DuckUtill.GetEquipTypeByItemID(_weaponInfo.itemId))
        {
            var item = GetItemFromSlot(ref _weaponInfo);
            if (item is not Weapon weapon)
                return;

            if (cachedPlayerStorage.IsEmptySlotSet())
            {
                var bullet = weapon.RemoveCurBullet();
                if (!bullet)
                    return;

                int emptyIndex = cachedPlayerStorage.GetEmptyIndex();
                if (emptyIndex == -1)
                    return;

                cachedPlayerStorage.InsertItem(emptyIndex, bullet);
            }
        } 
    }
    private void AttachToItemInTtemInfo(ref FItemSlotInfo _attachInfo)
    {
        var item = GetItemFromSlot(ref _attachInfo);
        if (item is not Attachment attachment)
            return;

        var attachItemInfo = new FItemSlotInfo();
        attachItemInfo.slotType = EItemSlotType.Attach;
        attachItemInfo.slotIndex = (int)attachment.GetAttachType();
        Transfer_ToAttach(ref _attachInfo, ref attachItemInfo);
    }
    //private void CheckAndRenewBackpackQuickSlotTrack(int backpackSlotIndex)
    //{
    //    if (!cachedPlayerQuickSlot)
    //        return;
    //
    //    if (!cachedPlayerQuickSlot.HasSlotByTrackIndex(backpackSlotIndex))
    //        return;
    //
    //    int quickSlotIndex =
    //        cachedPlayerQuickSlot.GetSlotByTrackId(backpackSlotIndex);
    //
    //    cachedPlayerQuickSlot.DisableOrChangeTrack(quickSlotIndex);
    //}
} 
 