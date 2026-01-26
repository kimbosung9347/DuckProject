using UnityEngine;
using System.Collections.Generic;

public class DuckMeleeAttack : MonoBehaviour
{
    private enum EAttackState
    {
        None,
        Delay,   // 선딜 (아직 공격 아님)
        Active,  // 실제 공격 판정
    }

    [Header("Timing")]
    [SerializeField] private float attackDelay = 0.25f;
    [SerializeField] private float activeTime = 0.15f;
    [SerializeField] private float cooldown = 1.0f;

    [Header("Damage")]
    [SerializeField] private int damage = 10;

    [Header("Visual / Collider Root")]
    [SerializeField] private GameObject attackRangeVisual;

    [Header("Option")]
    [SerializeField] private bool isAi = true;

    [Header("Effect")]
    [SerializeField] private ParticleSystem preAttackEffect;
    [SerializeField] private float preEffectTime = 0.1f;

    private Collider attackCollider;
    private EDuckType myType;

    private EAttackState state = EAttackState.None;
    private float stateTimer;
    private float lastAttackTime = -999f;
    private bool preEffectPlayed;

    // 다중 히트 방지
    private readonly HashSet<DuckHit> hitSet = new();

    private void Awake()
    {
        myType = GetComponent<DuckAbility>().GetDuckType();

        if (!attackRangeVisual)
            return;

        attackCollider = attackRangeVisual.GetComponent<Collider>();
        attackCollider.enabled = false;
        attackRangeVisual.SetActive(false);
    }

    private void Update()
    {
        if (state == EAttackState.None)
            return;

        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case EAttackState.Delay:
                UpdateDelay();
                break;

            case EAttackState.Active:
                UpdateActive();
                break;
        }
    }

    // -----------------------------
    // State Update
    // -----------------------------

    private void UpdateDelay()
    {
        // 선딜 끝나기 직전 예고 이펙트
        if (!preEffectPlayed && stateTimer <= preEffectTime)
        {
            preEffectPlayed = true;
            PlayPreAttackEffect();
        }

        if (stateTimer <= 0f)
            EnterActive();
    }

    private void UpdateActive()
    {
        if (stateTimer <= 0f)
            ExitAttack();
    }

    // -----------------------------
    // Public API
    // -----------------------------

    public bool IsAi() => isAi;
    public bool IsAttacking() => state != EAttackState.None;

    public bool CanMeleeAttack()
    {
        if (state != EAttackState.None)
            return false;

        if (Time.time < lastAttackTime + cooldown)
            return false;

        return true;
    }

    public void DoAttack()
    {
        lastAttackTime = Time.time;
        EnterDelay();
    }

    // -----------------------------
    // State Enter / Exit
    // -----------------------------

    private void EnterDelay()
    {
        state = EAttackState.Delay;
        stateTimer = attackDelay;
        preEffectPlayed = false;
        hitSet.Clear();

        if (attackRangeVisual)
            attackRangeVisual.SetActive(true);

        // 선딜이 너무 짧으면 즉시 예고 이펙트
        if (preAttackEffect && attackDelay <= preEffectTime)
        {
            preEffectPlayed = true;
            PlayPreAttackEffect();
        }
    }

    private void EnterActive()
    {
        state = EAttackState.Active;
        stateTimer = activeTime;

        if (attackCollider)
            attackCollider.enabled = true;

        PlayHitEffect(); 
    }

    private void ExitAttack()
    {
        state = EAttackState.None;

        if (attackCollider)
            attackCollider.enabled = false;

        if (attackRangeVisual)
            attackRangeVisual.SetActive(false);

        if (preAttackEffect)
            preAttackEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    // -----------------------------
    // Hit Logic
    // -----------------------------

    private void OnTriggerEnter(Collider other)
    {
        if (state != EAttackState.Active)
            return;

        var targetDuckHit = other.GetComponent<DuckHit>();
        if (!targetDuckHit)
            return;

        EDuckRelation relation =
            GameInstance.Instance.TABLE_GetDuckRelation(
                myType,
                targetDuckHit.GetDuckType()
            );

        if (relation != EDuckRelation.Hostile)
            return;

        if (!hitSet.Add(targetDuckHit))
            return;

        targetDuckHit.TakeDamage(damage, this); // ✅ 실제 데미지
    }

    // -----------------------------
    // Effect Wrapper (의미 분리)
    // -----------------------------

    private void PlayPreAttackEffect()
    {
        if (preAttackEffect)
            preAttackEffect.Play(true);

        GameInstance.Instance.SOUND_PlaySoundSfx(
        ESoundSfxType.Punch,
        transform.position);
    } 

    private void PlayHitEffect()
    {
         
    } 
}
