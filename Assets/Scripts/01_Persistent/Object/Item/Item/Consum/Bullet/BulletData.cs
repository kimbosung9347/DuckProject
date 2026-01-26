using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BulletData", menuName = "Scriptable Objects/BulletData")]
public class BulletData : ConsumData 
{
    [Tooltip("추가 대미지")]
    public float addDamage;
      
    protected override void AddStats(List<FStatPair> _list)
    {
        base.AddStats(_list);

        _list.Add(new("추가 대미지", addDamage.ToString()));
    }
}  
   