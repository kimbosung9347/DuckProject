using UnityEngine;
  
public enum EDetectionState
{
    Detecting,
    Interacting,
    End, 
}

public class DetectionTarget : MonoBehaviour 
{
    // [SerializeField] private  
    // Todo GameInstance에서 API 제공할꺼임  
    [SerializeField] private UIDetection uiDetection;
    [SerializeField] private HandleInteractionBase interactionHandle; 
     
    [SerializeField] private string interactionDesc;
    [SerializeField] private string interactionKey; 
    [SerializeField] private float dectectDistance;

    ////////////////////////////////
    // Interaction 컴포넌트를 만들어주자

    private EDetectionState state;
    private Transform cachedPlayerDetectorTransform;
    private UIInteractionBillboard cachedUIInteractionBillboard;
    private SphereCollider cachedCollider;

    /////////////////////////////
    // 자신의 아이템 정보를 받아와야함

    private void Awake() 
    {
        // 임시 - 하드코딩해도 상관없음 나중에 처리  
        uiDetection.SetMaxDistance(dectectDistance);
         
        cachedUIInteractionBillboard = GameInstance.Instance.UI_GetPersistentUIGroup().GetInteractionBillboard();
        cachedPlayerDetectorTransform = GameInstance.Instance.PLAYER_GetPlayerTransform();

        MakeCollider(); 
    }  

    private void OnEnable()
    {
        ChangeInteractionState(EDetectionState.End);
    }   
    private void Update() 
    {
        switch (state)
        { 
        case EDetectionState.Detecting:
        case EDetectionState.Interacting:
            {
                UpdateCheckDetected();
            }
            break;
        }   
    }

    public void SetInteractionKey(string _key)
    {
        interactionKey = _key;
    }
    public void SetInteractionDesc(string _desc)
    {
        interactionDesc = _desc;
    } 

    public void ChangeInteractionState(EDetectionState _state)
    {
        if (state == _state)
            return;

        // Clear
        {
            switch (state)
            {
                case EDetectionState.Detecting:
                    {
                        ClearDetecting();
                    }
                    break;

                case EDetectionState.Interacting:
                    {
                        ClearInteracting();
                    }
                    break;

                case EDetectionState.End:
                    {
                        ClearEnd();
                    }
                    break;
            }
        }

        state = _state;
         
        // Change 
        {
            switch (state)
            {
                case EDetectionState.Detecting:
                    {
                        ChangeDectecting();
                    }
                    break;

                case EDetectionState.Interacting:
                    {
                        ChangeInteracting();
                    }
                    break;

                case EDetectionState.End:
                    {
                        ChangeEnd();
                    }
                    break;
            }

        }
    } 
    public void ActiveInteraction(bool _isLeft)
    {
        ChangeInteractionState(EDetectionState.Interacting);
        cachedUIInteractionBillboard.SetIsLeft(_isLeft);
    }
    public void ComplateAllSearch()
    {
        uiDetection.ComplateAllSearch();
    } 
    public void DoInteractionToPlayer(PlayerInteraction _interaction)
    {
        HandleInteractionBase handle = (interactionHandle == null) ? GetComponent<HandleInteractionBase>() : interactionHandle;
        handle.DoInteractionToPlayer(_interaction);
    }
    public void EndInteractionToPlayer()
    {
        HandleInteractionBase handle = (interactionHandle == null) ? GetComponent<HandleInteractionBase>() : interactionHandle;
        handle?.EndInteractionToPlayer();
    }  

    public void ActiveCollider()
    {
        cachedCollider.enabled = true;
    }
    public void DisableCollider()
    {
        cachedCollider.enabled = false;
    }
     
    private void ClearDetecting()
    {

    } 
    private void ClearInteracting()
    { 
        // Interaction 빌보드 초기화 
        cachedUIInteractionBillboard.Dsiable(); 
    } 
    private void ClearEnd()
    {

    }

    private void ChangeDectecting()
    {
        CheckAndActiveUIDitection();
        uiDetection.ChangeDetection();
    } 
    private void ChangeInteracting()
    { 
        uiDetection.ChangeInteraction();
        cachedUIInteractionBillboard.Active(transform, interactionDesc, interactionKey); 
    }   
    private void ChangeEnd()
    {
        uiDetection.gameObject.SetActive(false);
    }

    private void MakeCollider()
    {
        cachedCollider = gameObject.AddComponent<SphereCollider>();
        cachedCollider.isTrigger = true;
        cachedCollider.radius = 0.1f; 
        gameObject.layer = LayerMask.NameToLayer("Interaction"); 
    }   
    private void UpdateCheckDetected()
    {
        float distance = Vector3.Distance(transform.position, cachedPlayerDetectorTransform.position); 
        uiDetection.SetDistance(distance); 
        if (distance > dectectDistance)
        {
            CancleDetected();
        }
    } 
    private void CancleDetected()
    {
        ChangeInteractionState(EDetectionState.End);
    } 
    private void CheckAndActiveUIDitection()
    {
        if (!uiDetection.gameObject.activeInHierarchy)
        {
            uiDetection.gameObject.SetActive(true);
        }
    }
}
   