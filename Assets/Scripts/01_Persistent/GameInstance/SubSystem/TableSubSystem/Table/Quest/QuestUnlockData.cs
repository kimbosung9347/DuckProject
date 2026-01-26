using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestUnlockData", menuName = "Scriptable Objects/Quest/Unlock/QuestUnlockData")]
public class QuestUnlockData : ScriptableObject
{
    public EQuestID questID;
    public List<QuestUnlockCondition> listObjective;
     
    public bool IsSuccesUnLock(PlayerQuest _playerUnlock)
    { 
        if (listObjective.Count <= 0) 
            return false;
         
        foreach (QuestUnlockCondition condition in listObjective)
        { 
            if (!condition.IsSatisfied(_playerUnlock))
                return false;
        }

        return true;
    }
}
