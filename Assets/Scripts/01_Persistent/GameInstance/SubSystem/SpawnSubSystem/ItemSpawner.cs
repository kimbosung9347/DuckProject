using System.Net.Mail;
using UnityEngine;
using UnityEngine.UI;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] ItemTable cachedItemTable;
       
    ///////////////////////////////////// 
    public ItemBase MakeItem(EItemID _id)
    {
        ItemPair itemPiar = cachedItemTable.GetItemPair(_id);
        switch (itemPiar.data.itemType)
        {
            case EItemType.Equipment: 
            {
                return MakeEquip(itemPiar);
            }

            case EItemType.Attachment:
            {
                return MakeAttachment(itemPiar); 
            } 
            
            case EItemType.Consumable:
            {
                return MakeConsum(itemPiar);
            }
            
            case EItemType.Material:
            {
                return MakeStuff(itemPiar);
            } 
        }
         
        // Crash 
        return InvalidItem();
    }  
    public ItemBase MakeItem(FItemShell _entry)
    {
        var item = GameInstance.Instance.SPAWN_MakeItem(_entry.itemID);

        if (item is ConsumBase consum) 
        {
            consum.SetCurCnt(_entry.cnt);
            consum.SetCapacityRatio(_entry.durabilityRatio);
        }

        else if (item is EquipBase equip)
        {
            equip.SetCurDrabilityRtio(_entry.durabilityRatio);

            // 무기라면,  
            if (equip is Weapon weapon)
            {
                weapon.SetCurBullet(_entry.bulletCnt);
                weapon.SetCurBulletId(_entry.bulletId);

                weapon.SetCurBullet(_entry.bulletCnt);
                weapon.SetCurBulletId(_entry.bulletId);

                TryAttach(weapon, _entry.muzzleAttach);
                TryAttach(weapon, _entry.scopeAttach);
                TryAttach(weapon, _entry.stockeAttach);
            }
        }

        return item;
    }

     
    public ItemBase MakeWeapon(EWeaponType _type, EItemGrade _grade)
    {
        ItemPair pair = cachedItemTable.GetRandomWeaponDataPair(_type, _grade);
        if (pair == null || pair.data == null /*|| pair .visual == null*/ || pair.data is not WeaponData weaponData)
        { 
            return null;
        }

        GameObject obj = new GameObject($"Weapon_{weaponData.GetItemDisplayName()}");
        Weapon weapon = obj.AddComponent<Weapon>(); 
        weapon.Init(weaponData, pair.visual);
        return weapon;
    }
    public ItemBase MakeArmor(EArmorType _type, EItemGrade _grade)
    { 
        ItemPair pair = cachedItemTable.GetRandomArmorDataPair(_type, _grade);
        if (pair == null || pair.data == null /*|| pair .visual == null*/ || pair.data is not ArmorData armorData)
        {
            return null;
        } 

        GameObject obj = null;
        if (armorData.armorType == EArmorType.Armor)
        {
            obj = new GameObject($"Armor_{armorData.GetItemDisplayName()}");
        }
        else if (armorData.armorType == EArmorType.Helmet)
        {
            obj = new GameObject($"Helmat_{armorData.GetItemDisplayName()}");
        } 

        Armor armor = obj.AddComponent<Armor>();
        armor.Init(armorData, pair.visual);
        return armor;
    } 
      
    public ItemBase MakeBullet(int _cnt, EItemGrade _grade)
    {
        ItemPair pair = cachedItemTable.GetBulletDataPair(_grade);
        if (pair == null || pair.data == null /*|| pair .visual == null*/ || pair.data is not BulletData bulletData)
        {
            return null;
        }
         
        GameObject obj = new GameObject($"Bullet_{bulletData.GetItemDisplayName()}");
        Bullet bullet = obj.AddComponent<Bullet>();
        bullet.Init(bulletData, pair.visual);
        bullet.SetCurCnt(_cnt);
        return bullet; 
    }   
    public ItemBase MakeRandomHeal(EItemGrade _grade)
    {
        ItemPair pair = cachedItemTable.GetHealDataPair(_grade);
        if (pair == null || pair.data == null /*|| pair .visual == null*/ || pair.data is not HealData healData)
        {
            return null;
        }

        GameObject obj = new GameObject($"Heal_{healData.GetItemDisplayName()}");
        HealItem heal = obj.AddComponent<HealItem>();
        heal.Init(healData, pair.visual);
        return heal; 
    }
    public ItemBase MakeRandomFood(EItemGrade _grade)
    {
        ItemPair pair = cachedItemTable.GetFoodDataPair(_grade);
        if (pair == null || pair.data == null /*|| pair .visual == null*/ || pair.data is not FoodData foodData)
        {
            return null;
        } 
      
        GameObject obj = new GameObject($"Food_{foodData.GetItemDisplayName()}");
        Food food = obj.AddComponent<Food>();
        food.Init(foodData, pair.visual);
        return food; 
    }
    public ItemBase MakeRandomBackpack(EItemGrade _grade)
    {
        ItemPair pair = cachedItemTable.GetBackpackDataPair(_grade);
        if (pair == null || pair.data == null /*|| pair .visual == null*/ || pair.data is not BackpackData backpackData)
        {
            return null;
        }
         
        GameObject obj = new GameObject($"Backpack_{backpackData.GetItemDisplayName()}");
        Backpack backpack = obj.AddComponent<Backpack>();
        backpack.Init(backpackData, pair.visual);
        return backpack;
    }
    public ItemBase MakeRandomAttachment(EItemGrade _grade)
    {
        ItemPair pair = cachedItemTable.GetAttachmentDataPair(_grade);
        if (pair == null || pair.data == null /*|| pair .visual == null*/ || pair.data is not AttachmentData attachmentData)
        {
            return null;
        }

        GameObject obj = new GameObject($"Attachment_{attachmentData.GetItemDisplayName()}");
        Attachment attachment = obj.AddComponent<Attachment>();
        attachment.Init(attachmentData, pair.visual);
        return attachment;
    } 
    public ItemBase MakeRandomStuff(EItemGrade _grade)
    {
        ItemPair pair = cachedItemTable.GetStuffDataPair(_grade);
        if (pair == null || pair.data == null /*|| pair .visual == null*/ || pair.data is not StuffData stuffData)
        {
            return null; 
        } 

        GameObject obj = new GameObject($"Stuff_{stuffData.GetItemDisplayName()}");
        Stuff stuff = obj.AddComponent<Stuff>();
        stuff.Init(stuffData, pair.visual);
        return stuff; 
    }


    private ItemBase MakeEquip(ItemPair _itemPair)
    {
        EEquipSlot equipSlot = DuckUtill.GetEquipTypeByItemID(_itemPair.data.itemID);
        switch (equipSlot)
        {
            case EEquipSlot.Weapon:
            {
                if (_itemPair.data is WeaponData weaponData)
                {
                    GameObject obj = new GameObject($"Weapon_{weaponData.GetItemDisplayName()}");
                    Weapon weapon = obj.AddComponent<Weapon>();
                    weapon.Init(weaponData, _itemPair.visual);
                    return weapon;
                }
            }
            break; 
                  

            case EEquipSlot.Helmet:
            {
                if (_itemPair.data is ArmorData armor)
                {
                    GameObject obj = new GameObject($"Helmat_{armor.GetItemDisplayName()}");
                    Armor helmet = obj.AddComponent<Armor>();
                    helmet.Init(armor, _itemPair.visual);
                    return helmet; 
                }
            } 
            break;

            case EEquipSlot.Armor:
            {
                if (_itemPair.data is ArmorData armor)
                { 
                    GameObject obj = new GameObject($"Armor_{armor.GetItemDisplayName()}");
                    Armor bodyArmor = obj.AddComponent<Armor>();
                    bodyArmor.Init(armor, _itemPair.visual);
                    return bodyArmor; 
                }
            }
            break;

            case EEquipSlot.Backpack:
            {
                if (_itemPair.data is BackpackData backpackData)
                {
                    GameObject obj = new GameObject($"Backpack_{backpackData.GetItemDisplayName()}");
                    Backpack backpack = obj.AddComponent<Backpack>();
                    backpack.Init(backpackData, _itemPair.visual);
                    return backpack; 
                }
            }
            break;
        }
         
        return null;
    }
    private ItemBase MakeConsum(ItemPair _itemPair)
    {
        EConsumableType consumType = DuckUtill.GetConsumTypeByItemID(_itemPair.data.itemID);
        switch (consumType)
        {
            case EConsumableType.Heal:
            {
                if (_itemPair.data is HealData healData)
                {
                    GameObject obj = new GameObject($"Heal_{healData.GetItemDisplayName()}");
                    HealItem heal = obj.AddComponent<HealItem>();
                    heal.Init(healData, _itemPair.visual);
                    return heal;
                }
            } 
            break;

            case EConsumableType.Food:
            {
                if (_itemPair.data is FoodData foodData)
                {

                    GameObject obj = new GameObject($"Food_{foodData.GetItemDisplayName()}");
                    Food food = obj.AddComponent<Food>();
                    food.Init(foodData, _itemPair.visual);
                    return food; 
                }
            }
            break;

            case EConsumableType.Bullet:
            {
                if (_itemPair.data is BulletData bulletData)
                {
                    GameObject obj = new GameObject($"Bullet_{bulletData.GetItemDisplayName()}");
                    Bullet bullet = obj.AddComponent<Bullet>();
                    bullet.Init(bulletData, _itemPair.visual);
                    bullet.SetCurCnt(Random.Range(25, 40)); 
                    return bullet;  
                }
            }
            break;
        }
          
        return null;
    }
    private ItemBase MakeAttachment(ItemPair _itemPiar)
    {
        if (_itemPiar.data is AttachmentData attachmentData)
        {
            GameObject obj = new GameObject($"Attechment_{attachmentData.GetItemDisplayName()}");
            Attachment attachment = obj.AddComponent<Attachment>();
            attachment.Init(attachmentData, _itemPiar.visual);
            return attachment;
        }
         
        return null;
    }
    private ItemBase MakeStuff(ItemPair _itemPiar)
    {
        if (_itemPiar.data is StuffData stuffData)
        {
            GameObject obj = new GameObject($"Stuff_{stuffData.GetItemDisplayName()}");
            Stuff stuff = obj.AddComponent<Stuff>();
            stuff.Init(stuffData, _itemPiar.visual);
            return stuff;
        }

        return null;
    }

    private ItemBase InvalidItem()
    { 
        Debug.Log("NotFound"); 
        return null;
    }
    private void TryAttach(Weapon weapon, EItemID attachId)
    {
        if (attachId == EItemID._END)
            return;

        var item = GameInstance.Instance.SPAWN_MakeItem(attachId);
        if (item is Attachment attach)
        {
            weapon.InsertAttachment(attach);
        }
        else 
        {
            if (item != null)
                Destroy(item.gameObject);
        }
    }

}
