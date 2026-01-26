using UnityEngine;

public class HandleMusic : HandleInteractionBase
{
    private EBgmType curBgmType = EBgmType.DuckFunk;
      
    private void Start() 
    {
        GameInstance.Instance.SOUND_PlayBGM(curBgmType); 
    }

    private void OnDestroy()
    {
        GameInstance.Instance.SOUND_StopBGM();
    }  

    public override void DoInteractionToPlayer(PlayerInteraction _playerInteraction)
    {
        int next = (int)curBgmType + 1;
        if (next >= (int)EBgmType.End)
            next = 0;
        curBgmType = (EBgmType)next;
        GameInstance.Instance.SOUND_PlayBGM(curBgmType);
         
        var bubble = _playerInteraction.gameObject.GetComponent<DuckSpeechBubble>();
        bubble.ActiveAutoDeleteSpeech(curBgmType.ToString());
    }   
    public override void EndInteractionToPlayer()
    {
         
    }
}
