using System.Collections.Generic;
using UnityEngine;

// =========================
// Pool ID
// =========================
public enum EPoolId
{
    None = 0,

    // FX
    MuzzleFlash,
    Blood,
    ExploadBlood,
      
    // Projectile
    Bullet = 100,
}

// =========================
// Pool System
// =========================
public class PlayPoolSystem : MonoBehaviour
{
    [System.Serializable]
    public class PoolInfo
    {
        public EPoolId poolId;
        public GameObject prefab;
        public int preloadCount = 8;
        public int maxCapacity = 16; // 총 생성 상한
    }

    [Header("Pools")]
    [SerializeField] private List<PoolInfo> poolInfos = new();

    // free objects
    private readonly Dictionary<EPoolId, Queue<GameObject>> poolMap = new();
    // instance → poolId
    private readonly Dictionary<GameObject, EPoolId> instanceToPoolId = new();
    // poolId → info
    private readonly Dictionary<EPoolId, PoolInfo> infoMap = new();
    // poolId → created count
    private readonly Dictionary<EPoolId, int> createdCountMap = new();
    // poolId → in-pool set (중복 Return 방지)
    private readonly Dictionary<EPoolId, HashSet<GameObject>> inPoolSetMap = new();

    // =========================
    // Init / Preload
    // ========================= 
    public void PreloadPools()
    {
        infoMap.Clear();

        for (int i = 0; i < poolInfos.Count; i++)
        {
            var info = poolInfos[i];
            if (info.poolId == EPoolId.None || !info.prefab)
                continue;

            infoMap[info.poolId] = info;

            if (!poolMap.ContainsKey(info.poolId))
                poolMap[info.poolId] = new Queue<GameObject>();

            if (!inPoolSetMap.ContainsKey(info.poolId))
                inPoolSetMap[info.poolId] = new HashSet<GameObject>();

            if (!createdCountMap.ContainsKey(info.poolId))
                createdCountMap[info.poolId] = 0;

            int targetCount = Mathf.Min(info.preloadCount, info.maxCapacity);
            while (poolMap[info.poolId].Count < targetCount)
            {
                var go = CreateInstance(info.poolId);
                ReturnInternal(go, info.poolId);
            }
        }
    }

    // =========================
    // Spawn
    // =========================
    public GameObject Spawn(EPoolId poolId, Vector3 pos, Quaternion rot)
    {
        if (!poolMap.TryGetValue(poolId, out var queue))
            return null;

        GameObject go = null;

        // free object 사용
        while (queue.Count > 0)
        {
            go = queue.Dequeue();
            if (go)
                break;
        }

        // 없으면 생성 (hard cap 적용)
        if (!go)
        {
            int created = createdCountMap[poolId];
            int maxCap = infoMap[poolId].maxCapacity;

            if (created >= maxCap)
                return null; // hard cap 초과 → 생성 금지

            go = CreateInstance(poolId);
        }

        inPoolSetMap[poolId].Remove(go);

        go.transform.SetPositionAndRotation(pos, rot);
        go.SetActive(true);
        return go;
    }

    // =========================
    // Return (외부 호출)
    // =========================
    public void Return(GameObject go)
    {
        if (!go || !instanceToPoolId.TryGetValue(go, out var poolId))
        {
            Destroy(go);
            return;
        }

        // 중복 Return 방지
        if (inPoolSetMap[poolId].Contains(go))
            return;

        ReturnInternal(go, poolId);
    }

    // =========================
    // Internal Return
    // =========================
    private void ReturnInternal(GameObject go, EPoolId poolId)
    {
        go.SetActive(false);
        poolMap[poolId].Enqueue(go);
        inPoolSetMap[poolId].Add(go);
    }

    // =========================
    // Clear
    // =========================
    public void ClearAllPools()
    {
        foreach (var pair in poolMap)
        {
            var queue = pair.Value;
            while (queue.Count > 0)
            {
                var go = queue.Dequeue();
                if (go)
                    Destroy(go);
            }
        }

        poolMap.Clear();
        instanceToPoolId.Clear();
        infoMap.Clear();
        createdCountMap.Clear();
        inPoolSetMap.Clear();
    }

    // =========================
    // Create
    // =========================
    private GameObject CreateInstance(EPoolId poolId)
    {
        var info = infoMap[poolId];
        var go = Instantiate(info.prefab, transform);
        go.SetActive(false);

        instanceToPoolId.Add(go, poolId);
        createdCountMap[poolId]++;

        return go;
    }
}
