using UnityEngine;

[CreateAssetMenu(
    fileName = "DeliverQuestObjective",
    menuName = "Scriptable Objects/Quest/Objectives/Deliver"
)]
public class DeliverQuestObjectiveData : QuestObjectiveData
{
    [SerializeField] private EItemID itemID;
    public EItemID ItemID  => itemID;
}
    