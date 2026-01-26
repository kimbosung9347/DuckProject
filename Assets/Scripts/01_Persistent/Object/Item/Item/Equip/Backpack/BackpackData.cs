using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BackpackData", menuName = "Scriptable Objects/BackpackData")]
public class BackpackData : EquipData
{ 
    [Tooltip("추가 무게")]
    public float addWeight;
     
    [Tooltip("추가 보관공간")]
    public int addStorgeSize;
     
    protected override void AddStats(List<FStatPair> _list)
    {
        base.AddStats(_list);

        _list.Add(new("추가 증량", addWeight.ToString()));
        _list.Add(new("추가 보관공간", addStorgeSize.ToString()));
    } 
} 
