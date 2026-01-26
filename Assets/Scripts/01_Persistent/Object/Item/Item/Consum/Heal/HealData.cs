using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[CreateAssetMenu(fileName = "HealData", menuName = "Scriptable Objects/HealData")]
public class HealData : ConsumData  
{
    [Tooltip("추가로 차는 체력")]
    public float addHp;

    [Tooltip("회복까지 걸리는 시간")]
    public float recoverTime;
      
    protected override void AddStats(List<FStatPair> _list)
    {
        base.AddStats(_list);

        _list.Add(new("회복량", addHp.ToString()));
        _list.Add(new("회복 시간", recoverTime.ToString()));
    }
}
  