using UnityEngine;
  
public class DuckAbility : MonoBehaviour 
{ 
    [SerializeField] protected EDuckType duckType; 
     
    // 합쳐진 - 이걸 기반으로 계산 되어야함
    protected AnimInfo curAnimInfo = new();
    protected LocoInfo curLocoInfo = new();
    protected ShotInfo curShotInfo = new();
    protected ArmorInfo curArmorInfo = new();
    protected StatInfo curStatInfo = new();
    protected CapacityInfo curCapacityInfo = new();
     
    // 보정
    protected LocoInfo        corrLocoInfo = new();
	protected ShotInfo        corrShotInfo = new();
	protected StatInfo        corrStatInfo = new();
    protected ArmorInfo       corrArmorInfo = new();
    protected CapacityInfo    corrCapacityInfo = new();
      
    // 기본값
    protected AnimInfo        defaultAnimInfo = new();           
	protected LocoInfo        defaultLocoInfo = new();
    protected ShotInfo        defaultShotInfo = new();
    protected ArmorInfo       defaultArmorInfo = new();
    protected StatInfo        defaultStatInfo = new();
    protected CapacityInfo    defaultCapacityInfo = new();
       
    private float corrRecoil = 0;
    private DuckStat cachedDuckStat;
     
    private void Awake()
    {
        Cache();
        RenewAllAbilityInfo();
    }
  
    public EDuckType GetDuckType() => duckType; 
    public void MultiplyBodyAnimSpeed(float _ratio)
    {
        if (curAnimInfo != null)
        {
            curAnimInfo.bodyAnimInfo.moveSpeed = defaultAnimInfo.bodyAnimInfo.moveSpeed * _ratio;
        }
    }
    public void MultiplyFootAnimSpeed(float _ratio)
    {
        if (curAnimInfo != null)
        {
            curAnimInfo.footAnimInfo.moveSpeed = defaultAnimInfo.footAnimInfo.moveSpeed * _ratio;
        } 
    }

    public void AddRecoilAcc(float _recoilAdd)
    {
        corrRecoil = _recoilAdd;
         
        RenewShotInfo(EShotStatType.AccControl);
    }
    public void AddCorrLocoAll(LocoMoveInfo move)
    {
        corrLocoInfo.moveInfo.moveSpeed += move.moveSpeed;
        corrLocoInfo.moveInfo.rotSpeed += move.rotSpeed;
        corrLocoInfo.moveInfo.sprintSpeed += move.sprintSpeed;
        corrLocoInfo.moveInfo.sprintRotSpeed += move.sprintRotSpeed;
        corrLocoInfo.moveInfo.aimReduceSpeed += move.aimReduceSpeed;

        // 전체 갱신 1회
        RenewAllLocoInfo();
    }
    public void AddCorrLocoInfo(ELocoStatType _type, float _modifier)
	{
		corrLocoInfo.ApplyModifier(_type, _modifier);
		RenewLocoInfo(_type); 
	}
    public void AddCorrShotAll(ShotInfo shot)
    {
        corrShotInfo.accControl += shot.accControl;
        corrShotInfo.recoilControl += shot.recoilControl;
        corrShotInfo.recoilRecovery += shot.recoilRecovery;
        corrShotInfo.attackRange += shot.attackRange;
        corrShotInfo.toAimSpeed += shot.toAimSpeed;

        // 전체 갱신 1회
        RenewAllShotInfo();
    }
    public void AddCorrShotInfo(EShotStatType _type, float _modifier)
    {
		corrShotInfo.ApplyModifier(_type, _modifier);
        RenewShotInfo(_type);
    }
    public void AddCorrStat(EStatType type, float modifier)
    {
        switch (type)
        {
            case EStatType.MaxHp:
                corrStatInfo.maxHp += modifier;
                break;

            case EStatType.MaxMp:
                corrStatInfo.maxMp += modifier;
                break;

            case EStatType.HpRecovery:
                corrStatInfo.hpRecovery += modifier;
                break;

            case EStatType.MpRecovery:
                corrStatInfo.mpRecovery += modifier;
                break;
        }

        RenewStatInfo(type);
    }
    public void AddCorrCapacityInfo(int _capacitySize, float _weight)
    {
        corrCapacityInfo.capacitySize += _capacitySize;
        corrCapacityInfo.maxWeight += _weight;
        RenewCapacityInfo();
    }
    public void AddCorrArmor(EArmorPart part, float value)
    {
        switch (part)
        {
            case EArmorPart.Head:
                corrArmorInfo.headDefense += value;
                break;

            case EArmorPart.Body:
                corrArmorInfo.bodyDefense += value;
                break;
        }

        RenewArmorInfo(part);
    }
      

    protected virtual void Cache()
    {
        cachedDuckStat = GetComponent<DuckStat>();

        // 다른 컴포넌트에게 관련정보 전달 
        GetComponent<DuckLocomotion>()?.CacheLocoInfo(curLocoInfo);
        GetComponent<DuckState>()?.CacheLocoInfo(curLocoInfo);
        GetComponent<DuckAnimation>()?.CacheAnimInfo(curAnimInfo);
        GetComponent<DuckStat>()?.CacheStatInfo(curStatInfo);
        GetComponent<DuckAttack>()?.CacheShotInfo(curShotInfo);
        GetComponent<DuckAiming>()?.CacheShotInfo(curShotInfo);
        // 자기 자식에 있는 모든 DuckHit
        DuckHit[] duckHits = GetComponentsInChildren<DuckHit>(true);
        foreach (DuckHit hit in duckHits)
        {
            hit.CacheArmorInfo(curArmorInfo);
        }
    }
    protected virtual void RenewAllAbilityInfo()
    {
        // 실제 전체 데이터 업데이트 
        curAnimInfo.CopyFrom(defaultAnimInfo);
        RenewAllLocoInfo(); 
        RenewAllShotInfo();
        RenewAllArmorInfo();
        RenewAllStatInfo();
        RenewCapacityInfo();
    }

    protected void RenewLocoInfo(ELocoStatType type)
    {
        switch (type)
        {
            case ELocoStatType.MoveSpeed:
                curLocoInfo.moveInfo.moveSpeed = defaultLocoInfo.moveInfo.moveSpeed + corrLocoInfo.moveInfo.moveSpeed;
                break;

            case ELocoStatType.RotSpeed:
                curLocoInfo.moveInfo.rotSpeed = defaultLocoInfo.moveInfo.rotSpeed + corrLocoInfo.moveInfo.rotSpeed;
                break;

            case ELocoStatType.SprintSpeed:
                curLocoInfo.moveInfo.sprintSpeed = defaultLocoInfo.moveInfo.sprintSpeed + corrLocoInfo.moveInfo.sprintSpeed;
                break;

            case ELocoStatType.SprintRotSpeed:
                curLocoInfo.moveInfo.sprintRotSpeed = defaultLocoInfo.moveInfo.sprintRotSpeed + corrLocoInfo.moveInfo.sprintRotSpeed;
                break;

            case ELocoStatType.AimReduceSpeed:
                curLocoInfo.moveInfo.aimReduceSpeed = defaultLocoInfo.moveInfo.aimReduceSpeed + corrLocoInfo.moveInfo.aimReduceSpeed;
                break;

            case ELocoStatType.RollSpeed:
                curLocoInfo.rollInfo.rollSpeed = defaultLocoInfo.rollInfo.rollSpeed + corrLocoInfo.rollInfo.rollSpeed;
                break;

            case ELocoStatType.RollRotSpeed:
                curLocoInfo.rollInfo.rollRotSpeed = defaultLocoInfo.rollInfo.rollRotSpeed + corrLocoInfo.rollInfo.rollRotSpeed;
                break;

            default:
                Debug.LogWarning($"[RenewLocoInfo] Unknown type: {type}");
                break;
        }
    }
    protected void RenewAllLocoInfo()
    {
        curLocoInfo.moveInfo.moveSpeed = defaultLocoInfo.moveInfo.moveSpeed + corrLocoInfo.moveInfo.moveSpeed;
        curLocoInfo.moveInfo.rotSpeed = defaultLocoInfo.moveInfo.rotSpeed + corrLocoInfo.moveInfo.rotSpeed;
        curLocoInfo.moveInfo.sprintSpeed = defaultLocoInfo.moveInfo.sprintSpeed + corrLocoInfo.moveInfo.sprintSpeed;
        curLocoInfo.moveInfo.sprintRotSpeed = defaultLocoInfo.moveInfo.sprintRotSpeed + corrLocoInfo.moveInfo.sprintRotSpeed;
        curLocoInfo.moveInfo.aimReduceSpeed = defaultLocoInfo.moveInfo.aimReduceSpeed + corrLocoInfo.moveInfo.aimReduceSpeed;
        curLocoInfo.rollInfo.rollSpeed = defaultLocoInfo.rollInfo.rollSpeed + corrLocoInfo.rollInfo.rollSpeed;
        curLocoInfo.rollInfo.rollRotSpeed = defaultLocoInfo.rollInfo.rollRotSpeed + corrLocoInfo.rollInfo.rollRotSpeed;
    }
    protected void RenewShotInfo(EShotStatType type)
    {
        switch (type)
        {
            case EShotStatType.AccControl:
                curShotInfo.accControl = Mathf.Clamp(defaultShotInfo.accControl + corrShotInfo.accControl - corrRecoil, 0f, 100f);
                break;

            case EShotStatType.RecoilControl:
                curShotInfo.recoilControl = Mathf.Clamp(defaultShotInfo.recoilControl + corrShotInfo.recoilControl, 0f, 100f);
                break;

            case EShotStatType.RecoilRecovery:
                curShotInfo.recoilRecovery = Mathf.Clamp(defaultShotInfo.recoilRecovery + corrShotInfo.recoilRecovery, 0f, 100f);
                break;

            case EShotStatType.AttackRange:
                curShotInfo.attackRange = Mathf.Clamp(defaultShotInfo.attackRange + corrShotInfo.attackRange, 0f, 100f);
                break;

            case EShotStatType.ToAimSpeed:
                curShotInfo.toAimSpeed = Mathf.Clamp(defaultShotInfo.toAimSpeed + corrShotInfo.toAimSpeed, 0f, 1f);
                break;

            default:
                Debug.LogWarning($"[RenewShotInfo] Unknown type: {type}");
                break;
        }
    }
    protected void RenewAllShotInfo() 
    {
        curShotInfo.accControl = Mathf.Clamp(defaultShotInfo.accControl + corrShotInfo.accControl - corrRecoil, 0f, 100f);
        curShotInfo.recoilControl = Mathf.Clamp(defaultShotInfo.recoilControl + corrShotInfo.recoilControl, 0f, 100f);
        curShotInfo.recoilRecovery = Mathf.Clamp(defaultShotInfo.recoilRecovery + corrShotInfo.recoilRecovery, 0f, 100f);
        curShotInfo.attackRange = Mathf.Clamp(defaultShotInfo.attackRange + corrShotInfo.attackRange, 0f, 100f);
        curShotInfo.toAimSpeed = Mathf.Clamp(defaultShotInfo.toAimSpeed + corrShotInfo.toAimSpeed, 0f, 1f);
    }
    protected void RenewStatInfo(EStatType type)
    {
        switch (type)
        {
            case EStatType.MaxHp:
                curStatInfo.maxHp = defaultStatInfo.maxHp + corrStatInfo.maxHp;
                break;

            case EStatType.MaxMp:
                curStatInfo.maxMp = defaultStatInfo.maxMp + corrStatInfo.maxMp;
                break;

            case EStatType.HpRecovery:
                curStatInfo.hpRecovery = defaultStatInfo.hpRecovery + corrStatInfo.hpRecovery;
                break;

            case EStatType.MpRecovery:
                curStatInfo.mpRecovery = defaultStatInfo.mpRecovery + corrStatInfo.mpRecovery;
                break;
        } 
          
        cachedDuckStat.HandleStatChanged(type, curStatInfo);
    }
    protected void RenewAllStatInfo()
    {
        curStatInfo.maxHp = defaultStatInfo.maxHp + corrStatInfo.maxHp;
        curStatInfo.maxMp = defaultStatInfo.maxMp + corrStatInfo.maxMp;

        curStatInfo.hpRecovery = defaultStatInfo.hpRecovery + corrStatInfo.hpRecovery;
        curStatInfo.mpRecovery = defaultStatInfo.mpRecovery + corrStatInfo.mpRecovery;
    }
    protected void RenewArmorInfo(EArmorPart part)
    {
        switch (part)
        {
            case EArmorPart.Head:
                curArmorInfo.headDefense =
                    defaultArmorInfo.headDefense + corrArmorInfo.headDefense;
                break;

            case EArmorPart.Body:
                curArmorInfo.bodyDefense =
                    defaultArmorInfo.bodyDefense + corrArmorInfo.bodyDefense;
                break;
        }
    }
    protected void RenewAllArmorInfo()
    {
        curArmorInfo.headDefense =
            defaultArmorInfo.headDefense + corrArmorInfo.headDefense;

        curArmorInfo.bodyDefense =
            defaultArmorInfo.bodyDefense + corrArmorInfo.bodyDefense;
    }
    protected void RenewCapacityInfo()
    {
        curCapacityInfo.capacitySize = defaultCapacityInfo.capacitySize + corrCapacityInfo.capacitySize;
        curCapacityInfo.maxWeight = defaultCapacityInfo.maxWeight + corrCapacityInfo.maxWeight;
    } 
      
}     

 