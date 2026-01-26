using System.Collections.Generic;
using UnityEngine;

public enum EWeightState
{ 
    None,
    Heavy,
    OverWeight,
      
    End,
}

public class DuckStorage : MonoBehaviour
{
    protected DuckEquip cachedDuckEquip;
    protected DuckStat cachedDuckStat;
    protected DuckState cachedDuckState;
    protected DuckAppearance cachedDuckAppearance;
    protected DuckBuff cachedDuckBuff;

    protected List<ItemBase> listItems = new();
    protected int itemMaxCnt = 0;
      
    // 사용중인 소비 아이템 인덱스
    protected int curUseConsumIndex = -1;
    protected ItemBoxBase cachedItemBox;
     
    // 현재 무게 
    protected float curWeight;

    protected virtual void Awake()
    {
        cachedDuckEquip = GetComponent<DuckEquip>();
        cachedDuckStat = GetComponent<DuckStat>();
        cachedDuckState = GetComponent<DuckState>();
        cachedDuckBuff = GetComponent<DuckBuff>();
        cachedDuckAppearance = GetComponent<DuckAppearance>();
    }

    public virtual void RefillBullet(EItemID _bulletId, int _cnt)
    {
        // AI는 그냥 총알 바로 리필
        var weapon = cachedDuckEquip.GetWeapon();
        weapon.SetCurBulletId(_bulletId);
        weapon.SetCurBullet(_cnt);

        // 상태 원복  
        cachedDuckState.ChangeState(EDuckState.Default);
    }
    public virtual void RollbackBullet(EItemID _bulletId, int _cnt)
    {
        // AI는 안쓸듯
    }

    public bool CanCancleUse()
    {
        return (curUseConsumIndex != -1);
    }
    public bool CanUseConsum()
    {
        return (curUseConsumIndex == -1);
    }
    public ItemBase GetItem(int _index)
    {
        if (_index < 0 || _index >= listItems.Count)
            return null;

        ItemBase item = listItems[_index];
        return item;
    }
    public List<ItemBase> GetItemList() { return listItems; } 

    public int GetCurItemCnt()
    {
        return itemMaxCnt;
    }
    public void TryUseConsum(int _index)
    {
        var item = GetItem(_index);
        if (item && item is UseConsumBase consum)
        {
            curUseConsumIndex = _index;
            consum.Attach();
            cachedDuckAppearance.AttachConsum(consum);
            cachedDuckState.ChangeState(EDuckState.UseConsum);
        }
    }
    public void TryUseConsum(ItemBoxBase _itemBox, int _index)
    {
        // 아이템 박스 전용 
        cachedItemBox = _itemBox;

        var item = _itemBox.GetItem(_index);
        if (item && item is UseConsumBase consum)
        {
            curUseConsumIndex = _index;
            consum.Attach();
            cachedDuckAppearance.AttachConsum(consum);
            cachedDuckState.ChangeState(EDuckState.UseConsum);
        }
    }
    public void CancleUseConsum()
    {
        var item = cachedItemBox ? cachedItemBox.GetItem(curUseConsumIndex) : GetItem(curUseConsumIndex);
        if (item is UseConsumBase consum)
        {
            consum.Detach();
            curUseConsumIndex = -1;
            cachedDuckAppearance.DetachConsum();
              
            Transform targetTransform = cachedItemBox ? cachedItemBox.transform : transform;
            consum.Insert(targetTransform);
        }

        cachedItemBox = null;
    } 
     
    public void SuccessUseHealItem(float _add)
    {
        var item = cachedItemBox ? cachedItemBox.GetItem(curUseConsumIndex) : GetItem(curUseConsumIndex);
        if (item is HealItem heal)
        {
            float remainValue = cachedDuckStat.RecoverHp(_add);
            bool isRemain = heal.SuccessUseHeal(remainValue);
            AfterUseConsum(isRemain, heal);
        }
    }  
    public void SuccesUseFood(float _full, float _thrist)
    {
        var item = cachedItemBox ? cachedItemBox.GetItem(curUseConsumIndex) : GetItem(curUseConsumIndex);
        if (item is Food foodItem)
        {
            cachedDuckStat.RecoverFood(_full);
            cachedDuckStat.RecoverWater(_thrist);
            bool isRemain = foodItem.SuccessUseFood();
            AfterUseConsum(isRemain, foodItem);

            EBuffID buffId = foodItem.GetBuffID();
            if (buffId != EBuffID._END)
            {
                cachedDuckBuff.InsertBuff(buffId);
            } 
        }
    }

    protected virtual void AfterUseConsum(bool isRemain, ConsumBase consum)
    { 
        // 아이템 박스에서 사용 
        if (cachedItemBox)
        {
            int prevIndex = curUseConsumIndex;
            CancleUseConsum();
            cachedDuckState.ChangeState(EDuckState.Default);
            return;
        }
          
        // 인벤토리에서 사용
        {
            int prevIndex = curUseConsumIndex;
            CancleUseConsum();
            cachedDuckState.ChangeState(EDuckState.Default);
        }
    }
} 
