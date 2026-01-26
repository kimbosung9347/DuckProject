using System.Runtime.ConstrainedExecution;
using TMPro;
using UnityEngine;

public class QuestSubDesc : MonoBehaviour
{
    [SerializeField] private UIAnimation uiAnimation;

    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private TextMeshProUGUI questMasterName;
    [SerializeField] private TextMeshProUGUI questDesc;

    [SerializeField] private RectTransform objectiveRT; 
    [SerializeField] private GameObject objectivePrefab;

    [SerializeField] private RectTransform rewardRT;
    [SerializeField] private GameObject rewardPrefab;
       
    public void Active(EQuestDomainButtonType _type, string _questName, string _masterName, string _questDesc)
    {
        gameObject.SetActive(true);
         
        questName.text = _questName;
        questMasterName.text = _masterName;
        questDesc.text = _questDesc;
         
        uiAnimation?.Action_Animation();
    } 
    public void ActiveObjective(bool _isActive)
    {
        objectiveRT.gameObject.SetActive(_isActive);
    } 
    public void Disable()
    {
        ClearChildren(objectiveRT); 
        ClearChildren(rewardRT);

        gameObject.SetActive(false);
    } 
     
    public void InsertDeliverObjective(int _cur, int _max, EItemID _id)
    {
        // Prefab 인스턴스화 + 부모 지정
        var go = Instantiate(objectivePrefab, objectiveRT);
        var questObjective = go.GetComponent<UIQuestObjective>();

        var pairData = DuckUtill.GetItemPair(_id);

        // 아이템 이름 (cur/max)
        string text = $"{pairData.data.itemName} ({_cur}/{_max})";
        bool isCompleted = (_cur >= _max);

        questObjective.ActiveCheckImage(isCompleted);
        questObjective.ActiveSpriteImage(true);
        questObjective.RenewSpriteImage(pairData.visual.iconSprite);
        questObjective.RenewObjectiveText(text);
    }
    public void InsertDeliverObjective(int _max, EItemID _id)
    {
        // Prefab 인스턴스화 + 부모 지정
        var go = Instantiate(objectivePrefab, objectiveRT);
        var questObjective = go.GetComponent<UIQuestObjective>();

        var pairData = DuckUtill.GetItemPair(_id);

        // 아이템 이름 (cur/max)
        string text = $"{pairData.data.itemName} ({_max})";
          
        questObjective.ActiveCheckImage(false);
        questObjective.ActiveSpriteImage(true);
        questObjective.RenewSpriteImage(pairData.visual.iconSprite);
        questObjective.RenewObjectiveText(text);
    }

    public void InsertKillObjective(bool _isHead, int _cur, int _max, EDuckType _duckType)
    {
        var go = Instantiate(objectivePrefab, objectiveRT);
        var questObjective = go.GetComponent<UIQuestObjective>();

        string duckName = GameInstance.Instance.TABLE_GetDuckName(_duckType);

        if (_isHead)
        {
            duckName += " 헤드샷";
        } 

        string text = $"{ duckName} ({_cur}/{_max})";
          
        bool isCompleted = (_cur >= _max);
        questObjective.ActiveCheckImage(isCompleted);
        questObjective.ActiveSpriteImage(false);
        questObjective.RenewObjectiveText(text);
    }
    public void InsertKillObjective(bool _isHead, int _max, EDuckType _duckType)
    {
        var go = Instantiate(objectivePrefab, objectiveRT);
        var questObjective = go.GetComponent<UIQuestObjective>();

        string duckName = GameInstance.Instance.TABLE_GetDuckName(_duckType);

        if (_isHead)
        {
            duckName += " 헤드샷";
        }

        string text = $"{duckName} ({_max})";
         
        questObjective.ActiveCheckImage(false);
        questObjective.ActiveSpriteImage(false);
        questObjective.RenewObjectiveText(text);
    }

    public void InsertRewardExp(float _exp)
    {
        if (_exp <= 0) 
            return; 

        int intExp = (int)_exp;
        var go = Instantiate(rewardPrefab, rewardRT);
        var questObjective = go.GetComponent<UIQeustReward>();
        questObjective.Init($"경험치 ({intExp})");
    } 
    public void InsertRewardMoney(int _money)
    {
        if (_money <= 0)
            return;
         
        var go = Instantiate(rewardPrefab, rewardRT);
        var questObjective = go.GetComponent<UIQeustReward>();
        questObjective.Init($"돈 ({_money})"); 
    }  
    public void InsertRewardItem(int _cnt, EItemID _itemID)
    {
        var itemPair = DuckUtill.GetItemPair(_itemID);
        
        var go = Instantiate(rewardPrefab, rewardRT);
        var questObjective = go.GetComponent<UIQeustReward>();
        questObjective.Init($"{itemPair.data.itemName} ({_cnt}) ", itemPair.visual.iconSprite);
    } 
    
    public void ClearObjectiveInfo()
    {
        ClearChildren(objectiveRT);
    } 

    private void ClearChildren(RectTransform rt)
    {
        for (int i = rt.childCount - 1; i >= 0; i--)
            Destroy(rt.GetChild(i).gameObject);
    }
}
  