using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HandleItemBox : HandleInteractionBase
{
    private ItemBoxCanvas cachedItemBoxCanvas;
    private ItemBoxBase cachedItemBox;
    private PlayerInteraction cachedInteraction;
     
    protected override void Awake()
    {
        base.Awake();
         
        cachedItemBoxCanvas = GameInstance.Instance.UI_GetPersistentUIGroup().GetItemBoxCanvas();
        interactionType = EInteractionType.ItemBox;
        cachedItemBox = GetComponent<ItemBoxBase>();
    }
       
    ////////////////////////////
    // 인벤토리에 갱신해주기 
    public override void DoInteractionToPlayer(PlayerInteraction _interaction)
    {
        ItemBase[] arrItemBox = cachedItemBox.GetItemArray();
        FItemBoxSlotInfo[] arrItemState = cachedItemBox.GetItemStateArray();

        // 슬롯 갱신
        for (int i = 0; i < DuckDefine.MAX_ITEMBOX_COUNT; i++)
        {
            // showSlotDesc
            ItemBase item = arrItemBox[i];
            if (item)
            {
                RenewItemRenderInfo(i, item);
            } 

            EItemBoxSlotState state = arrItemState[i].state;
            RenewItemSlotState(i, state);
        }

        // 아이템 박스 이름
        RenewItemBoxNameAndDesc(cachedItemBox.GetItemDesc());
         
        // 아이템 박스 크기
        const int oneLineCnt = 5;
        float targetHeight = (oneLineCnt < cachedItemBox.GetMaxInvenCnt()) ? 280f : 145f;
        RenewBackgroundHeight(targetHeight);

        // Search 및 플레이어 로직 실행 
        cachedItemBox.SetSearch(true);
        cachedItemBoxCanvas.Active(cachedItemBox); 
            
        //  플레이어도 아이템박스에 상태에 들어감 
        _interaction.EnterItemBox();

        cachedInteraction = _interaction; 
    }
    public override void EndInteractionToPlayer()
    {
        cachedItemBox.SetSearch(false);
        cachedItemBoxCanvas.Disable();
        cachedInteraction = null;
    } 
    
     
    public PlayerInteraction GetCurInteractionTarget() { return cachedInteraction; }
     
    public void RenewItemBoxNameAndDesc(string _desc)
    {
        cachedItemBoxCanvas.RenewItemBoxNameAndDesc(_desc);
    }
    public void RenewBackgroundHeight(float _height) 
    {
        cachedItemBoxCanvas.RenewBackgroundHeight(_height);
    }
    public void RenewItemSlotState(int _index, EItemBoxSlotState _state)
    {
        cachedItemBoxCanvas.RenewItemSlotState(_index, _state);
    } 
    public void RenewItemRenderInfo(int _index, ItemBase _item)
    {
        ItemData itemData = _item.GetItemData();
        ItemVisualData visualData = _item.GetItemVisualData();

        bool isShowAttach = false;

        if (_item is EquipBase equip)
        {  
            if (_item is Weapon weapon)
            {
                isShowAttach = true;
                var muzzle = weapon.GetAttachSlotState(EAttachmentType.Muzzle);
                var scope = weapon.GetAttachSlotState(EAttachmentType.Scope);
                var stock = weapon.GetAttachSlotState(EAttachmentType.Stock);
                cachedItemBoxCanvas.RenewItemSlotAttachment(_index, muzzle, scope, stock);
            }  

            if (equip.IsUseDuration())
            {
                RenewDurabilityItem(_index, itemData.itemID, itemData.grade, itemData.itemName, visualData.iconSprite, equip.GetCurDurabilityRatio());
            }  
             
            else
            {
                RenewRenderInfo(_index, itemData.itemID, itemData.grade, itemData.itemName, visualData.iconSprite);
            }
        }

        else if (_item is ConsumBase consum)
        {
            if (consum.IsCapacity())
            {
                RenewDurabilityItem(_index, itemData.itemID, itemData.grade, itemData.itemName, visualData.iconSprite, consum.GetCurDurabilityRatio());
            }

            else
            {
                RenewConsumItemCnt(_index, itemData.itemID, itemData.grade, itemData.itemName, visualData.iconSprite, consum.GetCurCnt());
            } 
        } 

        else if (_item is Attachment attachment)
        {
            RenewRenderInfo(_index, itemData.itemID, itemData.grade, itemData.itemName, visualData.iconSprite);
        }

        else if (_item is Stuff stuff)
        {
            RenewRenderInfo(_index, itemData.itemID, itemData.grade, itemData.itemName, visualData.iconSprite);
        } 

        if (!isShowAttach)
        {
            cachedItemBoxCanvas.HideItemSlotAttachment(_index);
        }
    }  


    private void RenewRenderInfo(int _index, EItemID _id, EItemGrade _grade, string _name, Sprite _sprite)
    {
        cachedItemBoxCanvas.RenewItemSlotRenderInfo(_index, _id, _grade, _name, _sprite);
    }  
    private void RenewConsumItemCnt(int _index, EItemID _id, EItemGrade _grade, string _name, Sprite _sprite, int _cnt)
    {
        cachedItemBoxCanvas.RenewItemSlotRenderInfo(_index, _id, _grade, _name, _sprite);
        cachedItemBoxCanvas.RenewItemSlotConsumCnt(_index, _cnt);
    }
    private void RenewDurabilityItem(int _index, EItemID _id, EItemGrade _grade, string _name, Sprite _sprite, float _ratio)
    {
        cachedItemBoxCanvas.RenewItemSlotRenderInfo(_index, _id, _grade, _name, _sprite);
        cachedItemBoxCanvas.RenewItemSlotEquipDurability(_index, _ratio);
    }
}    
 