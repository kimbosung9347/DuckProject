using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;
using Unity.VisualScripting;

public class SaveCanvas : MonoBehaviour
{
    [SerializeField] private MainmenuUiGroup menuUiGroup;
     
    [SerializeField] private Button backButton;
    [SerializeField] private Button saveButton1;
    [SerializeField] private Button saveButton2;
    [SerializeField] private Button saveButton3; 

    [SerializeField] private Color saveButtonBaseColor;
    [SerializeField] private Color saveButtonSelectColor;
    [SerializeField] private SaveSlotDesc saveSlotDesc;
      
    private LobbyData cachedLobbyData;
    private Button[] cachedSaveButtons;
    private int curSelectIndex = -1;  

    private void Awake()
    {
        cachedSaveButtons = new Button[] { saveButton1, saveButton2, saveButton3 };
        cachedLobbyData = GameInstance.Instance.SAVE_GetLobbyData(); 
        curSelectIndex = cachedLobbyData.selectedSaveSlotIndex;
        BindActionBackButton();
        BindActionSaveButton();
        RenewSubDesc(); 
        Disable();
    }   

    public void Active()
    {
        gameObject.SetActive(true);
        GetComponent<UIAnimation>()?.Action_Animation();
         
        //
        curSelectIndex = cachedLobbyData.selectedSaveSlotIndex;
        PressSelectSaveSlot(curSelectIndex);
        RenewSubDesc(); 
    } 
    public void Disable()
    {
        gameObject.SetActive(false);
    } 
     
    public void PressGoBack()
    { 
        Disable();
        menuUiGroup.GetLobbyCanvas().Active();
    }      
    public void PressSelectSaveSlot(int _index)
    {
        if (_index == -1) 
            return;

        if (curSelectIndex == _index)
            return;
        
        if (curSelectIndex != -1)
            cachedSaveButtons[curSelectIndex].GetComponent<Image>().color = saveButtonBaseColor;
          
        curSelectIndex = _index;
        cachedLobbyData.SetSelectSaveSlotIndex(curSelectIndex);
        RenewSubDesc();
        cachedSaveButtons[curSelectIndex].gameObject.GetComponent<UIAnimation>()?.Action_Animation();
    }      
          
    private void BindActionBackButton()
    {
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(PressGoBack);
    }
    private void BindActionSaveButton()
    {
        for (int i = 0; i < cachedSaveButtons.Length; i++)
        {
            int idx = i;
            var button = cachedSaveButtons[i];

            // 기존 리스너 초기화
            button.onClick.RemoveAllListeners(); 
            button.onClick.AddListener(() => PressSelectSaveSlot(idx));
             
            // LocalizedStringEvent 처리
            var localizedEvent = button.GetComponentInChildren<LocalizeStringEvent>();
            if (localizedEvent != null) 
            {
                // Smart Format 파라미터 지정 — {0} 자리에 들어갈 값
                localizedEvent.StringReference.Arguments = new object[] { idx + 1 };
                localizedEvent.RefreshString();
            } 
        }  
    } 
     
    private void RenewSubDesc()
    {
        if (curSelectIndex == -1)
        { 
            saveSlotDesc.Disable();
        }

        else
        {
            // 버튼 색 갱신 
            cachedSaveButtons[curSelectIndex].GetComponent<Image>().color = saveButtonSelectColor;
             
            // 플레이 데이터 갱신
            PlayData playData = GameInstance.Instance.SAVE_GetPlatData(curSelectIndex);
            if (playData.bIsEmpty)
            {
                saveSlotDesc.Disable();
            }
             
            else
            {
                float playTime = playData.characterData.playTime;
                EPlayDifficultType type = playData.difficultData.CurrentDifficult;
                saveSlotDesc.Active(type, playTime); 
            } 
        }
    } 
}   
   