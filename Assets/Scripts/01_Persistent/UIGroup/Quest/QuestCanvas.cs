using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum EQuestDomainButtonType
{
    Acceptable,
    InProgress,
    Complate,
    End,
}
public class QuestCanvas : MonoBehaviour
{ 
    [SerializeField] private Button[] domainButtons = new Button[(int)EQuestDomainButtonType.End];
    [SerializeField] private RectTransform questListRT;
    [SerializeField] private GameObject questSubButtonPrefab;
    [SerializeField] private QuestSubDesc questSubDesc;
    [SerializeField] private Button accpetOrComplateButton;
    [SerializeField] private Button submitButton; 
     
    [SerializeField] private Color activeColor = Color.black;
    [SerializeField] private Color disableColor = Color.black; 
      
    private HandleQuest cachedHandleQuest;
      
    private EDuckType activingDuckType = EDuckType.End;
    private EQuestDomainButtonType curPressButton = EQuestDomainButtonType.End;
    private EQuestID curPressSubButton = EQuestID.End;
    private Image curSelectSubButtonImage = null;
      
    private void Awake() 
    {
        BindButton();
        Disable(); 
    } 
        
    public void ActiveInprogressAndComplate()
    {
        gameObject.SetActive(true);
         
        activingDuckType = EDuckType.End;

        // Acceptable 숨김
        domainButtons[(int)EQuestDomainButtonType.Acceptable].gameObject.GetComponent<QuestDomainButton>()?.Disable();

        var InprogressButton = domainButtons[(int)EQuestDomainButtonType.InProgress].gameObject.GetComponent<QuestDomainButton>();
        var ComplateButton = domainButtons[(int)EQuestDomainButtonType.Complate].gameObject.GetComponent<QuestDomainButton>();

        InprogressButton?.Active(); 
        ComplateButton?.Active();

        InprogressButton?.SetButtonWidth(227.5f);
        ComplateButton?.SetButtonWidth(227.5f); 

        // 서브 버튼들을 만들기
        PressDomainButton(EQuestDomainButtonType.InProgress); 
    }   
    public void ActiveAllDomainButton(HandleQuest _handleQuest)
    {
        gameObject.SetActive(true);

        cachedHandleQuest = _handleQuest;

        activingDuckType = cachedHandleQuest.GetQuestGiverDuckType();

        var accpetButton = domainButtons[(int)EQuestDomainButtonType.Acceptable].gameObject.GetComponent<QuestDomainButton>();
        var InprogressButton = domainButtons[(int)EQuestDomainButtonType.InProgress].gameObject.GetComponent<QuestDomainButton>();
        var ComplateButton = domainButtons[(int)EQuestDomainButtonType.Complate].gameObject.GetComponent<QuestDomainButton>();

        accpetButton?.Active();
        InprogressButton?.Active();
        ComplateButton?.Active();
         
        accpetButton?.SetButtonWidth(150f);
        InprogressButton?.SetButtonWidth(150f);
        ComplateButton?.SetButtonWidth(150f);

        PressDomainButton(EQuestDomainButtonType.Acceptable);
    } 
    public void Disable()
    {
        ClearSubDesc();

        activingDuckType = EDuckType.End;    
        curPressButton = EQuestDomainButtonType.End;
        cachedHandleQuest = null;
        ClearSubButtons();
        gameObject.SetActive(false);
    }
     
    private void BindButton()
    {
        domainButtons[(int)EQuestDomainButtonType.Acceptable].onClick.AddListener(() => PressDomainButton(EQuestDomainButtonType.Acceptable));
        domainButtons[(int)EQuestDomainButtonType.InProgress].onClick.AddListener(() => PressDomainButton(EQuestDomainButtonType.InProgress));
        domainButtons[(int)EQuestDomainButtonType.Complate].onClick.AddListener(() => PressDomainButton(EQuestDomainButtonType.Complate));
        accpetOrComplateButton.onClick.AddListener(() => PressAcceptOrComplateButton());
        submitButton.onClick.AddListener(() => PressSubmitButton());
    }
    private void PressDomainButton(EQuestDomainButtonType _type)
    {
        if (curPressButton == _type)
            return;
         
        for (int i = 0; i < domainButtons.Length; i++)
        {
            bool isActive = ((EQuestDomainButtonType)i == _type);
            var targetImage = domainButtons[i].GetComponent<Image>();
            targetImage.color = isActive ? activeColor : disableColor;
        }
        curPressButton = _type;
        RenewButton(curPressButton); 
    }
    private void PressSubButton(EQuestID _id, Image _image)
    {
        if (curPressSubButton == _id)
            return;
         
        if (curSelectSubButtonImage)
        {
            curSelectSubButtonImage.color = disableColor;
        }

        curSelectSubButtonImage = _image;
        curSelectSubButtonImage.color = activeColor;
        curPressSubButton = _id;

        var questData = GameInstance.Instance.TABLE_GetQuestData(_id);
        string duckName = GameInstance.Instance.TABLE_GetDuckName(questData.gaveQuestDuck);
        questSubDesc.Active(curPressButton, questData.questName, duckName, questData.questDesc);

        questSubDesc.InsertRewardExp(questData.reward.exp);
        questSubDesc.InsertRewardMoney(questData.reward.money);
        foreach (var reward in questData.reward.items)
        {
            questSubDesc.InsertRewardItem(reward.count, reward.itemID);
        }

        RenewSubButton(curPressButton);

        ActiveAccpetOrComplateButton(curPressButton);
        ActiveSubmitButton(curPressButton); 
    }
    private void PressAcceptOrComplateButton()
    { 
        if (curPressButton == EQuestDomainButtonType.Complate)
            return;
          
        var playerQeust = GameInstance.Instance.PLAYER_GetPlayerQuest();
        if (curPressButton == EQuestDomainButtonType.Acceptable)
        {
            cachedHandleQuest?.RemoveQuestID(curPressSubButton);
            playerQeust?.InsertQuest(EQuestDomainButtonType.InProgress, curPressSubButton);
        } 

        else if (curPressButton == EQuestDomainButtonType.InProgress)
        {
            if (!playerQeust.IsComplateCondition(curPressSubButton))
                return;  
             
            // 보상을 획득 시켜줘야함
            var questData = GameInstance.Instance.TABLE_GetQuestData(curPressSubButton);
            playerQeust?.GainRewardExp(questData.reward.exp);
            playerQeust.GainRewardMoney(questData.reward.money);
            foreach (var itemInfo in questData.reward.items)
            {
                playerQeust?.GainRewardItem(itemInfo.itemID, itemInfo.count);
            }
            playerQeust?.InsertQuest(EQuestDomainButtonType.Complate, curPressSubButton);
        }  
         
        RenewButton(curPressButton);
    }
    private void PressSubmitButton() 
    {
        var playerQeust = GameInstance.Instance.PLAYER_GetPlayerQuest();
        if (playerQeust.CheckInItemInStoreByDeliverItem(curPressSubButton))
        {
            questSubDesc.ClearObjectiveInfo();
            RenewSubButton(EQuestDomainButtonType.InProgress);
            submitButton.GetComponent<UIAnimation>()?.Action_Animation();
        } 
    }  
    private void MakeSubButtons(EQuestDomainButtonType _type)
    {
        ClearSubButtons();
         
        var playerQeust = GameInstance.Instance.PLAYER_GetPlayerQuest();
        switch (_type)
        {
            case EQuestDomainButtonType.Acceptable:
                {
                    CreateSubButtons(cachedHandleQuest.GetAcceptableQusetList());
                }  
                break; 

            case EQuestDomainButtonType.InProgress:
                {
                    CreateSubButtons(playerQeust.GetInProgressList());
                }
                break;
                 
            case EQuestDomainButtonType.Complate:
                {
                    CreateSubButtons(playerQeust.GetComplateList());
                }
                break;
        }
    } 
    private void ClearSubButtons()
    {
        // 기존 버튼 제거
        foreach (Transform child in questListRT)
            Destroy(child.gameObject);
    }
    private void CreateSubButtons(List<EQuestID> questList)
    {
        if (questList == null || questList.Count == 0)
            return;

        foreach (var questID in questList)
        {
            var go = Instantiate(questSubButtonPrefab, questListRT);
            var subBtn = go.GetComponent<QuestSubButton>();
            subBtn.Init(questID, PressSubButton);
        }  
    }
    private void ClearSubDesc()
    {
        curSelectSubButtonImage = null; 
        questSubDesc.Disable();
        curPressSubButton = EQuestID.End;
    } 
    private void ActiveAccpetOrComplateButton(EQuestDomainButtonType _type)
    {
        if (activingDuckType == EDuckType.End || !DuckUtill.IsMatch_NpcBetweenQeust(curPressSubButton, activingDuckType)) 
        {
            accpetOrComplateButton.gameObject.SetActive(false);
            return;
        }
 
        if (_type == EQuestDomainButtonType.Acceptable)
        {
            accpetOrComplateButton.gameObject.SetActive(true);
            var textMesh = accpetOrComplateButton.GetComponentInChildren<TextMeshProUGUI>();
            textMesh.text = "수락"; 
        }
         
        else if (_type == EQuestDomainButtonType.InProgress)
        {
            accpetOrComplateButton.gameObject.SetActive(true);
            var textMesh = accpetOrComplateButton.GetComponentInChildren<TextMeshProUGUI>();
            textMesh.text = "완료";
        }  

        else if (_type == EQuestDomainButtonType.Complate)
        {
            accpetOrComplateButton.gameObject.SetActive(false);
        }
    } 
    private void ActiveSubmitButton(EQuestDomainButtonType _type)
    {
        if (activingDuckType == EDuckType.End || !DuckUtill.IsMatch_NpcBetweenQeust(curPressSubButton, activingDuckType))
        {
            submitButton.gameObject.SetActive(false);
            return;
        }

        if (_type == EQuestDomainButtonType.InProgress)
        {
            submitButton.gameObject.SetActive(true);
        }
         
        else
        {
            submitButton.gameObject.SetActive(false);
        }
    }
     
    private void RenewButton(EQuestDomainButtonType _type)
    {
        MakeSubButtons(_type);
        ClearSubDesc();
        accpetOrComplateButton.gameObject.SetActive(false);
        submitButton.gameObject.SetActive(false);
    } 
    private void RenewSubButton(EQuestDomainButtonType _type)
    {
        var questData = GameInstance.Instance.TABLE_GetQuestData(curPressSubButton);
         
        switch (curPressButton)
        {
            case EQuestDomainButtonType.Acceptable:
                {
                    questSubDesc.ActiveObjective(true);

                    foreach (var objective in questData.listObjective)
                    {
                        if (objective.ObjectiveType == EObjectiveType.Deliver)
                        {
                            if (objective is DeliverQuestObjectiveData deliver)
                            {
                                questSubDesc.InsertDeliverObjective(deliver.RequiredCount, deliver.ItemID);
                            }
                        }

                        else if (objective.ObjectiveType == EObjectiveType.Kill)
                        {
                            if (objective is KillQuestObjectiveData kill)
                            {
                                questSubDesc.InsertKillObjective(kill.IsHeadShot, kill.RequiredCount, kill.TargetDuckType);
                            }
                        }
                    }
                }
                break;  

            case EQuestDomainButtonType.InProgress:
                {
                    questSubDesc.ActiveObjective(true);

                    var playerQeust = GameInstance.Instance.PLAYER_GetPlayerQuest();
                    QuestInstance questInstance = playerQeust.GetQuestInstance(curPressSubButton);
                    List<FQuestDeliverRuntime> listDeliverRunTime = questInstance.GetDeliverRuntimeData();
                    List<FQuestKillRuntime> listKillRunTime = questInstance.GetKillRuntimeData();

                    foreach (var runTime in listDeliverRunTime)
                    {
                        questSubDesc.InsertDeliverObjective(runTime.curCount, runTime.maxCount, runTime.itemID);
                    }
                    foreach (var runTime in listKillRunTime)
                    {
                        questSubDesc.InsertKillObjective(runTime.isHead, runTime.curCount, runTime.maxCount, runTime.duckType);
                    }
                }
                break;

            case EQuestDomainButtonType.Complate:
                {
                    questSubDesc.ActiveObjective(false);
                }
                break;
        }
    }
} 
 