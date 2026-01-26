using UnityEngine;

[CreateAssetMenu(fileName = "QuestUnlock_HasItem", menuName = "Scriptable Objects/Quest/Unlock/Condition/HasItem")]
public class QuestUnlock_HasItem : QuestUnlockCondition
{
    public EItemID itemID; 
    public int count = 1;
     
    public override bool IsSatisfied(PlayerQuest _unlock)
    { 
        return _unlock.HasItem(itemID, count);
    }
}  
 