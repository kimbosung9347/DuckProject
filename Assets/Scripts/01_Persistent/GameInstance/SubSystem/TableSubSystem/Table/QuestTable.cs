using System.Collections.Generic;
using UnityEngine;

public class QuestTable : MonoBehaviour
{
    private readonly Dictionary<EQuestID, QuestData> hashQuestData = new();
    private readonly Dictionary<EQuestID, QuestUnlockData> hashQuestUnlockData = new();

    private void Awake()
    {
        LoadQuestData();
    } 
    private void LoadQuestData()
    {
        hashQuestData.Clear();
        hashQuestUnlockData.Clear();

        {
            QuestData[] questDatas = Resources.LoadAll<QuestData>("Data/Quest");
            foreach (var data in questDatas)
            {
                if (data == null)
                    continue;

                if (hashQuestData.ContainsKey(data.questID))
                {
                    Debug.LogError($"[QuestTable] Duplicate QuestID: {data.questID}");
                    continue;
                }

                hashQuestData.Add(data.questID, data);
            }
        }

        { 
            QuestUnlockData[] questDatas = Resources.LoadAll<QuestUnlockData>("Data/Quest");
            foreach (var data in questDatas)
            {
                if (data == null)
                    continue;

                if (hashQuestUnlockData.ContainsKey(data.questID))
                {
                    Debug.LogError($"[QuestTable] Duplicate QuestID: {data.questID}");
                    continue;
                }

                hashQuestUnlockData.Add(data.questID, data);
            }
        } 
    } 
      
    public QuestData GetQuestData(EQuestID questID)
    { 
        if (hashQuestData.TryGetValue(questID, out var data))
            return data;
         
        return null;
    }
    public QuestUnlockData GetQuestUnlockData(EQuestID questID)
    {
        if (hashQuestUnlockData.TryGetValue(questID, out var data))
            return data;
          
        return null;
    }
}
 