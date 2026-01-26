using System.Collections.Generic;
using UnityEngine;

public class Food : UseConsumBase
{
    FoodData foodData; 

    public override void Init(ItemData _data, ItemVisualData _visual)
    {
        base.Init(_data, _visual);

        if (_data is FoodData food)
        {
            foodData = food;
        }
    }
   
    public bool SuccessUseFood()
    { 
        if (foodData.isCapacity)
        { 
            curCapacity -= foodData.amountPerUse;
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
    public EBuffID GetBuffID() { return foodData.buffID; }   

    protected override float GetRecoverTime()
    {
        return foodData.recoverTime;
    }
    protected override void OnUseCompleted() 
    {
        float addWater = foodData.addThirst;
        float addFull = foodData.addFull;

        // TODO NULL - iNSERT해줘야함  
        if (cachedDuckStorage)
        {
            cachedDuckStorage.SuccesUseFood(addFull, addWater);
            return;
        }
         
        // Item
        if (cachedItemBoxBase)
        {
            var target = cachedItemBoxBase.GetComponent<HandleItemBox>()?.GetCurInteractionTarget();
            if (target)
            {
                var storage = target.gameObject.GetComponent<DuckStorage>();
                storage.SuccesUseFood(addFull, addWater);
            }
            return; 
        }
    }
}
