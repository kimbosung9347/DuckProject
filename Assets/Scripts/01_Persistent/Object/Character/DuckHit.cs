using UnityEngine;

public class DuckHit : MonoBehaviour, IHitTarget
{
    [SerializeField] protected bool isHead = false;
    [SerializeField] protected DuckStat cachedDuckStat;
    [SerializeField] protected DuckState cachedDuckState;
    
    protected ArmorInfo cachedArmorInfo;
    protected BattleTable cachedBattleTable;
    private DuckMeshSetter cachedMeshSetter;
    private EDuckType duckType;

     
    protected virtual void Awake()
    { 
        duckType = GetComponentInParent<DuckAbility>().GetDuckType();
        cachedBattleTable = GameInstance.Instance.TABLE_GetBattleTable();
        cachedMeshSetter = transform.root.GetComponentInChildren<DuckMeshSetter>(true);
    }  

    public virtual bool TakeDamage(float _damage, DuckAttack _duckAttack)
    { 
        if (!CanHit())
            return false;
         
        float damage = cachedBattleTable.Calculate_HitDamage(isHead, _damage, cachedArmorInfo.GetDefense(isHead));
        cachedDuckStat?.HitDamage(isHead, damage, _duckAttack);
        ActiveHitEffect(false);
        return true; 
    }
    public virtual bool TakeDamage(float _damage, DuckMeleeAttack _duckMeleeAttack)
    {  
        if (!CanHit())
            return false;
         
        float damage = cachedBattleTable.Calculate_HitDamage(isHead, _damage, cachedArmorInfo.GetDefense(isHead));
        cachedDuckStat?.HitDamage(isHead, damage, _duckMeleeAttack);

        ActiveHitEffect(true);

        return true;  
    }

    public void CacheArmorInfo(ArmorInfo armorInfo)
    {
        cachedArmorInfo = armorInfo;
    }
    public bool IsHead() { return isHead; }
    public EDuckType GetDuckType() { return duckType; } 
     
    protected bool CanHit()
    {
        if (!cachedDuckState)
            return false; 
         
        if (!cachedDuckState.CanHit())
            return false;

        return true;
    }

    private void ActiveHitEffect(bool _isMelee)
    { 
        GameInstance.Instance.POOL_Spawn(EPoolId.Blood, transform.position, Quaternion.identity);
    }  
}   
