using System.Collections.Generic;
using UnityEngine;

public class DuckBuff : MonoBehaviour
{
    protected GameInstance cachedGameInstance;

    private DuckAbility cachedAbility;
    private DuckLocomotion cachedLocomotion;

    // =========================
    // Buff Instance
    // =========================
    private sealed class BuffInstance
    {
        public readonly EBuffID id;
        public readonly float duration;   // -1 = permanent
        public float remain;              // -1 = permanent

        public bool IsPermanent => duration < 0f;

        public BuffInstance(EBuffID _id, float _duration)
        {
            id = _id;
            duration = _duration;
            remain = _duration;
        }

        public void Reset()
        {
            if (!IsPermanent)
                remain = duration;
        }
    }
    private readonly Dictionary<EBuffID, BuffInstance> hashBuffDuration = new();
    private EWeightState weightState = EWeightState.None;
     
    protected virtual void Awake()
    {
        cachedGameInstance = GameInstance.Instance;
        cachedAbility = GetComponent<DuckAbility>();
        cachedLocomotion = GetComponent<DuckLocomotion>();
    }

    private void Update()
    {
        UpdateBuffRemainTime();
    }

    // =========================
    // Public API (유지)
    // =========================
    public void InsertBuff(EBuffID _buffId)
    { 
        if (_buffId == EBuffID._END)
            return;
         
        SuccesInsert(_buffId);
    }
    public void InsertWeightBuff(EWeightState _weightState)
    {
        if (weightState == _weightState)
            return;

        // 이전 상태 제거
        switch (weightState)
        {
            case EWeightState.Heavy:
                RemoveBuff(EBuffID.Heavy);
                break;

            case EWeightState.OverWeight:
                RemoveBuff(EBuffID.OverWeight);
                break;
        }

        weightState = _weightState;

        // 새 상태 적용
        switch (weightState)
        {
            case EWeightState.Heavy:
                InsertBuff(EBuffID.Heavy);
                break;

            case EWeightState.OverWeight:
                cachedLocomotion.DoMove(Vector3.zero);
                InsertBuff(EBuffID.OverWeight);
                break;
        }
    }
    public virtual void RemoveBuff(EBuffID _buffId)
    {
        if (!hashBuffDuration.ContainsKey(_buffId))
            return;

        hashBuffDuration.Remove(_buffId);

        BuffData buffData = cachedGameInstance.TABLE_GetBuffData(_buffId);

        if (buffData.isModifyMoveStat)
            cachedAbility.AddCorrLocoAll(-buffData.moveInfo);

        if (buffData.isModifyShotStat)
            cachedAbility.AddCorrShotAll(-buffData.shotInfo);
    }

    public bool CanMove()
    {
        return weightState != EWeightState.OverWeight;
    }
     
    protected virtual void UpdateBuffDuration(EBuffID _id, float _prev, float _cur)
    {
        // UI쪽 로직이긴함
    }
    protected virtual void SuccesInsert(EBuffID _buffId)
    {
        // 이미 존재 → 갱신 정책 (Reset)
        if (hashBuffDuration.TryGetValue(_buffId, out var exist) && exist != null)
        {
            float prev = exist.remain;
            exist.Reset();

            if (!exist.IsPermanent)
                UpdateBuffDuration(_buffId, prev, exist.remain);

            return;
        }

        BuffData buffData = cachedGameInstance.TABLE_GetBuffData(_buffId);
        float duration = buffData.isUseDuration ? buffData.duration : -1f;

        // 인스턴스 등록
        hashBuffDuration[_buffId] = new BuffInstance(_buffId, duration);

        // 효과 적용
        if (buffData.isModifyMoveStat)
            cachedAbility.AddCorrLocoAll(buffData.moveInfo);
        if (buffData.isModifyShotStat)
            cachedAbility.AddCorrShotAll(buffData.shotInfo); 
    }

    private void UpdateBuffRemainTime()
    {
        if (hashBuffDuration.Count == 0)
            return;

        float dt = Time.deltaTime;
        List<EBuffID> removeList = null;

        foreach (var kv in hashBuffDuration)
        {
            BuffInstance inst = kv.Value;
            if (inst == null || inst.IsPermanent)
                continue;

            float prev = inst.remain;
            float cur = prev - dt;

            if (cur <= 0f)
            {
                removeList ??= new List<EBuffID>();
                removeList.Add(inst.id);
                continue;
            }

            inst.remain = cur;
            UpdateBuffDuration(inst.id, prev, cur);
        }

        if (removeList != null)
        {
            for (int i = 0; i < removeList.Count; i++)
                RemoveBuff(removeList[i]);
        }
    }

}
