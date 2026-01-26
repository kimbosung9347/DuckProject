using UnityEngine;

public class HandleRestaurant : HandleInteractionBase
{
    public override void DoInteractionToPlayer(PlayerInteraction _playerInteraction)
    {
        var playerStat = _playerInteraction.gameObject.GetComponent<PlayerStat>();
        playerStat.RecoverFood(125f);
        playerStat.RecoverWater(125f); 
            
        _playerInteraction.gameObject.GetComponent<DuckSpeechBubble>()?.ActiveAutoDeleteSpeech("배불러");
    }  
    public override void EndInteractionToPlayer()
    {

    } 
}
