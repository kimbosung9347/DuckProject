using UnityEngine;

public enum EInteractionType
{
    ItemBox,
    Item,
    Quest, 
    Store, 
    WareHouse,
    Adorn,
    Bed,
    SetLevel,
    MoveTo,
    End, 
} 

public abstract class HandleInteractionBase : MonoBehaviour
{
    [SerializeField] protected EInteractionType interactionType = EInteractionType.End;
      
    protected virtual void Awake()
    {
    }
     
    public EInteractionType GetInteractionType() { return interactionType; }

    public abstract void DoInteractionToPlayer(PlayerInteraction _playerInteraction);
    public abstract void EndInteractionToPlayer();
     
}
 