using UnityEngine;
using System.Collections.Generic;
 
public class PlayerAchievement : MonoBehaviour
{
    private PlayerStorage cachedStorage;
    private PlayerQuest cachedQuest;

    // 플레이어가 모험을 나가서 획득한 아이템 목록들 (스냅샷)
    private Dictionary<EDuckType, int> hashDuck = new();
    private Dictionary<EItemID, int> hashItem = new();
      
    private void Awake()
    {
        cachedStorage = GetComponent<PlayerStorage>();
        cachedQuest = GetComponent<PlayerQuest>();
    }
    public int GetCalculateExp()
    {
        int exp = 0;

        // ======================
        // Kill EXP
        // ======================
        {
            foreach (var pair in hashDuck)
            {
                EDuckType type = pair.Key;
                int killCount = pair.Value;

                int baseExp = type switch
                {
                    EDuckType.Farmer => 20,
                    EDuckType.Mercenary => 60,
                    EDuckType.Boxer => 30,
                    _ => 10,
                };

                exp += baseExp * killCount;
            }
        }

        // ======================
        // Item EXP (스냅샷 대비 증가분)
        // ======================

        {
            Dictionary<EItemID, int> currentItems = new();
            List<ItemBase> itemList = cachedStorage.GetItemList();
            for (int i = 0; i < itemList.Count; i++)
            {
                ItemBase item = itemList[i];
                if (!item)
                    continue;

                EItemID itemId = item.GetItemID();
                EItemType itemType = DuckUtill.GetItemTypeByItemID(itemId);

                if (itemType == EItemType.Consumable)
                    continue;

                if (!currentItems.TryAdd(itemId, 1))
                    currentItems[itemId]++;
            }

            foreach (var pair in currentItems)
            {
                int before = hashItem.TryGetValue(pair.Key, out int cnt) ? cnt : 0;
                int gained = Mathf.Max(0, pair.Value - before);
                if (gained <= 0)
                    continue;

                ItemPair itemPair = DuckUtill.GetItemPair(pair.Key);
                EItemGrade grade = itemPair.data.GetItemGrade();

                int itemExp = grade switch
                {
                    EItemGrade.Normal => 5,
                    EItemGrade.Rare => 10,
                    EItemGrade.Unique => 20,
                    EItemGrade.Epic => 40,
                    EItemGrade.Legend => 80,
                    _ => 0,
                };

                exp += itemExp * gained;
            }
        }

        return exp;
    }

    // 출격 시 스냅샷
    public void SallyForth()
    {
        hashDuck.Clear();
        hashItem.Clear();
         
        List<ItemBase> itemList = cachedStorage.GetItemList();
        for (int i = 0; i < itemList.Count; i++)
        {
            ItemBase item = itemList[i];
            if (!item)
                continue;

            EItemID itemId = item.GetItemID();
            EItemType itemType = DuckUtill.GetItemTypeByItemID(itemId);

            // Consumable 제외
            if (itemType == EItemType.Consumable)
                continue;

            if (!hashItem.TryAdd(itemId, 1))
                hashItem[itemId]++;
        }
    }
    public void Withdrawal()
    {
        hashDuck.Clear();
        hashItem.Clear();
    }
    public void KillDuck(bool isHead, EDuckType type)
    {
        if (hashDuck.TryGetValue(type, out int count))
            hashDuck[type] = count + 1;
        else
            hashDuck[type] = 1;

        // 해당 타겟 죽임 
        cachedQuest.SuccessKillTarget(isHead, type);
    }
} 
  