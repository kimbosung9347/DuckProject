using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealItem : UseConsumBase
{
    private HealData headData;

    public override void Init(ItemData _data, ItemVisualData _visual)
    {
        base.Init(_data, _visual);
        headData = _data as HealData;
    }
    public bool SuccessUseHeal(float _useValue)
    {
        if (headData.isCapacity)
        {
            curCapacity -= _useValue;
            if (curCapacity <= 0)
                return false;
        }

        else
        {
            curCnt--;
            if (curCnt <= 0)
                return false;
        }

        return true;
    } 
     
    protected override float GetRecoverTime()
    {
        return headData.recoverTime;
    }  
    protected override void OnUseCompleted() 
    {
        float add = headData.addHp;
         
        if (headData.isCapacity)
        {
            if (curCapacity < headData.addHp)
            {
                add = curCapacity;
            } 
        }  
        cachedDuckStorage.SuccessUseHealItem(add);
    }
} 
