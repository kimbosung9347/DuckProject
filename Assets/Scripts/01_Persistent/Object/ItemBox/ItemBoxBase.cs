using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using UnityEngine.Localization.SmartFormat.Core.Parsing;

enum EItemBoxType
{ 
    Weapon,     // 무기
    Armor,      // 방어구
    Attachment, // 부착물
    Backpack,   // 가방 
    Bullet,     // 총알
    Heal,       // 힐 아이템
    Food,       // 음식 아이템
    Material,   // 재료
    Force,      // 강제 아이템 기반
     
    End,
}
public enum EItemBoxSlotState
{
    NotSearch,
    Searching,
    Complate,
    Empty,
    NotExist,

    End
}
public struct FItemBoxSlotInfo
{
    public EItemBoxSlotState state;
    public float elapsed;
    public float searchTime;
} 
  
public class ItemBoxBase : MonoBehaviour
{
    [Header("박스 타입")]
    [SerializeField] EItemBoxType boxType;
    [SerializeField] string boxName = string.Empty; 
     
    [Header("등급 확률")]
    [SerializeField, UnityEngine.Range(0, 1)] private float normalRate = 0.7f;
    [SerializeField, UnityEngine.Range(0, 1)] private float rareRate = 0.2f;
    [SerializeField, UnityEngine.Range(0, 1)] private float uniqueRate = 0.07f;
    [SerializeField, UnityEngine.Range(0, 1)] private float epicRate = 0.025f;
    [SerializeField, UnityEngine.Range(0, 1)] private float legendaryRate = 0.005f;

    [Header("아이템 개수 확률 (1~10개)")]
    [SerializeField, UnityEngine.Range(0f, 1f)] private float count1 = 0.02f;
    [SerializeField, UnityEngine.Range(0f, 1f)] private float count2 = 0.05f;
    [SerializeField, UnityEngine.Range(0f, 1f)] private float count3 = 0.15f;
    [SerializeField, UnityEngine.Range(0f, 1f)] private float count4 = 0.20f;
    [SerializeField, UnityEngine.Range(0f, 1f)] private float count5 = 0.18f;
    [SerializeField, UnityEngine.Range(0f, 1f)] private float count6 = 0.15f;
    [SerializeField, UnityEngine.Range(0f, 1f)] private float count7 = 0.10f;
    [SerializeField, UnityEngine.Range(0f, 1f)] private float count8 = 0.08f;
    [SerializeField, UnityEngine.Range(0f, 1f)] private float count9 = 0.05f;
    [SerializeField, UnityEngine.Range(0f, 1f)] private float count10 = 0.02f;
     
    [Header("강제 아이템")]
    [SerializeField] private List<EItemID> listForceItem;
       
    protected HandleItemBox cachedHandleItemBox;
    private ItemBase[] arrItem = new ItemBase[DuckDefine.MAX_ITEMBOX_COUNT];
    private FItemBoxSlotInfo[] arrItemState = new FItemBoxSlotInfo[DuckDefine.MAX_ITEMBOX_COUNT];
     
    private bool isActive = false;
    private bool isSearch = false;
    private int maxInvenCnt = 0;
    private int curIndex = 0;
  
#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        if (boxType != EItemBoxType.Force)
        {
            NormalizeGradeRates();
            NormalizeDropRates();
        } 
    }
#endif 
    private void Awake()
    {
        cachedHandleItemBox = GetComponent<HandleItemBox>();
        Array.Clear(arrItem, 0, arrItem.Length);
    } 
    private void Start()
    {
        if (boxType != EItemBoxType.Force)
        {
            NormalizeGradeRates();
        }
        FillUpItemBox();
    }   
      
    public string GetItemDesc()
    {
        string curItemCnt = GetStringCurItemCnt();
        return $"<b>{GetBoxName()} <size=65%>({curItemCnt})";
    }  
    public ItemBase[] GetItemArray() => arrItem;
    public FItemBoxSlotInfo[] GetItemStateArray() => arrItemState;
    public ItemBase RemoveItem(int _index)
    {
        ItemBase item = arrItem[_index];
        if (!item)
            return null;

        //// 제거
        item.CacheItemBox(null);
        arrItem[_index] = null;
        arrItemState[_index].state = EItemBoxSlotState.Empty;
         
        //// UI갱신
        cachedHandleItemBox.RenewItemSlotState(_index, EItemBoxSlotState.Empty);
        cachedHandleItemBox.RenewItemBoxNameAndDesc(GetItemDesc());
        item.RemoveFromInven();
        return item; 
    } 
    public ItemBase GetItem(int _index)
    {
        return arrItem[_index];
    } 
    public int GetMaxInvenCnt() => maxInvenCnt;  
    public bool IsActiveInCanvas() { return isActive; }
     
    public void PushForceItem(EItemID _itemId)
    {
        listForceItem.Add(_itemId);
    } 
    public void InsertItem(int _index, ItemBase _item)
    { 
        if (!_item)
            return;

        PushItem(_index, _item);
         
        arrItemState[_index].state = EItemBoxSlotState.Complate;
        cachedHandleItemBox.RenewItemRenderInfo(_index, _item);
        cachedHandleItemBox.RenewItemSlotState(_index, EItemBoxSlotState.Complate);
        cachedHandleItemBox.RenewItemBoxNameAndDesc(GetItemDesc());
    }
    public bool CanInput(int _index)
    { 
        if (arrItem[_index] == null)
            return false;

        if (arrItemState[_index].state != EItemBoxSlotState.Complate)
            return false;
         
        return true;
    }
    public void SetSearch(bool _key)
    {
        isSearch = _key; 
    }  
    public void UpdateState()
    {
        if (!isSearch)
            return;

        if (curIndex >= arrItemState.Length)
        {
            isSearch = false;
            GetComponent<DetectionTarget>().ComplateAllSearch();
            return;
        }

        ref FItemBoxSlotInfo slot = ref arrItemState[curIndex];
        switch (slot.state)
        {
            case EItemBoxSlotState.NotSearch:
                {
                    slot.state = EItemBoxSlotState.Searching;
                    slot.elapsed = 0f;
                    cachedHandleItemBox.RenewItemSlotState(curIndex, slot.state);
                    break;
                }
            case EItemBoxSlotState.Searching:
                {
                    slot.elapsed += Time.deltaTime;
                    if (slot.elapsed >= slot.searchTime)
                    {
                        slot.state = EItemBoxSlotState.Complate;
                        cachedHandleItemBox.RenewItemSlotState(curIndex, slot.state);
                        var item = cachedHandleItemBox.GetComponent<ItemBoxBase>().GetItem(curIndex);
                        PlayFoundSound(item.GetItemData().GetItemGrade());
                      
                        // 
                        curIndex++;
                    }
                    break;
                }
            case EItemBoxSlotState.Complate:
            case EItemBoxSlotState.Empty:
            case EItemBoxSlotState.NotExist:
                {
                    curIndex++;
                    break;
                }
        }
    }
    public void SetBoxName(string _name)
    {
        boxName = _name;
    }
 
    public void AddItem(ItemBase _item, EItemBoxSlotState _state)
    {
        if (_item == null)
            return;
         
        // 빈 슬롯 찾기
        for (int i = 0; i < DuckDefine.MAX_ITEMBOX_COUNT; i++)
        {
            if (arrItem[i] != null)
                continue;

            // 아이템 삽입
            arrItem[i] = _item;
            _item.transform.SetParent(transform, false);
            _item.gameObject.SetActive(false);

            // 슬롯 상태 세팅
            ref FItemBoxSlotInfo slot = ref arrItemState[i];
            slot.elapsed = 0f;
            slot.state = _state;

            if (_state == EItemBoxSlotState.NotSearch || _state == EItemBoxSlotState.Searching)
            {
                ItemData data = _item.GetItemData();
                slot.searchTime = CalculateSearchTime(data);
            }
            else
            {
                slot.searchTime = 0f;
            }

            // 실제 들어간 개수 갱신
            if (i + 1 > maxInvenCnt)
                maxInvenCnt = i + 1;

            return;
        }

        // 여기 오면 가득 찬 상태 → 아무것도 안 함
    }
    public void RenewAttachState(Weapon _weapon)
    {
        if (_weapon == null)
            return;

        EAttachSlotState muzzle = _weapon.GetAttachSlotState(EAttachmentType.Muzzle);
        EAttachSlotState scople = _weapon.GetAttachSlotState(EAttachmentType.Scope);
        EAttachSlotState stock = _weapon.GetAttachSlotState(EAttachmentType.Stock);

        var itemInfoCanvas = GameInstance.Instance.UI_GetPersistentUIGroup().GetItemInfoCanvas();
        itemInfoCanvas.RenewAttachment(muzzle, scople, stock); 
    } 
    public void SetActiveInCanvas(bool _active)
    {
        isActive = _active; 
    }
      
    protected virtual string GetBoxName()
    {
        if (boxName != string.Empty)
            return boxName;

        switch (boxType)
        {
            case EItemBoxType.Weapon:
                {
                    return "무기 상자";
                }

            case EItemBoxType.Armor:
                {
                    return "방어구 상자";
                }

            case EItemBoxType.Attachment:
                {
                    return "부착물 상자";
                }

            case EItemBoxType.Backpack:
                {
                    return "가방 상자";
                }

            case EItemBoxType.Bullet:
                {
                    return "총알 상자";
                }

            case EItemBoxType.Heal:
                {
                    return "구급 상자";
                }

            case EItemBoxType.Food:
                {
                    return "음식 상자";
                }

            case EItemBoxType.Material:
                {
                    return "재료 상자";
                }
        }
        return string.Empty;
    }
    protected virtual string GetStringCurItemCnt()
    {
        int curCount = 0;
        for (int i = 0; i < arrItem.Length; i++)
        {
            if (arrItem[i] != null)
                curCount++;
        }

        return $"{curCount}/{maxInvenCnt}";
    }
    protected virtual ItemBase MakeItem()
    {
        EItemGrade grade = GetRandomGrade();
         
        switch (boxType)
        {
            case EItemBoxType.Weapon:
            {
                var values = (EWeaponType[])System.Enum.GetValues(typeof(EWeaponType));
                int count = values.Length;
                if (values[count - 1].ToString() == "End")
                    count--;

                // 해당 Grade
                // DuckUtill.IsMatch_NpcBetweenQeust();
                 
                EWeaponType weaponType = values[UnityEngine.Random.Range(0, count)];
                return GameInstance.Instance.SPAWN_MakeWeapon(weaponType, grade);
            }

        case EItemBoxType.Armor:
            {
                EArmorType armorType = UnityEngine.Random.value < 0.5f ? EArmorType.Helmet : EArmorType.Armor;
                ItemBase item = GameInstance.Instance.SPAWN_MakeArmor(armorType, grade);
                return item;
            }

        case EItemBoxType.Attachment:
            {
                ItemBase item = GameInstance.Instance.SPAWN_MakeRandomAttachment(grade);
                return item;
            }

        case EItemBoxType.Backpack:
            {
                return GameInstance.Instance.SPAWN_MakeRandomBackpack(grade);
            } 

        case EItemBoxType.Bullet:
            {
                int bulletCnt = UnityEngine.Random.Range(10, 31);
                ItemBase item = GameInstance.Instance.SPAWN_MakeBullet(bulletCnt, grade);
                return item;
            }

        case EItemBoxType.Heal:
            {
                int healCnt = UnityEngine.Random.Range(1, 3);
                return GameInstance.Instance.SPAWN_MakeRandomHeal(grade);
            }

        case EItemBoxType.Food:
            {
                return GameInstance.Instance.SPAWN_MakeRandomFood(grade);
            }

        case EItemBoxType.Material:
            {
                ItemBase item = GameInstance.Instance.SPAWN_MakeRandomStuff(grade);
                return item;
            }
        }
        return null;
    }
    protected EItemGrade GetRandomGrade()
    {
        float rand = UnityEngine.Random.value;
         
        float n = normalRate;
        float r = n + rareRate;
        float u = r + uniqueRate;
        float e = u + epicRate;
        // 나머지 = 레전더리

        if (rand < n) return EItemGrade.Normal;
        if (rand < r) return EItemGrade.Rare;
        if (rand < u) return EItemGrade.Unique;
        if (rand < e) return EItemGrade.Epic;
        return EItemGrade.Legend; 
    } 
      
    private void FillUpItemBox()
    {
        if (boxType == EItemBoxType.Force)
        {
            maxInvenCnt = 0;
            foreach (var item in arrItem)
            {
                if (item != null)
                    maxInvenCnt++;
            }
             
            // Force로 채워야 하는 개수(요구치)
            int forceCount = Mathf.Min(listForceItem.Count, DuckDefine.MAX_ITEMBOX_COUNT);
             
            // 기존에 AddItem 등으로 이미 들어간 슬롯은 유지하면서, 비어있는 슬롯(null)만 force 아이템을 채운다.
            int forceCursor = 0;

            for (int slotIndex = 0; slotIndex < DuckDefine.MAX_ITEMBOX_COUNT; slotIndex++)
            {
                // 이미 아이템이 있으면 건드리지 않음
                if (arrItem[slotIndex])
                {
                    // 상태/시간도 기존 유지(원하면 여기서만 보정 가능)
                    continue;
                }

                // force 아이템을 다 채웠으면 종료
                if (forceCursor >= forceCount)
                    break;

                EItemID itemId = listForceItem[forceCursor++];
                ItemBase item = GameInstance.Instance.SPAWN_MakeItem(itemId);
                if (!item)
                    continue;
                 
                PushItem(slotIndex, item);

                // 상태 업데이트 
                ref FItemBoxSlotInfo slot = ref arrItemState[slotIndex];
                slot.elapsed = 0f;
                slot.searchTime = CalculateSearchTime(arrItem[slotIndex].GetItemData());
                slot.state = EItemBoxSlotState.NotSearch;

                maxInvenCnt++;
            }  
 
            // 나머지 "비어있는 슬롯"만 NotExist로 만들기 (이미 들어있는 슬롯은 절대 건드리지 않음)
            for (int i = 0; i < DuckDefine.MAX_ITEMBOX_COUNT; i++)
            {
                if (arrItem[i])
                    continue;

                arrItemState[i].state = EItemBoxSlotState.NotExist;
                arrItemState[i].elapsed = 0f;
                arrItemState[i].searchTime = 0f;
            }

            return;
        }
        // ===== 기존 랜덤 박스 로직 =====
        maxInvenCnt = GetRandomCount();
        int filledCnt = 0;

        for (int i = 0; i < maxInvenCnt && filledCnt < arrItem.Length; i++)
        {
            ItemBase item = MakeItem();
            if (!item)
                continue;
            
            PushItem(filledCnt, item);
            filledCnt++;
        }

        maxInvenCnt = filledCnt;
        for (int i = 0; i < DuckDefine.MAX_ITEMBOX_COUNT; i++)
        {
            ref FItemBoxSlotInfo slot = ref arrItemState[i];
            slot.elapsed = 0f;

            if (arrItem[i])
            {
                slot.searchTime = CalculateSearchTime(arrItem[i].GetItemData());
                slot.state = EItemBoxSlotState.NotSearch;
            }
            else
            {
                slot.state = EItemBoxSlotState.NotExist;
            }
        }
    }
    private void NormalizeGradeRates()
    { 
        normalRate = Mathf.Max(0f, normalRate);
        rareRate = Mathf.Max(0f, rareRate);
        uniqueRate = Mathf.Max(0f, uniqueRate);
        epicRate = Mathf.Max(0f, epicRate);
        legendaryRate = Mathf.Max(0f, legendaryRate);

        float total =
            normalRate +
            rareRate +
            uniqueRate +
            epicRate +
            legendaryRate;

        if (total <= 0f)
        {
            normalRate = 1f;
            rareRate = uniqueRate = epicRate = legendaryRate = 0f;
            return;
        }

        normalRate /= total;
        rareRate /= total;
        uniqueRate /= total;
        epicRate /= total;
        legendaryRate /= total;
    }
    private void NormalizeDropRates()
    {
        count1 = Mathf.Max(0f, count1);
        count2 = Mathf.Max(0f, count2);
        count3 = Mathf.Max(0f, count3);
        count4 = Mathf.Max(0f, count4);
        count5 = Mathf.Max(0f, count5);
        count6 = Mathf.Max(0f, count6);
        count7 = Mathf.Max(0f, count7);
        count8 = Mathf.Max(0f, count8);
        count9 = Mathf.Max(0f, count9);
        count10 = Mathf.Max(0f, count10);

        float total =
            count1 + count2 + count3 + count4 + count5 +
            count6 + count7 + count8 + count9 + count10;

        if (total <= 0f)
        {
            float u = 0.1f;
            count1 = count2 = count3 = count4 = count5 =
            count6 = count7 = count8 = count9 = count10 = u;
            return;
        }

        float inv = 1f / total;
        count1 *= inv; count2 *= inv; count3 *= inv; count4 *= inv; count5 *= inv;
        count6 *= inv; count7 *= inv; count8 *= inv; count9 *= inv; count10 *= inv;
    }

    private int GetRandomCount()
    {
        float r = UnityEngine.Random.value;
        float c = 0f;
         
        c += count1; if (r < c) return 1;
        c += count2; if (r < c) return 2;
        c += count3; if (r < c) return 3;
        c += count4; if (r < c) return 4;
        c += count5; if (r < c) return 5;
        c += count6; if (r < c) return 6;
        c += count7; if (r < c) return 7;
        c += count8; if (r < c) return 8;
        c += count9; if (r < c) return 9;
        return 10; 
    }
    private float CalculateSearchTime(ItemData data)
    {
        float baseTime = data.grade switch
        {
            EItemGrade.Normal => 0.2f,
            EItemGrade.Rare => 0.8f,
            EItemGrade.Unique => 1.5f,
            EItemGrade.Epic => 2.2f,
            EItemGrade.Legend => 3.5f,
            _ => 1.0f   
        }; 

        float priceFactor = Mathf.Clamp01(data.price / 10000f);
        float priceBonus = Mathf.Lerp(0.1f, 2.0f, priceFactor);
        return baseTime + priceBonus;
    }
    private void PlayFoundSound(EItemGrade _grade)
    {
        var gameInstance = GameInstance.Instance;
          
        switch (_grade)
        {
            case EItemGrade.Normal:
            case EItemGrade.Rare:
            case EItemGrade.Unique:
            case EItemGrade.Epic:
            {
                gameInstance.SOUND_PlaySoundSfx(ESoundSfxType.Found_Normal, Vector3.zero);
            }
            break;

            case EItemGrade.Legend:
            {
                gameInstance.SOUND_PlaySoundSfx(ESoundSfxType.Found_Big, Vector3.zero);
            }
            break;
        }
    }
     
    private void PushItem(int _index, ItemBase _item)
    {
        arrItem[_index] = _item;
        _item.CacheItemBox(this);
        _item.transform.SetParent(transform, false);
        _item.gameObject.SetActive(false);
    }
}
   