using UnityEngine;

public static class DuckDefine
{
    public const int PLAYER_MAX_LEVEL = 50;
      
    public const int DISABLE_CINEMACHIN_PRIORITY = 0;
    public const int NORMAL_CINEMACHIN_PRIORITY = 20;
    public const int INTERACTION_CINEMACHIN_PRIORITY = 40;
     
    public const int MAX_SLOT_CNT = 3;
    public const int MAX_QUICKSLOT_CNT = 7;
    public const int MAX_ITEMBOX_COUNT = 10; 
    public const int MAX_WAREHOUSE_COUNT = 30;

    public const float MAX_RECOIL_SIZE = 75f;

    public const float WARNING_WEIGHT_RATIO = 0.75f;
    public const float STAMINA_ROLL_VALUE = 10f;

    public const float DEFAULT_ZOOM_CAMERA_DISTANCE = 12f;
    public const float AIM_ZOOM_CAMERA_DISTANCE = 11f;
     
    public static string GetSceneName(ELevelType levelType)
    {
        switch (levelType)
        {
            case ELevelType.Persistent: return "Persistent";
            case ELevelType.Loading: return "Loading";
            case ELevelType.Mainmenu: return "Mainmenu";
            case ELevelType.Home: return "Home"; 
            case ELevelType.Farm: return "Farm";
            case ELevelType.Test: return "Test";
            default: return string.Empty;
        }
    } 

    public static float GetHitDamageByDifficult(EPlayDifficultType _type)
    {
        switch (_type)
        {
            case EPlayDifficultType.Easy:
            {
                return 0.4f;
            }
                 
            case EPlayDifficultType.Hard:
            {
                return 0.8f;  
            }
        }
        return 1;
    }
    public static float GetHpByDifficult(EPlayDifficultType _type)
    {
        switch (_type)
        {
            case EPlayDifficultType.Easy:
                {
                    return 0.5f;
                }

            case EPlayDifficultType.Hard:
                {
                    return 0.7f;
                } 
        }
        return 1;
    } 
}      
   
public static class DuckUtill  
{
    // ================================
    // 대분류 (Equipment / Attachment / Consumable / Material)
    // ================================
    public static EItemType GetItemTypeByItemID(EItemID id)
    {  
        int v = (int)id;

        if ((int)EItemID._EQUIP_START < v && v < (int)EItemID._EQUIP_END)
            return EItemType.Equipment;

        if ((int)EItemID._ATTACH_START < v && v < (int)EItemID._ATTACH_END)
            return EItemType.Attachment;

        if ((int)EItemID._CONSUM_START < v && v < (int)EItemID._CONSUM_END)
            return EItemType.Consumable;

        if ((int)EItemID._MATERIAL_START < v && v < (int)EItemID._MATERIAL_END)
            return EItemType.Material;

        return EItemType.None;
    }

     
    // ================================
    // 장비 슬롯 (Weapon / Helmet / Armor / Backpack)
    // ================================
    public static EEquipSlot GetEquipTypeByItemID(EItemID id)
    {
        int v = (int)id;

        if ((int)EItemID._WEAPON_START < v && v < (int)EItemID._WEAPON_END)
            return EEquipSlot.Weapon;

        if ((int)EItemID._HELMET_START < v && v < (int)EItemID._HELMET_END)
            return EEquipSlot.Helmet; 

        if ((int)EItemID._ARMOR_START < v && v < (int)EItemID._ARMOR_END)
            return EEquipSlot.Armor;

        if ((int)EItemID._BACKPACK_START < v && v < (int)EItemID._BACKPACK_END)
            return EEquipSlot.Backpack;

        return EEquipSlot.End;
    }

     

    // ===========
    // 부착물 슬롯 
    // ===========
    public static EAttachmentType GetAttachTypeByItemID(EItemID id)
    {
        int v = (int)id;

        if ((int)EItemID._COMPENSATOR_START < v && v < (int)EItemID._COMPENSATOR_END)
            return EAttachmentType.Muzzle;

        if ((int)EItemID._SCOPE_START < v && v < (int)EItemID._SCOPE_END)
            return EAttachmentType.Scope;

        if ((int)EItemID._BUTTSTOCK_START < v && v < (int)EItemID._BUTTSTOCK_END)
            return EAttachmentType.Stock;
          
        return EAttachmentType.End;
    } 

    // ================================
    // 소비품 소분류 (Bullet / Heal / Food)
    // ================================
    public static EConsumableType GetConsumTypeByItemID(EItemID id)
    {
        int v = (int)id;

        if ((int)EItemID._BULLET_START < v && v < (int)EItemID._BULLET_END)
            return EConsumableType.Bullet;

        if ((int)EItemID._HEAL_START < v && v < (int)EItemID._HEAL_END)
            return EConsumableType.Heal;

        if ((int)EItemID._FOOD_START < v && v < (int)EItemID._FOOD_END)
            return EConsumableType.Food;

        return EConsumableType.None;
    }
    public static ItemPair GetItemPair(EItemID itemId)
    { 
        return GameInstance.Instance.TABLE_GetItemTable().GetItemPair(itemId);
    } 
    public static Sprite GetItemSprite(EItemID id)
    { 
        var pair = GameInstance.Instance.TABLE_GetItemTable().GetItemPair(id);
        return pair?.visual?.iconSprite;
    }
    public static string GetItemName(EItemID id)
    { 
        var pair = GameInstance.Instance.TABLE_GetItemTable().GetItemPair(id);
        return pair.data.itemName;
    } 
    public static float GetBulletDamage(EItemID id)
    {
        if (id == EItemID._END)
            return 0f;
         
        var pair = GameInstance.Instance.TABLE_GetItemTable().GetItemPair(id);
        if (pair.data is BulletData bulletData)
        {
            return bulletData.addDamage;
        }
         
        return 0f;
    }   
    public static float GetItemWeight(EItemID id)
    {  
        var pair = GameInstance.Instance.TABLE_GetItemTable().GetItemPair(id);
        return pair.data.weight;
    } 

    // ================================
    // 버프 종류 
    // ================================
    public static bool IsBuff(EBuffID _buffId)
    {
        int v = (int)_buffId;

        if ((int)EBuffID._BUFF_START < v && v < (int)EBuffID._BUFF_END)
            return true;

        else if ((int)EBuffID._DEBUFF_START < v && v < (int)EBuffID._DEBUFF_END)
            return false;

        return false;
    }

    // ===========
    // NPC 
    // ===========
    public static bool IsMatch_NpcBetweenQeust(EQuestID _questId, EDuckType _duckType)
    { 
        if (_questId == EQuestID.End ||
            _duckType == EDuckType.End)
            return false;

        var questData = GameInstance.Instance.TABLE_GetQuestData(_questId);
        bool isMatch = questData.gaveQuestDuck == _duckType;
        return isMatch;
    }

    // ===========
    // Sound 
    // ===========
     
    public static float CalcTimeFitPitch(
        float clipLength,
        float targetTime,
        float blend = 0.35f,
        float minPitch = 0.85f,
        float maxPitch = 1.25f) 
    {   
        if (targetTime <= 0.01f || clipLength <= 0f)
            return 1f;

        float target = clipLength / targetTime;
        target = Mathf.Lerp(1f, target, blend);
        return Mathf.Clamp(target, minPitch, maxPitch);
    }
}
