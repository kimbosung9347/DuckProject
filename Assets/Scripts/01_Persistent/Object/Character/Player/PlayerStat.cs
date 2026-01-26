using UnityEngine;
using System.Collections;

public class PlayerStat : DuckStat
{
    [SerializeField] private float maxFull = 100f;
    [SerializeField] private float maxThist = 100f;
    [SerializeField] private GameObject staminaObj;

    private DuckSpeechBubble cachedSpeech;
    private PlayerState cachedState;
    private PlayerUIController cachedUIController;
    private PlayerDifficult cachedPlayerDifficult;
    private PlayerBuff cachedPlayerBuff;
    private CircleGauge cachedStaminaGuage;

    private float curFull = 0f;
    private float curThirst = 0f;

    private bool isHungry = false;
    private bool isThirst = false;
    private bool isInvincibility = false;
     
    private Coroutine staminaOffCoroutine;

    protected override void Awake()
    {
        base.Awake();

        cachedSpeech = GetComponent<DuckSpeechBubble>();
        cachedState = GetComponent<PlayerState>();
        cachedPlayerDifficult = GetComponent<PlayerDifficult>();
        cachedUIController = GetComponent<PlayerUIController>();
        cachedPlayerBuff = GetComponent<PlayerBuff>();
        cachedStaminaGuage = staminaObj.GetComponentInChildren<CircleGauge>(true);
    }
    protected override void Start()
    {
        base.Start();

        cachedUIController.HUD_RenewHp(curHp, cachedStatInfo.maxHp);
        cachedUIController.HUD_RenewFood(curFull / maxFull);
        cachedUIController.HUD_RenewWater(curThirst / maxThist);
    }
    protected override void Update()
    {
        base.Update();
        UpdateFood();
    }

    public override void CacheStatInfo(StatInfo _statInfo)
    {
        base.CacheStatInfo(_statInfo);
        curFull = maxFull;
        curThirst = maxThist;
        isHungry = false;
        isThirst = false;
    }
    public override float RecoverHp(float _recover)
    {
        float remain = base.RecoverHp(_recover);
        cachedUIController.HUD_RenewHp(curHp, cachedStatInfo.maxHp);
        return remain;
    }
    public override void RecoverFood(float _add)
    {
        bool wasZero = curFull <= 0f;

        curFull = Mathf.Clamp(curFull + _add, 0f, maxFull);
        cachedUIController.HUD_RenewFood(curFull / maxFull);

        if (wasZero && curFull > 0f && isHungry)
        {
            cachedPlayerBuff.RemoveBuff(EBuffID.Hungry);
            isHungry = false;
        } 
    }
    public override void RecoverWater(float _add)
    {
        bool wasZero = curThirst <= 0f;

        curThirst = Mathf.Clamp(curThirst + _add, 0f, maxThist);
        cachedUIController.HUD_RenewWater(curThirst / maxThist);

        if (wasZero && curThirst > 0f && isThirst)
        {
            cachedPlayerBuff.RemoveBuff(EBuffID.Thirst);
            isThirst = false;
        }
    } 
    public override void HitDamage(bool _isHead, float _damage, DuckAttack _duckAttack)
    {
        base.HitDamage(_isHead, _damage, _duckAttack);
        cachedUIController.HUD_RenewHp(curHp, cachedStatInfo.maxHp);
    }
    public override void InitHP()
    {
        base.InitHP();

        curFull = maxFull;
        curThirst = maxThist;
        isHungry = false;
        isThirst = false;

        staminaObj.SetActive(false);
        staminaOffCoroutine = null;

        cachedPlayerBuff.RemoveBuff(EBuffID.Thirst);
        cachedPlayerBuff.RemoveBuff(EBuffID.Hungry); 

        cachedUIController.HUD_RenewHp(curHp, cachedStatInfo.maxHp);
    } 

    public void ActiveInvincibility(bool _isActive)
    {
        isInvincibility = _isActive; 
    }
    public bool CanRoll()
    {
        if (curMp <= DuckDefine.STAMINA_ROLL_VALUE)
        {
            cachedSpeech.ActiveAutoDeleteSpeech("힘들어");
            ActiveStaminaGuage(true);
            ActiveStaminaGuage(false);
            return false;
        }
        return true;
    }

    public void ReduceStamina(float _value)
    {
        curMp = Mathf.Max(curMp - _value, 0f);
        cachedStaminaGuage.RenewGauge(curMp / cachedStatInfo.maxMp);
    }

    public void ActiveStaminaGuage(bool _active)
    {
        if (_active)
        {
            if (staminaOffCoroutine != null)
            {
                StopCoroutine(staminaOffCoroutine);
                staminaOffCoroutine = null;
            }
            staminaObj.SetActive(true);
        }
        else
        {
            if (staminaOffCoroutine != null)
                StopCoroutine(staminaOffCoroutine);

            staminaOffCoroutine = StartCoroutine(Co_OffStaminaGauge());
        }
    }

    protected override void ApplyDamage(bool _isHead, float _damage)
    {
        float damage = cachedPlayerDifficult.ConvertDamageByDifficult(_damage);
        if (isInvincibility)
            damage = 0;
         
        base.ApplyDamage(_isHead, damage);
    }

    protected override void UpdateStamina()
    {
        if (cachedState.IsSprint())
        {
            const float reduceStamina = 5f;
            curMp = Mathf.Max(curMp - reduceStamina * Time.deltaTime, 0f);
            cachedStaminaGuage.RenewGauge(curMp / cachedStatInfo.maxMp);

            if (curMp <= 0f)
                cachedState.ChangeState(EDuckState.Default);
        }
        else
        {
            curMp = Mathf.Min(curMp + cachedStatInfo.mpRecovery * Time.deltaTime, cachedStatInfo.maxMp);
        }
    }

    private void UpdateFood()
    {
        float dt = Time.deltaTime; 
        float reduceSpeed = 0.75f; 
        float newFull = Mathf.Clamp(curFull - reduceSpeed * dt, 0f, maxFull);
        float newThirst = Mathf.Clamp(curThirst - reduceSpeed * dt, 0f, maxThist);

        if (curFull > 0f && newFull <= 0f && !isHungry)
        {
            cachedPlayerBuff.InsertBuff(EBuffID.Hungry);
            isHungry = true;
        }
        if (curThirst > 0f && newThirst <= 0f && !isThirst)
        {
            cachedPlayerBuff.InsertBuff(EBuffID.Thirst);
            isThirst = true;
        }

        bool changedFull = newFull != curFull;
        bool changedThirst = newThirst != curThirst;

        curFull = newFull;
        curThirst = newThirst;

        if (changedFull)
            cachedUIController.HUD_RenewFood(curFull / maxFull);

        if (changedThirst)
            cachedUIController.HUD_RenewWater(curThirst / maxThist);
    }

    private IEnumerator Co_OffStaminaGauge()
    {
        yield return new WaitForSeconds(0.5f);
        staminaObj.SetActive(false);
        staminaOffCoroutine = null;
    }
}
