using System.Collections.Generic;
using UnityEngine;

public class Bullet : ConsumBase
{
    private BulletData bulletData;

    public override void Init(ItemData _data, ItemVisualData _visual)
    {
        base.Init(_data, _visual);

        bulletData = _data as BulletData;
    }
}
 