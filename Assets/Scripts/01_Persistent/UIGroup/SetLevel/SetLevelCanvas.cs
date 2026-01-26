using TMPro;
using UnityEngine; 
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class SetLevelCanvas : MonoBehaviour
{  
    [SerializeField] SetLevelButton easyButton;  
    [SerializeField] SetLevelButton hardButton;
    [SerializeField] SetLevelButton survivalButton;
    [SerializeField] SetLevelButton limitButton;
    [SerializeField] SetLevelButton impossibilityButton;
    [SerializeField] Button confirmButton;
    [SerializeField] TMP_Text domainDescPanel;
 
    [SerializeField] private Color baseColor;
    [SerializeField] private Color activeColor;
    [SerializeField] private Color disableColor;

    private SetLevelButton[] cachedDomainButtons;
    private LocalizeStringEvent domainLocalizeDesc;
     
    private EPlayDifficultType curSelect = EPlayDifficultType.End;
      
    // 내일 여기 리펙토링 하자 - 버튼 색상 쪽 부터 시작해야함 
    private void Awake()
    { 
        cachedDomainButtons = new SetLevelButton[(int)EPlayDifficultType.End]
        {
               easyButton,
               hardButton,
               survivalButton,
               limitButton,
               impossibilityButton
        };
        domainLocalizeDesc = domainDescPanel.GetComponent<LocalizeStringEvent>();
        BindButton();
        Disable(); 
    }
            
    public void Active()
    {    
        gameObject.SetActive(true);
        RenewDomainButtons();
        ///
        var playerDifficult = GameInstance.Instance.PLAYER_GetPlayerDifficult();
        PressDomainButton(playerDifficult.GetCurDifficult()); 
    }  
    public void Disable()
    {
        gameObject?.SetActive(false);
        curSelect = EPlayDifficultType.End; 
    }  
     
    public void PressDomainButton(EPlayDifficultType _type)
    {
        // 애니메이션
        int buttonIndex = (int)_type;
        var targetButton = cachedDomainButtons[buttonIndex];
        bool isTargetLock = targetButton.IsLock();
        EUIAnimationType animType = isTargetLock ? EUIAnimationType.ShakeHorizontal : EUIAnimationType.GrewBigger;
        ActionAnimationButton(_type, animType);

        if (isTargetLock)
            return;
         
        // 색 및 정보 갱신 
        if (curSelect != _type)
        {
            // 이전 버튼 갱신
            if (curSelect != EPlayDifficultType.End)
            {
                int prev = (int)curSelect;
                cachedDomainButtons[prev].SetColor(baseColor);
            } 
             
            // 현재 버튼 갱신
            curSelect = _type;
            int cur = (int)curSelect;
            cachedDomainButtons[(int)curSelect].SetColor(activeColor);
            RenewSubDesc(curSelect);
        }
    }
    public void PressConfirmButton()
    {
        var gameInstance = GameInstance.Instance;
        var uiGroup = gameInstance.UI_GetPersistentUIGroup();
        var canvas = uiGroup.GetRadiusCollaspeCanvas();
         
        FRadiuseCollaspeInfo data = new();
        data.radType = ERadiuseCollaspeType.Small_Clear_Bigger;
        data.startPosA = new Vector2(0.5f, 0.5f); 
        data.transformB = gameInstance.PLAYER_GetPlayerTransform();
        data.clearAction = ConfirmButtonAction;
        canvas.Active(data);
    }  

    private void BindButton() 
    {
        // Domain
        {
            for (int i = 0; i < cachedDomainButtons.Length; i++)
            {
                int index = i; // 로컬 캡처 방지
                cachedDomainButtons[i].GetComponent<Button>().onClick.RemoveAllListeners();
                cachedDomainButtons[i].GetComponent<Button>().onClick.AddListener(() =>
                { 
                    PressDomainButton((EPlayDifficultType)index);
                });
            }
        }

        // Confir
        {
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(PressConfirmButton);
        }
    }
    private void RenewDomainButtons()
    {
        var playerDifficult = GameInstance.Instance.PLAYER_GetPlayerDifficult();
        EPlayDifficultFlag flags = playerDifficult.GetCurPlayDifficultFlag();
          
        for (int i = 0; i < cachedDomainButtons.Length; i++)
        {
            var button = cachedDomainButtons[i];
            var type = (EPlayDifficultType)i;
            bool unlocked = flags.Has(type);
             
            button.ActiveLock(!unlocked);
            button.SetColor(unlocked ? baseColor : disableColor);
        }
    }
    private void RenewSubDesc(EPlayDifficultType _type)
    {
        string entryKey = $"{_type}Desc"; // EasyDesc, HardDesc, ...
        var localized = domainLocalizeDesc.GetComponent<LocalizeStringEvent>();
        if (localized != null)
        {
            localized.StringReference.TableReference = "Difficult";     
            localized.StringReference.TableEntryReference = entryKey;   
            localized.RefreshString();
        }
    } 

    private void ActionAnimationButton(EPlayDifficultType _type, EUIAnimationType _anim)
    {
        var targetButton = cachedDomainButtons[(int)_type];
        targetButton.GetComponent<UIAnimation>().Action_Animation(_anim);
    } 
    private void ConfirmButtonAction()
    {
        var gameInstance = GameInstance.Instance;
        var playerDifficult = gameInstance.PLAYER_GetPlayerDifficult();
        var playerSave = gameInstance.PLAYER_GetPlayerSave();
        var playerController = gameInstance.PLAYER_GetPlayerController();

        playerDifficult.SetCurDifficultType(curSelect);
        playerSave.SaveDifficult();
        if (curSelect != EPlayDifficultType.Easy)
        {
            playerSave.ClearAllSaveChickenPrefab();
        }
        playerController.ChangePlay();
    }  
}  
  