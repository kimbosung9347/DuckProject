using NUnit.Framework.Interfaces;
using UnityEngine;

public class ConsumBase : ItemBase
{
    private ConsumData consumeData;
    protected int curCnt = 0;
    protected float curCapacity = 0;

    public override void Init(ItemData _data, ItemVisualData _visual)
    {
        base.Init(_data, _visual);

        consumeData = _data as ConsumData;

        curCapacity = consumeData.maxCapacity;
        curCnt = consumeData.maxStoreCnt;
           
        // 아이템이 아닌 용량으로 사용됨 
    }
    public override void Attach()
    {
        base.Attach();
         
        // 빌보드 켜주기
        billboardObject.gameObject.SetActive(true);
    }
    public override void Detach()
    {
        base.Detach();

        // 빌보드꺼주기
        billboardObject.SetActive(false);
    }
    public override float GetItemWeight()
    {
        if (IsCapacity())
        {
            return consumeData.weight;
        }
         
        return consumeData.weight * curCnt;
    }
    public override int GetPrice()
    { 
        if (consumeData.isCapacity)
        {
            float multiplier = Mathf.Lerp(0.2f, 1f, GetCurDurabilityRatio());
            return Mathf.RoundToInt(base.GetPrice() * multiplier);
        } 
         
        return base.GetPrice() * curCnt;
    }
    public override FItemShell GetItemShell()
    {
        FItemShell entry = base.GetItemShell(); 
        if (consumeData.isCapacity)
        {  
            entry.durabilityRatio = GetCurDurabilityRatio();
            entry.cnt = 1; 
        }
        else 
        {
            entry.cnt = curCnt;
        }
        return entry;
    }
    public override bool IsMerge()
    {
        // 개수 기반 아이템인지
        return !consumeData.isCapacity;
    }
     
    public virtual float GetUseRatio()
    {
        return 0;
    }
    public virtual void SetCurCnt(int _cnt)
    {
        curCnt = _cnt;
    }
    public virtual void SetCapacityRatio(float _capacityRatio)
    {
        curCapacity = consumeData.maxCapacity * _capacityRatio;
    }

    public bool isFull()
    {
        if (IsCapacity())
        {
            if (Mathf.Abs(curCapacity - consumeData.maxCapacity) < 0.0001f)
                return true;
        }
        else
        { 
            if (curCnt == consumeData.maxStoreCnt)
                return true;
        }

        return false;
    }
    public bool IsRemain()
    {
        if (IsCapacity())
        {
            if (curCapacity <= 0)
                return false;

        }
        else
        {
            if (curCnt <= 0)
                return false;
        }
         
        return true; 
    } 
    public bool IsCapacity() { return consumeData.isCapacity; } 
    public int GetCurCnt() { return curCnt; }
    public int PushAndCheckRemain(int addCount)
    {
        int max = consumeData.maxStoreCnt;
        if (curCapacity == max)
            return -1;

        // 현재 + 추가 = 총합
        int newTotal = curCnt + addCount;
         
        // 여유 공간
        int space = max - curCnt;

        // 완전히 들어가는 경우
        if (newTotal <= max)
        {
            curCnt = newTotal;
            SetCurCnt(newTotal);
            return 0;
        } 

        // 일부만 들어가는 경우
        // 넘친 수량 반환
        SetCurCnt(max); 
        return newTotal - max;
    }
    public float GetCurDurabilityRatio()
    {
        return curCapacity / consumeData.maxCapacity; 
    }
}
 