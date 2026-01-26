using UnityEngine;
using UnityEngine.InputSystem;
    
public class PlayerController : MonoBehaviour  
{
    [SerializeField] private PlayerCameraFocus cachedCameraFocus;
      
    private SlotController slotController;
    private PlayerInput cachedPlayerInput;
    private PlayerAiming cachedAiming;
    private PlayerDetector cachedDetector;
    private PlayerSkill cachedSkill;
    private PlayerInteraction cachedInteraction;
    private PlayerStorage cachedStorage;
    private PlayerEquip cachedEquip;
    private PlayerState cachedState;
    private PlayerStat cachedStat;
    private PlayerUIController cachedUIController;
    private PlayerQuickSlot cachedQuickSlot;

    private DuckBuff cachedBuff;
    private DuckLocomotion cachedLocomotion;
    private DuckAttack cachedAttack;
    private DuckMeleeAttack cachedMeleeAttack;

    private bool isAttack = false;
    private bool isPlay = false;
    private Transform cachedCam;
      
    private void Awake()
    {
        cachedState = GetComponent<PlayerState>();
        cachedStat = GetComponent<PlayerStat>(); 
        cachedEquip = GetComponent<PlayerEquip>();  
        cachedAiming = GetComponent<PlayerAiming>();
        cachedDetector = GetComponent<PlayerDetector>();
        cachedInteraction = GetComponent<PlayerInteraction>(); 
        cachedPlayerInput = GetComponent<PlayerInput>(); 
        cachedStorage = GetComponent<PlayerStorage>();
        cachedUIController = GetComponent<PlayerUIController>();
        cachedQuickSlot = GetComponent<PlayerQuickSlot>();
        cachedSkill = GetComponent<PlayerSkill>(); 
        cachedBuff = GetComponent<DuckBuff>(); 
        cachedLocomotion = GetComponent<DuckLocomotion>();
        cachedAttack = GetComponent<DuckAttack>();
        cachedMeleeAttack = GetComponent<DuckMeleeAttack>();

        slotController = GameInstance.Instance.SLOT_GetSlotController(); 
    }

    private void Start()
    {
        cachedCam = Camera.main.transform;
        GameInstance.Instance.CAMERA_ActiveCinemachinCamera(EPlayCameraType.TopView);
        ChangePlay();  
    } 

    private void Update()
    { 
        UpdateAttack();
        UpdateMousePos();
    } 
      
    /* On */

    // Play
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!cachedState.CanMove())
            return;

        if (!cachedBuff.CanMove())
            return;

        if (context.performed)
        {
            Vector2 input = context.ReadValue<Vector2>();
            if (input.sqrMagnitude < 0.0001f)
                return;

            Vector3 forward = cachedCam.forward;
            Vector3 right = cachedCam.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 moveDir = forward * input.y + right * input.x;
            if (!cachedLocomotion.CanMove(moveDir))
            {
                cachedLocomotion.DoMove(Vector3.zero);
                return;
            } 

            cachedLocomotion.DoMove(moveDir);
        }

        else if (context.canceled)
        {
            cachedLocomotion.DoMove(Vector3.zero);
        }
    }  
    public void OnLook(InputAction.CallbackContext context)
    { 
        // if (!CanLook())
        //     return;
        // 
        // Vector2 mousePos = context.ReadValue<Vector2>();
        // if (context.performed)
        // {
        //     cachedAiming.RenewMousePos(mousePos);
        // }    
    }
    public void OnSprint(InputAction.CallbackContext context)
    { 
        if (context.started)
        {
            if (CanSprint())
            {
                cachedState.ChangeState(EDuckState.Sprint);
            }
        }   
        else if (context.canceled)
        {
            if (cachedState.IsSprint())
            {
                cachedState.ChangeState(EDuckState.Default);
            }
        }  
    }
    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (CanAim())
            {
                cachedState.ChangeState(EDuckState.Aiming);
            } 
        }  
          
        else if (context.canceled)
        { 
            if (cachedState.IsAiming())
            {
                cachedState.ChangeState(EDuckState.Default);
            }
        }  
    } 
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isAttack = true;
        }
        else if (context.canceled)
        {
            isAttack = false;
        }
    }
    public void OnMeleeAttack(InputAction.CallbackContext context)
    {
        if (!CanMeleeAttack())
            return; 
             
        if (context.performed)
        {
            cachedMeleeAttack.DoAttack();
        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (!CanReload()) 
            return;

       // 장전 
       if (context.performed)
       {
            cachedEquip.TryReload();
       } 
    }
    public void OnCancle(InputAction.CallbackContext context)
    { 
        // 총알 바꾸는거 취소
        if (CanCancleChangeBullet())
        {
            cachedStorage.CancleChangeBullet();
            return;     
        } 
         
        // 장전하는거 취소
        if (CanCancleReload())
        { 
            cachedEquip.CancleReload();
            cachedState.ChangeState(EDuckState.Default);
            return; 
        }
         
        // 아이템 사용하는거 취소
        if (CanCancleUseConsum())
        {
            cachedStorage.CancleUseConsum();
            cachedState.ChangeState(EDuckState.Default);
            return;
        }  
    }
    public void OnQuickSlot(InputAction.CallbackContext context)
    { 
        int slotIndex = (int)context.ReadValue<float>() - 1;
        if (!CanQuickSlot(slotIndex))
            return;
           
        if (context.performed) 
        {
            cachedQuickSlot.TryUseConsum(slotIndex);
        }    
    }
    public void OnInteraction(InputAction.CallbackContext context)
    {
        if (!CanInteraction())
            return; 
         
        if (context.performed)
        {
            cachedInteraction.DoInteraction();
        }
    }
    public void OnMenu(InputAction.CallbackContext context)
    {
        if (!CanEnterMenu()) 
            return;

        if (context.performed)
        {
            cachedInteraction.EnterMenu();
        }    
    }
    public void OnChangeBullet(InputAction.CallbackContext context)
    {
        if (!CanChangeBullet())
            return;
           
        if (context.performed)
        { 
            cachedStorage.ChangeBullet();
        }   
    } 
    public void OnScrollBullet(InputAction.CallbackContext context)
    { 
        if (!CanScrollSelectBullet())
            return;
           
        if (context.started)
        {
            cachedStorage.ChangeSelectBullet(context.ReadValue<float>());
        }  
    }
    public void OnEsc(InputAction.CallbackContext context)
    {
        if (!CanEsc())
            return;

        if (context.started)
        {
            cachedInteraction.EnterESC();
        } 
    }


    // Skill
    public void OnRoll(InputAction.CallbackContext context)
    {
        if (!CanRoll())
            return;
            
        if (context.performed)
        {
            cachedSkill.Roll();
        }
    } 
    public void OnDetectorEnemy(InputAction.CallbackContext context)
    {
        if (!CanDetectEnemy())
            return;
         
        // 탐지 
        if (context.performed)
        { 
            cachedSkill.DetetectorEnemy();
        } 
    }  

    // UI 
    public void OnUICancle(InputAction.CallbackContext context)
    {
        if (!CanCancleUI())
            return;
         
        if (context.performed)
        {
            // cachedInteraction; 
            ChangePlay(); 
        }
    } 
       
    public void OnUISlot_Pick(InputAction.CallbackContext context)
    {
        if (!CanUISlot_Pick())
            return;
         
        if (context.performed)
        {
            slotController.Action_Instant(EItemSlotSelectType.Pick);
        }  

    }
    public void OnUISlot_Use(InputAction.CallbackContext context)
    {
        if (!CanUISlot_Use())
            return;
         
        if (context.started)
        {
            slotController.Action_Instant(EItemSlotSelectType.Use);
        } 
    }
    public void OnUISlot_Throw(InputAction.CallbackContext context)
    {
        if (!CanUISlot_Throw())
            return;

        if (context.performed) 
        {
            slotController.Action_Instant(EItemSlotSelectType.Throw);
        }
    }

    public void OnUISlot_Select(InputAction.CallbackContext context)
    {
        if (!CanUISlot_Select())
            return;

        if (context.performed)
        {
            slotController.Action_Select();
        }
    }
    public void OnUISlot_CancleSelect(InputAction.CallbackContext context)
    {
        if (!CanUISlot_CancleSelect())
            return;
         
        if (context.performed)
        { 
            slotController.Action_CancleSelct();
        } 
    }
     
    public void ChangeUIAndLookDetection()
    {
        ChangeUI(); 
         
        // 캐릭터가 아이템박스를 바라보게
        Transform target = cachedDetector.GetDetectionTarget().transform;
        Vector3 dir = (target.position - transform.position);
        dir.y = 0f; 
        if (dir.sqrMagnitude < 0.0001f)
            dir = transform.forward; // 거의 같은 위치면 정면 사용
        else
            dir.Normalize();
         
        Vector3 lookPos = transform.position + dir * 2f;
        cachedAiming.ChangeUI(lookPos);
    }
    public void ChangeUI()
    {
        isPlay = false;
        isAttack = false;

        cachedPlayerInput.SwitchCurrentActionMap("UI");

        // 카메라 
        GameInstance.Instance.CAMERA_ActievMainCamera(EDuckCameraType.BillboardCam, false);
         
        // 커서
        cachedUIController.ChangeUI();

        // 탐지 불가능
        cachedDetector.ChangeUI();
         
        // Slot 
        slotController.ChangeUI();

        // 
        cachedCameraFocus.Active(false);
    } 
    public void ChangePlay()
    {
        isPlay = true;
         
        cachedPlayerInput.SwitchCurrentActionMap("Player");
         
        // 카메라 원복
        GameInstance.Instance.CAMERA_ActievMainCamera(EDuckCameraType.BillboardCam, true);
         
        // 커서 
        cachedUIController.ChangePlay();

        // 조준 가능하게끔 설정
        cachedAiming.ChangePlay();

        // 상호 작용 원본 
        cachedInteraction.ChangePlay();

        // 탐지 가능
        cachedDetector.ChangePlay();

        // Slot 
        slotController.ChangePlay();

        // CameraFocuse
        cachedCameraFocus.Active(true);
    } 
     
    /* Update*/
    private void UpdateAttack()
    {
        if (CanAttack())
        {
            if (cachedState.IsAiming())
            {
                cachedAttack.DoAimAttack();
            }
             
            else
            {
                cachedAttack.DoAttack();
            }
        } 
    }
    private void UpdateMousePos()
    {
        if (!CanLook())
            return;
         
        cachedAiming.RenewMousePos(Mouse.current.position.ReadValue());
    }

    private bool CanRoll()
    {
        if (!cachedState.CanRoll())
            return false;

        if (!cachedStat.CanRoll())
            return false;
         
        if (!cachedSkill.CanRoll())
            return false;
         
        return true;
    }  
    private bool CanLook()
    {
        if (cachedState.IsDead())
            return false;
         
        if (!isPlay)
            return false;

        return true;
    }
    private bool CanSprint()
    { 
        if (!cachedState.CanSprint())
            return false;
 
        return true;
    }
    private bool CanAim()
    {
        if (!cachedState.CanAiming())
            return false;

        if (!cachedAttack.CanAim())
            return false;
         
        return true;
    } 
    private bool CanReload()
    {
        if (!cachedState.CanRelad())
            return false;

        if (!cachedStorage.CanReload())
            return false;
         
        return true;
    }

    private bool CanChangeBullet()
    {
        if (cachedState.IsDead())
            return false;

        if (!cachedStorage.CanChangeBullet())
            return false;

        return true;
    }
    private bool CanCancleChangeBullet()
    {
        // todo State 
        if (!cachedStorage.CanCancleChangeBullet())
            return false;
        return true;
    }
    private bool CanCancleReload()
    {
        // todo State

        if (!cachedEquip.CanCancleReload())
            return false;

        // 장전 취소
        return true;
    }
    private bool CanCancleUseConsum()
    {
        if (!cachedStorage.CanCancleUse())
            return false;
         
        return true;
    } 

    private bool CanAttack() 
    { 
        if (!isAttack)
            return false;

        if (!cachedState.CanAttack())
            return false;

        if (cachedMeleeAttack.IsAttacking())
            return false;
         
        if (!cachedAttack.CanAttack())
            return false;
               
        return true; 
    }
    private bool CanMeleeAttack()
    {
        if (!cachedState.CanAttack())
            return false;

        if (!cachedMeleeAttack.CanMeleeAttack())
            return false;
         
        return true;
    }
    private bool CanInteraction()
    {
        if (!cachedState.CanInteraction())
            return false;

        if (!cachedDetector.CanInteraction())
            return false;
          
        return true;
    }
    private bool CanEnterMenu()
    {
        if (!cachedState.CanEnterMenu())
            return false;

        if (!cachedInteraction.CanEnterMenu())
            return false;
         
        return true;
    }
    private bool CanCancleUI()
    {
        if (!cachedInteraction.CanCancleUI())
            return false;

        return true;
    } 
    private bool CanScrollSelectBullet()
    {
        if (cachedState.IsDead())
            return false;
          
        if (!cachedStorage.CanScrollBullet())
            return false;

        return true; 
    }
    private bool CanEsc()
    { 
        return true;
    }
    private bool CanQuickSlot(int _index)
    {
        // 죽지 않고, 구르지 않고 기타 등등
         
        // 이미 사용중이라면 안됨 - cancle눌러야함
        if (!cachedStorage.CanUseConsum())
            return false;

        // 퀵슬롯에 있는걸 체크를 해줘야함 
        if (!cachedQuickSlot.CanUseQuickSlot(_index))
            return false;
         
        return true;
    }
    private bool CanDetectEnemy()
    {
        if (cachedState.IsDead())
            return false;

        // 쿨타임 체크
        if (!cachedSkill.CanDetector())
            return false;
          
        return true;
    } 


    private bool CanUISlot_Pick()
    {
        if (!slotController.CanInstantAction())
            return false; 
          
        if (!cachedStorage.IsEmptySlotSet())
            return false;

        return true;
    }
    private bool CanUISlot_Use()
    {
        // 해당 아이템을 사용할 수 있는지 체크 

        return true;
    }
    private bool CanUISlot_Throw()
    {
        if (!slotController.CanInstantAction())
            return false;
         
        // 해당 아이템을 버릴 수 있는지 체크?

        return true;
    }
    private bool CanUISlot_Select()
    { 
        return true; 
    }
    private bool CanUISlot_CancleSelect()
    {
        return true; 
    } 
     

}
