using UnityEngine;
using UnityEngine.Rendering;

public class HandleItem : HandleInteractionBase
{
    private ItemBase cachedItem;

    protected override void Awake()
    {
        base.Awake();
         
        interactionType = EInteractionType.Item;
    } 

    // 아이템 획득 해줘야함
    public override void DoInteractionToPlayer(PlayerInteraction _interaction)
    {
        var playerStorage = _interaction.gameObject.GetComponent<PlayerStorage>();

        if (playerStorage.IsEmptySlotSet())
        {
            playerStorage.AutoInsertItem(cachedItem);
        }

        // 인벤토리에 넣을 수 없습니다 같은 유아이 호출해줘도 될듯
        else
        { 
            // _interaction -> 말풍선 기능 todo 
            _interaction.gameObject.GetComponent<DuckSpeechBubble>()?.Active("인벤토리가 찼어");
        }
    }
    public override void EndInteractionToPlayer()
    { 

    }
    public void CacheItem(ItemBase _item)
    {
        cachedItem = _item;
    }
}
