using UnityEngine;

public class DuckAiming : MonoBehaviour 
{
    protected DuckAbility cachedAbility;
    protected ShotInfo cachedShotInfo;

    protected Vector3 lookDir;
    protected Vector3 targetPoint;
     
    /*반동 관련*/
    protected bool isRecoiling = true;
    protected float recoilSize = 0f;

    /* 조준관련 */ 
    private bool isAiming = false; 
    private bool isComplateAim = false;
    private float prevAimProgress = 0f;
    private float increaseProgress = 0f; 
    private float aimTime = 0f;

    protected virtual void Awake() 
    {
        cachedAbility = GetComponent<DuckAbility>();
    } 
    protected virtual void Update()
    {
        UpdateRecoil();
        UpdateAiming();
    } 

    public virtual Vector3 GetLookDir()
    {   
        return lookDir;
    } 
    public virtual Vector3 GetTargetPos()
    {
        return targetPoint; 
    } 
    public virtual void DoRecoil(float _add)
    {  
        isRecoiling = true;
        recoilSize += _add;
        recoilSize = Mathf.Clamp(recoilSize, 0f, DuckDefine.MAX_RECOIL_SIZE);
        cachedAbility.AddRecoilAcc(recoilSize);   // 현재 누적치를 Ability에 전달
    } 

    public virtual void ChangeAim()
    {
        isAiming = true;
        isComplateAim = false;
        aimTime = 0f; 
        prevAimProgress = 0f;
        increaseProgress = 0f;
    } 
    public virtual void ChangeDefault()
    {
        ClearAim();
    } 
    public virtual void ChangeReload()
    {
        ClearAim();
    } 
    
    public void CacheShotInfo(ShotInfo _info)
    {
        cachedShotInfo = _info;
    }
    public float GetAttackRange()
    {
        return cachedShotInfo.attackRange; 
    }


    protected void UpdateRecoil()
    {
        if (!isRecoiling)
            return;
         
        recoilSize -= (2 * cachedShotInfo.recoilRecovery) * Time.deltaTime;
        recoilSize = Mathf.Clamp(recoilSize, 0f, DuckDefine.MAX_RECOIL_SIZE);
        cachedAbility.AddRecoilAcc(recoilSize);   // 회복된 누적치를 Ability에 다시 전달
         
        if (recoilSize <= 0f)
        {
            isRecoiling = false;
            recoilSize = 0f;
            cachedAbility.AddRecoilAcc(0f);       // 완전 초기화 전달
        }
    }
    protected void UpdateAiming()
    { 
        if (!isAiming)
            return;
        
        if (isComplateAim)
            return;
         
        aimTime += Time.deltaTime;  
              
        float aimDuration = Mathf.Lerp(3f, 0.5f, cachedShotInfo.toAimSpeed);
        float aimProgress = Mathf.Clamp01(aimTime / aimDuration);
         
        // 현재 프레임의 delta 진행률
        float deltaProgress = aimProgress - prevAimProgress;
        prevAimProgress = aimProgress;
         
        // 명중 보정 최대치 80
        float accMax = 80f;  
        // 이번 프레임에 추가해야 할 보정량
        float accDelta = accMax * deltaProgress;
        
        // 누적 보정
        cachedAbility.AddCorrShotInfo(EShotStatType.AccControl, accDelta);
        increaseProgress += accDelta;
         
        // 완료 체크
        if (aimProgress >= 1f)
            isComplateAim = true;
    }

    private void ClearAim()
    {
        if (isAiming)
        {
            if (increaseProgress > 0f)
            {
                cachedAbility.AddCorrShotInfo(EShotStatType.AccControl, -increaseProgress);
            }
             
            isAiming = false;
            isComplateAim = true;
            aimTime = 0f;
            prevAimProgress = 0f;
            increaseProgress = 0f;
        }
    }
} 
