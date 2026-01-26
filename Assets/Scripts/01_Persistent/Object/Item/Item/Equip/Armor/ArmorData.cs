using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
 
// 방어구
[CreateAssetMenu(fileName = "ArmorData", menuName = "Scriptable Objects/ArmorData")]
public class ArmorData : EquipData
{
    [Tooltip("방어구 타입")]
    public EArmorType armorType;
    
    [Tooltip("머리 피해 감소율")]
    [Range(0f, 1f)] public float damageHeadReduction;

    [Tooltip("바디 피해 감소율")]
    [Range(0f, 1f)] public float damageBodyReduction;
     
    [Tooltip("이동 속도 패널티")]
    public float moveSpeedPenalty;

    protected override void AddStats(List<FStatPair> _list)
    {
        base.AddStats(_list);

        _list.Add(new("머리 방어도", damageHeadReduction.ToString()));
        _list.Add(new("몸통 방어도", damageBodyReduction.ToString()));
        _list.Add(new("움직임 패널티", moveSpeedPenalty.ToString()));
    } 
} 
 
