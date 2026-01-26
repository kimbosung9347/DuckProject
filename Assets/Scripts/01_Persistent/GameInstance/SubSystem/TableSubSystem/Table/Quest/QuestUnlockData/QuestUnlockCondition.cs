using UnityEngine;

public abstract class QuestUnlockCondition : ScriptableObject
{
    public abstract bool IsSatisfied(PlayerQuest _unlock);
}   