using System;
using System.Collections.Generic;
using UnityEngine;
 
public enum EItemCatergory
{
    Weapon,
    Armor,
    Backpack,
    Attachment,
    Bullet,
    Heal,
    Food,
    Stuff
}
 
[System.Serializable]
public class ItemPair
{
    public ItemData data;
    public ItemVisualData visual;

    public ItemPair(ItemData _data, ItemVisualData _visual)
    {
        data = _data;
        visual = _visual; 
    }
} 

public class ItemTable : MonoBehaviour
{  
    [Header("등급별 색상")]
    [SerializeField] public Color gradeNormalColor   = Color.black;
    [SerializeField] public Color gradeRareColor     = new Color(0.3f, 0.6f, 1f, 207/255f);
    [SerializeField] public Color gradeUniqueColor   = new Color(1f, 0.8f, 0.2f, 207/255f);
    [SerializeField] public Color gradeEpicColor     = new Color(1f, 0.8f, 0.2f, 207/255f);
    [SerializeField] public Color gradeLegendColor   = new Color(1f, 0.8f, 0.2f, 207/255f);

    [Header("내구도 색상")]
    [SerializeField] public Color durabilityUp75 = Color.black;
    [SerializeField] public Color durabilityBetween25_75 = new Color(0.3f, 0.6f, 1f, 207 / 255f);
    [SerializeField] public Color durabilityUnder25 = new Color(1f, 0.8f, 0.2f, 207 / 255f);
      
    [Header("부착물 상태별 색상")]
    [SerializeField] public Color attachAtiveColor;
    [SerializeField] public Color attachEmptyColor;
    [SerializeField] public Color attachDisableColor;

    [Header("선택된 아이템슬롯 테두리 색상")]
    [SerializeField] public Color selectBorderColor;


    private readonly Dictionary<EItemID, ItemPair> hashItemData = new();
    
    // 무기
    private readonly Dictionary<EWeaponType, Dictionary<EItemGrade, List<EItemID>>> hashWeaponByTypeGrade = new();
    // 방어구
    private readonly Dictionary<EArmorType, Dictionary<EItemGrade, List<EItemID>>> hashArmorByTypeGrade = new();
    // 가방
    private readonly Dictionary<EItemGrade, List<EItemID>> hashBackpackByGrade = new();
    // 부착물
    private readonly Dictionary<EItemGrade, List<EItemID>> hashAttachByGrade = new(); 
    // 총알
    private readonly Dictionary<EItemGrade, EItemID> hashBulletByGrade = new();
    // 회복
    private readonly Dictionary<EItemGrade, List<EItemID>> hashHealByGrade = new();
    // 음식 
    private readonly Dictionary<EItemGrade, List<EItemID>> hashFoodByGrade = new();
    // 재료 
    private readonly Dictionary<EItemGrade, List<EItemID>> hashStufflByGrade = new();
    // 모든 아이디
    private readonly Dictionary<EItemCatergory, List<EItemID>> hashAllItemList = new();

    private void Awake()
    { 
        CacheItemTable();
    } 

    public ItemPair GetItemPair(EItemID id)
    {
        if (hashItemData.TryGetValue(id, out var pair))
            return pair;
         
        Debug.LogWarning($"[ItemTable] 존재하지 않는 아이템 ID 요청: {id}");

        return null;
    }
    public ItemPair GetRandomWeaponDataPair(EWeaponType _type, EItemGrade _grade)
    { 
        // 무기는 총기 종류별로 등급이 모두 존재하지 않음 -> 구현안했거든, 그에 대한 예외처리
        if (!hashWeaponByTypeGrade.TryGetValue(_type, out var gradeDict))
            return null;

        // 요청 grade
        if (TryPick(_type, gradeDict, _grade, out var pair))
            return pair;

        // 하향
        for (int g = (int)_grade - 1; g >= (int)EItemGrade.Normal; g--)
        {
            if (TryPick(_type, gradeDict, (EItemGrade)g, out pair))
                return pair;
        } 

        // 상향
        for (int g = (int)_grade + 1; g <= (int)EItemGrade.Legend; g++)
        {
            if (TryPick(_type, gradeDict, (EItemGrade)g, out pair))
                return pair;
        }
         
        return null;
    }
    public ItemPair GetRandomArmorDataPair(EArmorType _type, EItemGrade _grade)
    {
        if (!hashArmorByTypeGrade.TryGetValue(_type, out var gradeDict))
            return null; 

        if (!gradeDict.TryGetValue(_grade, out var idList) || idList.Count == 0)
            return null;

        EItemID randomID = idList[UnityEngine.Random.Range(0, idList.Count)];
        return GetItemPair(randomID);
    }
    public ItemPair GetBulletDataPair(EItemGrade grade)
    {
        if (!hashBulletByGrade.TryGetValue(grade, out var id))
            return null;

        return GetItemPair(id);
    }
    public ItemPair GetHealDataPair(EItemGrade grade)
    {
        if (!hashHealByGrade.TryGetValue(grade, out var idList) || idList.Count == 0)
            return null;
         
        // 리스트에서 랜덤 1개 선택
        EItemID randomID = idList[UnityEngine.Random.Range(0, idList.Count)];
        return GetItemPair(randomID);
    }
    public ItemPair GetFoodDataPair(EItemGrade grade)
    {
        if (!hashFoodByGrade.TryGetValue(grade, out var idList) || idList.Count == 0)
            return null;
         
        // 리스트에서 랜덤 1개 선택
        EItemID randomID = idList[UnityEngine.Random.Range(0, idList.Count)];
        return GetItemPair(randomID);
    } 
    public ItemPair GetBackpackDataPair(EItemGrade grade)
    {
        if (!hashBackpackByGrade.TryGetValue(grade, out var idList) || idList.Count == 0)
            return null;
         
        // 리스트에서 랜덤 1개 선택
        EItemID randomID = idList[UnityEngine.Random.Range(0, idList.Count)];
        return GetItemPair(randomID);
    }
    public ItemPair GetAttachmentDataPair(EItemGrade grade)
    {
        if (!hashAttachByGrade.TryGetValue(grade, out var idList) || idList.Count == 0)
            return null;
         
        // 리스트에서 랜덤 1개 선택
        EItemID randomID = idList[UnityEngine.Random.Range(0, idList.Count)];
        return GetItemPair(randomID);
    }
    public ItemPair GetStuffDataPair(EItemGrade grade)
    {
        if (!hashStufflByGrade.TryGetValue(grade, out var idList) || idList.Count == 0)
            return null;

        // 리스트에서 랜덤 1개 선택
        EItemID randomID = idList[UnityEngine.Random.Range(0, idList.Count)];
        return GetItemPair(randomID);
    }  
     
    public List<EItemID> GetItemPool(EItemCatergory category)
    {
        return hashAllItemList.TryGetValue(category, out var list) ? list : null;
    }

    private void CacheItemTable()
    {
        InitAllItemCategory();
         
        //아이템 데이터 및 비주얼 로드
        ItemData[] allItemDatas = Resources.LoadAll<ItemData>("Data/Item");
        ItemVisualData[] allItemVisuals = Resources.LoadAll<ItemVisualData>("Data/Item");
         
        // 비주얼 딕셔너리
        Dictionary<EItemID, ItemVisualData> visualLookup = new();
        foreach (var visual in allItemVisuals)
        {
            if (visual == null)
                continue;

            if (visualLookup.TryGetValue(visual.itemID, out var exist))
            {
                Debug.LogWarning(
                    $"[ItemTable] Visual ID 중복 감지: {visual.itemID}\n" +
                    $"- Existing : {exist.name}\n" +
                    $"- Duplicate: {visual.name}"
                );
                continue;
            } 

            visualLookup.Add(visual.itemID, visual);
        }
         
        // 아이템 데이터 기준으로 캐싱
        foreach (var item in allItemDatas)
        {
            if (item == null) 
                continue;
             
            // 등급 계산
            EItemGrade grade = item.GetItemGrade();
            if (grade == EItemGrade.None)
            {
                grade = CalcGradeByPrice(item.price);
                item.SetItemGrade(grade);
            }  
  
            // 중복 체크
            if (hashItemData.ContainsKey(item.itemID))
            {
                Debug.LogWarning($"[ItemTable] 중복된 ItemID 감지: {item.itemID}");
                continue;
            }
              
            // 비주얼 연결
            visualLookup.TryGetValue(item.itemID, out var visual);

            // Pair로 묶어서 캐싱
            var pair = new ItemPair(item, visual);
            hashItemData.Add(item.itemID, pair);

            // 무기
            if (item is WeaponData weapon)
            {
                CacheWeaponByTypeAndGrade(weapon.weaponType, grade, item.itemID);
                hashAllItemList[EItemCatergory.Weapon].Add(item.itemID);
            }

            // 방어구
            else if (item is ArmorData armor)
            {
                CacheArmorByTypeAndGrade(armor.armorType, grade, item.itemID);
                hashAllItemList[EItemCatergory.Armor].Add(item.itemID);
            }

            // 총알
            else if (item is BulletData bullet)
            {
                CacheBulletByGrade(grade, item.itemID);
                hashAllItemList[EItemCatergory.Bullet].Add(item.itemID);
            }

            // 회복
            else if (item is HealData heal)
            {
                CacheHealByGrade(grade, item.itemID);
                hashAllItemList[EItemCatergory.Heal].Add(item.itemID);
            }

            // 음식
            else if (item is FoodData food)
            {
                CacheFoodByGrade(grade, item.itemID);
                hashAllItemList[EItemCatergory.Food].Add(item.itemID);
            }

            // 가방
            else if (item is BackpackData backpack)
            {
                CacheBackpackByGrade(grade, item.itemID);
                hashAllItemList[EItemCatergory.Backpack].Add(item.itemID);
            }
             
            // 부착물
            else if (item is AttachmentData attachment)
            {
                CacheAttachmentByGrade(grade, item.itemID);
                hashAllItemList[EItemCatergory.Attachment].Add(item.itemID);
            }

            // 재료
            else if (item is StuffData stuff)
            {
                CacheStuffByGrade(grade, item.itemID);
                hashAllItemList[EItemCatergory.Stuff].Add(item.itemID);
            }
        }  
    }
    private void InitAllItemCategory()
    {
        hashAllItemList.Clear();

        foreach (EItemCatergory cat in Enum.GetValues(typeof(EItemCatergory)))
        {
            hashAllItemList.Add(cat, new List<EItemID>());
        }
    }

    // 무기 캐싱
    private void CacheWeaponByTypeAndGrade(EWeaponType weaponType, EItemGrade grade, EItemID id)
    {
        if (!hashWeaponByTypeGrade.TryGetValue(weaponType, out var gradeDict))
        {
            gradeDict = new Dictionary<EItemGrade, List<EItemID>>();
            hashWeaponByTypeGrade.Add(weaponType, gradeDict);
        }

        if (!gradeDict.TryGetValue(grade, out var idList))
        {
            idList = new List<EItemID>();
            gradeDict.Add(grade, idList);
        } 

        idList.Add(id);
    }
    // 방어구 캐싱
    private void CacheArmorByTypeAndGrade(EArmorType armorType, EItemGrade grade, EItemID id)
    {
        if (!hashArmorByTypeGrade.TryGetValue(armorType, out var gradeDict))
        {
            gradeDict = new Dictionary<EItemGrade, List<EItemID>>();
            hashArmorByTypeGrade.Add(armorType, gradeDict);
        }

        if (!gradeDict.TryGetValue(grade, out var idList))
        {
            idList = new List<EItemID>();
            gradeDict.Add(grade, idList);
        }

        idList.Add(id);
    }
    // 총알 캐싱  
    private void CacheBulletByGrade(EItemGrade grade, EItemID id)
    {
        if (hashBulletByGrade.ContainsKey(grade))
        {
            return;
        }

        hashBulletByGrade.Add(grade, id);
    }
    // 힐템 캐싱 
    private void CacheHealByGrade(EItemGrade grade, EItemID id)
    {
        if (!hashHealByGrade.TryGetValue(grade, out var list))
        {
            list = new List<EItemID>();
            hashHealByGrade.Add(grade, list);
        }

        list.Add(id);
    }
    // 음식 캐싱  
    private void CacheFoodByGrade(EItemGrade grade, EItemID id)
    {
        if (!hashFoodByGrade.TryGetValue(grade, out var list))
        {
            list = new List<EItemID>();
            hashFoodByGrade.Add(grade, list);
        }
         
        list.Add(id);
    }
    // 가방 캐싱  
    private void CacheBackpackByGrade(EItemGrade grade, EItemID id)
    {
        if (!hashBackpackByGrade.TryGetValue(grade, out var list))
        {
            list = new List<EItemID>();
            hashBackpackByGrade.Add(grade, list);
        }

        list.Add(id);
    }
    // 부착물 캐싱  
    private void CacheAttachmentByGrade(EItemGrade grade, EItemID id)
    {
        if (!hashAttachByGrade.TryGetValue(grade, out var list))
        {
            list = new List<EItemID>();
            hashAttachByGrade.Add(grade, list);
        }

        list.Add(id);
    }
    // 재료 캐싱  
    private void CacheStuffByGrade(EItemGrade grade, EItemID id)
    {
        if (!hashStufflByGrade.TryGetValue(grade, out var list))
        {
            list = new List<EItemID>();
            hashStufflByGrade.Add(grade, list);
        }
         
        list.Add(id);
    }
     


    // ===========================================
    // Utility
    // ===========================================
    private EItemGrade CalcGradeByPrice(int price)
    {
        if (price < 1000)
            return EItemGrade.Normal;
        else if (price < 5000) 
            return EItemGrade.Rare;
        else
            return EItemGrade.Unique;
    }
    private bool TryPick(EWeaponType type, Dictionary<EItemGrade, List<EItemID>> gradeDict, EItemGrade grade, out ItemPair pair)
    {
        pair = null;

        if (!gradeDict.TryGetValue(grade, out var idList))
            return false;

        if (idList == null || idList.Count == 0)
            return false;

        var randomID = idList[UnityEngine.Random.Range(0, idList.Count)];
        pair = GetItemPair(randomID);
        return true;
    }
}
 