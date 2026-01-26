using UnityEngine;

public class PlayerDetector : MonoBehaviour
{  
    [Header("탐지 설정")]
    [Tooltip("탐지 반경")]
    [SerializeField] private float detectRadius = 5f;
    [SerializeField] private float interactionRadius = 0.5f;
       
    [Tooltip("탐지할 대상의 레이어")]
    [SerializeField] private LayerMask detectLayer; 
    [SerializeField] private float detectInterval = 0.1f;

    private bool isActive = true;
    private float elapsedTime = 0f;
    private DetectionTarget cachedCanInteractionTarget;
     
    private void Update()
    {
        if (!isActive)
            return;

        elapsedTime += Time.deltaTime;
        if (elapsedTime < detectInterval)
            return; 

        elapsedTime = 0f; 
        UpdateDetector(); 
    } 

    public DetectionTarget GetDetectionTarget() 
    {  
        return cachedCanInteractionTarget;
    } 
    public bool CanInteraction()
    { 
        return (cachedCanInteractionTarget != null);
    }
    
    public void ChangeUI()
    {
        isActive = false;
    }
    public void ChangePlay()
    {
        isActive = true;
    } 
    
    private void UpdateDetector()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectRadius, detectLayer);
        if (hits.Length == 0)
        {
            // 모든 대상이 탐지 범위 밖일 때 — 기존 인터랙션 대상 해제
            if (cachedCanInteractionTarget != null)
            {
                cachedCanInteractionTarget.ChangeInteractionState(EDetectionState.Detecting);
                cachedCanInteractionTarget = null;
            }
            return;
        } 

        DetectionTarget nearestTarget = null;
        float nearestDist = float.MaxValue;
      
        // 가장 가까운 타깃 탐색
        foreach (var hit in hits)
        {
            DetectionTarget target = hit.GetComponent<DetectionTarget>();
            if (target == null)
                continue;

            float dist = Vector3.Distance(transform.position, hit.transform.position);
         
            // 모든 타깃은 탐지 상태
            if (target != cachedCanInteractionTarget)
                target.ChangeInteractionState(EDetectionState.Detecting);

            if (dist <= interactionRadius && dist < nearestDist)
            {
                nearestDist = dist;
                nearestTarget = target;
            }
        }

        // 상호작용 가능한 타깃 갱신
        if (nearestTarget != null)
        {
            // 기존 대상이 아니라면 새로 갱신
            if (cachedCanInteractionTarget != nearestTarget)
            {
                // 이전 대상 복원
                if (cachedCanInteractionTarget != null)
                    cachedCanInteractionTarget.ChangeInteractionState(EDetectionState.Detecting);
                
                cachedCanInteractionTarget = nearestTarget;

                bool isLeft = true;
                Vector3 dir = nearestTarget.transform.position - transform.position;
                float dot = Vector3.Dot(Camera.main.transform.right, dir);
                if (dot > 0f)
                {
                    isLeft = false;
                }
                cachedCanInteractionTarget.ActiveInteraction(isLeft);
            }   
        }
        else
        {
            // 인터랙션 대상이 사라졌을 경우
            if (cachedCanInteractionTarget != null)
            {
                cachedCanInteractionTarget.ChangeInteractionState(EDetectionState.Detecting);
                cachedCanInteractionTarget = null;
            }
        }
    }  
}  
