using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemData : ScriptableObject
{
    [Tooltip("고유 아이디")]
    public EItemID itemID;

    [Tooltip("아이템 이름")]
    public string itemName; 
     
    [Tooltip("아이템 설명")]
    [TextArea] public string itemDesc;
     
    [Tooltip("아이템 종류")]
    public EItemType itemType;

    [Tooltip("가격")]
    public int price;
     
    [Tooltip("무게")]
    public float weight;

    [Tooltip("등급")]
    public EItemGrade grade;
     
    public List<FStatPair> GetStats()
    {
        var newStats =  new List<FStatPair>
        {
            new ("가격", price.ToString()),
        };
         
        AddStats(newStats);

        return newStats;
    }
    public EItemGrade GetItemGrade() { return grade; }
    public string GetItemDisplayName()
    {
        if (string.IsNullOrEmpty(name))
            return string.Empty;
         
        string[] parts = name.Split('_');
        return parts[^1]; // 마지막 요소 (C# 8.0 이상)
    } 
    public void SetItemGrade(EItemGrade _grade)
    {
        grade = _grade;
    }
    protected virtual void AddStats(List<FStatPair> _list)
    {  
    } 
}

/* ==================
        장비
 ================== */
public enum EEquipmentType
{
    None,
    Weapon,
    Armor,
    Bag,
}
 
public class EquipData : ItemData
{
    [Tooltip("장비 타입")]
    public EEquipmentType equipType;
     
    [Tooltip("최대 내구도")]
    public float maxDurability;

    [Tooltip("내구도 감소율")]
    public float reduceDurability;
} 

/* ===================
        소비
==================== */
public enum EConsumableType
{
    None,
    Heal,
    Food, 
    Bullet,
}   
public class ConsumData : ItemData
{ 
    [Tooltip("소비 타입")]
    public EConsumableType consumType;

    [Tooltip("개수 혹은 내구도")]
    public bool isCapacity;
     
    [Tooltip("최대 보관 개수")]
    public int maxStoreCnt;
    
    // 최대 보관 개수가 1이다 - 이 경우 최대 용량을 사용함  
    [Tooltip("최대 용량")]
    public float maxCapacity;

    //////////////////////////////
    // 개수를 여러개 담을 수 있는지 

    protected override void AddStats(List<FStatPair> _list)
    {
        base.AddStats(_list);

        if (isCapacity)
        {
            _list.Add(new("최대 용량", maxCapacity.ToString()));
        }
        else
        { 
            _list.Add(new("최대 보관 개수", maxCapacity.ToString()));
        }
    }

}


/* ============
    열거형 
 ============ */
public enum EItemID
{
    ////////////////////////////////
    // ## EQUIPMENT (1 ~ 400)
    ////////////////////////////////

    [InspectorName(null)]
    _EQUIP_START = 0,


    //////////////////////////////////
    // # WEAPON (1 ~ 200)
    //////////////////////////////////
    ///
    [InspectorName(null)]
    _WEAPON_START = 1,


    // ---------- PISTOL (2 ~ 50)

    [InspectorName(null)]
    _PISTOL_START = 2,

    PISTOL_PM = 3,
    PISTOL_Magunm44,

    [InspectorName(null)]
    _PISTOL_END = 50,


    // ---------- RIFLE (50 ~ 100)
    [InspectorName(null)]
    _RIFLE_START = 51,

    RIFLE_AK47 = 52,
    RIFLE_FAMAS,
    RIFLE_M16,
    RIFLE_MP5,

    [InspectorName(null)]
    _RIFLE_END = 100,


    // ---------- ShotGun (100 ~ 150)
    [InspectorName(null)]
    _SHOTGUN_START = 101,

    SHOTGUN_DobbleBerrel = 102,
    SHOTGUN_Remington870,

    [InspectorName(null)]
    _SHOTGUN_END = 150,


    // ---------- Snipe (150 ~ 200)
    [InspectorName(null)]
    _SNIPE_START = 151,

    SNIPE_Kar98k = 152,

    [InspectorName(null)]
    _SNIPE_END = 199,


    // WEAPON 전체 범위 끝

    [InspectorName(null)]
    _WEAPON_END = 200,


    //////////////////////////////////
    // # HELMET (201 ~ 250)
    //////////////////////////////////

    [InspectorName(null)]
    _HELMET_START = 201,

    HELMET_LV1,
    HELMET_LV2,
    HELMET_LV3,
    HELMET_LV4,
    HELMET_LV5,

    [InspectorName(null)]
    _HELMET_END = 250,

    //////////////////////////////////
    // # ARMOR (251 ~ 300)
    //////////////////////////////////

    [InspectorName(null)]
    _ARMOR_START = 251,

    ARMOR_LV1,
    ARMOR_LV2,
    ARMOR_LV3,
    ARMOR_LV4,
    ARMOR_LV5,

    [InspectorName(null)]
    _ARMOR_END = 300,


    //////////////////////////////////
    // # BACKPACK (301 ~ 350)
    //////////////////////////////////
    [InspectorName(null)]
    _BACKPACK_START = 301,

    BACKPACK_Schoolbag,
    BACKPACK_Travalbag,
    BACKPACK_Survivalbag,
    BACKPACK_Battlebag,

    [InspectorName(null)]
    _BACKPACK_END = 350,


    [InspectorName(null)]
    _EQUIP_END = 400,


    ////////////////////////////////
    // ## ATTACHMENT (401 ~ 450)
    ////////////////////////////////

    [InspectorName(null)]
    _ATTACH_START = 401,

    [InspectorName(null)]
    _COMPENSATOR_START = 402,

    COMPENSATOR_A,
    COMPENSATOR_B,
    COMPENSATOR_C,

    [InspectorName(null)]
    _COMPENSATOR_END = 409,

    [InspectorName(null)]
    _SCOPE_START = 410,

    SCOPE_A,
    SCOPE_B,
    SCOPE_C,

    [InspectorName(null)]
    _SCOPE_END = 419,

    [InspectorName(null)]
    _BUTTSTOCK_START = 420,

    BUTTSTOCK_A,
    BUTTSTOCK_B,
    BUTTSTOCK_C,

    [InspectorName(null)]
    _BUTTSTOCK_END = 429,

    [InspectorName(null)]
    _ATTACH_END = 450,

    ////////////////////////////////////
    // ## CONSUMABLE (451 ~ 500)
    ////////////////////////////////////

    [InspectorName(null)]
    _CONSUM_START = 451,


    // BULLET (452 ~ 470)
    [InspectorName(null)]
    _BULLET_START = 452,

    BULLET_Rust,
    BULLET_General,
    BULLET_Armor,
    BULLET_AdvanceArmor,
    BULLET_Speical,

    [InspectorName(null)]
    _BULLET_END = 470,


    // HEAL (471 ~ 485)
    [InspectorName(null)]
    _HEAL_START = 471,

    HEAL_bandage,
    HEAL_SmallMedkit,
    HEAL_MiddleMedkit,
    HEAL_LargeMedkit,

    [InspectorName(null)]
    _HEAL_END = 485,


    // FOOD (486 ~ 500) 
    [InspectorName(null)]
    _FOOD_START = 486,

    FOOD_Apple,
    FOOD_Candy,
    FOOD_Carrot,
    FOOD_Chcolate,
    FOOD_CocoaMilk,
    FOOD_Potato,
    FOOD_Pumpkin,
    FOOD_Water,
    FOOD_Yogurt,

    [InspectorName(null)]
    _FOOD_END = 500,

    [InspectorName(null)]
    _CONSUM_END = 500,


    ////////////////////////////////
    // ## Stuff (501 ~ 700)
    ////////////////////////////////

    [InspectorName(null)]
    _MATERIAL_START = 501,

    STUFF_BOLT,
    STUFF_CUP,
    STUFF_NoteFiles,
    STUFF_Fiber,
    STUFF_FootBall,
    STUFF_Glasses,
    STUFF_Kleenex,
    STUFF_MedicineBottle,
    STUFF_MetalSlice,
    STUFF_Newspaper,
    STUFF_Scissor,
    STUFF_Tambourine,
    STUFF_Tape,
    STUFF_WeaponPart1,
    STUFF_Wheel,
    STUFF_Wood,
    STUFF_Airplane,
    STUFF_Basket,
    STUFF_BasketBall,
    STUFF_Battery1,
    STUFF_Book,
    STUFF_Book2,
    STUFF_ColdCore,
    STUFF_EnergySavingBulp,
    STUFF_Feather,
    STUFF_FiverGray,
    STUFF_Mike,
    STUFF_Radio,
    STUFF_Rocket,
    STUFF_WeaponPart2,

    STUFF_Battery2,
    STUFF_Compass,
    STUFF_EnergySavingBulp2,
    STUFF_FiberGreen,
    STUFF_FlamingCore,
    STUFF_MobilePhone,
    STUFF_Trophy,
    STUFF_WeaponPart3,

    STUFF_AncientCoin,
    STUFF_Bitcoin,
    STUFF_GPU,
    STUFF_GoldBadge,

    STUFF_LEDX,
    STUFF_Crown,

    [InspectorName(null)]
    _MATERIAL_END = 700,

    // End 
    _END,
}
public enum EItemGrade
{
    None = 0,
    Normal,
    Rare,
    Unique,
    Epic,
    Legend,
}
public enum EItemType
{
    None,
    Equipment,
    Attachment,
    Consumable,
    Material,
}
public enum EArmorType
{
    None,
    Helmet,
    Armor,
} 
public enum EMaterialType
{
    None,
    Generic,
}
  