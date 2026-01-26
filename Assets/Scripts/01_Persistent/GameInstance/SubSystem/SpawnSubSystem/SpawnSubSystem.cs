using UnityEngine;

public class SpawnSubSystem : GameInstanceSubSystem
{ 
    private ItemSpawner cachedItemSpawner;
    private PrefabSpawner cachedPrefabSpawner;
    // Spawner?

    public override void Init()
    {
        CachedSpawner(); 
    }
    public override void LevelStart(ELevelType _type)
    {

    }
    public override void LevelEnd(ELevelType _type)
    {

    }

    public PrefabSpawner GetPrefebSpawner() { return cachedPrefabSpawner; }
      
    public ItemBase MakeItem(EItemID _id)
    {
        return cachedItemSpawner.MakeItem(_id);
    }
    public ItemBase MakeItem(FItemShell _entry)
    { 
        return cachedItemSpawner.MakeItem(_entry);
    } 
    public ItemBase MakeWeapon(EWeaponType _type, EItemGrade _grade)
    {
        return cachedItemSpawner.MakeWeapon(_type, _grade);   
    }
    public ItemBase MakeArmor(EArmorType _type, EItemGrade _grade)
    {
        return cachedItemSpawner.MakeArmor(_type, _grade);
    } 

    public ItemBase MakeBullet(int _cnt, EItemGrade _grade)
    {
        return cachedItemSpawner.MakeBullet(_cnt, _grade);
    }
    public ItemBase MakeRandomHeal(EItemGrade _grade)
    {
        return cachedItemSpawner.MakeRandomHeal(_grade);
    }
    public ItemBase MakeRandomFood(EItemGrade _grade)
    {
        return cachedItemSpawner.MakeRandomFood(_grade);
    }
    public ItemBase MakeRandomBackpack(EItemGrade _grade)
    {
        return cachedItemSpawner.MakeRandomBackpack(_grade);
    }
    public ItemBase MakeRandomAttachment(EItemGrade _grade)
    {
        return cachedItemSpawner.MakeRandomAttachment(_grade);
    }
    public ItemBase MakeRandomStuff(EItemGrade _grade)
    {
        return cachedItemSpawner.MakeRandomStuff(_grade);
    }
     
    public GameObject MakeBillboardPrefab()
    {
        return cachedPrefabSpawner.SpawnItemBillboard();
    }
    public GameObject MakeRaserBeamPrefab()
    {
        return cachedPrefabSpawner.SpawnRaserBeam();
    }
    public GameObject MakeHitValuePrefab()
    {
        return cachedPrefabSpawner.SpawnHitValue();
    } 
    private void CachedSpawner()
    {
        var arrItem = Object.FindObjectsByType<ItemSpawner>(FindObjectsSortMode.None);
        if (arrItem.Length > 0)
            cachedItemSpawner = arrItem[0];

        var arrPrefab = Object.FindObjectsByType<PrefabSpawner>(FindObjectsSortMode.None);
        if (arrPrefab.Length > 0)
            cachedPrefabSpawner = arrPrefab[0];
    }

}
