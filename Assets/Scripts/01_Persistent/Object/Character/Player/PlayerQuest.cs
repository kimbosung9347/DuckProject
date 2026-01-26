using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;

public class PlayerQuest : MonoBehaviour
{
    private PlayerStorage cachedStorage;
    private PlayerGrow cachedGrow;

    // 런타임 퀘스트
    private Dictionary<EQuestID, QuestInstance> hashQuestInstance = new();
    private List<EQuestID> listComplateQuest = new();
     
    private void Awake()
    {
        cachedStorage = GetComponent<PlayerStorage>();
        cachedGrow = GetComponent<PlayerGrow>();
    }

    private void Start()
    { 
        var playData = GameInstance.Instance.SAVE_GetCurPlayData();

        hashQuestInstance.Clear();
        listComplateQuest.Clear();

        listComplateQuest = new List<EQuestID>(playData.questData.complateQuest);
        foreach (var shell in playData.questData.inProgressQuest)
        { 
            var inst = new QuestInstance();
            inst.Init(shell);
            hashQuestInstance.Add(shell.questID, inst);
        }
    }

    // =========================
    // Save / Load
    // =========================
    public QuestDataShell CreateQuestData()
    {
        var data = new QuestDataShell();

        for (int i = 0; i < listComplateQuest.Count; i++)
            data.complateQuest.Add(listComplateQuest[i]);

        foreach (var pair in hashQuestInstance)
        {
            if (pair.Value == null)
                continue;

            data.inProgressQuest.Add(pair.Value.CreateRuntimeShell());
        }

        return data;
    }

    // =========================
    // Quest State Query
    // =========================
    public List<EQuestID> GetComplateList() => listComplateQuest;

    public List<EQuestID> GetInProgressList()
    {
        return new List<EQuestID>(hashQuestInstance.Keys);
    }

    public QuestInstance GetQuestInstance(EQuestID id)
    {
        if (!hashQuestInstance.TryGetValue(id, out var inst))
            return null;

        return inst;
    }

    public bool IsComplateQuest(EQuestID id)
    {
        return listComplateQuest.Contains(id);
    }

    public bool IsComplateOrInProgress(EQuestID id)
    {
        if (listComplateQuest.Contains(id))
            return true;

        return hashQuestInstance.ContainsKey(id);
    }

    // =========================
    // Insert / Complete
    // =========================
    public void InsertQuest(EQuestDomainButtonType type, EQuestID id)
    {
        switch (type)
        {
            case EQuestDomainButtonType.InProgress:
                {
                    if (hashQuestInstance.ContainsKey(id))
                        return;

                    var inst = new QuestInstance();
                    inst.Init(id);
                    hashQuestInstance.Add(id, inst);
                }
                break;

            case EQuestDomainButtonType.Complate:
                {
                    if (!listComplateQuest.Contains(id))
                        listComplateQuest.Add(id);

                    hashQuestInstance.Remove(id);
                }
                break;
        }
    }

    public bool IsComplateCondition(EQuestID questId)
    {
        var inst = GetQuestInstance(questId);
        return inst != null && inst.IsComplate();
    }

    // =========================
    // Unlock Logic (통합)
    // =========================
    public bool CanUnlock(EQuestID id)
    {
        var unlockData = GameInstance.Instance.TABLE_GetQuestUnlockData(id);
        return unlockData.IsSuccesUnLock(this);
    } 
       
    public bool HasItem(EItemID id, int count)
    {
        // TODO : PlayerStorage에 전체 수량 조회 함수 생기면 연결
        // return cachedStorage.GetItemCount(id) >= count;
        return false;
    }

    public bool IsAllClearQuest(List<EQuestID> questList)
    {
        for (int i = 0; i < questList.Count; i++)
        {
            if (!IsComplateQuest(questList[i]))
                return false;
        }
        return true;
    }

    public bool IsReachLevel(int targetLevel)
    {
        return cachedGrow.GetLevel() >= targetLevel;
    }

    // =========================
    // Reward
    // =========================
    public void GainRewardExp(float exp)
    {
        if (exp <= 0f)
            return;

        cachedGrow.AddExp(exp);
    }

    public void GainRewardMoney(int money)
    {
        if (money <= 0)
            return;

        cachedStorage.AcquireMoney(money);
    }

    public void GainRewardItem(EItemID id, int cnt)
    {
        for (int i = 0; i < cnt; i++)
        {
            var item = GameInstance.Instance.SPAWN_MakeItem(id);
            int emptyIndex = cachedStorage.GetEmptyIndex();

            if (emptyIndex == -1)
            {
                item.transform.position = transform.position;
                item.Drop();
                continue;
            }

            cachedStorage.InsertItemNotRenewUI(emptyIndex, item);
        }
    }

    // =========================
    // Runtime Progress Update
    // =========================
    public void SuccessKillTarget(bool isHead, EDuckType type)
    {
        foreach (var questInstance in hashQuestInstance.Values)
        {
            var killList = questInstance.GetKillRuntimeData();
            if (killList == null)
                continue;

            for (int i = 0; i < killList.Count; i++)
            {
                var runtime = killList[i];

                if (runtime.curCount >= runtime.maxCount)
                    continue;

                if (runtime.duckType != EDuckType.Anyone && runtime.duckType != type)
                    continue;

                if (runtime.isHead && !isHead)
                    continue;

                runtime.curCount++;
                killList[i] = runtime;
            }
        }
    }

    public bool CheckInItemInStoreByDeliverItem(EQuestID questId)
    {
        var questInstance = GetQuestInstance(questId);
        if (questInstance == null)
            return false;

        var deliverList = questInstance.GetDeliverRuntimeData();
        bool changed = false;

        for (int i = 0; i < deliverList.Count; i++)
        {
            var runtime = deliverList[i];

            int need = runtime.maxCount - runtime.curCount;
            if (need <= 0)
                continue;

            int removed = cachedStorage.RemoveByItemId(runtime.itemID, need);
            if (removed <= 0)
                continue;

            runtime.curCount += removed;
            deliverList[i] = runtime;
            changed = true;
        }

        return changed;
    }
}
