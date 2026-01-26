using UnityEngine;

public class DuckDead : MonoBehaviour
{
    private DuckState cachedState;
    
    // 죽음을 호출해줘야함
    // 상태  
    protected virtual void Awake()
    {
        cachedState = GetComponent<DuckState>(); 
    } 

    public virtual void Dead(bool _isHead, DuckAttack _killedTarget)
    {   
        cachedState.ChangeState(EDuckState.Dead);
        PlayDeadEffect();
        //      
    }
    public virtual void Dead(bool _isHead, DuckMeleeAttack _killedTarget)
    {
        cachedState.ChangeState(EDuckState.Dead);
        PlayDeadEffect();
        //        
    }
     
    private void PlayDeadEffect()
    {
        var instance = GameInstance.Instance;
        instance.POOL_Spawn(EPoolId.ExploadBlood, transform.position, Quaternion.identity);
        instance.SOUND_PlaySoundSfx(ESoundSfxType.Dead, transform.position);
    }  
}
