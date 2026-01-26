using UnityEngine;
using System.Collections;

public class PlayerSight : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Material darkFovMat;
    [SerializeField] private PlayerConeSight coneSight;
    [SerializeField] private MeshFilter circleSight;
    [SerializeField] private LayerMask enemyLayerMask;

    [Header("Base Values")]
    [SerializeField] private float coneBaseDistance = 10f;

    [Header("Day Base")]
    [SerializeField] private float dayConeFov = 90f;
    [SerializeField] private float dayDarkOut = 0.3f;
    [SerializeField] private float dayDarkIn = 0.1f;

    [Header("Night Base")]
    [SerializeField] private float nightConeFov = 70f;
    [SerializeField] private float nightDarkOut = 0.75f;
    [SerializeField] private float nightDarkIn = 0.35f;

    [Header("Transition")]
    [SerializeField] private float transitionDuration = 3f;
    [SerializeField]
    private AnimationCurve transitionCurve =
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    // ================= Runtime =================

    // Base (환경)
    private float baseFov;
    private float baseDarkOut;
    private float baseDarkIn;

    // Corr (버프 / 디버프 누적)
    private float corrFov;
    private float corrDist;
    private float corrDarkOut;
    private float corrDarkIn;

    private Coroutine transitionRoutine;
    private float circleDetectTimer;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying)
            return;

        ApplyFinal();
    }
#endif

    private void Awake()
    {
        // 기본 Day 상태로 시작
        baseFov = dayConeFov;
        baseDarkOut = dayDarkOut;
        baseDarkIn = dayDarkIn;

        ApplyFinal();
    }
    private void Update()
    {
        circleDetectTimer += Time.deltaTime;
        if (circleDetectTimer >= 0.1f)
        {
            circleDetectTimer = 0f;
            DetectEnemiesInCircle();
        }
    }

    // ================= Day / Night =================

    public void ChangeDay(bool isInstant = false)
    {
        SetBase(dayConeFov, dayDarkOut, dayDarkIn, isInstant);
    }

    public void ChangeNight(bool isInstant = false)
    {
        SetBase(nightConeFov, nightDarkOut, nightDarkIn, isInstant);
    }

    // ❗ API 유지: "Add" 처럼 보이지만 실제로는 Base 목표 변경
    public void AddBaseSight(float fov, float darkOut, float darkIn)
    {
        SetBase(
            baseFov + fov,
            baseDarkOut + darkOut,
            baseDarkIn + darkIn,
            false
        );
    }

    private void SetBase(
        float targetFov,
        float targetDarkOut,
        float targetDarkIn,
        bool instant
    )
    {
        if (transitionRoutine != null)
            StopCoroutine(transitionRoutine);

        if (instant)
        {
            baseFov = targetFov;
            baseDarkOut = targetDarkOut;
            baseDarkIn = targetDarkIn;
            ApplyFinal();
        }
        else
        {
            transitionRoutine = StartCoroutine(
                TransitionBase(
                    baseFov,
                    baseDarkOut,
                    baseDarkIn,
                    targetFov,
                    targetDarkOut,
                    targetDarkIn
                )
            );
        }
    }

    // ================= Buff / Debuff =================

    public void AddConeFov(float add)
    {
        corrFov += add;
        ApplyFinal();
    }

    public void AddConeDistance(float add)
    {
        corrDist += add;
        ApplyFinal();
    }

    public void AddDark(float outAdd, float inAdd)
    {
        corrDarkOut += outAdd;
        corrDarkIn += inAdd;
        ApplyFinal();
    }

    // ================= Apply =================

    private void ApplyFinal()
    {
        if (!coneSight)
            return;

        coneSight.SetFov(baseFov + corrFov);
        coneSight.SetDistance(coneBaseDistance + corrDist);

        darkFovMat.SetFloat("_DarknessOut", baseDarkOut + corrDarkOut);
        darkFovMat.SetFloat("_DarknessIn", baseDarkIn + corrDarkIn);
    }

    private IEnumerator TransitionBase(
        float fromFov,
        float fromDarkOut,
        float fromDarkIn,
        float toFov,
        float toDarkOut,
        float toDarkIn
    )
    {
        float time = 0f;

        while (time < transitionDuration)
        {
            float t = transitionCurve.Evaluate(time / transitionDuration);

            baseFov = Mathf.Lerp(fromFov, toFov, t);
            baseDarkOut = Mathf.Lerp(fromDarkOut, toDarkOut, t);
            baseDarkIn = Mathf.Lerp(fromDarkIn, toDarkIn, t);

            ApplyFinal();

            time += Time.deltaTime;
            yield return null;
        }

        baseFov = toFov;
        baseDarkOut = toDarkOut;
        baseDarkIn = toDarkIn;
        ApplyFinal();
    }

    // ================= Dark FOV =================

    public void ActiveDarkFov(bool isActive)
    {
        darkFovMat?.SetFloat("_Active", isActive ? 1f : 0f);
    }

    // ================= Circle Detect =================

    private void DetectEnemiesInCircle()
    {
        float radius = GetCircleWorldRadius();
        if (radius <= 0f)
            return;

        Vector3 origin = transform.position;

        Collider[] hits = Physics.OverlapSphere(
            origin,
            radius,
            enemyLayerMask
        );

        for (int i = 0; i < hits.Length; i++)
        {
            hits[i]
                .GetComponentInParent<AiDetected>()
                ?.ActiveAiRenderer();
        }
    }
     
    private float GetCircleWorldRadius()
    {
        if (!circleSight || !circleSight.sharedMesh)
            return 0f;

        Bounds b = circleSight.sharedMesh.bounds;
        float localRadius = Mathf.Max(b.extents.x, b.extents.z);
        Vector3 scale = circleSight.transform.lossyScale;

        return localRadius * Mathf.Max(scale.x, scale.z);
    }
}
