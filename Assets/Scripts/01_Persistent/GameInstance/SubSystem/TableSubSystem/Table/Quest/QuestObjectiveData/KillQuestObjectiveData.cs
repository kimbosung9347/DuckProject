using UnityEngine;

[CreateAssetMenu(
    fileName = "KillQuestObjective",
    menuName = "Scriptable Objects/Quest/Objectives/Kill"
)]
public class KillQuestObjectiveData : QuestObjectiveData
{
    [SerializeField] private EDuckType targetDuckType;
    [SerializeField] private bool isHeadShot;
     
    public EDuckType TargetDuckType => targetDuckType;
    public bool IsHeadShot => isHeadShot;
}
 