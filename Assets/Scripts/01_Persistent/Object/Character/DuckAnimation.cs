using Unity.VisualScripting;
using UnityEngine;
public class DuckAnimation : MonoBehaviour
{
    [SerializeField] private Transform charTransform;
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Transform lFootTransform;
    [SerializeField] private Transform rFootTransform;
    [SerializeField] private Transform lArmTransform;
    [SerializeField] private Transform rArmTransform;

    // Ability에서 전달받은 설정값
    private AnimBodyInfo cachedBodyInfo;
    private AnimFootInfo cachedFootInfo;
    private AnimArmInfo cachedArmInfo;
    private AnimRollInfo cachedRollInfo;
    
    // DuckAnimation 내부에서 관리하는 런타임 상태
    private FBodyAnimRuntime bodyState;
    private FFootAnimRuntime footState;
    private FArmAnimRuntime armState; 
    private FRollAnimRuntime rollState;

    // 총 - 하드코딩
    private const float ShotRecoilDuration = 0.12f;
    private const float ShotRecoilMaxAngle = 15f;
    private FShotAnimRuntime shotState;  
       
    private Quaternion defaultBodyRot;
    private Quaternion defaultCharRot;

    private Vector3 defaultCharPos;
    private Vector3 defaultLFootPos;
    private Vector3 defaultRFootPos;
    private Vector3 defaultRArmPos;

    private void Awake()
    {
        defaultCharRot = charTransform.localRotation;
        defaultBodyRot = bodyTransform.localRotation;
         
        defaultCharPos = charTransform.localPosition;
        defaultLFootPos = lFootTransform.localPosition;
        defaultRFootPos = rFootTransform.localPosition;
        defaultRArmPos = rArmTransform.localPosition;
    }  
    private void Start()
    {
    }  
    private void Update()
    {
        UpdateBodyAnim();
        UpdateFootAnim();
        UpdateArmAnim(); 
        UpdateRollAnim();
    } 

    public void CacheAnimInfo(AnimInfo _animInfo)
    {
        cachedBodyInfo = _animInfo.bodyAnimInfo;
        cachedFootInfo = _animInfo.footAnimInfo;
        cachedRollInfo = _animInfo.rollAnimInfo;
        cachedArmInfo = _animInfo.armAnimInfo;
    }
    public float GetRollDurationRatio() { return rollState.time / cachedRollInfo.rollDuration; }
    public float GetRollDuration() { return cachedRollInfo.rollDuration; }   

    // Active 
    public void ActiveBodyAnim()
    {
        if (bodyState.bIsFinished || !bodyState.bIsActive)
        {  
            bodyState.time = 0f;
            bodyState.remainTime = cachedBodyInfo.stopTime;
            bodyState.bIsFinished = false;
        }
        else
        {
            bodyState.remainTime = cachedBodyInfo.stopTime;
        }

        bodyState.bIsActive = true;
    }
    public void ActiveFootAnim()
    {
        if (footState.bIsFinished || !footState.bIsActive)
        {
            footState.time = -Mathf.PI * 0.25f;
            footState.currentAmplitude = cachedFootInfo.moveAmplitude;
            footState.remainTime = cachedFootInfo.stopTime;
            footState.bIsFinished = false;
        }
        else
        {
            footState.remainTime = cachedFootInfo.stopTime;
        }

        footState.bIsActive = true;
    }
    public void ActiveRoll()
    {
        rollState.currentAngle = 0f;
        rollState.time = 0f;
        rollState.bIsActive = true;
        rollState.bIsFinished = false;
        charTransform.localRotation = defaultCharRot;
        charTransform.localPosition = defaultCharPos;
    }
    public void ActiveShotAnim()
    {
        // 현재 팔 각도 (Z축)
        float currentZ = rArmTransform.localEulerAngles.z;
        if (currentZ > 180f) currentZ -= 360f;

        // 기준(Base) 각도
        float baseZ = armState.bIsEquipWeapon
            ? cachedArmInfo.baseAngleWeapon
            : cachedArmInfo.baseAngleBareHand;
         
        // 차이 저장
        shotState.startDeltaAngle = currentZ - baseZ;

        shotState.time = 0f;
        shotState.bIsActive = true;
    }  

    public void ChangeEquip(bool _isEquip)
    {
        armState.bIsEquipWeapon = _isEquip;
          
        if (_isEquip)
        {
            // 오른팔 무기 들기 기본 포즈
            rArmTransform.localRotation = Quaternion.Euler(0f, -90f, cachedArmInfo.baseAngleWeapon);
            rArmTransform.localPosition = defaultRArmPos + new Vector3(0f, 0.02f, 0.05f);
        } 
        else  
        {
            // 맨손 기본 포즈
            rArmTransform.localRotation = Quaternion.Euler(0f, -90f, cachedArmInfo.baseAngleBareHand);
            rArmTransform.localPosition = defaultRArmPos;
        } 
    } 
    public void ChangeConsum(bool _isAttach)
    {
        armState.bIsAttachConsum = _isAttach;
        armState.time = 0f;
          
        // 소비 아이템 사용시 두손 다 들어주자
        if (_isAttach)
        {
            rArmTransform.localRotation = Quaternion.Euler(0f, -90f, cachedArmInfo.baseAngleWeapon);
            lArmTransform.localRotation = Quaternion.Euler(0f, -90f, cachedArmInfo.baseAngleWeapon);
        }
         
        else
        { 
            lArmTransform.localRotation = Quaternion.Euler(0f, -90f, cachedArmInfo.baseAngleBareHand);
            ChangeEquip(armState.bIsEquipWeapon);
        } 
    }
    public void ChangeSprint(bool _isSprint)
    { 
        if (_isSprint)
        {
            if (armState.bIsEquipWeapon)
            {
                rArmTransform.localRotation = Quaternion.Euler(0f, -90f, cachedArmInfo.baseSprintAngleWeapon); 
            }

            else
            {
                rArmTransform.localRotation = Quaternion.Euler(0f, -90f, cachedArmInfo.baseAngleBareHand);
            } 
        } 

        else
        {
            if (armState.bIsEquipWeapon)
            {
                rArmTransform.localRotation = Quaternion.Euler(0f, -90f, cachedArmInfo.baseAngleWeapon);
            } 

            else
            {
                rArmTransform.localRotation = Quaternion.Euler(0f, -90f, cachedArmInfo.baseAngleBareHand);
            }
        }
         
    }
    public bool IsEndRoll() => rollState.bIsFinished;
      
    // Update 
    private void UpdateBodyAnim()
    {
        if (bodyState.bIsFinished)
            return;

        bodyState.time += Time.deltaTime;

        // --- 감쇠 제어 ---
        if (bodyState.bIsActive)
        {
            bodyState.currentAmplitude = cachedBodyInfo.moveAngle;
            bodyState.remainTime -= Time.deltaTime;
            if (bodyState.remainTime <= 0f)
                bodyState.bIsActive = false;
        }

        else
        {
                // 진폭을 부드럽게 줄이기
                bodyState.currentAmplitude = Mathf.MoveTowards(
                bodyState.currentAmplitude,
                0f,
                cachedBodyInfo.dampingSpeed * Time.deltaTime
            );

            if (bodyState.currentAmplitude <= 0.0001f)
            {
                bodyTransform.localRotation = defaultBodyRot;
                bodyState.bIsFinished = true;
                return;
            }
        }
         
        // --- 감쇠된 진폭을 이용해 흔들림 계산 ---
        float angleY = Mathf.Sin(bodyState.time * cachedBodyInfo.moveSpeed) * bodyState.currentAmplitude;
        float angleX = Mathf.Sin(bodyState.time * cachedBodyInfo.moveSpeed * 0.95f) * bodyState.currentAmplitude * 0.7f;
        bodyTransform.localRotation = defaultBodyRot * Quaternion.Euler(angleX, angleY, 0f);
    } 
    private void UpdateFootAnim()
    {
        if (footState.bIsFinished)
            return;

        footState.time += Time.deltaTime;

        float offsetL = 0f;
        float offsetR = 0f;

        if (footState.bIsActive)
        {
            float sinL = Mathf.Sin(footState.time * cachedFootInfo.moveSpeed);
            float sinR = Mathf.Sin(footState.time * cachedFootInfo.moveSpeed + cachedFootInfo.phaseOffset);

            offsetL = Mathf.Max(0f, sinL) * cachedFootInfo.moveAmplitude;
            offsetR = Mathf.Max(0f, sinR) * cachedFootInfo.moveAmplitude;

            footState.remainTime -= Time.deltaTime;
            if (footState.remainTime <= 0f)
                footState.bIsActive = false;
        }
        else
        {
            float sinL = Mathf.Sin(footState.time * cachedFootInfo.moveSpeed);
            float sinR = Mathf.Sin(footState.time * cachedFootInfo.moveSpeed + cachedFootInfo.phaseOffset);

            float curL = Mathf.Max(0f, sinL) * cachedFootInfo.moveAmplitude;
            float curR = Mathf.Max(0f, sinR) * cachedFootInfo.moveAmplitude;

            offsetL = Mathf.MoveTowards(curL, 0f, cachedFootInfo.dampingSpeed * Time.deltaTime);
            offsetR = Mathf.MoveTowards(curR, 0f, cachedFootInfo.dampingSpeed * Time.deltaTime);

            if (offsetL <= 0.0001f && offsetR <= 0.0001f)
            {
                if (lFootTransform) lFootTransform.localPosition = defaultLFootPos;
                if (rFootTransform) rFootTransform.localPosition = defaultRFootPos;
                footState.bIsFinished = true;
                return;
            }
        }

        if (lFootTransform)
            lFootTransform.localPosition = defaultLFootPos + new Vector3(0f, offsetL, 0f);
        if (rFootTransform)
            rFootTransform.localPosition = defaultRFootPos + new Vector3(0f, offsetR, 0f);
    }
    private void UpdateArmAnim()
    {
        // 손 흔들기
        if (armState.bIsAttachConsum)
        {
            float swayAmplitude = 80f; // 흔들림 폭 (하드코딩)
            float swaySpeed = 5f;      // 흔들림 속도 (하드코딩)

            armState.time += Time.deltaTime;
            float sway = Mathf.Sin(armState.time * swaySpeed) * swayAmplitude;

            float baseAngle = cachedArmInfo.baseAngleWeapon;

            // 오른팔
            rArmTransform.localRotation =
                Quaternion.Euler(0f, -90f, baseAngle + sway);

            // 왼팔
            lArmTransform.localRotation =
                Quaternion.Euler(0f, -90f, baseAngle - sway * 0.7f);
        }

        // 총 쏘기
        if (shotState.bIsActive)
        { 
            shotState.time += Time.deltaTime;

            float t = shotState.time / ShotRecoilDuration;

            if (t >= 1f)
            {
                shotState.bIsActive = false;
                shotState.startDeltaAngle = 0f;
            }
            else
            {
                // 복귀 비율 (빠르게 튀고 천천히 복귀해도 됨)
                float restore = Mathf.Lerp(
                    shotState.startDeltaAngle,
                    0f,
                    t
                );

                float recoil =
                    Mathf.Sin(t * Mathf.PI) * ShotRecoilMaxAngle;

                float finalZ = restore + recoil;

                // Base + (복귀중인 차이 + 반동)
                float baseZ = armState.bIsEquipWeapon
                    ? cachedArmInfo.baseAngleWeapon
                    : cachedArmInfo.baseAngleBareHand;

                rArmTransform.localRotation =
                    Quaternion.Euler(0f, -90f, baseZ + finalZ);
            }
        }
    }
    private void UpdateRollAnim()
    {
        if (!rollState.bIsActive)
            return;

        rollState.time += Time.deltaTime;
        float t = Mathf.Clamp01(rollState.time / cachedRollInfo.rollDuration);
        float angle = Mathf.Lerp(0f, 360f, t);

        if (charTransform)
        {
            charTransform.localRotation = Quaternion.Euler(angle, 0f, 0f);

            float y = (t <= 0.5f)
                ? Mathf.Lerp(0f, cachedRollInfo.yOffset, t * 2f)
                : Mathf.Lerp(cachedRollInfo.yOffset, 0f, (t - 0.5f) * 2f);

            Vector3 pos = defaultCharPos;
            pos.y += y;
            charTransform.localPosition = pos;
        }

        if (t >= 1f)
        {
            rollState.bIsActive = false;
            rollState.bIsFinished = true;
            charTransform.localRotation = defaultCharRot;
            charTransform.localPosition = defaultCharPos;
        }
    }
}
 

/* ----------------------------------------
 * 런타임 상태 구조체
 * ---------------------------------------- */
[System.Serializable]
public struct FBodyAnimRuntime
{
    public float time;
    public float currentAmplitude;
    public float remainTime;
    public bool bIsActive;
    public bool bIsFinished;
}

[System.Serializable]
public struct FFootAnimRuntime
{
    public float time;
    public float currentAmplitude;
    public float remainTime;
    public bool bIsActive;
    public bool bIsFinished;
}

[System.Serializable]
public struct FRollAnimRuntime
{
    public float time;
    public float currentAngle;
    public bool bIsActive;
    public bool bIsFinished;
}


// 나중에 하자 
[System.Serializable]
public struct FArmAnimRuntime
{ 
    public float time;
    public bool bIsEquipWeapon; 
    public bool bIsAttachConsum;
}

[System.Serializable]
public struct FShotAnimRuntime
{ 
    public float time;
    public float startDeltaAngle; // 시작 시점 각도 차이
    public bool bIsActive;
}