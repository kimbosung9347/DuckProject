using Unity.VisualScripting;
using UnityEngine.LowLevelPhysics;

public class PlayerEquip : DuckEquip
{
    private PlayerUIController cachedUIController;
    private PlayerStorage cachedStorage;
    private PlayerState cachedState;
    private DuckSpeechBubble cachedSpeech;

    private bool isTryReload = false;
    protected override void Awake()
    {
        base.Awake();

        cachedUIController = GetComponent<PlayerUIController>();
        cachedStorage = GetComponent<PlayerStorage>();
        cachedState = GetComponent<PlayerState>();
        cachedSpeech = GetComponent<DuckSpeechBubble>();
    }
    private void Start()
    {
        cachedUIController.HUD_DetachWeapon();

        var playData = GameInstance.Instance.SAVE_GetCurPlayData();
        for (int i= 0; i < (int)EEquipSlot.End; i++)
        {
            cachedUIController.INVEN_DisableEquipSlot((EEquipSlot)i);
        } 
        foreach (var itemInfo in playData.equipData.items)
        {
            ItemBase created = GameInstance.Instance.SPAWN_MakeItem(itemInfo.shell);
            if (!created)
                continue;

            EquipItem((EEquipSlot)itemInfo.index, created);
        }
    } 
    private void Update()
    {
        if (isTryReload)
        {
            var weapon = GetWeapon();
            float ratio = weapon.GetReloadRatio();
            cachedUIController.RenewReloadGuage(ratio);
        }
    }  
     
    public override ItemBase DetachEquip(EEquipSlot _slot)
    {
        var prevEquip = base.DetachEquip(_slot);
        cachedUIController.INVEN_DisableEquipSlot(_slot);
        return prevEquip;
    }
    public override void RenewCurBullet()
    {
        var weapon = GetWeapon();

        // 총알이 없고, 
        if (weapon.GetCurBullet() == 0)
        {
            // 그 총알이 현재 하나도 없는 상태라면,
            if (cachedStorage.GetTotalConsumCnt(weapon.GetCurBulletId()) == 0)
            {
                // 총알이 없음으로 설정해줘야함 - 그래야 다음번에 총알이 들어와도 Select할 수 있음
                weapon.SetCurBulletId(EItemID._END);
            }
        }

        cachedUIController.HUD_RenewCurBullet(GetWeapon().GetCurBullet());
    }
    public override void RenewCurDurability(EEquipSlot _slotType)
    {
        // 가방은 내구도 없음 - 무시시킴
        if (_slotType == EEquipSlot.Backpack)
        {
            return;
        }

        EquipBase targetEquip = GetEquip(_slotType);
        if (!targetEquip)
            return;

        cachedUIController.INVEN_ReuewEquipSlotDurability(_slotType, targetEquip.GetCurDurabilityRatio());
    }

    public void DelateAllEquip()
    {
        for (int i = 0; i < (int)EEquipSlot.End; i++)
        {
            var equip = DetachEquip((EEquipSlot)i);
            if (!equip)
                continue;
            Destroy(equip.gameObject);
        } 
    } 
    public void RenewAllEquipInfo()
    {
        for (int i = 0; i < (int)EEquipSlot.End; i++)
        { 
            var item = arrEquip[i];
            if (!item)
                continue;

            if (item is not EquipBase equip)
                continue;
             
            RenewUI(i, equip);
        }
    }
     
    public PlayerEquipItemData CreateEquipItemData()
    {
        PlayerEquipItemData data = new();
        for (int i = 0; i < arrEquip.Length; i++)
        {
            var equip = arrEquip[i];
            if (equip == null)
                continue;

            data.items.Add(new FItemShellIncludeIndex
            {
                index = i,
                shell = equip.GetItemShell()
            });
        } 

        return data;
    }
    
    public bool CanEquipInput(int _index)
    {
        if (arrEquip[_index] == null)
            return false;

        return true;
    }
    public bool CanCancleReload()
    {
        var Weapon = GetWeapon();
        if (!Weapon)
            return false;

        if (!Weapon.IsReloading())
            return false;
         
        return true;
    }
     
    public void TryReload(EItemID _bulletId)
    {
        isTryReload = true;
        cachedState.ChangeState(EDuckState.Reload);
        GetWeapon().TryReload(_bulletId);
    } 
    public void TryReload() 
    {
        isTryReload = true;
        cachedState.ChangeState(EDuckState.Reload);
        GetWeapon().TryReload();
    }
    public void CancleReload()
    {
        isTryReload = false;
         
        var weapon = GetWeapon();
        if (weapon)
        {
            // 장전 중이다 -> 장전에 실패함
            if (weapon.IsReloading())
            {
                // 장전 취소
                weapon.CancleReload();  
                cachedSpeech.ActiveAutoDeleteSpeech("장전 취소");
            } 
        }
    }
       
    public void RenewAttachState(Weapon _weapon)
    {
        if (!_weapon)
            return;

        EAttachSlotState muzzle = _weapon.GetAttachSlotState(EAttachmentType.Muzzle);
        EAttachSlotState scople = _weapon.GetAttachSlotState(EAttachmentType.Scope);
        EAttachSlotState stock = _weapon.GetAttachSlotState(EAttachmentType.Stock);

        cachedUIController.INVEN_RenewEquipSlotAttachment(muzzle, scople, stock);
        cachedUIController.ITEMINFO_RenewSlotAttachment(muzzle, scople, stock);
    }  
    public void RenewCursorScopeUI(EItemID _id)
    {
        cachedUIController.CURSOR_SetAimCursor(_id);
    }
     
    protected override void EquipWeapon(ItemBase _item)
    { 
        base.EquipWeapon(_item);

        if (!_item)
            return;

        if (_item is not EquipBase equip)
            return;
         
        RenewUI((int)EEquipSlot.Weapon, equip);
    }
    protected override void EquipHelmet(ItemBase _item) 
    {
        base.EquipHelmet(_item);

        if (!_item)
            return; 

        if (_item is not EquipBase equip)
            return; 

        RenewUI((int)EEquipSlot.Helmet, equip);
    }
    protected override void EquipArmor(ItemBase _item)
    {
        base.EquipArmor(_item);

        if (!_item)
            return;

        if (_item is not EquipBase equip)
            return;

        RenewUI((int)EEquipSlot.Armor, equip); 
    }
    protected override void EquipBackpack(ItemBase _item)
    {
        base.EquipBackpack(_item);
         
        if (!_item)
            return;

        if (_item is not EquipBase equip)
            return; 

        // 가방 공간 갱신 
        cachedStorage.MakeStorageByCapacity();

        RenewUI((int)EEquipSlot.Backpack, equip);
    } 
     
    protected override ItemBase DetachWeapon()
    {
        ItemBase item = base.DetachWeapon();
        if (!item)
            return null;

        RenewDisableUI(EEquipSlot.Weapon);
          
        return item;  
    } 
    protected override ItemBase DetachHelmet()
    {
        var item =  base.DetachHelmet();
        if (!item)
            return null;

        RenewDisableUI(EEquipSlot.Helmet);
         
        return item;
    }
    protected override ItemBase DetachArmor()
    {
        var item = base.DetachArmor();
        if (!item)
            return null; 

        RenewDisableUI(EEquipSlot.Armor);

        return item; 
    }
    protected override ItemBase DetachBackpack()
    {
        var item = base.DetachBackpack();
        if (!item) 
            return null; 
         
        // 가방 공간 갱신 
        cachedStorage.MakeStorageByCapacity();

        RenewDisableUI(EEquipSlot.Backpack);

        return item;
    }
     
    private void RenewUI(int _index, EquipBase _equip)
    {
        var itemData = _equip.GetItemData();
        var visualData = _equip.GetItemVisualData();

        // 인벤토리 슬롯 갱신
        cachedUIController.INVEN_RenewEquipSlot((EEquipSlot)_index, itemData.itemID, itemData.grade, itemData.itemName, visualData.iconSprite);

        // 무기일 경우
        if (_equip is Weapon weapon)
        {
            cachedUIController.INVEN_ReuewEquipSlotDurability((EEquipSlot)_index, _equip.GetCurDurabilityRatio());

            // 퀵슬롯 갱신
            cachedUIController.HUD_EquipWeapon(itemData.itemID);
            EItemID curBulletId = weapon.GetCurBulletId();
            cachedUIController.HUD_RenewBulletType(curBulletId);

            // 총알 개수갱신   
            int haveBulletCnt = cachedStorage.GetTotalConsumCnt(curBulletId);
            cachedUIController.HUD_RenewMaxBullet(haveBulletCnt);
            cachedUIController.HUD_RenewCurBullet(weapon.GetCurBullet());

            RenewAttachState(weapon);

            var scope = weapon.GetAttach(EAttachmentType.Scope);
            RenewCursorScopeUI(scope ? scope.GetItemData().itemID : EItemID._END);
        }

        // 방어구일 경우
        else if (_equip is Armor _armor)
        {
            cachedUIController.INVEN_ReuewEquipSlotDurability((EEquipSlot)_index, _equip.GetCurDurabilityRatio());
        }

        // 가방일 경우
        else if (_equip is Backpack backpack)
        {
            cachedStorage.RenewBackpackSlot();
        }
    }
    private void RenewDisableUI(EEquipSlot equipSlot)
    {
        cachedUIController.INVEN_DisableEquipSlot(equipSlot);
         
        switch (equipSlot)
        {
            case EEquipSlot.Weapon:
            {
                cachedUIController.HUD_DetachWeapon();
                cachedUIController.CURSOR_SetAimCursor(EItemID._END);
            }
            break;

            case EEquipSlot.Backpack:
            {
                cachedStorage.RenewBackpackSlot();
            } 
            break;
                 
            case EEquipSlot.Helmet:
            case EEquipSlot.Armor:
            break;
        }
    }

}  
 