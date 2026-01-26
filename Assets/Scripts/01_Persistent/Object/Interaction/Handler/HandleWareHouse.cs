using UnityEngine;

public class HandleWareHouse : HandleInteractionBase
{
    WareHouse wareHouse;

    private void OnDisable()
    { 
        if (wareHouse)
        {
            wareHouse.gameObject.SetActive(false);
        }
    }
     
    protected override void Awake() 
    {
        interactionType = EInteractionType.WareHouse;
         
        // 캐싱
        wareHouse = GameInstance.Instance.PLAYER_GetWareHouse();
        wareHouse?.gameObject.SetActive(true);
    }  

    public override void DoInteractionToPlayer(PlayerInteraction _playerInteraction)
    {
        _playerInteraction.EnterWareHouse(wareHouse);
    }    
     
    public override void EndInteractionToPlayer()
    {

    }
}
