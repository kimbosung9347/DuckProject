using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestUnlock_ClearQuest", menuName = "Scriptable Objects/Quest/Unlock/Condition/ClearQuest")]
public class QuestUnlock_ClearQuest : QuestUnlockCondition
{
    public List<EQuestID> listClearQuest;
    public override bool IsSatisfied(PlayerQuest _unlock)
    { 
        bool isSatisted = _unlock.IsAllClearQuest(listClearQuest);
        return isSatisted;
    } 
} 
