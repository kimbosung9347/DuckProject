using UnityEngine;
using System.Collections.Generic;

public enum EQuestID
{
    NewHunter,
    AimForTheHead,
    MedicalSupplies,
    KillLittleDuck,

    End,
}

[System.Serializable]
public struct FQuestRewardItem
{
    public EItemID itemID;
    public int count;
} 

[System.Serializable]
public struct FQuestReward
{
    public float exp;
    public int money;
    public List<FQuestRewardItem> items; // 수량 필요하면 나중에 Pair로 확장
} 

[CreateAssetMenu(fileName = "QuestData", menuName = "Scriptable Objects/Quest/QuestData")]
public class QuestData : ScriptableObject
{
    public EQuestID questID;
    public EDuckType gaveQuestDuck;
     
    [Header("퀘스트 정보")]
    public string questName;
    public string questDesc;

    [Header("달성목표")]
    [SerializeReference]    
    public List<QuestObjectiveData> listObjective;
       
    [Header("보상")]
    public FQuestReward reward;
}
 