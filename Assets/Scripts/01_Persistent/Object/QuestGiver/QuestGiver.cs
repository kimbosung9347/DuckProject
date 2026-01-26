using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    // 줄 수 있는 모든 퀘스트 종류
    [SerializeField] private List<EQuestID> sourceQuest; // 고정
    [SerializeField] private EDuckType giverDuckType;
     
    // 현재 줄 수 있는 퀘스트 종류
    private readonly List<EQuestID> cachedAvailableQuest = new();
     
    public List<EQuestID> GetQuestList() => cachedAvailableQuest;
    public EDuckType GetDuckType() => giverDuckType;

    public void Refresh(PlayerQuest quest)
    {
        cachedAvailableQuest.Clear();

        foreach (var id in sourceQuest)
        {
            if (quest.IsComplateOrInProgress(id)) 
                continue;

            if (!quest.CanUnlock(id))
                continue;
             
            cachedAvailableQuest.Add(id);
        }
    }
     
    public void RemoveQuestID(EQuestID questID)
    {
        sourceQuest.Remove(questID);
        cachedAvailableQuest.Remove(questID);
    }
}
