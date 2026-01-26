using UnityEngine;
 
public class DuckStat : MonoBehaviour
{ 
    [SerializeField] private Color hpColor;
    [SerializeField] private float maxHitEffectTime = 0.2f;
    [SerializeField] private GameObject hitValuePrefab;
      
    protected DuckHpbar cachedDuckHpBar;
    protected StatInfo cachedStatInfo; 
    protected float curHp = 0f;
    protected float curMp = 0f;

    private const float maxHitSize = 1.5f;
    private bool isHit = false;
    private float hitTime = 0f; 
    private float prevHp = 0f;
     
    protected virtual void Awake()
    {  
        cachedDuckHpBar = gameObject.GetComponentInChildren<DuckHpbar>();
    } 
    protected virtual void Start()
    {
        cachedDuckHpBar.SetHPColor(hpColor);
        var duckType = GetComponent<DuckAbility>().GetDuckType();
        if (duckType == EDuckType.Player)
        {
            cachedDuckHpBar.SetName(string.Empty);
        }
        else
        {
            var ducnName = GameInstance.Instance.TABLE_GetDuckName(GetComponent<DuckAbility>().GetDuckType());
            cachedDuckHpBar.SetName(ducnName);
        }

        InitHP(); 
    } 

    protected virtual void Update()
    {
        UpdateHP(); 
        UpdateStamina();
    } 
    public virtual void CacheStatInfo(StatInfo _statInfo)
    { 
        cachedStatInfo = _statInfo;
    }  

    public virtual float RecoverHp(float _recover)
    {
        float prevHp = curHp;
        curHp += _recover;
         
        // 최대체력일 경우
        if (curHp > cachedStatInfo.maxHp)
        {
            curHp = cachedStatInfo.maxHp;
            cachedDuckHpBar.SetHP(curHp / cachedStatInfo.maxHp);
            return cachedStatInfo.maxHp - prevHp;
        }

        cachedDuckHpBar.SetHP(curHp / cachedStatInfo.maxHp);
        return _recover;   
    }
    public virtual void RecoverFood(float _add)
    {
        // AI는 Food나 Water를 구현할까말까?
    }
    public virtual void RecoverWater(float _add)
    {
        // AI는 Food나 Water를 구현할까말까?
    } 

    public virtual void HitDamage(bool _isHead, float _damage, DuckAttack _hitAttack)
    {
        ApplyDamage(_isHead, _damage);

        bool isDead = (curHp == 0);
        if (isDead)
            GetComponent<DuckDead>()?.Dead(_isHead, _hitAttack);
         
        // UI 갱신해줘야함
        if (_hitAttack is PlayerAttack playerAttack)
        {
            ECursorHitState hitState;
             
            if (isDead)
            {
                hitState = ECursorHitState.Dead;
            } 

            else if (_isHead)
            {
                hitState = ECursorHitState.Head;
            }

            else
            {
                hitState = ECursorHitState.Normal;
            }
             
            playerAttack.ActionMouseHit(hitState);
        } 
    }
    public virtual void HitDamage(bool _isHead, float _damage, DuckMeleeAttack _hitAttack)
    {
        ApplyDamage(false, _damage);
         
        if (curHp == 0)
            GetComponent<DuckDead>()?.Dead(false, _hitAttack);
    }
    public virtual void InitHP()
    { 
        curHp = cachedStatInfo.maxHp;
        curMp = cachedStatInfo.maxMp;
        cachedDuckHpBar.SetHP(curHp / cachedStatInfo.maxHp);
    } 

    public void HandleStatChanged(EStatType type, StatInfo newStat)
    {
        switch (type)
        {
            case EStatType.MaxHp:
                curHp = Mathf.Clamp(curHp, 0, newStat.maxHp);
                cachedDuckHpBar.SetHP(curHp / newStat.maxHp);
                break;

            case EStatType.MaxMp:
                curMp = Mathf.Clamp(curMp, 0, newStat.maxMp);
                break;
        }
    }
 
    public float GetCurHpRatio()
    {
        return curHp / cachedStatInfo.maxHp;
    }
      
    protected virtual void UpdateHP()
    {
        if (isHit)
        {
            hitTime += Time.deltaTime;

            float t = hitTime / maxHitEffectTime;

            float curRatio = curHp / cachedStatInfo.maxHp;
            float prevRatio = prevHp / cachedStatInfo.maxHp;
            float lerped = Mathf.Lerp(prevRatio, curRatio, t);

            cachedDuckHpBar.SetLossRange(curRatio, lerped);
             
            // 크기보간 
            float size = Mathf.Lerp(maxHitSize, 1.0f, t);
            cachedDuckHpBar.SetHpBarSize(size);

            if (t >= 1f)
            {
                isHit = false;
                cachedDuckHpBar.SetHpBarSize(1.0f);
                cachedDuckHpBar.ShowLossBar(false);
            }
        }
    }
    protected virtual void UpdateStamina()
    {
      
    }
    protected virtual void ApplyDamage(bool _isHead, float _damage)
    {
        prevHp = curHp;
        isHit = true;
        hitTime = 0f;

        curHp -= _damage;
        if (curHp < 0f)
            curHp = 0f;

        cachedDuckHpBar.ShowLossBar(isHit);
        cachedDuckHpBar.SetHP(curHp / cachedStatInfo.maxHp);
        cachedDuckHpBar.SetLossRange(curHp / cachedStatInfo.maxHp, prevHp / cachedStatInfo.maxHp);
        cachedDuckHpBar.SetHpBarSize(maxHitSize);

        Vector3 worldPos = transform.position; 
        worldPos.y += 2f;
        var hitValue = GameInstance.Instance.SPAWN_MakeHitValuePrefab();
        hitValue.transform.position = worldPos;     
        hitValue.GetComponent<HitValue>()?.Active(_isHead, _damage);
    }
}  
 