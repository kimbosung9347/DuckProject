using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;

public class HandleQuest : HandleInteractionBase
{
    [SerializeField] private CinemachineCamera questCamera;
    private QuestGiver cachedQuestGiver; 

    protected override void Awake()
    {
        cachedQuestGiver = GetComponent<QuestGiver>();
        interactionType = EInteractionType.Quest;
    }
     
    public override void DoInteractionToPlayer(PlayerInteraction _playerInteraction)
    {
        Refresh();

        _playerInteraction.EnterQuest(this);

        if (questCamera)
        {
            questCamera.Priority = DuckDefine.INTERACTION_CINEMACHIN_PRIORITY; 
        } 
    }  
    public override void EndInteractionToPlayer()
    { 
        if (questCamera)
        {
            questCamera.Priority = DuckDefine.DISABLE_CINEMACHIN_PRIORITY; 
        } 
    }

    public List<EQuestID> GetAcceptableQusetList()
    {
        Refresh();
        return cachedQuestGiver.GetQuestList(); 
    }  
    public EDuckType GetQuestGiverDuckType()
    {
        return cachedQuestGiver.GetDuckType();
    } 
    public void RemoveQuestID(EQuestID _id)
    {
        cachedQuestGiver.RemoveQuestID(_id);
    } 
     
    private void Refresh()
    {
        var instance = GameInstance.Instance;
        var playerQuest = instance.PLAYER_GetPlayerQuest();
        cachedQuestGiver.Refresh(playerQuest);
    } 
     
}
 