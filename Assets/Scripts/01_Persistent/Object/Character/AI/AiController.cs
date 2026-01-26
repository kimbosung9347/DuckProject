using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

[BlackboardEnum]
public enum EAIState
{
    Patrol,
    Chase,
    Attack,
}
public enum EAttackMoveIntent
{
    Strafe,
    Approach,
    Retreat,
} 

public class AiController : MonoBehaviour
{
    [SerializeField] private List<Transform> listPatrolTransform;
    [SerializeField] private LayerMask obstacleMask;
     
    private NavMeshAgent cachedNavMesh;
    private AiAiming cachedAiming;
    private DuckLocomotion cachedLocomotion;

    private CapsuleCollider cachedCollider;
    private Vector3 shotPos = Vector3.zero;

    private void Awake()
    {
        cachedCollider = GetComponent<CapsuleCollider>();
        cachedNavMesh = GetComponent<NavMeshAgent>();
        cachedAiming = GetComponent<AiAiming>();
        cachedLocomotion = GetComponent<DuckLocomotion>();

        cachedNavMesh.updatePosition = false;
        cachedNavMesh.updateRotation = false;
        cachedNavMesh.updateUpAxis = true;
    }
    private void LateUpdate()
    {
        cachedNavMesh.nextPosition = transform.position;
    }

    public bool IsArriveTargetPos(Transform _transform, float arriveDist = 0.3f)
    {
        if (!_transform)
            return false;

        Vector3 cur = transform.position;
        Vector3 trg = _transform.position;
        cur.y = trg.y = 0f;

        return (cur - trg).sqrMagnitude <= arriveDist * arriveDist;
    }
    public bool IsArriveTargetPos(float arriveDist = 0.5f) 
    {
        if (!cachedNavMesh.hasPath)
            return false;

        Vector3 trg = cachedNavMesh.destination;
        Vector3 cur = transform.position;
        cur.y = trg.y = 0f;

        return (cur - trg).sqrMagnitude <= arriveDist * arriveDist;
    }

    public Vector3 ResolveAttackMovePosition(Transform target, EAttackMoveIntent intent)
    {
        Vector3 origin = transform.position;
        if (!target)
            return origin;

        switch (intent)
        {
            // ==================================================
            // STRAFE : 시선 기준 좌/우 이동
            // ==================================================
            case EAttackMoveIntent.Strafe:
                {
                    Vector3 lookDir = cachedAiming
                        ? cachedAiming.GetLookDir()
                        : transform.forward;

                    lookDir.y = 0f;
                    if (lookDir.sqrMagnitude < 0.0001f)
                        return origin;

                    lookDir.Normalize();

                    Vector3 side = Vector3.Cross(Vector3.up, lookDir);
                    float dir = Random.value > 0.5f ? 1f : -1f;
                    float distance = 3f;

                    Vector3 intentPos = origin + side * dir * distance;
                    intentPos.y = origin.y;

                    return ResolveNavMeshMove(origin, intentPos);
                }

            // ==================================================
            // APPROACH : 타겟 쪽으로 접근
            // ==================================================
            case EAttackMoveIntent.Approach:
                {
                    Vector3 dir = target.position - origin;
                    dir.y = 0f;

                    if (dir.sqrMagnitude < 0.0001f)
                        return origin;

                    dir.Normalize();

                    float distance = 3f;
                    Vector3 intentPos = origin + dir * distance;
                    intentPos.y = origin.y;

                    return ResolveNavMeshMove(origin, intentPos);
                }

            // ==================================================
            // RETREAT : 타겟 반대 방향으로 후퇴
            // ==================================================
            case EAttackMoveIntent.Retreat:
                {
                    Vector3 dir = origin - target.position;
                    dir.y = 0f;

                    if (dir.sqrMagnitude < 0.0001f)
                        return origin;

                    dir.Normalize();

                    float distance = 3f;
                    Vector3 intentPos = origin + dir * distance;
                    intentPos.y = origin.y;

                    return ResolveNavMeshMove(origin, intentPos);
                }
        }
         
        return origin;
    }


    public Transform SetPatrolDestination(Transform _excludeTransform)
    {
        Transform target = GetRandomPatrolTransform(_excludeTransform);
        if (target)
            cachedNavMesh.SetDestination(target.position);
        return target;
    }
    public void SetDestinaion(Transform _tf)
    {
        if (_tf)
            cachedNavMesh.SetDestination(_tf.position);
    }
    public void SetDestinaion(Vector3 _pos)
    {
        cachedNavMesh.SetDestination(_pos);
    }

    public void SetShotPos(Vector3 _pos)
    {
        shotPos = _pos;
    }
    public void DisableShotPos()
    {
        shotPos = Vector3.zero;
    }
    public Vector3 GetShotPos()
    {
        return shotPos;
    }
    public void DoStop()
    {
        cachedNavMesh.SetDestination(transform.position);
        cachedLocomotion.DoMove(Vector3.zero);
    }  
    public void UpdateDir(bool _isAiming = true)
    {
        if (cachedNavMesh.pathPending || !cachedNavMesh.hasPath)
        {
            cachedLocomotion.DoMove(Vector3.zero);
            return;
        }

        Vector3 target = cachedNavMesh.steeringTarget;
        Vector3 moveDir = target - transform.position;
        moveDir.y = 0f;

        if (moveDir.sqrMagnitude > 0.0001f)
        {
            moveDir.Normalize();
        }
         
        else
        {
            moveDir = Vector3.zero;
        }

        if (_isAiming)
            cachedAiming.SetLookDir(moveDir);

        cachedLocomotion.DoMove(moveDir);
    }
     
    private Vector3 ResolveNavMeshMove(Vector3 origin, Vector3 intentPos)
    {
        // ======================
        // NavMesh 스냅
        // ======================
        if (!NavMesh.SamplePosition(intentPos, out NavMeshHit snapHit, 2.0f, NavMesh.AllAreas))
            return origin;

        if (!IsValidStandPosition(snapHit.position))
            return origin;

        // ======================
        // NavMesh 직선 이동 가능
        // ======================
        if (!NavMesh.Raycast(origin, snapHit.position, out NavMeshHit blockHit, NavMesh.AllAreas))
        {
            cachedNavMesh.SetDestination(snapHit.position);
            return snapHit.position;
        }

        // ======================
        // 막힌 지점 fallback
        // ======================
        Vector3 fallback = blockHit.position;
        fallback.y = origin.y;

        if (!IsValidStandPosition(fallback))
            return origin;

        cachedNavMesh.SetDestination(fallback);
        return fallback;
    }

    private Transform GetRandomPatrolTransform(Transform _excludeTransform)
    {
        if (listPatrolTransform == null || listPatrolTransform.Count == 0)
            return null;

        if (!_excludeTransform)
            return listPatrolTransform[Random.Range(0, listPatrolTransform.Count)];

        List<Transform> candidates = new();
        for (int i = 0; i < listPatrolTransform.Count; i++)
        {
            var t = listPatrolTransform[i];
            if (t && t != _excludeTransform)
                candidates.Add(t);
        }

        if (candidates.Count == 0)
            return null;

        return candidates[Random.Range(0, candidates.Count)];
    }
     
    private bool IsValidStandPosition(Vector3 pos)
    {
        if (!cachedCollider)
            return true;

        Transform ct = cachedCollider.transform;

        // CapsuleCollider → 월드 기준 반영
        float radius = cachedCollider.radius *
                       Mathf.Max(ct.lossyScale.x, ct.lossyScale.z);

        float height = cachedCollider.height * ct.lossyScale.y;

        Vector3 centerOffset = Vector3.Scale(
            cachedCollider.center,
            ct.lossyScale
        );

        Vector3 worldCenter = pos + transform.rotation * centerOffset;

        Vector3 bottom = worldCenter + Vector3.up * radius;
        Vector3 top = bottom + Vector3.up * (height - radius * 2f);

        return !Physics.CheckCapsule(
            bottom,
            top,
            radius,
            obstacleMask, // 필요하면 여기서 obstacleMask로 제한 가능 // obstacleMask로
            QueryTriggerInteraction.Ignore 
        );
    } 
}
