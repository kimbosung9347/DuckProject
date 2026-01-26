using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

public class Backpack : EquipBase
{
    private BackpackData backpackData;

    public override void Init(ItemData _data, ItemVisualData _visual)
    {
        base.Init(_data, _visual);

        InitBackpackData(_data);
    }
    public override int GetPrice()
    {
        return backpackData.price;
    } 

    public BackpackData GetBackpackData() { return backpackData; }

    private void InitBackpackData(ItemData _data)
    {
        backpackData = _data as BackpackData;
    }  
}
