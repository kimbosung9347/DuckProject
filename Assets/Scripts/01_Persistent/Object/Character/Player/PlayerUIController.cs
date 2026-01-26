using NUnit.Framework.Interfaces;
using System;
using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
    // Cursor
    private CursorInPlay playCursor;
    private CursorInUI uiCursor;
     
    // Canvas
    private MainHudCanvas cachedHudCanvas;
    private ItemBoxCanvas cachedItemBoxCanvas;
    private InventoryCanvas cachedInvenCanvas;
    private MenuCanvas cachedMenuCanvas;
    private InvenQuickCanvas cachedInvenQuickCanvas;
    private InteractionCanvas cachedInteractionCanvas;
    private ItemInfoCanvas cachedItemInfoCanvas;
    private StoreCanvas cachedStoreCanvas;
    private QuestCanvas cachedQuestCanvas;
    private WareHouseCanvas cachedWareHouseCanvas;
    private AdornCanvas cachedAdornCanvas;
    private SleepCanvas cachedSleepCanvas;
    private SetLevelCanvas cachedSetLevelCanvas;
    private EndSallyForthCanvas cachedSallyCanvas;
    private StopCanvas cachedStopCanvas; 
    private RadiusCollapseCanvas cachedCollapseCanvas;
    private DisableCanvas cachedDisableCanvas; 

    
    private void Awake() 
    {
        var gameInstance = GameInstance.Instance;
        var uiGroup = GameInstance.Instance.UI_GetPersistentUIGroup();
        var cursorCanvas = uiGroup.GetCursorCanvas();

        playCursor = cursorCanvas.GetPlayCursor();
        uiCursor = cursorCanvas.GetUICursor();

        cachedHudCanvas = uiGroup.GetHudCanvas();
        cachedItemBoxCanvas = uiGroup.GetItemBoxCanvas();
        cachedInvenCanvas = uiGroup.GetInventoryCanvas();
        cachedMenuCanvas = uiGroup.GetMenuCanvas();
        cachedInvenQuickCanvas = uiGroup.GetInvenQuickCanvas();
        cachedInteractionCanvas = uiGroup.GetInteractionCanvas();
        cachedItemInfoCanvas = uiGroup.GetItemInfoCanvas();
        cachedStoreCanvas = uiGroup.GetStoreCanvas(); 
        cachedWareHouseCanvas = uiGroup.GetWareHouseCanvas();
        cachedQuestCanvas = uiGroup.GetQuestCanvas();
        cachedAdornCanvas = uiGroup.GetAdornCanvas();
        cachedSleepCanvas = uiGroup.GetSleepCanvas();
        cachedSetLevelCanvas = uiGroup.GetSetLevelCanvas(); 
        cachedCollapseCanvas = uiGroup.GetRadiusCollaspeCanvas();
        cachedSallyCanvas = uiGroup.GetEndSallyForthCanvas();
        cachedStopCanvas = uiGroup.GetStopCanvas();
        cachedDisableCanvas = uiGroup.GetDisableCanvas(); 
    }  
    private void Start()
    {
        cachedHudCanvas.gameObject.SetActive(true);
    }

    public void ChangeUI()
    {
        uiCursor.gameObject.SetActive(true);
        playCursor.gameObject.SetActive(false);
    } 
    public void ChangePlay()
    {
        uiCursor.gameObject.SetActive(false);
        uiCursor.DisableItemTooltip();
         
        playCursor.gameObject.SetActive(true);
    }
     
    public void RenewReloadGuage(float _ratio)
    {
        playCursor.RenewReloadGauge(_ratio);
        cachedInteractionCanvas.RenewInteactionGuage(_ratio);
    } 
    public void RenewUseGuage(float _ratio)
    {
        cachedInteractionCanvas.RenewInteactionGuage(_ratio); 
    } 

    public void RenewQuickSlotRenderInfo(int _index, ItemBase _item)
    {
        var itemData = _item.GetItemData();
        var vsData = _item.GetItemVisualData();
        RenewQuickSlotRenderInfo(_index, itemData.itemID, itemData.grade, itemData.itemName, vsData.iconSprite);
    } 
    public void RenewQuickSlotRenderInfo(int _index, EItemID _id, EItemGrade _grade, string _name, Sprite _sprite)
    {
        cachedInvenQuickCanvas.RenewQuickSlot(_index, _id, _grade, _name, _sprite);
        cachedHudCanvas.RenewItemSlotRenderInfo(_index, _grade, _name, _sprite);
    } 
    public void RenewQuickDurability(int _index, float _ratio)
    {
        cachedInvenQuickCanvas.RenewQuickSlotDurability(_index, _ratio);
        cachedHudCanvas.RenewCapacity(_index, _ratio); 
    }
    public void RenewQuickSlotConsumCnt(int _index, int _cnt)
    {
        cachedInvenQuickCanvas.RenewQuickSlotConsumCnt(_index, _cnt);
        cachedHudCanvas.RenewConsumCnt(_index, _cnt);
    }
    public void RenewMoney(int money)
    {
        cachedInvenCanvas.RenewMoney(money);
    } 
    public void DisableQuickSlot(int _index)
    {
        cachedInvenQuickCanvas.DisableQuickSlot(_index);
        cachedHudCanvas.DisableConsumQuick(_index);
    } 

    /* Cursor */
    public void CURSOR_ChangeCursorState(ECursorState _state)
    {
        playCursor.ChangeCursorState(_state);
    }
    public void CURSOR_RenewMousePos(Vector3 _pos)
    {
        playCursor.RenewMousePos(_pos);
    } 
    public void CURSOR_CacheShotInfo(ShotInfo _shotInfo)
    {
        playCursor.CacheShotInfo(_shotInfo);
    }
    public void CURSOR_SetAimCursor(EItemID _scopedId)
    {
        playCursor.SetAimCursor(_scopedId); 
    } 

    public void CURSOR_ActiveHit(ECursorHitState _hitState)
    {
        playCursor.ActiveCursorHit(_hitState);
    } 
    public void CURSOR_Disable()
    {
        playCursor.gameObject.SetActive(false);
        uiCursor.gameObject.SetActive(false);
    } 

    /* HUD */
    public void HUD_RenewTime(float _gameTime)
    {
        cachedHudCanvas.RenewTime(_gameTime); 
    }
    public void HUD_RenewTimeState(ETimeState _state)
    {
        cachedHudCanvas.RenewTimeState(_state);
    }  

    public void HUD_AcitveHUDCanvas(bool _isActive)
    {
        cachedHudCanvas.gameObject.SetActive(_isActive); 
    }
    public void HUD_ActiveInteractionGuage()
    {
        cachedInteractionCanvas.ActiveInteractionGuage();
    } 
    public void HUD_DisableInteractionGauge()
    {
        cachedInteractionCanvas.DisableInteractioknGuage();
    }
    public void HUD_DetachWeapon()
    {
        cachedHudCanvas.DetachWeapon();
    }
    public void HUD_EquipWeapon(EItemID _id)
    {
        cachedHudCanvas.EquipWeapon(_id);
    }
    public void HUD_RenewBulletType(EItemID _id)
    {
        cachedHudCanvas.RenewBulletType(_id);
    }
    public void HUD_RenewCurBullet(int _cnt)
    {
        cachedHudCanvas.RenewCurBullet(_cnt);
    }
    public void HUD_RenewMaxBullet(int _cnt)
    {
        cachedHudCanvas.RenewMaxBullet(_cnt);
    }
    public void HUD_RenewAvaiableBulletCnt(EItemID _id, int _cnt)
    {
        cachedHudCanvas.RenewAvaiableBulletCnt(_id, _cnt);
    }
    public void HUD_NotSelectAvaiableBullet(EItemID _id)
    {
        cachedHudCanvas.NotSelectAvaiableBullet(_id);    
    }
    public void HUD_SelectAvaiableBullet(EItemID _id)
    {
        cachedHudCanvas.SelectAvaiableBullet(_id);
    }
    public void HUD_ExitAvaiableBullet()
    {
        cachedHudCanvas.ExitAvaiableBullet();
    }
    public void HUD_EnterAvaiableBullet()
    {
        cachedHudCanvas.EnterAvaiableBullet();
    }
    public void HUD_InsertAvaiableBullet(EItemID _id, int _cnt)
    {
        cachedHudCanvas.InsertAvaiableBullet(_id, _cnt);
    }
    public void HUD_RenewHp(float _cur, float _max)
    {
        cachedHudCanvas.RenewHp(_cur, _max); 
    }
    public void HUD_RenewFood(float _ratio)
    {
        cachedHudCanvas.RenewFoodGauge(_ratio);
    }  
    public void HUD_RenewWater(float _ratio)
    {
        cachedHudCanvas.RenewWaterGauge(_ratio);
    }
    public void HUD_InsertBuff(EBuffID _buffId, Sprite _sprite, string _name, float _time)
    {
        cachedHudCanvas.InsertBuff(_buffId, _sprite, _name, _time); 
    }
    public void HUD_InsertDeBuff(EBuffID _buffId, Sprite _sprite, string _name, float _time)
    {
        cachedHudCanvas.InsertDeBuff(_buffId, _sprite, _name, _time); 
    } 
    public void HUD_RenewBuffTime(EBuffID _buffId, float _time)
    {
        cachedHudCanvas.RenewBuffDuration(_buffId, _time);
    } 
    public void HUD_RemoveBuff(EBuffID _buffId)
    {
        cachedHudCanvas.RemoveBuff(_buffId);
    }
    public void HUD_MakeEnemyDetector(Transform _player, Transform _enemyTransform)
    {
        cachedHudCanvas.MakeEnemyDetector(_player, _enemyTransform); 
    }

    public void HUD_ActiveDetector(bool _isActive)
    {
        cachedHudCanvas.ActiveDetectorSkill(_isActive);
    } 
    public void HUD_ActiveRoll(bool _isActive)
    {
        cachedHudCanvas.ActiveRollSkill(_isActive);
    } 

    public void HUD_RenewDetectorCoolTime(float _cur, float _max)
    {
        cachedHudCanvas.RenewDetectorSkillTime(_cur, _max);
    } 
    public void HUD_RenewRollCoolTime(float _cur, float _max)
    {
        cachedHudCanvas.RenewRollSkillTime(_cur, _max);
    }

    /* QUICK */

    /* ItemInfo */
    public void ITEMINFO_RenewAttachSlot(EAttachmentType _type, EItemID _id, EItemGrade _grade, string _name, Sprite _sprite)
    {
        cachedItemInfoCanvas.RenewRenderInfoSlot(_type, _id, _grade, _name, _sprite);
    }
    public void ITEMINFO_ChangeEmpty(EAttachmentType _type)
    {
        cachedItemInfoCanvas.ChangeEmpty(_type);
    } 
    public void ITEMINFO_ChangeX(EAttachmentType _type)
    {
        cachedItemInfoCanvas.ChangeEmpty(_type);
    }
    public void ITEMINFO_RenewSlotAttachment(EAttachSlotState _muzzle, EAttachSlotState _scope, EAttachSlotState _stock)
    {
        cachedItemInfoCanvas.RenewAttachment(_muzzle, _scope, _stock);
    }
     
    /* Inven */
    public void INVEN_MakeBackpackSlot(int _index)
    {
        cachedInvenCanvas.MakeBackpackSlot(_index); 
    }
    public void INVEN_RenewBackpackDesc(string _desc)
    {
        cachedInvenCanvas.RenewBackpackDesc(_desc);
    }
    public void INVEN_RenewWeight(float _cur, float _max)
    {
        cachedInvenCanvas.RenewBackpackWeight(_cur, _max);
    } 
    public void INVEN_DisableEquipSlot(EEquipSlot _slot)
    {
        cachedInvenCanvas.DisableEquipSlot(_slot);
    }
    public void INVEN_RenewEquipSlot(EEquipSlot _slot, EItemID _id, EItemGrade _grade, string _name, Sprite _sprite)
    {
        cachedInvenCanvas.RenewEquipSlot(_slot, _id, _grade, _name, _sprite);
    }
    public void INVEN_ReuewEquipSlotDurability(EEquipSlot _slot, float _ratio)
    {
        cachedInvenCanvas.RenewEquipSlotDurability(_slot, _ratio);  
    }
    public void INVEN_RenewEquipSlotAttachment(EAttachSlotState _muzzle, EAttachSlotState _scope, EAttachSlotState _stock)
    {
        cachedInvenCanvas.RenewEquipSlotAttachment(_muzzle, _scope, _stock);
    } 

    public void INVEN_RenewBackpackSlot(int _index, EItemID _id, EItemGrade _grade, string _name, Sprite _sprite)
    {
        cachedInvenCanvas.RenewBackpackSlot(_index, _id, _grade, _name, _sprite);   
    }
    public void INVEN_RenewBackpackAttachment(int _index, EAttachSlotState _muzzle, EAttachSlotState _scope, EAttachSlotState _stock)
    { 
        cachedInvenCanvas.RenewBackpackAttachment(_index, _muzzle, _scope, _stock);
    } 
    public void INVEN_RenewBackpackSlotDurability(int _index, float _durabilityRatio)
    {
        cachedInvenCanvas.RenewBackpackSlotDurability(_index, _durabilityRatio);
    }
    public void INVEN_RenewBackpackSlotConsumCnt(int _index, int _cnt)
    {
        cachedInvenCanvas.RenewBackpackSlotConsumCnt(_index, _cnt); 
    } 
    public void INVEN_DisableBackpackSlot(int _index)
    {
        cachedInvenCanvas.DisableBackpackSlot(_index); 
    } 
    public void INVEN_HideBackpackAttachment(int _index)
    {
        cachedInvenCanvas.HideBackpackAttachment(_index);
    }  
    public void INVEN_HideGageAndCnt(int _index)
    {
        cachedInvenCanvas.HideGuageAndCnt(_index);
         
    }

    /* ItemBox */
    public void EnterItemBox()
    {
        // cachedPlayerController.ChangeItemBoxUI();
        cachedInvenQuickCanvas.Active();
        cachedInvenCanvas.Active();
        cachedHudCanvas.gameObject.SetActive(false);
    } 
    public void DisableItemBox()
    { 
        cachedInvenQuickCanvas.Disable();
        cachedItemBoxCanvas.Disable();
        cachedInvenCanvas.Disable();
        cachedHudCanvas.gameObject.SetActive(true); 
    }

    /* Menu */
    public void EnterMenu()
    {
        cachedMenuCanvas.Active();
        cachedHudCanvas.gameObject.SetActive(false);
    }  
    public void DisableMenu()
    {
        cachedMenuCanvas.Disable();
        cachedHudCanvas.gameObject.SetActive(true);
    }

    /* Store */
    public void EnterStore(Store _Store)
    { 
        cachedInvenCanvas.Active();
        cachedStoreCanvas.Active(_Store);
        cachedHudCanvas.gameObject.SetActive(false);
    } 
    public void DisableStore()
    {  
        cachedInvenCanvas.Disable();
        cachedStoreCanvas.Disable();
        cachedHudCanvas.gameObject.SetActive(true);
    }

    /* WareHouse */
    public void EnterWareHouse(WareHouse _wareHouse)
    {
        cachedInvenCanvas.Active();
        cachedWareHouseCanvas.Active(_wareHouse);
        cachedHudCanvas.gameObject.SetActive(false);
    }
    public void DisableWareHouse() 
    { 
        cachedInvenCanvas.Disable();
        cachedWareHouseCanvas.Disable();
        cachedHudCanvas.gameObject.SetActive(true);
    }

    /* Quest */
    public void EnterQuest(HandleQuest _handleQuest)
    {  
        cachedQuestCanvas.ActiveAllDomainButton(_handleQuest);
        cachedHudCanvas.gameObject.SetActive(false);
    }
    public void DisableQuest()
    {
        cachedQuestCanvas.Disable();
        cachedHudCanvas.gameObject.SetActive(true);
    }

    /* Adorn */
    public void ActiveAdornCanvas()
    {
        cachedAdornCanvas.Active();
    }
    public void DisableAdornCanvas() 
    {
        cachedAdornCanvas.Disable();
    }
     
    public void EnterAdorn(FRadiuseCollaspeInfo _info)
    {  
        cachedHudCanvas.gameObject.SetActive(false);
        cachedCollapseCanvas.Active(_info);
    }    
    public void DisableAdorn()
    {
        DisableAdornCanvas();
        HUD_AcitveHUDCanvas(true);
    }

    /* Sleep */
    public void EnterSleepCanvas()
    { 
        cachedHudCanvas.gameObject.SetActive(false);
        cachedSleepCanvas.Active();
    } 
    public void DisableSleepCanvas()
    {
        cachedHudCanvas.gameObject.SetActive(true);
        cachedSleepCanvas.Disable();
    } 
  
    /* SetLevel */
    public void EnterSetLevel(FRadiuseCollaspeInfo _info)
    {
        cachedHudCanvas.gameObject.SetActive(false);
        cachedCollapseCanvas.Active(_info);
    } 
    public void DisableSetLevel()
    {
        cachedHudCanvas.gameObject.SetActive(true);
        cachedSetLevelCanvas.Disable();
    } 
    public void ActiveSetLevelCanvas()
    {
        cachedSetLevelCanvas.Active(); 
    }

    /* EnterMoveToLevel */
    public void EnterMoveToLevel(FRadiuseCollaspeInfo _info)
    {
        cachedHudCanvas.gameObject.SetActive(false);
        ActiveCollaspeCanvas(_info); 
    } 
     
    /* CollaspesCanvas */
    public void ActiveCollaspeCanvas(FRadiuseCollaspeInfo _info)
    {
        cachedCollapseCanvas.Active(_info);
    }

    /* SallyForthCanvas */
    public void EnterSallyCanvas(Action _bindAction)
    { 
        cachedHudCanvas.gameObject.SetActive(false);
        cachedSallyCanvas.Actvie(_bindAction);
    }

    /* SallyForthCanvas */
    public void EnterSallyCanvas(string _duck, string _weapon, Action _bindAction)
    { 
        cachedHudCanvas.gameObject.SetActive(false);
        cachedSallyCanvas.Active(_duck, _weapon, _bindAction);
    } 

    /* ESC */
    public void EnterStopCanvas()
    {
        cachedHudCanvas.gameObject.SetActive(false);
        cachedStopCanvas.Active();
    } 
    public void DisableStopCanvas()
    {
        cachedHudCanvas.gameObject.SetActive(true);
        cachedStopCanvas.Disable();
    }

    /* DisableCanvas */
    public void InstantActiveDisableCanvas()
    { 
        cachedDisableCanvas.ActiveInstant();
    } 
    public void InstantDisableDisableCanvas()
    { 
        cachedDisableCanvas.DisableInstant();
    }
}
 