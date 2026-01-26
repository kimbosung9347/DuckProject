using UnityEngine;
public enum EDuckState
{
    Default,
    Sprint,
    Roll,
    Aiming,
    Reload,
    UseConsum, 
    Dead, 
      
    End,
} 
public class DuckState : MonoBehaviour
{
    private DuckAbility cachedAbility;
    private DuckAnimation cachedAnim;
    private DuckAttack cachedAttack;
    private LocoMoveInfo cachedLocoMoveInfo;
      
    private EDuckState state = EDuckState.End;
       
    protected virtual void Awake()
    {
        cachedAbility = GetComponent<DuckAbility>();
        cachedAnim = GetComponent<DuckAnimation>();
        cachedAttack = GetComponent<DuckAttack>(); 
    }   
    private void Start()
    {
    } 
    private void OnEnable()
    {
        ChangeState(EDuckState.Default); 
    }

    public void CacheLocoInfo(LocoInfo _loco)
    {
        cachedLocoMoveInfo = _loco.moveInfo;
    }
    public void ChangeState(EDuckState _state)
    {
        if (state == _state)
            return;

        switch (state)
        {
            case EDuckState.Default:    ClearDefault(); break;
            case EDuckState.Sprint:     ClearSprint(); break;
            case EDuckState.Roll:       ClearRoll(); break;
            case EDuckState.Aiming:     ClearAiming(); break;
            case EDuckState.Reload:     ClearReload(); break;
            case EDuckState.UseConsum:  ClearUseConsum(); break;
            case EDuckState.Dead:       ClearDead(); break;
        } 
          
        state = _state;
        switch (state)
        {
            case EDuckState.Default:    ChangeDefault(); break;
            case EDuckState.Sprint:     ChangeSprint(); break;
            case EDuckState.Roll:       ChangeRoll(); break;
            case EDuckState.Aiming:     ChangeAiming(); break;
            case EDuckState.Reload:     ChangeReload(); break;
            case EDuckState.UseConsum:  ChangeUseConsum(); break;
            case EDuckState.Dead:       ChangeDead(); break;
        }
    } 

    public bool CanMove()
    {  
        if (IsDead())
            return false;
         
        return true;
    } 
    public bool CanAttack()
    {
        if (IsDead() || IsSprint() || IsRoll())
            return false; 

        return true;
    }  
    public bool CanRoll()
    {
        if (IsDead() || IsRoll())
            return false;

        return true;
    }  
    public bool CanLook()
    {
        if (IsDead() || IsRoll())
            return false;
         
        return true;
    }
    public bool CanSprint()
    {
        if (IsDead() || IsRoll() || IsSprint())
            return false; 
         
        return true;
    }
    public bool CanAiming()
    {
        if (IsDead() || IsRoll() || IsAiming())
            return false;
         
        return true;
    }
    public bool CanHit()
    {
        if (IsDead() || IsRoll())
            return false;
         
        return true;
    }

    public bool CanBodyAnim()
    {
        if (IsDead() || IsRoll() || IsAiming())
            return false;

        return true;
    }
    public bool CanFootAnim()
    {
        if (IsDead() || IsRoll())
            return false;

        return true;
    } 
    public bool CanArmAnim()
    {
        if (IsDead() || IsRoll())
            return false;
         
        return true;
    } 

    public bool IsSprint() { return (state == EDuckState.Sprint); }
    public bool IsDead() { return (state == EDuckState.Dead); }
    public bool IsRoll() { return (state == EDuckState.Roll); }
    public bool IsAiming() { return (state == EDuckState.Aiming); }
    public bool IsReloading() { return (state == EDuckState.Reload); }
     
    protected virtual void ClearAiming()
    {
        cachedAbility.AddCorrLocoInfo(ELocoStatType.MoveSpeed, +cachedLocoMoveInfo.aimReduceSpeed);
    }
    protected virtual void ClearReload()
    {
        cachedAbility.AddCorrLocoInfo(ELocoStatType.MoveSpeed, +cachedLocoMoveInfo.aimReduceSpeed);
    }
    protected virtual void ClearUseConsum()
    {
        cachedAbility.AddCorrLocoInfo(ELocoStatType.MoveSpeed, +cachedLocoMoveInfo.aimReduceSpeed);
        cachedAnim.ChangeConsum(false);
    } 
    protected virtual void ClearSprint()
    {
        cachedAbility.AddCorrLocoInfo(ELocoStatType.MoveSpeed, -cachedLocoMoveInfo.sprintSpeed);
        cachedAbility.MultiplyBodyAnimSpeed(1f);
        cachedAbility.MultiplyFootAnimSpeed(1f);

        cachedAttack.ChangeSprint(false);
        cachedAnim.ChangeSprint(false);
    }
    protected virtual void ClearRoll()
    {
    }

    protected virtual void ChangeAiming()
    {
        cachedAbility.AddCorrLocoInfo(ELocoStatType.MoveSpeed, -cachedLocoMoveInfo.aimReduceSpeed);
    } 
    protected virtual void ChangeReload()
    {
        cachedAbility.AddCorrLocoInfo(ELocoStatType.MoveSpeed, -cachedLocoMoveInfo.aimReduceSpeed);
    }
    protected virtual void ChangeUseConsum()
    { 
        cachedAbility.AddCorrLocoInfo(ELocoStatType.MoveSpeed, -cachedLocoMoveInfo.aimReduceSpeed);
        cachedAnim.ChangeConsum(true);
    }
    protected virtual void ChangeSprint()
    {
        cachedAbility.AddCorrLocoInfo(ELocoStatType.MoveSpeed, cachedLocoMoveInfo.sprintSpeed);
        cachedAbility.MultiplyBodyAnimSpeed(1.5f);
        cachedAbility.MultiplyFootAnimSpeed(1.5f);
        cachedAttack.ChangeSprint(true);
        cachedAnim.ChangeSprint(true);
    } 
    protected virtual void ChangeRoll()
    {
          
    }

    protected virtual void ChangeDead()
    { 
         
    }

    protected virtual void ClearDead()
    {

    }
     


    private void ClearDefault() 
    { 
    }

    private void ChangeDefault()
    {  
        //cachedAbility.MultiplyBodyAnimSpeed(1f); 
        //cachedAbility.MultiplyFootAnimSpeed(1f);
        //cachedAbility.MultiplyArmAinimSpeed(1f); 
    }
}  
