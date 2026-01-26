using UnityEngine;
using System.Collections;

public class DuckEquip : MonoBehaviour
{
    protected ItemBase[] arrEquip = new ItemBase[(int)EEquipSlot.End];

    private DuckDetected cachedDuckDetected;
    private DuckStorage cachedDuckStorage;
    private DuckAttack cachedAttack; 
    private DuckAbility cachedAbility;
    private DuckAnimation cachedAnim;
    private DuckAppearance cachedSocket;
      
    protected virtual void Awake()
    {
        cachedDuckDetected = GetComponentInChildren<DuckDetected>(); 
        cachedDuckStorage = GetComponent<DuckStorage>(); 
        cachedAttack = GetComponent<DuckAttack>();
        cachedSocket = GetComponent<DuckAppearance>();
        cachedAbility = GetComponent<DuckAbility>();
        cachedAnim = GetComponent<DuckAnimation>();
    }

    public ItemBase GetItem(EEquipSlot _equipSlot)
    {
        return arrEquip[(int)_equipSlot];
    } 
    public Weapon GetWeapon()
    {
        var equip = arrEquip[(int)EEquipSlot.Weapon];
        if (equip is Weapon weapon)
        {
            return weapon;
        }

        return null;
    }
    public EquipBase GetEquip(EEquipSlot _slotType)
    {
        var equip = arrEquip[(int)_slotType];
        if (equip is EquipBase equipbase)
        {
            return equipbase;
        }
        return null;
    }
     
    public virtual void EquipItem(EEquipSlot _slot, ItemBase _item)
    {
        switch (_slot)
        {
            case EEquipSlot.Weapon:
                {
                    EquipWeapon(_item);
                }
                break;

            case EEquipSlot.Helmet:
                {
                    EquipHelmet(_item);
                }
                break;

            case EEquipSlot.Armor:
                {
                    EquipArmor(_item);
                }
                break;

            case EEquipSlot.Backpack:
                {
                    EquipBackpack(_item);
                }
                break;
        } 

        if (_item is EquipBase equip)
        {
            Transform meshRoot = equip.GetMesh().transform; // Mesh 루트 Transform
            if (meshRoot)
            {
                // meshRoot 아래의 모든 렌더러 자동 추가
                Renderer[] renderers = meshRoot.GetComponentsInChildren<Renderer>(true);
                cachedDuckDetected.AddRenderers(renderers);
            }
        } 
    }
    public virtual ItemBase DetachEquip(EEquipSlot _slot)
    {
        switch (_slot)
        {
            case EEquipSlot.Weapon:
                {
                    return DetachWeapon();
                }
                 
            case EEquipSlot.Helmet:
                {
                    return DetachHelmet();
                }

            case EEquipSlot.Armor:
                {
                    return DetachArmor();
                }

            case EEquipSlot.Backpack:
                {
                    return DetachBackpack();
                }
        }

        return null;
    }
    public virtual void RenewCurDurability(EEquipSlot _slotType)
    {
        // AI가 쓸까? 일단 보류  
    }
    public virtual void RenewCurBullet()
    {
        // AI가 쓸까? 일단 보류 
    }

    public void RenewShotInfo(ShotInfo _shotInfo)
    { 
        cachedAbility.AddCorrShotInfo(EShotStatType.AccControl, _shotInfo.accControl);
        cachedAbility.AddCorrShotInfo(EShotStatType.RecoilControl, _shotInfo.recoilControl);
        cachedAbility.AddCorrShotInfo(EShotStatType.RecoilRecovery, _shotInfo.recoilRecovery);
        cachedAbility.AddCorrShotInfo(EShotStatType.AttackRange, _shotInfo.attackRange);
        cachedAbility.AddCorrShotInfo(EShotStatType.ToAimSpeed, _shotInfo.toAimSpeed);
    }
    public void RefillBullet(EItemID _bulletId, int _cnt)
    {
        cachedDuckStorage.RefillBullet(_bulletId, _cnt);
    }
    public void RollbackBullet(EItemID _bulletId, int _cnt)
    {
        cachedDuckStorage.RollbackBullet(_bulletId, _cnt); 
    }

    protected virtual void EquipWeapon(ItemBase _item)
    {
        if (!_item)
            return;

        Weapon newWeapon = _item as Weapon;
        if (!newWeapon)
            return;
         
        newWeapon.CacheDuckEquip(this);

        // 부착
        arrEquip[(int)EEquipSlot.Weapon] = newWeapon;
        cachedAnim.ChangeEquip(true);
        cachedSocket.AttachWeapon(newWeapon);
         
        ShotInfo newShotInfo = newWeapon.GetShotInfo();
        RenewShotInfo(newShotInfo);
          
        // 건내주기
        cachedAttack?.SetWeapon(newWeapon); 
    }
    protected virtual void EquipHelmet(ItemBase _item)
    {
        Armor newArmor = _item as Armor;
        if (!newArmor)
            return;

        newArmor.CacheDuckEquip(this);

        // 부착
        arrEquip[(int)EEquipSlot.Helmet] = newArmor; 
        cachedSocket.AttachHelmat(newArmor);
        cachedSocket.HideHair();

        cachedAbility.AddCorrArmor(EArmorPart.Head, newArmor.GetHeadDefanceValue());
        cachedAbility.AddCorrArmor(EArmorPart.Body, newArmor.GetBodyDefanceValue());
        cachedAbility.AddCorrLocoInfo(ELocoStatType.MoveSpeed, -newArmor.GetArmorData().moveSpeedPenalty);
    } 
    protected virtual void EquipArmor(ItemBase _item)
    {
        Armor newArmor = _item as Armor;
        if (!newArmor)
            return;

        newArmor.CacheDuckEquip(this);

        // 부착
        arrEquip[(int)EEquipSlot.Armor] = newArmor;
        cachedSocket.AttachArmor(newArmor);
         
        cachedAbility.AddCorrArmor(EArmorPart.Head, newArmor.GetHeadDefanceValue());
        cachedAbility.AddCorrArmor(EArmorPart.Body, newArmor.GetBodyDefanceValue());
        cachedAbility.AddCorrLocoInfo(ELocoStatType.MoveSpeed, -newArmor.GetArmorData().moveSpeedPenalty);
    }
    protected virtual void EquipBackpack(ItemBase _item)
    {
        Backpack newBackpack = _item as Backpack;
        if (!newBackpack)
            return;
         
        newBackpack.CacheDuckEquip(this);

        arrEquip[(int)EEquipSlot.Backpack] = newBackpack;
        cachedSocket.AttachBackpack(newBackpack);

        BackpackData backpackData = newBackpack.GetBackpackData();
        cachedAbility.AddCorrCapacityInfo(backpackData.addStorgeSize, backpackData.addWeight);
           
        //// 인벤토리 갱신
        //cachedDuckStorage.MakeStorageByCapacity();
    }  

    protected virtual ItemBase DetachWeapon()
    {
        Weapon curWeapon = arrEquip[(int)EEquipSlot.Weapon] as Weapon;
        if (!curWeapon)
            return null;

        curWeapon.CacheDuckEquip(null);

        // 장착 해제 
        cachedAnim.ChangeEquip(false);

        // 무기 비워주기
        cachedAttack.SetWeapon(null);

        // 소켓 비워주기
        cachedSocket.DetachWeapon(); 

        // 능력치 제거
        ShotInfo shot = curWeapon.GetShotInfo();
        RenewShotInfo(-shot);
          
        // 장착 배열 비우기
        arrEquip[(int)EEquipSlot.Weapon] = null;

        // 부착 랜더러 제거 
        DetectedFromDetector(curWeapon);

        return curWeapon;
    } 
    protected virtual ItemBase DetachHelmet()
    {
        Armor curArmor = arrEquip[(int)EEquipSlot.Helmet] as Armor;
        if (!curArmor)
            return null;

        curArmor.CacheDuckEquip(null);

        // 벗기
        cachedSocket.DetachHelmat();
        cachedSocket.ShowHair();
        
        // 능력치 제거 코드
        cachedAbility.AddCorrArmor(EArmorPart.Head, -curArmor.GetHeadDefanceValue());
        cachedAbility.AddCorrArmor(EArmorPart.Body, -curArmor.GetBodyDefanceValue());
        cachedAbility.AddCorrLocoInfo(ELocoStatType.MoveSpeed, curArmor.GetArmorData().moveSpeedPenalty);

        arrEquip[(int)EEquipSlot.Helmet] = null;

        DetectedFromDetector(curArmor);

        return curArmor;
    } 
    protected virtual ItemBase DetachArmor()
    {
        Armor curArmor = arrEquip[(int)EEquipSlot.Armor] as Armor;
        if (!curArmor)
            return null;

        curArmor.CacheDuckEquip(null);
 
        // 벗기
        cachedSocket.DetachArmor(); 

        // 능력치 제거 코드
        cachedAbility.AddCorrArmor(EArmorPart.Head, -curArmor.GetHeadDefanceValue());
        cachedAbility.AddCorrArmor(EArmorPart.Body, -curArmor.GetBodyDefanceValue());
        cachedAbility.AddCorrLocoInfo(ELocoStatType.MoveSpeed, curArmor.GetArmorData().moveSpeedPenalty);
         
        arrEquip[(int)EEquipSlot.Armor] = null;

        DetectedFromDetector(curArmor);

        return curArmor;
    } 
    protected virtual ItemBase DetachBackpack()
    {
        Backpack cur = arrEquip[(int)EEquipSlot.Backpack] as Backpack;
        if (!cur) 
            return null;

        cur.CacheDuckEquip(null);

        cachedSocket.DetachBackpack();

        // 능력치 제거 코드
        if (cur is Backpack backpack)
        {
            BackpackData backpackData = backpack.GetBackpackData();
            cachedAbility.AddCorrCapacityInfo(-backpackData.addStorgeSize, -backpackData.addWeight);
        }  
         
        arrEquip[(int)EEquipSlot.Backpack] = null;

        DetectedFromDetector(cur);
         
        return cur;
    } 

    private void DetectedFromDetector(ItemBase _item)
    {
        if (_item is EquipBase equip)
        {
            Transform meshRoot = equip.GetMesh().transform;
            if (meshRoot)
            {
                // meshRoot 아래의 모든 렌더러 자동 제거
                Renderer[] renderers = meshRoot.GetComponentsInChildren<Renderer>(true);
                cachedDuckDetected.RemoveRenderers(renderers);
            }
        }
    } 
} 

