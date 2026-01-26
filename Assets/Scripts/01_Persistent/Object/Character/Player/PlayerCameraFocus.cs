using UnityEngine;

public class PlayerCameraFocus : MonoBehaviour
{  
    [Header("Focus Settings")]
    [SerializeField] private float moveRange = 3f;   // 중심에서 움직일 수 있는 최대 거리
    [SerializeField] private float followSpeed = 5f; // 부드럽게 따라가는 속도
     
    private PlayerAiming cachedAiming;
    private Transform duckTransform;
    private Vector3 targetPos;
    private bool bActive = true; 
    
    private void Awake()
    {
        cachedAiming = transform.parent.GetComponentInChildren<PlayerAiming>();
        duckTransform = transform.parent.GetComponentInChildren<PlayerController>().transform;
        GameInstance.Instance.CAMERA_SetCameraFocus(transform);
    }
      
    private void FixedUpdate()
    {
        if (duckTransform == null)
            return;

        if (!bActive)
            return;
         
        LateUpdateFocusPosition();
    }
     
    public void CacheAiming(PlayerAiming _cachedAiming)
    {
        cachedAiming = _cachedAiming;
    }
    public void CacheTransform(Transform t)
    {
        duckTransform = t;
    } 

    public void Active(bool _active)
    {
        bActive = _active;  
    }
    private void LateUpdateFocusPosition()
    {
        Vector3 targetPoint = cachedAiming.GetTargetPos();
        Vector3 offset = targetPoint - duckTransform.position;

        float attackRange = cachedAiming.GetAttackRange();
        float normalizedRange = Mathf.Clamp01(attackRange / 100f); // 0~100 → 0~1
        float rangeMultiplier = Mathf.Lerp(1f, 1.5f, normalizedRange);
        float finalMoveRange = moveRange * rangeMultiplier;
           
        offset.x = Mathf.Clamp(offset.x, -finalMoveRange, finalMoveRange);
        offset.z = Mathf.Clamp(offset.z, -finalMoveRange, finalMoveRange);
        offset.y = 0f;

        Vector3 desiredPos = duckTransform.position + offset;
        targetPos = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);
        transform.position = targetPos;
    }
}  
