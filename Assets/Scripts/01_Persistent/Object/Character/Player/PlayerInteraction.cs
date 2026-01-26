using UnityEngine;

enum EPlayerInteractionState
{
    None,
    ItemBox,
    Store,  
    WareHouse, 
    Quest,
    Adorn, 
    Sleep,
    SetLevel,
    MoveToOtherLevel,
       
    Menu,       // TAB
    ESC,    // ESC
         
    End,
}

public class PlayerInteraction : MonoBehaviour
{
    private EPlayerInteractionState interactionState = EPlayerInteractionState.None;

    private DuckDetected cachedDuckDetected;
    private PlayerController cachedPlayerController; 
    private PlayerUIController cachedUIController;
    private PlayerDetector cachedPlayerDetector;
    private PlayerStat cachedPlayerStat;  

    private void Awake()   
    {
        cachedDuckDetected = GetComponentInChildren<DuckDetected>(); 
        cachedPlayerController = GetComponent<PlayerController>();
        cachedUIController = GetComponent<PlayerUIController>();
        cachedPlayerDetector = GetComponent<PlayerDetector>();
        cachedPlayerStat = GetComponent<PlayerStat>(); 
    } 

    // 현재 활성화 되어 있는 유아이 상태에 따라서 취소
    public bool CanCancleUI()
    {
        if (interactionState == EPlayerInteractionState.None)
            return false;

        return true;
    }  
    public bool CanEnterMenu()
    {
        if (interactionState == EPlayerInteractionState.SetLevel)
            return false;

        if (interactionState == EPlayerInteractionState.ESC) 
            return false;
         
        return true;
    } 

    public void DoInteraction()
    {
        DetectionTarget detectedTarget = cachedPlayerDetector.GetDetectionTarget();
        detectedTarget.DoInteractionToPlayer(this); 
    }
    public void ChangePlay()
    {
        switch (interactionState)
        {
            case EPlayerInteractionState.ItemBox:
            {
                DisableItemBox();
            }
            break;

            case EPlayerInteractionState.ESC:
            {
                DisableESC(); 
            } 
            break;

            case EPlayerInteractionState.Menu:
            {
                DisableMenu();
            }
            break;

            case EPlayerInteractionState.Store:
            {
                DisableStore();
            }
            break;

            case EPlayerInteractionState.WareHouse:
            {
                DisableWareHouse();
            } 
            break;

            case EPlayerInteractionState.Quest:
            {
                DisableQuest();
            } 
            break;

            case EPlayerInteractionState.Adorn:
            { 
                DisableAdorn();
            }
            break;

            case EPlayerInteractionState.Sleep:
            {
                DisableSleep();
            }
            break;

            case EPlayerInteractionState.SetLevel:
            {
                DisableSetLevel();
            }
            break;

            case EPlayerInteractionState.MoveToOtherLevel:
            {
                DisableMoveToOtherLevel();
            } 
            break;
        } 
    } 
       
    public void EnterItemBox()
    {
        interactionState = EPlayerInteractionState.ItemBox;
        cachedPlayerController.ChangeUIAndLookDetection();
        cachedUIController.EnterItemBox(); 
    } 
    public void EnterMenu()
    {
        interactionState = EPlayerInteractionState.Menu;
        cachedPlayerController.ChangeUI();
        cachedUIController.EnterMenu(); 
    }  
    public void EnterAdorn()
    {
        interactionState = EPlayerInteractionState.Adorn;
        cachedPlayerController.ChangeUI();
         
        var info = new FRadiuseCollaspeInfo();
        info.radType = ERadiuseCollaspeType.Small_Clear_Bigger;
        info.startPosA = new Vector2(0.5f, 0.5f);
        info.startPosB = new Vector2(0.5f, 0.5f);
        info.clearAction = AdornEnterAction;   
        cachedUIController.EnterAdorn(info); 
    }     
    public void EnterSleep()
    {
        interactionState = EPlayerInteractionState.Sleep;
        cachedPlayerController.ChangeUIAndLookDetection();
        cachedUIController.EnterSleepCanvas(); 
    } 
    public void EnterSetLevel()
    {
        interactionState = EPlayerInteractionState.SetLevel; 
        cachedPlayerController.ChangeUIAndLookDetection();
          
        var info = new FRadiuseCollaspeInfo();
        info.radType = ERadiuseCollaspeType.Small_Clear_Bigger;
        info.startPosA = new Vector2(0.5f, 0.5f);
        info.startPosB = new Vector2(0.5f, 0.5f);
        info.clearAction = EnterSetLevelAction;
        cachedUIController.EnterSetLevel(info); 
    }     
    public void EnterDeadAndMoveHome(string _duckName, string _weaponName)
    {
		interactionState = EPlayerInteractionState.MoveToOtherLevel;
        cachedPlayerController.ChangeUI();

		var info = new FRadiuseCollaspeInfo();
		info.radType = ERadiuseCollaspeType.Smaller_Clear;
		info.transformA = transform;
		info.clearAction = () => DeadAndMoveHome(_duckName, _weaponName);
		cachedUIController.EnterMoveToLevel(info); 
	}
	public void EnterMoveToOtherLevel(ELevelType _moveLevel, ELoadingScreenType _screenType)
    {
        // 무적판정 해주자
        cachedPlayerStat.ActiveInvincibility(true);

        // 해당 레벨로 이동해줘야함 
        interactionState = EPlayerInteractionState.MoveToOtherLevel; 
        cachedPlayerController.ChangeUIAndLookDetection();

        var info = new FRadiuseCollaspeInfo();
        info.radType = ERadiuseCollaspeType.Smaller_Clear;
        info.transformA = transform;
        info.clearAction = () => MoveToOtherLevelAction(_moveLevel, _screenType);
        cachedUIController.EnterMoveToLevel(info);
    }   
      
    public void EnterStore(Store _store) 
    {  
        interactionState = EPlayerInteractionState.Store;
        cachedPlayerController.ChangeUIAndLookDetection();
        cachedUIController.EnterStore(_store);
    }    
    public void EnterWareHouse(WareHouse _wareHouse)
    {
        interactionState = EPlayerInteractionState.WareHouse;
        cachedPlayerController.ChangeUIAndLookDetection();
        cachedUIController.EnterWareHouse(_wareHouse);
    } 
    public void EnterQuest(HandleQuest _handleQuest)
    {
        interactionState = EPlayerInteractionState.Quest;
        cachedPlayerController.ChangeUIAndLookDetection();
        cachedUIController.EnterQuest(_handleQuest);
    }  
    public void SuccesSleep(float _time)
    {
        var info = new FRadiuseCollaspeInfo();
        info.radType = ERadiuseCollaspeType.Small_Clear_Bigger;
        info.startPosA = new Vector2(0.5f, 0.5f);
        info.transformB = transform; 
        info.clearAction = () => SuccesSleepAction(_time);
        cachedUIController.ActiveCollaspeCanvas(info);
    }   

    public void EnterESC()
    {
        interactionState = EPlayerInteractionState.ESC;
        cachedPlayerController.ChangeUI();
        cachedUIController.EnterStopCanvas(); 
    } 
     
    private void DisableItemBox()
    {
        EndToInteraction();

        interactionState = EPlayerInteractionState.None;
        cachedUIController.DisableItemBox();

    }
    private void DisableESC()
    {
        interactionState = EPlayerInteractionState.None;
        cachedPlayerController.ChangePlay();
        cachedUIController.DisableStopCanvas();
    }  

    private void DisableMenu()
    {
        interactionState = EPlayerInteractionState.None;
        cachedUIController.DisableMenu();
    }
    private void DisableAdorn()
    {
        interactionState = EPlayerInteractionState.None;
        EndToInteraction();
         
        // 카메라 다시 세팅 해주기
        var gi = GameInstance.Instance;
        gi.CAMERA_ActievMainCamera(EDuckCameraType.BillboardCam, true);
        gi.CAMERA_ActiveCinemachinCamera(EPlayCameraType.TopView);
        
        // 랜더러 보이게끔
        cachedDuckDetected.ActiveDuckRenderer();
        cachedUIController.DisableAdorn();
    }
    private void DisableStore()
    {
        EndToInteraction();
        interactionState = EPlayerInteractionState.None;
        cachedUIController.DisableStore();
    }
    private void DisableSleep()
    {
        EndToInteraction();
        interactionState = EPlayerInteractionState.None;
        cachedUIController.DisableSleepCanvas();
    } 
    private void DisableWareHouse()
    {
        EndToInteraction();

        interactionState = EPlayerInteractionState.None;
        cachedUIController.DisableWareHouse();
    } 
    private void DisableQuest()
    {
        EndToInteraction();

        interactionState = EPlayerInteractionState.None;
        cachedUIController.DisableQuest();
    }
    private void DisableSetLevel()
    { 
        EndToInteraction();
          
        interactionState = EPlayerInteractionState.None;
        cachedUIController.DisableSetLevel();
    }   
    private void DisableMoveToOtherLevel()
    {
        // 무적판정 없애주자 
        cachedPlayerStat.ActiveInvincibility(false);

        // 레벨 전환 후라 Detect타겟은 없어질것임
        interactionState = EPlayerInteractionState.None;
        cachedUIController.HUD_AcitveHUDCanvas(true);
        // EndToInteraction(); 
    }
     
    private void EndToInteraction()
    { 
        DetectionTarget detectedTarget = cachedPlayerDetector.GetDetectionTarget();
        detectedTarget?.EndInteractionToPlayer();
    } 
    private void AdornEnterAction()
    {
        var gameInstance = GameInstance.Instance;
        gameInstance.CAMERA_ActievMainCamera(EDuckCameraType.BillboardCam, false);
                
        // 모든 Renderer를 숨겨줘야함
        cachedDuckDetected.DisableDuckRenderer();
        cachedUIController.ActiveAdornCanvas();
    }
     
	private void DeadAndMoveHome(string _duckName, string _weapon)
	{  
        cachedUIController.EnterSallyCanvas(_duckName , _weapon, () => GameInstance.Instance.LOADING_ChangeNextLevel(ELevelType.Home, ELoadingScreenType.Dead));
	}  
	private void MoveToOtherLevelAction(ELevelType _moveLevel, ELoadingScreenType _lodingScreen)
    { 
        // 집으로 이동
        if (_moveLevel == ELevelType.Home)
        {
            // 철수 성공
            if (_lodingScreen == ELoadingScreenType.Withdraw)
            {
                cachedUIController.EnterSallyCanvas(() => ChangeNextLevel(_moveLevel, _lodingScreen));
            }  
        }   
          
        // 즉시 전환
        else
        {  
            ChangeNextLevel(_moveLevel, _lodingScreen);
        } 
    }

    private void SuccesSleepAction(float _time)
    {
        GetComponent<PlayerTime>().SetGameTime(_time);
         
        var tp = GameInstance.Instance.MAP_GetHouseRspawnTP();
        GetComponent<PlayerSpawn>().SpawnInPos(tp); 
    }   
    private void EnterSetLevelAction()
    {
        cachedUIController.ActiveSetLevelCanvas();
    } 
     
    // private void Success
    private void ChangeNextLevel(ELevelType _type, ELoadingScreenType _screen)
    {
        cachedUIController.InstantActiveDisableCanvas();
        cachedUIController.CURSOR_Disable();
        GameInstance.Instance.LOADING_ChangeNextLevel(_type, _screen);
    }  
}  
    