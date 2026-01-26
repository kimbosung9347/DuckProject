using UnityEngine;
using System.Collections.Generic;
using Unity.Behavior;

public class AiDetector : MonoBehaviour
{
    [Header("Detect Settings")]
    [SerializeField] private float nearRadius = 2f;
    [SerializeField] private float viewAngle = 60f;
    [SerializeField] private float viewDistance = 8f;
    [SerializeField] private float enemyKeepTime = 12f;

    [Header("Masks")]
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;

    private BehaviorGraphAgent cachedBehavior;
    private ShotInfo cachedShotInfo;

    private readonly Collider[] overlapBuffer = new Collider[16];

    // Enemy 기억
    private readonly Dictionary<Transform, float> detectedEnemySet = new();
    private Transform nearestEnemyTarget;

    // Attack 가능
    private readonly Dictionary<Transform, float> detectedAttackRangeSet = new();
    private Transform nearestAttackTarget;

    private readonly List<Transform> removeBuffer = new();

    private EDuckType myDuckType = EDuckType.End;
    private float detectTimer;
    private float cosViewAngle;
    private float nearRadiusSqr;

    // Alert State
    private bool hasAlerted = false;

    private void Awake()
    {
        cachedBehavior = GetComponent<BehaviorGraphAgent>();
        myDuckType = GetComponent<DuckAbility>().GetDuckType();

        cosViewAngle = Mathf.Cos(viewAngle * Mathf.Deg2Rad);
        nearRadiusSqr = nearRadius * nearRadius;
    }

    private void Start()
    {
        detectedEnemySet.Clear();
        detectedAttackRangeSet.Clear();
        nearestEnemyTarget = null;
        nearestAttackTarget = null;
        hasAlerted = false;
    } 

    private void Update()
    {
        detectTimer += Time.deltaTime;
        if (detectTimer < 0.1f)
            return;
        detectTimer = 0f;

        CleanupEnemySet();
        DetectTargetsBySight();
        DetectTargetsByAttackRange();
        UpdateNearestTarget();
    }

    /* =========================
     * Public API
     * ========================= */

    public Transform GetNearestTarget() => nearestEnemyTarget;
    public Transform GetNearestAttackTarget() => nearestAttackTarget;

    public void CacheShotInfo(ShotInfo shotInfo)
    {
        cachedShotInfo = shotInfo;
    }
     
    public void NotifyHitSource(DuckAttack attack)
    {
        if (!attack)
            return;

        if (!attack.TryGetComponent(out DuckAbility ability))
            return;

        var detected = attack.GetComponentInChildren<DuckDetected>();
        if (!detected)
            return;

        Transform targetTransform = detected.transform;
        if (detectedEnemySet.ContainsKey(targetTransform))
            return;

        EDuckRelation relation =
            GameInstance.Instance.TABLE_GetDuckRelation(myDuckType, ability.GetDuckType());

        if (relation != EDuckRelation.Hostile)
            return;

        RefreshEnemy(targetTransform);
    }

    /* =========================
     * Detect
     * ========================= */

    private void DetectTargetsBySight()
    {
        Vector3 pos = transform.position;
        Vector3 forward = transform.forward;

        int count = Physics.OverlapSphereNonAlloc(
            pos,
            viewDistance,
            overlapBuffer,
            targetMask
        );

        var gameInstance = GameInstance.Instance;

        for (int i = 0; i < count; i++)
        {
            var col = overlapBuffer[i];
            if (!col)
                continue;

            var detected = col.GetComponent<DuckDetected>();
            if (!detected)
                continue;

            EDuckRelation relation =
                gameInstance.TABLE_GetDuckRelation(myDuckType, detected.GetDuckType());

            if (relation != EDuckRelation.Hostile)
                continue;

            Transform t = detected.transform;
            Vector3 diff = t.position - pos;
            float sqr = diff.sqrMagnitude;

            // 근접 즉시 감지
            if (sqr <= nearRadiusSqr)
            {
                if (HasLineOfSight(pos, diff))
                    RefreshEnemy(t);
                continue;
            }

            // 시야각
            float dot = Vector3.Dot(forward, diff) / Mathf.Sqrt(sqr);
            if (dot < cosViewAngle)
                continue;

            if (HasLineOfSight(pos, diff))
                RefreshEnemy(t);
        }
    }

    private void DetectTargetsByAttackRange()
    {
        detectedAttackRangeSet.Clear();
        if (cachedShotInfo == null)
            return;

        Vector3 pos = transform.position;
        float rangeSqr = cachedShotInfo.attackRange * cachedShotInfo.attackRange;

        foreach (var kv in detectedEnemySet)
        {
            Transform t = kv.Key;
            if (!t)
                continue;

            Vector3 diff = t.position - pos;
            if (diff.sqrMagnitude > rangeSqr)
                continue;

            if (!HasLineOfSight(pos, diff))
                continue;

            detectedAttackRangeSet[t] = kv.Value;
        }
    }

    private void RefreshEnemy(Transform t)
    {
        if (!hasAlerted)
        {
            hasAlerted = true; 
            GetComponent<DuckSpeechBubble>().ActiveAutoDeleteSpeech("적 발견!");
            GameInstance.Instance.SOUND_PlayDuckQuack(myDuckType, transform.position);
        }

        detectedEnemySet[t] = Time.time + enemyKeepTime;
    }

    /* =========================
     * Cleanup
     * ========================= */

    private void CleanupEnemySet()
    {
        float now = Time.time;
        removeBuffer.Clear();

        foreach (var kv in detectedEnemySet)
        {
            Transform t = kv.Key;
            if (!t || kv.Value < now)
                removeBuffer.Add(t);
        } 

        for (int i = 0; i < removeBuffer.Count; i++)
            detectedEnemySet.Remove(removeBuffer[i]);

        if (detectedEnemySet.Count == 0)
            hasAlerted = false;
    }

    /* =========================
     * Target Select
     * ========================= */

    private void UpdateNearestTarget()
    {
        nearestEnemyTarget = null;
        nearestAttackTarget = null;

        Vector3 pos = transform.position;
        float minEnemySqr = float.MaxValue;
        float minAttackSqr = float.MaxValue;

        foreach (var t in detectedEnemySet.Keys)
        {
            if (!t) continue;

            float sqr = (t.position - pos).sqrMagnitude;
            if (sqr < minEnemySqr)
            {
                minEnemySqr = sqr;
                nearestEnemyTarget = t;
            }
        }

        foreach (var t in detectedAttackRangeSet.Keys)
        {
            if (!t) continue;

            float sqr = (t.position - pos).sqrMagnitude;
            if (sqr < minAttackSqr)
            {
                minAttackSqr = sqr;
                nearestAttackTarget = t;
            }
        }

        if (cachedBehavior)
        {
            cachedBehavior.SetVariableValue("isFoundEnemy", nearestEnemyTarget != null);
            cachedBehavior.SetVariableValue("isEnemyInAttRange", nearestAttackTarget != null);
        }
    } 

    /* =========================
     * Utils
     * ========================= */

    private bool HasLineOfSight(Vector3 origin, Vector3 diff)
    {
        float dist = diff.magnitude;
        return !Physics.Raycast(
            origin, 
            diff / dist,
            dist,
            obstacleMask,
            QueryTriggerInteraction.Ignore
        );
    }
}
