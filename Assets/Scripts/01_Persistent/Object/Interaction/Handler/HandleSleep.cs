using UnityEngine;

public class HandleSleep : HandleInteractionBase
{ 
    protected override void Awake()
    {
        interactionType = EInteractionType.Bed;
    } 

    public override void DoInteractionToPlayer(PlayerInteraction _playerInteraction)
    { 
        _playerInteraction.EnterSleep();
    } 
    public override void EndInteractionToPlayer()
    {

    }
}
