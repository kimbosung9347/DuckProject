using UnityEngine;
using System.Collections.Generic;
 
public class AiStorage : DuckStorage 
{
    /////////////////////////////////////////
    // 슬롯별 아이템 풀 + 내부 가중치
    [SerializeField] private List<FWeightedItemID> listWeapon;
    [SerializeField] private List<FWeightedItemID> listHelmet;
    [SerializeField] private List<FWeightedItemID> listArmor;
    [SerializeField] private List<FWeightedItemID> listBackpack;
    [SerializeField] private List<FWeightedItemID> listBullet;

    // 슬롯 생성 확률 (슬롯 자체가 생길지 말지)
    [SerializeField, Range(0f, 1f)] private float weaponSpawnChance = 1f;
    [SerializeField, Range(0f, 1f)] private float helmetSpawnChance = 0.7f;
    [SerializeField, Range(0f, 1f)] private float armorSpawnChance = 0.8f;
    [SerializeField, Range(0f, 1f)] private float backpackSpawnChance = 0.5f;

    private ItemBase selectWeapon;
    private ItemBase selectHelmet;
    private ItemBase selectArmor;
    private ItemBase selectBackpack;

    private EItemID selectBulletId = EItemID._END;

    private void Start()
    {
        MakeRandomEquip();
    } 
     
    public ItemBase GetSelectWeapon() { return selectWeapon; }
    public ItemBase GetSelectHelmat() { return selectHelmet; }
    public ItemBase GetSelectArmor() { return selectArmor; }
    public ItemBase GetSelectBackpack() { return selectBackpack; }
    public EItemID GetSelectBulletId() { return selectBulletId; }
     
    // =========================
    // Weighted Random Pick
    // =========================
    private EItemID GetRandomItemID(List<FWeightedItemID> list)
    {
        if (list == null || list.Count == 0)
            return EItemID._END;

        float total = 0f;
        foreach (var e in list)
            total += e.weight;

        if (total <= 0f)
            return EItemID._END;

        float roll = Random.value * total;
        float acc = 0f;

        foreach (var e in list)
        {
            acc += e.weight;
            if (roll <= acc)
                return e.itemId;
        }

        return EItemID._END;
    }
    private void MakeRandomEquip()
    {
        var gameInstance = GameInstance.Instance;

        // =========================
        // Weapon
        // =========================
        if (Random.value <= weaponSpawnChance)
        {
            EItemID weaponId = GetRandomItemID(listWeapon);
            if (weaponId != EItemID._END)
            {
                selectWeapon = gameInstance.SPAWN_MakeItem(weaponId);
                cachedDuckEquip.EquipItem(EEquipSlot.Weapon, selectWeapon);

                // Bullet
                if (listBullet != null && listBullet.Count > 0)
                {
                    selectBulletId = GetRandomItemID(listBullet);
                }
                else
                {
                    selectBulletId = EItemID.BULLET_Rust;
                }

                if (selectWeapon is Weapon realWeapon)
                {
                    realWeapon.SetCurBulletId(selectBulletId);
                    realWeapon.FillBullet();

                    realWeapon.CacheDuckEquip(GetComponent<DuckEquip>());
                    realWeapon.CacheDuckStorage(this);
                }
            }
        }

        // =========================
        // Helmet
        // =========================
        if (Random.value <= helmetSpawnChance)
        {
            EItemID helmetId = GetRandomItemID(listHelmet);
            if (helmetId != EItemID._END)
            {
                selectHelmet = gameInstance.SPAWN_MakeItem(helmetId);
                cachedDuckEquip.EquipItem(EEquipSlot.Helmet, selectHelmet);
            }
        }

        // =========================
        // Armor
        // =========================
        if (Random.value <= armorSpawnChance)
        {
            EItemID armorId = GetRandomItemID(listArmor);
            if (armorId != EItemID._END)
            {
                selectArmor = gameInstance.SPAWN_MakeItem(armorId);
                cachedDuckEquip.EquipItem(EEquipSlot.Armor, selectArmor);
            }
        }

        // =========================
        // Backpack
        // =========================
        if (Random.value <= backpackSpawnChance)
        {
            EItemID backpackId = GetRandomItemID(listBackpack);
            if (backpackId != EItemID._END)
            {
                selectBackpack = gameInstance.SPAWN_MakeItem(backpackId);
                cachedDuckEquip.EquipItem(EEquipSlot.Backpack, selectBackpack);
            }
        }

        GetComponentInChildren<AiDetected>()?.DisableDuckRenderer();
    }
}
