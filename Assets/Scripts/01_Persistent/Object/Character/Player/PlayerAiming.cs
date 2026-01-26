using UnityEngine;
 
public class PlayerAiming : DuckAiming
{  
    [SerializeField] Transform aimTarget;               // 임시
    [SerializeField] private float aimLerpSpeed = 20f;  // 보간 속도
    [SerializeField] private float recoilReduceAimlerp = 3f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask hitMask;   // 적 or 나무 등
      
    private PlayerUIController cachedUIController;
    private PlayerAttack cachedPlayerAttack;
    private PlayerState cacheadPlayerState;

    private Camera cachedCamera;
    private Vector2 mousePos; // 실제 mousePos
    private Vector2 lerpMousePos; 
    private bool isPlaying = true;
      
    protected override void Awake() 
    {
        base.Awake();
         
        cachedUIController = GetComponent<PlayerUIController>();
        cachedPlayerAttack = GetComponent<PlayerAttack>();
        cacheadPlayerState = GetComponent<PlayerState>(); 

    }
    private void Start()
    {
        cachedCamera = Camera.main;
    }
     
    protected override void Update()
    { 
        if (isPlaying) 
        {
            UpdateRecoil();  
            UpdateAiming();
            UpdateMousePos();
            UpdateRay();
        } 
         
        UpdateAimTarget();
    }
       
    public override void DoRecoil(float _remainSize)
    {
        base.DoRecoil(_remainSize);
    } 
 
    public override void ChangeAim()
    {
        base.ChangeAim(); 

        cachedUIController.CURSOR_ChangeCursorState(ECursorState.Aim);
    }
    public override void ChangeDefault()
    {
        base.ChangeDefault();

        cachedUIController.CURSOR_ChangeCursorState(ECursorState.Default);
    }
    public override void ChangeReload()
    {
        base.ChangeReload();
         
        cachedUIController.CURSOR_ChangeCursorState(ECursorState.Reload);
    }

    public Vector2 GetLerpMousePos() { return lerpMousePos; } 

    public void MoveCursorToDir(float _mouseMoveSize)
    {
        // 마우스 변화, 총을 쏠 때마다 계속 반동 위치로 변화시켜줘야함  
        // 조준 
        Vector3 screenDir = cachedCamera.WorldToScreenPoint(transform.position + lookDir) - cachedCamera.WorldToScreenPoint(transform.position);
        screenDir.Normalize();
         
        Vector2 newMousePos = mousePos + new Vector2(screenDir.x, screenDir.y) * _mouseMoveSize;
         
        // 화면 안으로 제한
        newMousePos.x = Mathf.Clamp(newMousePos.x, 0, (Screen.width));
        newMousePos.y = Mathf.Clamp(newMousePos.y, 0, (Screen.height));
        lerpMousePos = newMousePos + new Vector2(screenDir.x, screenDir.y);
    }
    public void RenewMousePos(Vector2 _pos)
    {
        mousePos = _pos;
    }

    public void ChangeUI(Vector3 _targetPoint) 
    { 
        targetPoint = _targetPoint;
        isPlaying = false;
    }
    public void ChangePlay()
    {
        isPlaying = true;
    }

    private void UpdateMousePos()
    {
        float lerpSpeed = isRecoiling
            ? aimLerpSpeed - recoilReduceAimlerp
            : aimLerpSpeed;

        lerpMousePos = Vector2.Lerp(
            lerpMousePos,
            mousePos,
            Time.deltaTime * lerpSpeed
        );
         
        cachedUIController.CURSOR_RenewMousePos(lerpMousePos);
    }
    private void UpdateRay() 
    {
        Ray screenRay = cachedCamera.ScreenPointToRay(lerpMousePos);
        Vector3 rayDir = screenRay.direction.normalized;

        if (Mathf.Abs(rayDir.y) < 0.0001f)
            return;

        float topY = 200f;
        float t = (topY - screenRay.origin.y) / rayDir.y;
        Vector3 rayStart = screenRay.origin + rayDir * t;

        bool hasGround = false;
        Vector3 groundPoint = Vector3.zero;

        // 1) 항상 그라운드부터 체크
        if (Physics.Raycast(rayStart, rayDir, out RaycastHit groundHit, 2000f, groundMask, QueryTriggerInteraction.Ignore))
        {
            groundPoint = groundHit.point;
            hasGround = true;
        }

        // 2) 조준 중이면 적/오브젝트 히트 우선
        if (cacheadPlayerState.IsAiming() &&
            Physics.Raycast(rayStart, rayDir, out RaycastHit hit, 2000f, hitMask, QueryTriggerInteraction.Collide))
        {
            targetPoint = hit.point;
            return;
        }

        // 3) 조준 중이 아닐 때: 그라운드 + 무기 높이 보정
        if (hasGround)
        {
            var weapon = cachedPlayerAttack.GetWeapon();
            float weaponY = weapon
                ? weapon.GetMuzzleTransform().position.y
                : transform.position.y;

            targetPoint = new Vector3(
                groundPoint.x,
                weaponY,
                groundPoint.z
            );
        } 

        else
        {
            // 4) 그라운드도 못 찾았을 때
            targetPoint = transform.position + lookDir * 10f;
        }
    } 
    private void UpdateAimTarget()
    {
        if (!aimTarget)
            return; 
         
        aimTarget.position = targetPoint;
        lookDir = (targetPoint - transform.position).normalized;
    }
}  
  