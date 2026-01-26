using UnityEngine;

public class AiAttack : DuckAttack
{
    public override bool CanAttack()
    {
        if (!weapon)
            return false;
          
        // 쿨타임 
        if (!weapon.IsExistBullet())
        { 
            // 재장전 시켜주기
            if (!weapon.IsReloading())
            {
                cachedSpeech.ActiveAutoDeleteSpeech("재장전");
                weapon.TryReload();
            }
            return false;
        }
         
        // 무기가 공격 가능한지
        if (!weapon.CanAttack())
            return false;
         
        return true;
    }
}
