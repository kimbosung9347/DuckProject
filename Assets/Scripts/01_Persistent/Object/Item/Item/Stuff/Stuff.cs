using NUnit.Framework.Interfaces;
using UnityEngine;
 
// 재료임 - 추가로 할것들이 있을까?
public class Stuff : ItemBase
{
    private StuffData stuffData;

    public override void Init(ItemData _data, ItemVisualData _visual)
    {
        base.Init(_data, _visual);

        if (_data is StuffData _stuffData)
        {
            stuffData = _stuffData;
        } 
    }
}
