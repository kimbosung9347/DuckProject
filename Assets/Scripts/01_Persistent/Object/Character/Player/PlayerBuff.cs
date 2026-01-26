using UnityEngine;

public class PlayerBuff : DuckBuff
{
    private PlayerUIController cachedUIController;
    private PlayerSight cachedPlayerSight;

    protected override void Awake()
    {
        base.Awake();

        cachedUIController = GetComponent<PlayerUIController>();
        cachedPlayerSight = GetComponent<PlayerSight>(); 
    }
 
    public override void RemoveBuff(EBuffID _buffId)
    {
        base.RemoveBuff(_buffId);
        cachedUIController.HUD_RemoveBuff(_buffId);

        // 하드코딩 하자 - 귀찮네
        if (EBuffID.VisionUp == _buffId)
        {
            cachedPlayerSight.AddBaseSight(-10, 0.05f, 0.1f);
        }
    }

    protected override void UpdateBuffDuration(EBuffID _id, float _prev, float _cur)
    {
        int prevSec = Mathf.CeilToInt(_prev);
        int curSec = Mathf.CeilToInt(_cur);
           
        if (prevSec != curSec)
        {
            cachedUIController.HUD_RenewBuffTime(_id, curSec);
        }
    }
    protected override void SuccesInsert(EBuffID _buffId)
    {
        base.SuccesInsert(_buffId); 

        BuffData buffData = cachedGameInstance.TABLE_GetBuffData(_buffId);
        if (DuckUtill.IsBuff(buffData.buffId))
        {
            cachedUIController.HUD_InsertBuff(buffData.buffId, buffData.buffSprite, buffData.buffName, Mathf.FloorToInt(buffData.duration));
        }
        else
        {
            cachedUIController.HUD_InsertDeBuff(buffData.buffId, buffData.buffSprite, buffData.buffName, Mathf.FloorToInt(buffData.duration));
        }             
          
        // 하드코딩 하자 - 귀찮네
        if (EBuffID.VisionUp == _buffId)
        {
            cachedPlayerSight.AddBaseSight(10, -0.05f, -0.1f);
        } 
    }  
}
  