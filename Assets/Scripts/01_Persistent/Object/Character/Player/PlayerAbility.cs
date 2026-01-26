using UnityEngine;

public class PlayerAbility : DuckAbility
{
    protected override void Cache()
    {
        base.Cache();
         
        // 스텟캐싱 
        GetComponent<PlayerStorage>()?.CacheCapacityInfo(curCapacityInfo);
          
        // UI 정보도 캐싱해주기
        var gameInstance = GameInstance.Instance;
        var uiGroup = gameInstance.UI_GetPersistentUIGroup();
        var statCanvas = uiGroup.GetMenuCanvas().GetStatCanvas();
        statCanvas.CachePlayerInfo(curLocoInfo, curShotInfo, curStatInfo, curArmorInfo, curCapacityInfo);
        uiGroup.GetCursorCanvas().GetPlayCursor().CacheShotInfo(curShotInfo);
    }  
    protected override void RenewAllAbilityInfo()
    { 
        // 플레이어는 저장된 
        var playData = GameInstance.Instance.SAVE_GetCurPlayData();
        defaultAnimInfo.CopyFrom(playData.statProgressData.anim);
        defaultStatInfo.CopyFrom(playData.statProgressData.stat);
        defaultLocoInfo.CopyFrom(playData.statProgressData.loco);
        defaultShotInfo.CopyFrom(playData.statProgressData.shot);
        defaultArmorInfo.CopyFrom(playData.statProgressData.armor);
        defaultCapacityInfo.CopyFrom(playData.statProgressData.capacity);
         
        curAnimInfo.CopyFrom(defaultAnimInfo);
        RenewAllLocoInfo();
        RenewAllShotInfo();
        RenewAllArmorInfo();
        RenewAllStatInfo();
        RenewCapacityInfo();
    } 
}  
  