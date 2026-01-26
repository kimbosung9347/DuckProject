using System;
using UnityEngine;
using UnityEngine.UI;

public enum ERadiuseCollaspeType
{
    Smaller_Clear,
    Small_Clear_Bigger,
    Small_Bigger_Clear,

    Bigger_Clear,
    Bigger_Clear_Smaller,
    Bigger_Smaller_Clear,
} 

public struct FRadiuseCollaspeInfo
{
    public Vector2 startPosA;
    public Transform transformA;

    public Vector2 startPosB;
    public Transform transformB;

    public ERadiuseCollaspeType radType;
    public Action clearAction;
}

public class RadiusCollapseCanvas : MonoBehaviour
{
    private static readonly int CenterID = Shader.PropertyToID("_Center");
    private static readonly int CollapseID = Shader.PropertyToID("_Collapse");

    private enum EState
    {
        None,
        PhaseA,
        PhaseB,
    }

    // --------------------------------------------------
    // 내부 의미 고정용 PhasePlan
    // --------------------------------------------------
    private struct PhasePlan
    {
        public bool hasPhaseB;
        public bool phaseAShrink;
        public bool phaseBShrink;
        public EState clearPhase;
    }

    [SerializeField] private Image targetImage;

    private Material cachedMaterial;

    private EState state = EState.None;
    private FRadiuseCollaspeInfo curInfo;
    private PhasePlan phasePlan;

    private float elapsed;          // 시간 전용
    private bool isShrink;           // 현재 Phase의 방향
    private bool actionCalled;

    private Vector2 centerA;
    private Vector2 centerB;

    // --------------------------------------------------
    // Unity
    // --------------------------------------------------
    private void Awake()
    {
        if (!targetImage)
            targetImage = GetComponentInChildren<Image>();

        cachedMaterial = Instantiate(targetImage.material);
        targetImage.material = cachedMaterial;

        cachedMaterial.SetFloat(CollapseID, 0f);
        cachedMaterial.SetVector(CenterID, Vector2.zero);

        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (state == EState.None)
            return;

        elapsed += Time.unscaledDeltaTime;
        TickPhase(state);
    }

    // --------------------------------------------------
    // Active
    // --------------------------------------------------
    public void Active(FRadiuseCollaspeInfo info)
    {
        if (state != EState.None)
            return;

        curInfo = info;
        elapsed = 0f;
        actionCalled = false;

        ResolveCenters(ref curInfo);

        phasePlan = BuildPhasePlan(curInfo.radType);

        state = EState.PhaseA;
        isShrink = phasePlan.phaseAShrink;

        cachedMaterial.SetVector(CenterID, centerA);
        cachedMaterial.SetFloat(CollapseID, isShrink ? 0f : 1f);

        gameObject.SetActive(true);
    }

    public void Disable()
    {
        state = EState.None;
        curInfo = default;
        gameObject.SetActive(false);
    }

    // --------------------------------------------------
    // Phase Tick
    // --------------------------------------------------
    private void TickPhase(EState phase)
    {
        bool finished = isShrink ? TickShrink() : TickEnlarge();
        if (!finished)
            return;

        OnPhaseFinished(phase);
    }

    private bool TickShrink()
    {
        float[] times = { 0f, 0.3f, 0.5f, 0.7f, 1f };
        float[] values = { 0f, 0.7f, 0.7f, 0.6f, 1f };

        cachedMaterial.SetFloat(CollapseID, CalculateRatio(elapsed, times, values));
        return elapsed >= times[^1];
    }

    private bool TickEnlarge()
    {
        float[] times = { 0f, 0.3f, 0.5f, 0.7f, 1f };
        float[] values = { 1f, 0.6f, 0.6f, 0.7f, 0f };

        cachedMaterial.SetFloat(CollapseID, CalculateRatio(elapsed, times, values));
        return elapsed >= times[^1];
    }

    // --------------------------------------------------
    // Phase Control
    // --------------------------------------------------
    private void OnPhaseFinished(EState phase)
    {
        elapsed = 0f;

        TryInvokeAction(phase);

        if (HasNextPhase(phase))
        {
            SwitchToNextPhase();
            return;
        }

        Disable();
    }

    private void TryInvokeAction(EState phase)
    {
        if (actionCalled)
            return;

        if (!ShouldCallAction(curInfo.radType, phase))
            return;

        actionCalled = true;
        curInfo.clearAction?.Invoke();
    }

    private void SwitchToNextPhase()
    {
        state = EState.PhaseB;
        isShrink = phasePlan.phaseBShrink;

        cachedMaterial.SetVector(CenterID, centerB);
        cachedMaterial.SetFloat(CollapseID, isShrink ? 0f : 1f);
    }

    // --------------------------------------------------
    // Center Resolve
    // --------------------------------------------------
    private void ResolveCenters(ref FRadiuseCollaspeInfo info)
    {
        centerA = info.startPosA;
        centerB = info.startPosB;

        var cam = Camera.main;
        if (!cam)
            return;

        if (info.transformA)
        {
            var vp = cam.WorldToViewportPoint(info.transformA.position + Vector3.up * 0.1f);
            centerA = new Vector2(vp.x, vp.y);
        }

        if (info.transformB)
        {
            var vp = cam.WorldToViewportPoint(info.transformB.position + Vector3.up * 0.1f);
            centerB = new Vector2(vp.x, vp.y);
        }
    }

    // --------------------------------------------------
    // Enum Logic (함수명 유지)
    // --------------------------------------------------
    private bool IsPhaseAShrink(ERadiuseCollaspeType type)
    {
        return phasePlan.phaseAShrink;
    }

    private bool HasNextPhase(EState phase)
    {
        return phase == EState.PhaseA && phasePlan.hasPhaseB;
    }

    private bool ShouldCallAction(ERadiuseCollaspeType type, EState phase)
    {
        return phase == phasePlan.clearPhase;
    }

    private PhasePlan BuildPhasePlan(ERadiuseCollaspeType type)
    {
        return type switch
        {
            ERadiuseCollaspeType.Smaller_Clear => new PhasePlan
            {
                hasPhaseB = false,
                phaseAShrink = true,
                phaseBShrink = false,
                clearPhase = EState.PhaseA
            },
            ERadiuseCollaspeType.Bigger_Clear => new PhasePlan
            {
                hasPhaseB = false,
                phaseAShrink = false,
                phaseBShrink = true,
                clearPhase = EState.PhaseA
            },
            ERadiuseCollaspeType.Small_Clear_Bigger => new PhasePlan
            {
                hasPhaseB = true,
                phaseAShrink = true,
                phaseBShrink = false,
                clearPhase = EState.PhaseA
            },
            ERadiuseCollaspeType.Bigger_Clear_Smaller => new PhasePlan
            {
                hasPhaseB = true,
                phaseAShrink = false,
                phaseBShrink = true,
                clearPhase = EState.PhaseA
            },
            ERadiuseCollaspeType.Small_Bigger_Clear => new PhasePlan
            {
                hasPhaseB = true,
                phaseAShrink = true,
                phaseBShrink = false,
                clearPhase = EState.PhaseB
            },
            ERadiuseCollaspeType.Bigger_Smaller_Clear => new PhasePlan
            {
                hasPhaseB = true,
                phaseAShrink = false,
                phaseBShrink = true,
                clearPhase = EState.PhaseB
            },
            _ => default
        };
    }

    // --------------------------------------------------
    // Util
    // --------------------------------------------------
    private float CalculateRatio(float elapsed, float[] times, float[] values)
    {
        for (int i = 1; i < times.Length; i++)
        {
            if (elapsed <= times[i])
            {
                float t = Mathf.InverseLerp(times[i - 1], times[i], elapsed);
                return Mathf.Lerp(values[i - 1], values[i], t);
            }
        }

        return values[^1];
    }
}
