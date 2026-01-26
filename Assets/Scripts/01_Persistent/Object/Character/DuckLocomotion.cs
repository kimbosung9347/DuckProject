using UnityEngine;
using UnityEngine.AI;
 
[RequireComponent(typeof(Rigidbody))]
public class DuckLocomotion : MonoBehaviour
{
    [SerializeField] private float rollColliderRadius = 0.3f;
    [SerializeField] private float rollColliderHeight = 1.2f;
    [SerializeField] private float moveCheckDistance = 0.4f;
    [SerializeField] private LayerMask rollBlockMask;
    [SerializeField] private float walkSound = 0.7f;
     
    private DuckState cachedState;
    private DuckAnimation cachedDuckAnim;
    private DuckAiming cacgedDuckAiming; 
    private LocoInfo cachedLocoInfo;
    private Rigidbody cachedRb;
     
    private bool activeRoll = false;
    private bool finishedRollRot = false;
    private Vector3 rollDir = Vector3.zero;
    private Vector3 moveInput;
     
    // WalkSound 
    private AudioSource walkSrc;
    private SoundSfx walkSfx;

    private void Awake()
    {
        cachedState = GetComponent<DuckState>();
        cachedDuckAnim = GetComponent<DuckAnimation>();
        cacgedDuckAiming = GetComponent<DuckAiming>(); 
         
        cachedRb = GetComponent<Rigidbody>();
        cachedRb.constraints = RigidbodyConstraints.FreezeRotation;
        cachedRb.interpolation = RigidbodyInterpolation.None; // 보간은 직접 처리

        // AudioSource 생성
        walkSrc = gameObject.AddComponent<AudioSource>();
        walkSfx = GameInstance.Instance.SOUND_GetSfx(ESoundSfxType.Walk);
         
        // AudioSource 생성
        walkSrc.playOnAwake = false;
        walkSrc.loop = true;
         
        // SoundSfx → AudioSource 세팅
        walkSrc.clip = walkSfx.clip;
        walkSrc.volume = walkSound; 
        walkSrc.pitch = walkSfx.pitch;
        walkSrc.spatialBlend = walkSfx.spatialBlend;
    }

    private void Start()
    {

    }  
    private void FixedUpdate()
    {
        if (IsRoll())
        {
            FixedUpdateRoll(); 
        } 
        else
        {
            FixedUpdateDirMove();
        } 
    }
    private void Update()
    { 
        if (IsRoll())
        {
            if (IsEndRollAnim())
            {
                ClearRoll();
            }
        }
         
        else
        {  
            UpdateRotation();

            if (IsInput())
            {
                UpdateDuckBodyAnim();
                UpdateDuckFootAnim(); 
            }
        } 
    }
      
    public void CacheLocoInfo(LocoInfo _locoInfo)
    {
        cachedLocoInfo = _locoInfo;
    }

    /* Can_ */
    public bool CanMove(Vector3 moveDir)
    {
        if (cachedLocoInfo.moveInfo.moveSpeed <= 0)
            return false;

        if (moveDir.sqrMagnitude < 0.0001f)
            return false;
          
        Vector3 origin = transform.position;
        Vector3 nextPos = origin + moveDir.normalized * moveCheckDistance; // ex) 0.4f
        return NavMesh.SamplePosition( 
            nextPos,
            out _, 
            0.25f, 
            NavMesh.AllAreas
        );
    } 
     
    /* Do_ */
    public void DoMove(Vector3 _dir)
    {
        moveInput = _dir;

        // 입력 있음 → 재생
        if (_dir.sqrMagnitude > 0.0001f)
        {
            if (!walkSrc.isPlaying)
                walkSrc.Play();
        }
        // 입력 없음 → 정지 
        else
        {
            if (walkSrc.isPlaying)
                walkSrc.Stop();
        }
    }
    public void DoRoll()
    {
        // 이미 구르는 중이면 무시
        if (IsRoll())
            return;
         
        Vector3 inputDir = moveInput;
        if (inputDir.sqrMagnitude > 0.01f)
        {
            finishedRollRot = false;
            rollDir = inputDir;
        }

        // 입력이 없으면 현재 바라보는 방향으로 구르기
        else
        {
            finishedRollRot = true;
            rollDir = transform.forward;
            cachedDuckAnim.ActiveRoll();
        }  
 
        cachedState.ChangeState(EDuckState.Roll);
        activeRoll = true;

        // 
        PlayEffectRoll();
    } 

    private void PlayEffectRoll()
    {
        float rollDuration = cachedDuckAnim.GetRollDuration();
        GameInstance.Instance.SOUND_PlaySoundSfx(
            ESoundSfxType.Roll,
            transform.position,
            rollDuration
        ); 
    }

    /* FixedUpdate */
    private void FixedUpdateDirMove()
    {
        if (!IsInput()) 
            return;   
         
        Vector3 moveDir = moveInput;
        Vector3 targetPos = cachedRb.position + moveDir * (cachedLocoInfo.moveInfo.moveSpeed * Time.fixedDeltaTime);
        cachedRb.MovePosition(targetPos);
    }
    private void FixedUpdateRoll()
    {
        // GetRollDuration()

        if (!finishedRollRot)
        {
            Quaternion targetRot = Quaternion.LookRotation(rollDir);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                cachedLocoInfo.rollInfo.rollRotSpeed * Time.fixedDeltaTime
            );

            if (IsCompleteRollRot(targetRot))
            {
                finishedRollRot = true;
                transform.rotation = targetRot;
                cachedDuckAnim.ActiveRoll();
            }
            return;
        }
         
        // 보간 
        float ratio = cachedDuckAnim.GetRollDurationRatio();
        float speedFactor = Mathf.Lerp(0.2f, 1f, Mathf.Sin(ratio * Mathf.PI));
        float moveDist =
            cachedLocoInfo.rollInfo.rollSpeed *
            speedFactor *
            Time.fixedDeltaTime;

        Vector3 dir = rollDir.normalized;
        Vector3 pos = cachedRb.position;
        Vector3 nextPos = pos + dir * moveDist;

        // ===== 캡슐 충돌 체크 =====
        Vector3 bottom = pos + Vector3.up * rollColliderRadius;
        Vector3 top = bottom + Vector3.up * (rollColliderHeight - rollColliderRadius * 2f);

        if (Physics.CapsuleCast(
                bottom, top,
                rollColliderRadius,
                dir,
                out _,
                moveDist,
                rollBlockMask,
                QueryTriggerInteraction.Ignore))
        {
            cachedRb.MovePosition(pos);
            return;
        }
         
        // ===== NavMesh 유효성 체크 =====
        if (!NavMesh.SamplePosition(
                nextPos,
                out _,
                0.3f,              // 허용 오차 (작게)
                NavMesh.AllAreas))
        {
            cachedRb.MovePosition(pos);
            return;
        }
         
        // ===== 이동 =====
        cachedRb.MovePosition(nextPos);
    }   
       
    /* Update */
    private void UpdateRotation()
    {
        if (cachedState.IsSprint())
        {
            if (moveInput.sqrMagnitude > 0.001f)
            {
                Vector3 moveDir = moveInput;
                UpdateLookRotation(moveDir, cachedLocoInfo.moveInfo.sprintRotSpeed);
            }   
        }  
          
        else
        {
            var lookDir = cacgedDuckAiming.GetLookDir();
            lookDir.y = 0f;
            if (lookDir != Vector3.zero)
            { 
                UpdateLookRotation(lookDir, cachedLocoInfo.moveInfo.rotSpeed);
            } 
        } 
    }   
    private void UpdateLookRotation(Vector3 _foward, float _rotSpeed)
    {
        Quaternion targetRot = Quaternion.LookRotation(_foward);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            Time.deltaTime * _rotSpeed
        );
    }
    private void UpdateDuckBodyAnim()
    {
        if (!CanDuckBodyAnim())
            return;

        cachedDuckAnim.ActiveBodyAnim();
    }
    private void UpdateDuckFootAnim()
    {
        if (!CanDuckFootAnim())
            return;
         
        cachedDuckAnim.ActiveFootAnim();
    }
 
    /* Check */
    private bool IsInput()
    {
        return (moveInput.sqrMagnitude > 0.001f);
    }
    private bool IsRoll() { return activeRoll; }
    private bool IsEndRollAnim()
    {
        return (finishedRollRot && cachedDuckAnim.IsEndRoll());
    } 
    private bool IsCompleteRollRot(Quaternion _targetRot)
    {
        float angle = Quaternion.Angle(transform.rotation, _targetRot);
        return angle < 1f;
    }

    private bool CanDuckBodyAnim()
    {
        if (!cachedState.CanBodyAnim() || IsRoll())
            return false;

        return true;
    }
    private bool CanDuckFootAnim()
    {
        if (!cachedState.CanFootAnim() || IsRoll())
            return false;

        return true;
    }
 
    /* Clear */
    private void ClearRoll()
    {
        cachedState.ChangeState(EDuckState.Default);
        activeRoll = false;
        rollDir = Vector3.zero;
    }   
}
