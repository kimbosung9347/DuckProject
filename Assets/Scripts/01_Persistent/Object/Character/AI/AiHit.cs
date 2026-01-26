using UnityEngine;

public class AiHit : DuckHit
{
    private AiDetector cachedDetector;
     
    protected override void Awake()
    {
        base.Awake();
         
        cachedDetector = GetComponentInParent<AiDetector>();
    } 

    public override bool TakeDamage(float _damage, DuckAttack _duckAttack)
    {
        if (!CanHit())
            return false; 

        base.TakeDamage(_damage, _duckAttack);
         
        cachedDetector?.NotifyHitSource(_duckAttack);
         
        return true; 
    }


} 
