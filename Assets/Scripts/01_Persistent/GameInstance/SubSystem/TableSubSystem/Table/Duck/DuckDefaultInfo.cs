using System.Dynamic;
using UnityEngine;

/* =========================================
    DuckDefaultInfo 
    캐릭터 능력치 기본 데이터 관리
    캐릭터의 타입에 맞게 데이터를 참조해줄 예정
 * ========================================= */

[CreateAssetMenu(fileName = "DuckDefaultInfo", menuName = "Scriptable Objects/DuckDefaultInfo")]
public class DuckDefaultInfo : ScriptableObject
{
    public EDuckType duckType; 

    [Header("기본 캐릭터 데이터")]
    public LocoInfo locoInfo;
    public AnimInfo animInfo;
    public ShotInfo shotInfo; 
    public ArmorInfo armorInfo;
    public StatInfo statInfo;
    public CapacityInfo capacityInfo;

    public LocoInfo GetDefaultLocoInfo() => locoInfo.Clone();
    public LocoMoveInfo GetLocoMoveInfo() => locoInfo.moveInfo;
    public LocoRollInfo GetLocoRollInfo() => locoInfo.rollInfo;
     
    public AnimInfo GetDefaultAnimInfo() => animInfo.Clone();
    public AnimBodyInfo GetAnimBodyInfo() => animInfo.bodyAnimInfo;
    public AnimFootInfo GetAnimFootInfo() => animInfo.footAnimInfo;
    public AnimRollInfo GetAnimRollInfo() => animInfo.rollAnimInfo;

    public ShotInfo GetDefaultShotInfo() => shotInfo.Clone();
    public StatInfo GetDefaultStatInfo() => statInfo.Clone();
    public ArmorInfo GetDefaultArmorInfo() => armorInfo.Clone();
    public CapacityInfo GetDefaultCapacityInfo() => capacityInfo.Clone();
} 

/* =============
    AnimInfo
 ============= */

[System.Serializable]
public class AnimInfo
{
    [SerializeField] public AnimBodyInfo bodyAnimInfo;
    [SerializeField] public AnimFootInfo footAnimInfo;
    [SerializeField] public AnimRollInfo rollAnimInfo;
    [SerializeField] public AnimArmInfo  armAnimInfo;

    public AnimInfo() 
    { 
        bodyAnimInfo = new AnimBodyInfo();
        footAnimInfo = new AnimFootInfo();
        rollAnimInfo = new AnimRollInfo();
        armAnimInfo = new AnimArmInfo();
    } 
      
    public AnimInfo Clone()
    {
        return new AnimInfo
        {
            bodyAnimInfo = bodyAnimInfo?.Clone(),
            footAnimInfo = footAnimInfo?.Clone(),
            rollAnimInfo = rollAnimInfo?.Clone(),
            armAnimInfo = armAnimInfo?.Clone(),
        }; 
    }
    public void CopyFrom(AnimInfo other)
    {
        if (other == null)
            return;

        bodyAnimInfo ??= new AnimBodyInfo();
        footAnimInfo ??= new AnimFootInfo();
        rollAnimInfo ??= new AnimRollInfo();
        armAnimInfo ??= new AnimArmInfo();

        bodyAnimInfo.CopyFrom(other.bodyAnimInfo);
        footAnimInfo.CopyFrom(other.footAnimInfo);
        rollAnimInfo.CopyFrom(other.rollAnimInfo);
        armAnimInfo.CopyFrom(other.armAnimInfo);
    }
}

[System.Serializable]
public class AnimBodyInfo
{
    public float moveSpeed = 0f;
    public float moveAngle = 0f;
    public float stopTime = 0f;
    public float dampingSpeed = 0f;

    public AnimBodyInfo Clone()
    {
        return new AnimBodyInfo
        {
            moveSpeed = moveSpeed,
            moveAngle = moveAngle,
            stopTime = stopTime,
            dampingSpeed = dampingSpeed
        };
    }
    public void CopyFrom(AnimBodyInfo other)
    {
        if (other == null)
            return;

        moveSpeed = other.moveSpeed;
        moveAngle = other.moveAngle;
        stopTime = other.stopTime;
        dampingSpeed = other.dampingSpeed;
    } 
}
 
[System.Serializable]
public class AnimFootInfo
{
    public float moveSpeed = 0f;
    public float moveAmplitude = 0f;
    public float stopTime = 0f;
    public float dampingSpeed = 0f;
    public float phaseOffset = 0f;

    public AnimFootInfo Clone()
    {
        return new AnimFootInfo
        {
            moveSpeed = moveSpeed,
            moveAmplitude = moveAmplitude,
            stopTime = stopTime,
            dampingSpeed = dampingSpeed,
            phaseOffset = phaseOffset
        };
    }
    public void CopyFrom(AnimFootInfo other)
    {
        if (other == null)
            return;

        moveSpeed = other.moveSpeed;
        moveAmplitude = other.moveAmplitude;
        stopTime = other.stopTime;
        dampingSpeed = other.dampingSpeed;
        phaseOffset = other.phaseOffset;
    }
}

[System.Serializable]
public class AnimRollInfo
{
    public float rollDuration = 0f;
    public float yOffset = 0f;
     
    public AnimRollInfo Clone()
    {
        return new AnimRollInfo
        {
            rollDuration = rollDuration,
            yOffset = yOffset
        };
    }
    public void CopyFrom(AnimRollInfo other)
    {
        if (other == null)
            return;

        rollDuration = other.rollDuration;
        yOffset = other.yOffset;
    } 
}

[System.Serializable]   
public class AnimArmInfo
{ 
    // ===== 기본 각도 =====
    public float baseAngleBareHand = 0f;    
    public float baseAngleWeapon = 0f;     
    public float baseSprintAngleWeapon = 0f;
    public AnimArmInfo Clone()
    {
        return (AnimArmInfo)this.MemberwiseClone();
    }
    public void CopyFrom(AnimArmInfo other)
    {
        if (other == null)
            return;

        baseAngleBareHand = other.baseAngleBareHand;
        baseAngleWeapon = other.baseAngleWeapon;
        baseSprintAngleWeapon = other.baseSprintAngleWeapon;
    }
}
 
/* =============
    LocoInfo
 ============= */
public enum ELocoStatType
{
    // 이동 관련
    MoveSpeed,
    RotSpeed,
    SprintSpeed,
    SprintRotSpeed,
    AimReduceSpeed,

    // 구르기 관련
    RollSpeed,
    RollRotSpeed,
     
    End,
} 

[System.Serializable]
public class LocoInfo
{
    [SerializeField] public LocoMoveInfo moveInfo;
    [SerializeField] public LocoRollInfo rollInfo;

    public LocoInfo()
    {
        moveInfo = new LocoMoveInfo();
        rollInfo = new LocoRollInfo();
    } 
     
    public LocoInfo Clone()
    {
        return new LocoInfo
        {
            moveInfo = moveInfo?.Clone(),
            rollInfo = rollInfo?.Clone()
        };
    }
    public void CopyFrom(LocoInfo other)
    {
        if (other == null)
            return;

        moveInfo ??= new LocoMoveInfo();
        rollInfo ??= new LocoRollInfo();

        moveInfo.CopyFrom(other.moveInfo);
        rollInfo.CopyFrom(other.rollInfo);
    } 
     
    /* =========================================
     * ApplyModifier
     * ========================================= */
    public void ApplyModifier(ELocoStatType type, float value)
    {
        switch (type) 
        {
            case ELocoStatType.MoveSpeed:
                moveInfo.moveSpeed += value;
                break;  

            case ELocoStatType.RotSpeed:
                moveInfo.rotSpeed += value;
                break;

            case ELocoStatType.SprintSpeed:
                moveInfo.sprintSpeed += value;
                break;

            case ELocoStatType.SprintRotSpeed:
                moveInfo.sprintRotSpeed += value;
                break;

            case ELocoStatType.AimReduceSpeed:
                moveInfo.aimReduceSpeed += value;
                break;

            case ELocoStatType.RollSpeed:
                rollInfo.rollSpeed += value;
                break;

            case ELocoStatType.RollRotSpeed:
                rollInfo.rollRotSpeed += value;
                break;

            default:
                Debug.LogWarning($"[LocoInfo] Unknown LocoStatType: {type}");
                break;
        }
    }
}

[System.Serializable]
public class LocoMoveInfo
{
    [Tooltip("이동 속도 (m/s)")]
    public float moveSpeed = 0f;

    [Tooltip("회전 속도 (deg/s)")]
    public float rotSpeed = 0f;

    [Tooltip("달리기 속도 (m/s)")]
    public float sprintSpeed = 0f;

    [Tooltip("달리기 시 회전 속도 (deg/s)")]
    public float sprintRotSpeed = 0f;

    [Tooltip("조준 중 이동속도 감소 비율 (0~1)")]
    public float aimReduceSpeed = 0f;

    public LocoMoveInfo Clone()
    {
        return new LocoMoveInfo 
        {
            moveSpeed = moveSpeed,
            rotSpeed = rotSpeed,
            sprintSpeed = sprintSpeed,
            sprintRotSpeed = sprintRotSpeed,
            aimReduceSpeed = aimReduceSpeed
        };
    }
    public void CopyFrom(LocoMoveInfo other)
    {
        if (other == null)
            return;

        moveSpeed = other.moveSpeed;
        rotSpeed = other.rotSpeed;
        sprintSpeed = other.sprintSpeed;
        sprintRotSpeed = other.sprintRotSpeed;
        aimReduceSpeed = other.aimReduceSpeed;
    } 

    public static LocoMoveInfo operator +(LocoMoveInfo a, LocoMoveInfo b)
    {
        return new LocoMoveInfo
        {
            moveSpeed = a.moveSpeed + b.moveSpeed,
            rotSpeed = a.rotSpeed + b.rotSpeed,
            sprintSpeed = a.sprintSpeed + b.sprintSpeed,
            sprintRotSpeed = a.sprintRotSpeed + b.sprintRotSpeed,
            aimReduceSpeed = a.aimReduceSpeed + b.aimReduceSpeed
        };
    }
    public static LocoMoveInfo operator -(LocoMoveInfo a, LocoMoveInfo b)
    {
        return new LocoMoveInfo
        {
            moveSpeed = a.moveSpeed - b.moveSpeed,
            rotSpeed = a.rotSpeed - b.rotSpeed,
            sprintSpeed = a.sprintSpeed - b.sprintSpeed,
            sprintRotSpeed = a.sprintRotSpeed - b.sprintRotSpeed,
            aimReduceSpeed = a.aimReduceSpeed - b.aimReduceSpeed
        }; 
    }
    public static LocoMoveInfo operator -(LocoMoveInfo a)
    {
        return new LocoMoveInfo
        {
            moveSpeed = -a.moveSpeed,
            rotSpeed = -a.rotSpeed,
            sprintSpeed = -a.sprintSpeed,
            sprintRotSpeed = -a.sprintRotSpeed,
            aimReduceSpeed = -a.aimReduceSpeed
        };
    }
}

[System.Serializable]
public class LocoRollInfo
{
    [Tooltip("구르기 속도 (m/s)")]
    public float rollSpeed = 0f;

    [Tooltip("구르기 회전 속도 (deg/s)")]
    public float rollRotSpeed = 0f;
     
    public LocoRollInfo Clone()
    {
        return new LocoRollInfo
        {
            rollSpeed = rollSpeed,
            rollRotSpeed = rollRotSpeed
        };
    }
    public void CopyFrom(LocoRollInfo other)
    {
        if (other == null)
            return;

        rollSpeed = other.rollSpeed;
        rollRotSpeed = other.rollRotSpeed;
    } 
}

/* =============
   ShotInfo
============= */ 
public enum EShotStatType
{ 
    // 조준 및 반동 제어
    AccControl,        // 명중률 제어
    RecoilControl,     // 반동 제어력
    RecoilRecovery,    // 반동 회복 속도

    // 사거리 및 조준 관련
    AttackRange,       // 공격 사거리
    ToAimSpeed,        // 조준 전환 속도

    End,
}

[System.Serializable]
public class ShotInfo
{
    [Tooltip("명중률 제어 (낮을수록 퍼짐이 큼)")]
    public float accControl = 0f;

    [Tooltip("반동 제어력 (높을수록 반동 억제)")]
    public float recoilControl = 0f;

    [Tooltip("반동 회복 속도 (높을수록 빨리 복구)")]
    public float recoilRecovery = 0f;

    [Tooltip("공격 사거리 (m)")]
    public float attackRange = 0f; 
      
    [Tooltip("조준 전환 속도 0 ~ 1")]
    public float toAimSpeed = 0f;
     
    public ShotInfo Clone()
    {
        return new ShotInfo
        {
            accControl = accControl,
            recoilControl = recoilControl,
            recoilRecovery = recoilRecovery,
            attackRange = attackRange,
            toAimSpeed = toAimSpeed
        };
    }
    public void CopyFrom(ShotInfo other)
    {
        if (other == null)
            return;

        accControl = other.accControl;
        recoilControl = other.recoilControl;
        recoilRecovery = other.recoilRecovery;
        attackRange = other.attackRange;
        toAimSpeed = other.toAimSpeed;
    }
     
    public void ApplyModifier(EShotStatType type, float value)
    {
        switch (type)
        {
            case EShotStatType.AccControl:
                accControl += value;
                break;

            case EShotStatType.RecoilControl:
                recoilControl += value;
                break;

            case EShotStatType.RecoilRecovery:
                recoilRecovery += value;
                break;

            case EShotStatType.AttackRange:
                attackRange += value;
                break;

            case EShotStatType.ToAimSpeed:
                toAimSpeed += value;
                break;

            default:
                break;
        }
    }

    public static ShotInfo operator +(ShotInfo a, ShotInfo b)
    {
        return new ShotInfo
        {
            accControl = a.accControl + b.accControl,
            recoilControl = a.recoilControl + b.recoilControl,
            recoilRecovery = a.recoilRecovery + b.recoilRecovery,
            attackRange = a.attackRange + b.attackRange,
            toAimSpeed = a.toAimSpeed + b.toAimSpeed
        };
    }
    public static ShotInfo operator -(ShotInfo a, ShotInfo b)
    {
        return new ShotInfo
        {
            accControl = a.accControl - b.accControl,
            recoilControl = a.recoilControl - b.recoilControl,
            recoilRecovery = a.recoilRecovery - b.recoilRecovery,
            attackRange = a.attackRange - b.attackRange,
            toAimSpeed = a.toAimSpeed - b.toAimSpeed
        }; 
    }
    public static ShotInfo operator -(ShotInfo a)
    {
        return new ShotInfo
        { 
            accControl = -a.accControl,
            recoilControl = -a.recoilControl,
            recoilRecovery = -a.recoilRecovery,
            attackRange = -a.attackRange,
            toAimSpeed = -a.toAimSpeed
        };
    }
}


/* ===============
   CapacityInfo
================ */

[System.Serializable]
public class CapacityInfo
{ 
    public int capacitySize = 0;
    public float maxWeight = 0f;
      
    public CapacityInfo Clone()
    {
        return new CapacityInfo
        {
            capacitySize = this.capacitySize,
            maxWeight = this.maxWeight
        };
    }
    public void CopyFrom(CapacityInfo other)
    {
        if (other == null)
            return;

        capacitySize = other.capacitySize;
        maxWeight = other.maxWeight;
    } 
}

/* ===============
   ArmorInfo
================ */

public enum EArmorPart
{
    Head,
    Body,
}

[System.Serializable]
public class ArmorInfo
{
    [Header("머리 방어구")]
    public float headDefense = 0f;     // 피해 감소량 또는 비율

    [Header("몸통 방어구")]
    public float bodyDefense = 0f;
     
    public ArmorInfo Clone()
    {
        return new ArmorInfo
        {
            headDefense = headDefense,
            bodyDefense = bodyDefense,
        };
    }
    public void CopyFrom(ArmorInfo other)
    {
        if (other == null)
            return;

        headDefense = other.headDefense;
        bodyDefense = other.bodyDefense;
    }
     
    public float GetDefense(bool isHead)
    {
        return isHead ? headDefense : bodyDefense;
    }
}

/* ===============
   StatInfo
================ */

public enum EStatType
{
    MaxHp,
    MaxMp,
    HpRecovery,
    MpRecovery,
}

[System.Serializable]
public class StatInfo  
{
    public float maxHp = 0f;
    public float maxMp = 0f;
    public float hpRecovery = 0f;
    public float mpRecovery = 0f;
     
    public StatInfo Clone()
    {
        return new StatInfo
        {
            maxHp = this.maxHp,
            maxMp = this.maxMp,
            hpRecovery = this.hpRecovery,
            mpRecovery = this.mpRecovery
        };
    }
    public void CopyFrom(StatInfo other)
    {
        if (other == null)
            return;

        maxHp = other.maxHp;
        maxMp = other.maxMp;
        hpRecovery = other.hpRecovery;
        mpRecovery = other.mpRecovery;
    }
}

