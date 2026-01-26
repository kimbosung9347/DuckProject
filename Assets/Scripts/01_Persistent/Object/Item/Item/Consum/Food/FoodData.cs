using System.Collections.Generic;
using UnityEngine;


// 체력과 물이 있는데 이거둘다 회복할수도 있고 

[CreateAssetMenu(fileName = "FoodData", menuName = "Scriptable Objects/FoodData")]
public class FoodData : ConsumData
{
    [Tooltip("포만감")] 
    public float addFull;

    [Tooltip("목마름")]
    public float addThirst;

    [Tooltip("1회사용량")]
    public float amountPerUse;

    [Tooltip("회복까지 걸리는 시간")]
    public float recoverTime;

    [Tooltip("버프 종류")]
    public EBuffID buffID = EBuffID._END;
      
    protected override void AddStats(List<FStatPair> _list)
    {
        base.AddStats(_list);

        _list.Add(new("포만감", addFull.ToString()));
        _list.Add(new("갈증", addThirst.ToString()));
        _list.Add(new("1회 사용량", amountPerUse.ToString()));
        _list.Add(new("소요 시간", recoverTime.ToString()));
    }
}
 