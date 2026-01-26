using UnityEngine;

public class HandleSetLevel : HandleInteractionBase
{
    protected override void Awake()
    {
        interactionType = EInteractionType.SetLevel;
    }

    public override void DoInteractionToPlayer(PlayerInteraction _playerInteraction)
    {
        _playerInteraction.EnterSetLevel(); 
    }
    public override void EndInteractionToPlayer()
    {

    } 
}
 