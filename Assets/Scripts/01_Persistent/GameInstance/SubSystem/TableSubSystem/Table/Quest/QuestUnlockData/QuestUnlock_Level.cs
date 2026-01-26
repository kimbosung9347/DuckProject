using UnityEngine;

[CreateAssetMenu(fileName = "QuestUnlock_Level", menuName = "Scriptable Objects/Quest/Unlock/Condition/Level")]
public class QuestUnlock_Level : QuestUnlockCondition
{
    public int needLevel;  

    public override bool IsSatisfied(PlayerQuest _unlock)
    { 
        return _unlock.IsReachLevel(needLevel);
    } 
}
 