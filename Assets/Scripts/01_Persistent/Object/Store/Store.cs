using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct FCategoryCount
{
    public EItemCatergory category;
    public int count;
}
 
public class Store : MonoBehaviour
{
    private const int TOTAL_COUNT = 30;

    [SerializeField] private List<FCategoryCount> categoryCounts = new();
    [SerializeField] private Transform itemListTransform;
    [SerializeField] private float maxRefreshTime = 30f;

    [SerializeField] private float playerSellRatio = 1.0f;
    [SerializeField] private float storeSellRatio = 1.0f; 
     
    private readonly Dictionary<EItemCatergory, int> categoryCnt = new();
    private readonly ItemBase[] items = new ItemBase[TOTAL_COUNT];

    private StoreCanvas cachedStoreCanvas;
    private float refreshTime = 0f;

    // =========================
    // Unity LifeCycle
    // =========================

    private void OnValidate()
    {
        if (categoryCounts == null || categoryCounts.Count == 0)
            return;

        int sum = 0;
        int lastIndex = categoryCounts.Count - 1;

        for (int i = 0; i < categoryCounts.Count; i++)
            sum += Mathf.Max(0, categoryCounts[i].count);

        int diff = TOTAL_COUNT - sum;
        if (diff != 0 && lastIndex >= 0)
        {
            var last = categoryCounts[lastIndex];
            last.count = Mathf.Max(0, last.count + diff);
            categoryCounts[lastIndex] = last;
        }
    }
    private void Awake()
    {
        cachedStoreCanvas = GameInstance.Instance
            .UI_GetPersistentUIGroup()
            .GetStoreCanvas();

        BuildCategoryDict();
        ClearAllItems();
    }
    private void Start()
    {
        RefreshItems();
    }

    private void Update()
    {
        if (refreshTime <= 0f)
            return;

        refreshTime -= Time.deltaTime;
        if (refreshTime < 0f)
            refreshTime = 0f;
    }

    // =========================
    // Internal
    // =========================

    private void BuildCategoryDict()
    {
        categoryCnt.Clear();

        foreach (var e in categoryCounts)
        {
            int cnt = Mathf.Max(0, e.count);

            if (categoryCnt.ContainsKey(e.category))
                categoryCnt[e.category] += cnt;
            else
                categoryCnt.Add(e.category, cnt);
        }
    }
    private void ClearAllItems()
    {
        for (int i = 0; i < TOTAL_COUNT; i++)
        {
            Destroy(items[i]);
            items[i] = null;
        } 
    } 

    // =========================
    // Public API
    // =========================

    public void RefreshItems()
    {
        refreshTime = maxRefreshTime;
        BuildCategoryDict();
        ClearAllItems();

        var itemTable = GameInstance.Instance.TABLE_GetItemTable();
        int writeIndex = 0;

        foreach (var pair in categoryCnt)
        {
            if (writeIndex >= TOTAL_COUNT)
                break;

            int needCount = pair.Value;
            if (needCount <= 0)
                continue;

            var pool = itemTable.GetItemPool(pair.Key);
            if (pool == null || pool.Count == 0)
                continue;

            // 풀 복사 + 셔플
            List<EItemID> tempPool = new(pool);
            Shuffle(tempPool);

            int spawnCount = Mathf.Min(needCount, tempPool.Count);

            // Grade 오름차순 정렬
            tempPool.Sort(0, spawnCount, Comparer<EItemID>.Create((a, b) =>
            {
                var ga = itemTable.GetItemPair(a).data.grade;
                var gb = itemTable.GetItemPair(b).data.grade;
                return ga.CompareTo(gb);
            }));

            for (int i = 0; i < spawnCount && writeIndex < TOTAL_COUNT; i++)
            {
                var item = GameInstance.Instance.SPAWN_MakeItem(tempPool[i]);
                if (item == null)
                    continue;

                item.Insert(itemListTransform);
                items[writeIndex++] = item;
            }
        }

        cachedStoreCanvas?.Refresh();
    }
     
    public float GetRefreshTime() { return refreshTime; }
    public bool CanInputRefresh()
    {
        return refreshTime <= 0f;
    }
    public bool IsExistItem(int index)
    {
        if (index < 0 || index >= TOTAL_COUNT)
            return false;

        return items[index] != null;
    }
     
    public int GetPlayerSellPrice(int _backpackItemPrice)
    {
        return Mathf.FloorToInt(_backpackItemPrice * playerSellRatio);
    }  
    public int GetStoreSellPrice(int _storeItemPrice)
    {
        return Mathf.CeilToInt(_storeItemPrice * storeSellRatio);
    } 


    public ItemBase[] GetItemList()
    {
        return items;
    }
    public ItemBase GetItem(int index)
    {
        if (index < 0 || index >= TOTAL_COUNT)
            return null;

        return items[index];
    }
    public ItemBase SellItem(int index)
    {
        if (!IsExistItem(index))
            return null;

        ItemBase item = items[index];
        items[index] = null;

        cachedStoreCanvas?.RenewEmpty(index);
        return item;
    }

 
    // =========================
    // Utils
    // =========================

    private static void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
} 
 