using UnityEngine;

public class HandleStore : HandleInteractionBase
{
    protected override void Awake()
    {
        base.Awake();
    }
    public override void DoInteractionToPlayer(PlayerInteraction _playerInteraction)
    {
        var store = GetComponent<Store>();
        _playerInteraction.EnterStore(store); 
    }
      
    public override void EndInteractionToPlayer()
    {
            
    }
}
