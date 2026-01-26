using UnityEngine;


public enum EObjectiveType
{ 
    Deliver,
    Kill,
     
    _End,
}

public abstract class QuestObjectiveData : ScriptableObject
{
    [SerializeField] private EObjectiveType objectiveType;
    [SerializeField] private int requiredCount;
     
    public EObjectiveType ObjectiveType => objectiveType;
    public int RequiredCount => requiredCount;
}
