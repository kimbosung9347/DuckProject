using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;
using UnityEngine.EventSystems;
 
public class LobbyCanvas : MonoBehaviour
{
    [SerializeField] private MainmenuUiGroup cachedMenuCanvas;
     
    [SerializeField] private Button ContinueOrNewButton;
    [SerializeField] private Button SaveSlotButton;
    [SerializeField] private Button DelateSlotButton;
    [SerializeField] private Button SettingButton; 
    [SerializeField] private Button EndGameButton;

    private LobbyData cachedLobbyData;
    private DelateSaveSlot cachedFillProgress;
    private LocalizeStringEvent continueOrNewLocalized;
    private LocalizeStringEvent saveSlotLocalized;
     
    private void Awake()
    {
        cachedFillProgress = DelateSlotButton.GetComponent<DelateSaveSlot>();
        cachedFillProgress.CacheLobbyCanvas(this);
         
        var disableCanvas = GameInstance.Instance.UI_GetPersistentUIGroup().GetDisableCanvas();
        disableCanvas?.DisableInstant(); 
             
        cachedLobbyData = GameInstance.Instance.SAVE_GetLobbyData();
        continueOrNewLocalized = ContinueOrNewButton.GetComponentInChildren<LocalizeStringEvent>();
        saveSlotLocalized = SaveSlotButton.GetComponentInChildren<LocalizeStringEvent>();

        BindActionContinueOrNewButton();
        BindActionSaveSlotButton();
        BindActionDelateSlotButton();
        BindActionEndGameButton();
        BindSettingButton();

        gameObject.SetActive(false);
    }
    private void Start()
    {
        GameInstance.Instance.SOUND_PlayBGM(EBgmType.DuckFunk);
    } 

    public void Active()
    {
        gameObject.SetActive(true); 
        GetComponent<UIAnimation>()?.Action_Animation();
        RenewContinueOrNewButton();
        RenewCurSelectedSaveSlot();
    }
    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void DelateSaveSlot()
    { 
        ContinueOrNewButton.GetComponent<UIAnimation>().Action_Animation();

        RenewContinueOrNewButton();
        RenewCurSelectedSaveSlot();
    } 
     
    /* === Actions === */
    private void PressNewGameOrContinue()
    {
        if (cachedLobbyData.selectedSaveSlotIndex == -1)
        {
            cachedLobbyData.SetSelectSaveSlotIndex(0);
        } 

        // Level전환해줘야함
        var gameInstance = GameInstance.Instance;
        var radiusCollCanvas = gameInstance.UI_GetPersistentUIGroup().GetRadiusCollaspeCanvas();
        var info = new FRadiuseCollaspeInfo();
         
        info.startPosA = new Vector2(0.5f, 0.5f);
        info.radType = ERadiuseCollaspeType.Smaller_Clear;
        info.clearAction = ChangeToHome;
 
        radiusCollCanvas.Active(info);
    }
    private void ChangeToHome()
    {
        Disable(); 

        var cam = Camera.main;
        if (cam) 
        {
            cam.enabled = false;
        } 
         
        cachedLobbyData.Save();
         
        var gameInstance = GameInstance.Instance;
        gameInstance.UI_GetPersistentUIGroup().GetDisableCanvas().ActiveInstant();
        var cursorCanvas = gameInstance.UI_GetPersistentUIGroup().GetCursorCanvas();
        cursorCanvas.GetUICursor().gameObject.SetActive(false);
        cursorCanvas.GetPlayCursor().gameObject.SetActive(false);
         
        gameInstance.LOADING_ChangeNextLevel(ELevelType.Home, ELoadingScreenType.LobbyToHome);
        gameInstance.SOUND_StopBGM(); 
    }  
     
    private void PressSaveSlot()
    { 
        Disable();
        cachedMenuCanvas.GetSaveCanvas().Active(); 
    }   
    private void PressEndGame()
    {
        Application.Quit();
    } 
    private void PressSetting()
    {
        GameInstance.Instance.UI_GetPersistentUIGroup().GetSettingCanvas().Active(this);
        Disable();  
    }
     
    /* === Bind === */
    private void BindActionContinueOrNewButton()
    {
        ContinueOrNewButton.onClick.RemoveAllListeners();
        ContinueOrNewButton.onClick.AddListener(PressNewGameOrContinue);
    }
    private void BindActionSaveSlotButton()
    {
        SaveSlotButton.onClick.RemoveAllListeners();
        SaveSlotButton.onClick.AddListener(PressSaveSlot);
        RenewCurSelectedSaveSlot();
    }
    private void BindActionDelateSlotButton()
    {
        DelateSlotButton.onClick.RemoveAllListeners();

        // IPointerDown / IPointerUp 처리용 이벤트 트리거 추가
        var trigger = DelateSlotButton.gameObject.AddComponent<EventTrigger>(); 

        var pointerDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        pointerDown.callback.AddListener(_ => cachedFillProgress.Active(true));
        trigger.triggers.Add(pointerDown);

        var pointerUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        pointerUp.callback.AddListener(_ => cachedFillProgress.Active(false));
        trigger.triggers.Add(pointerUp);
    }
    private void BindSettingButton()
    {
        SettingButton.onClick.RemoveAllListeners();
        SettingButton.onClick.AddListener(PressSetting);
    }
    private void BindActionEndGameButton()
    {
        EndGameButton.onClick.RemoveAllListeners();
        EndGameButton.onClick.AddListener(PressEndGame);
    } 
  
      
    /* === Renew === */
    private void RenewContinueOrNewButton()
    {
        int curSelectIndex = cachedLobbyData.selectedSaveSlotIndex;

        bool isNewGame = true;
        if (curSelectIndex != -1)
        {
            PlayData playData = GameInstance.Instance.SAVE_GetPlatData(curSelectIndex);
            if (playData.bIsEmpty == false)
            {
                isNewGame = false;
            }
        }
         
        if (isNewGame)
        {
            // Mainmenu/NewGame
            if (continueOrNewLocalized != null)
            {
                continueOrNewLocalized.StringReference.TableReference = "Mainmenu";
                continueOrNewLocalized.StringReference.TableEntryReference = "NewGame";
                continueOrNewLocalized.RefreshString();
            }
        }
        else
        {
            // Mainmenu/Continue
            if (continueOrNewLocalized != null)
            {
                continueOrNewLocalized.StringReference.TableReference = "Mainmenu";
                continueOrNewLocalized.StringReference.TableEntryReference = "Continue";
                continueOrNewLocalized.RefreshString();
            }
        }  
    }
    private void RenewCurSelectedSaveSlot()
    {
        if (!saveSlotLocalized)
            return;

        if (cachedLobbyData.selectedSaveSlotIndex == -1)
        {
            saveSlotLocalized.StringReference.SetReference(
                "Mainmenu",
                "ChoiceSave"
            );
            saveSlotLocalized.StringReference.Arguments = null;
            saveSlotLocalized.RefreshString();
            return;
        }

        saveSlotLocalized.StringReference.SetReference(
            "Mainmenu",
            "Save"
        );
        saveSlotLocalized.StringReference.Arguments = new object[]
        {
            cachedLobbyData.selectedSaveSlotIndex + 1
        };
        saveSlotLocalized.RefreshString();
    }
}
