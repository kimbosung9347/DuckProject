using System.Collections.Generic;
using Unity.AppUI.UI;
using UnityEngine;
 
public class Armor : EquipBase
{
    private ArmorData armorData;

    public override void Init(ItemData _data, ItemVisualData _visual)
    {
        base.Init(_data, _visual);

        InitArmorData(_data);
    }
      

    public ArmorData GetArmorData() { return armorData; } 

    public float GetHeadDefanceValue()
    {
        return CalculateReduceValue(armorData.damageHeadReduction);
    } 
    public float GetBodyDefanceValue()
    {
        return CalculateReduceValue(armorData.damageBodyReduction);
    } 

    private void InitArmorData(ItemData _data)
    {
        armorData = _data as ArmorData;
    }

    // 공통 방어 계산
    private float CalculateReduceValue(float baseReduce)
    {
        float durabilityRatio = GetCurDurabilityRatio(); // 0~1
        float durabilityMul = GetDurabilityMultiplier(durabilityRatio);

        return baseReduce * durabilityMul;
    }

    // 내구도 단계 보정
    private float GetDurabilityMultiplier(float ratio)
    {
        if (ratio >= 0.75f) return 1.0f;
        if (ratio >= 0.5f)  return 0.75f;
        if (ratio >= 0.25f) return 0.5f;
        return 0.25f;
    } 
} 
