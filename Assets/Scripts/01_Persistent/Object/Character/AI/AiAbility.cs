using UnityEngine;

public class AiAbility : DuckAbility
{
    protected override void Cache()
    {
        base.Cache();
         
        // Detector
        var aiDetector = GetComponent<AiDetector>();
        if (aiDetector)
        {
            aiDetector.CacheShotInfo(curShotInfo);
        } 
    } 
    protected override void RenewAllAbilityInfo()
    {
        // 캐릭터의 기본값
        var defaultInfo = GameInstance.Instance.TABLE_GetDuckDefaultInfo(duckType);
        defaultAnimInfo = defaultInfo.GetDefaultAnimInfo();
        defaultLocoInfo = defaultInfo.GetDefaultLocoInfo();
        defaultShotInfo = defaultInfo.GetDefaultShotInfo();
        defaultArmorInfo = defaultInfo.GetDefaultArmorInfo();
        defaultStatInfo = defaultInfo.GetDefaultStatInfo();
        defaultCapacityInfo = defaultInfo.GetDefaultCapacityInfo();

        // 체력정보 갱신  
        var playData = GameInstance.Instance.SAVE_GetCurPlayData();
        defaultStatInfo.maxMp *= DuckDefine.GetHpByDifficult(playData.difficultData.CurrentDifficult);
         
        // 전체 데이터 보정
        base.RenewAllAbilityInfo();
    }
}   
  