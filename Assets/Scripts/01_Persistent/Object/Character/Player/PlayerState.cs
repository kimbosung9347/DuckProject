using UnityEngine;

public class PlayerState : DuckState
{
    private PlayerAiming cachedAiming; // todo 분리해줘야함
    private PlayerEquip cachedEquip;
    private PlayerStat cachedStat;
    private PlayerUIController cachedUIController;
    private PlayerController cachedPlayerController; 
    private DuckLocomotion cachedDuckLocomotion;
    private DuckDetected cachedDetected; 

    protected override void Awake()
    {
        base.Awake();
         
        cachedUIController = GetComponent<PlayerUIController>();
        cachedPlayerController = GetComponent<PlayerController>(); 
        cachedAiming = GetComponent<PlayerAiming>();
        cachedEquip = GetComponent<PlayerEquip>();
        cachedStat = GetComponent<PlayerStat>();
        cachedDuckLocomotion = GetComponent<DuckLocomotion>();
        cachedDetected = GetComponentInChildren<DuckDetected>();
    } 

    public bool CanRelad()
    { 
        if (IsDead() || IsReloading())
            return false;
         
        return true; 
    }

    public bool CanInteraction()
    {
        if (IsDead() || IsRoll())
            return false;

        return true;
    }
    public bool CanEnterMenu()
    {
        if (IsDead() || IsRoll())
            return false;

        return true;
    }

    protected override void ClearAiming()
    {
        base.ClearAiming();

        cachedAiming.ChangeDefault();
         
        GameInstance.Instance.CAMERA_SetTopViewCameraDistance(DuckDefine.DEFAULT_ZOOM_CAMERA_DISTANCE);
    } 
    protected override void ChangeAiming()
    { 
        base.ChangeAiming();  
        
        cachedAiming.ChangeAim();

        GameInstance.Instance.CAMERA_SetTopViewCameraDistance(DuckDefine.AIM_ZOOM_CAMERA_DISTANCE);
    }
    protected override void ClearReload()
    {
        base.ClearReload();
           
        // 장전 취소
        cachedEquip.CancleReload();
        cachedAiming.ChangeDefault();
        cachedUIController.HUD_DisableInteractionGauge();
    }
    protected override void ClearUseConsum()
    {
        base.ClearUseConsum();
          
        // 사용 끝 
        cachedUIController.HUD_DisableInteractionGauge();
    }
    protected override void ClearSprint()
    {
        base.ClearSprint();

        cachedStat.ActiveStaminaGuage(false);
    }
    protected override void ClearRoll()
    {
        base.ClearRoll();
         
        cachedStat.ActiveStaminaGuage(false);
    }

    protected override void ClearDead()
    {
        base.ClearDead();
          
        cachedDetected.ActiveDuckRenderer();
        cachedStat.InitHP();
        cachedPlayerController.ChangePlay(); 
    }
     
    protected override void ChangeReload()
    {
        base.ChangeReload();

        cachedAiming.ChangeReload();
        cachedUIController.HUD_ActiveInteractionGuage();
    }
    protected override void ChangeUseConsum()
    {
        base.ChangeUseConsum();

        cachedUIController.HUD_ActiveInteractionGuage();
    }
    protected override void ChangeSprint()
    {
        base.ChangeSprint();

        cachedStat.ActiveStaminaGuage(true);
    }
    protected override void ChangeRoll()
    {
        base.ChangeRoll();
         
        cachedStat.ActiveStaminaGuage(true);
    }
    protected override void ChangeDead()
    {
        base.ChangeDead();

        // 멈추기 
        cachedDuckLocomotion.DoMove(Vector3.zero);
        cachedDetected.DisableDuckRenderer();
    } 


    public bool CanEnterInven()
    {
        if (IsDead() || IsRoll())
            return false;

        return true;
    }
}
