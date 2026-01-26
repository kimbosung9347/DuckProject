using System.Collections.Generic;
using System.Linq;
using UnityEngine;
 
public class PlayerStorage : DuckStorage
{ 
    private PlayerUIController cachedUIController; 
    private PlayerEquip cachedEquip;
    private PlayerState cachedState;
    private PlayerQuickSlot cachedQuick;
    private CapacityInfo cachedCapacityInfo;
     
    // 개수 기반 소비 아이템 추적
    private Dictionary<EItemID, List<int>> hashTrackConsum = new(); 
    private List<EItemID> listSelectBulletID = new();
     
    private EItemID curSelectBulletId = EItemID._END;
    private float lastScrollTime;
    private bool activeBulletSelect = false;
    private int curMoney = 1000;

    // 들어갈 수 있는 인벤토리 인덱스 추적
    private SortedSet<int> emptySlotSet = new SortedSet<int>();
     
    protected override void Awake()
    { 
        base.Awake();
         
        if (cachedDuckEquip is PlayerEquip equip)
        {
            cachedEquip = equip;
        } 

        cachedUIController = gameObject.GetComponent<PlayerUIController>();
        cachedState = GetComponent<PlayerState>();
        cachedQuick = GetComponent<PlayerQuickSlot>();
    }
    private void Start() 
    {
        MakeStorageByCapacity();

        var playData = GameInstance.Instance.SAVE_GetCurPlayData();
        foreach (FItemShellIncludeIndex itemInfo in playData.storageData.items)
        {
            ItemBase created = GameInstance.Instance.SPAWN_MakeItem(itemInfo.shell);
            if (!created)
                continue;

            InsertItemNotRenewUI(itemInfo.index, created);
        }
           
        RenewBackpackSlot(); 
        SetMoney(playData.characterData.moeny);
    } 
    private void Update()
    {
        // 현재 사용중인 아이템이 있다면
        if (curUseConsumIndex != -1)
        {
            var item = cachedItemBox ? cachedItemBox.GetItem(curUseConsumIndex) : GetItem(curUseConsumIndex);
            if (item is ConsumBase consum)
            {
                float ratio = consum.GetUseRatio();
                cachedUIController.RenewUseGuage(ratio);
            } 
        }  
    }

    public override void RollbackBullet(EItemID _bulletId, int _cnt)
    {
        // 총알아니면 return
        if (DuckUtill.GetConsumTypeByItemID(_bulletId) != EConsumableType.Bullet)
            return;

        int addCnt = _cnt;

        // 총알이 있는지 체크 
        if (hashTrackConsum.TryGetValue(_bulletId, out var listIndex))
        {
            foreach (int idx in listIndex)
            {
                if (addCnt == 0)
                    return;

                if (listItems[idx] is ConsumBase bullet)
                {
                    addCnt = bullet.PushAndCheckRemain(addCnt);
                    cachedUIController.INVEN_RenewBackpackSlotConsumCnt(idx, bullet.GetCurCnt());
                }
            }
        }

        // 이경우는 총알 아이템을 만들고 인벤에 다시 넣어줘야함
        if (addCnt > 0)
        {
            var newBullet = GameInstance.Instance.SPAWN_MakeBullet(addCnt, DuckUtill.GetItemPair(_bulletId).data.grade);
            if (emptySlotSet.Count == 0)
            {
                // todo 아이템 버리기
                return;
            }

            // 가장 낮은 빈칸 인덱스 사용
            int emptyIndex = emptySlotSet.Min;
            InsertItem(emptyIndex, newBullet);
        }
    }
    public override void RefillBullet(EItemID _bulletId, int _refillCnt)
    {
        // 총알아니면 return
        if (DuckUtill.GetConsumTypeByItemID(_bulletId) != EConsumableType.Bullet)
            return;

        if (!hashTrackConsum.TryGetValue(_bulletId, out List<int> listBulletIndex))
            return;

        // 현재 총합 및 실제 리필 개수 계산
        int totalBulletCnt = GetTotalConsumCnt(_bulletId);
        int realRefillCnt = (_refillCnt >= totalBulletCnt) ? totalBulletCnt : _refillCnt;

        // 총알 채워주기 
        var weapon = cachedEquip.GetWeapon();
        weapon.SetCurBullet(weapon.GetCurBullet() + realRefillCnt);
        weapon.SetCurBulletId(_bulletId);

        ////////////////////
        // 총알 개수 지워주기 
        int left = realRefillCnt;
        var copy = new List<int>(listBulletIndex);

        // 전부 다 날아가는 경우 - 전부다 삭제해주기 
        if (left == totalBulletCnt)
        {
            foreach (int idx in copy)
            {
                RemoveItem(idx, false);
            }
        }

        // 일부만 까이는 경우
        else
        {
            foreach (int idx in copy)
            {
                if (left <= 0)
                    break;

                if (listItems[idx] is ConsumBase bullet)
                {
                    int cur = bullet.GetCurCnt();

                    if (left >= cur)
                    {
                        left -= cur;
                        RemoveItem(idx, false);
                    }

                    else
                    { 
                        float weight = DuckUtill.GetItemWeight(_bulletId);
                        curWeight -= weight * left;
                        cachedUIController.INVEN_RenewWeight(curWeight, cachedCapacityInfo.maxWeight);

                        /////
                        bullet.SetCurCnt(cur - left);
                        cachedUIController.INVEN_RenewBackpackSlotConsumCnt(idx, bullet.GetCurCnt());
                        left = 0;
                    }
                }
            }
        }

        // UI 갱신
        cachedUIController.HUD_RenewCurBullet(weapon.GetCurBullet());
        cachedUIController.HUD_RenewMaxBullet(totalBulletCnt - realRefillCnt);
        cachedUIController.HUD_RenewBulletType(_bulletId);
         
        // 상태 원복  
        cachedState.ChangeState(EDuckState.Default);

        // 총알 무게 갱신
        var bulltPair = DuckUtill.GetItemPair(_bulletId);
        curWeight -= bulltPair.data.weight * _refillCnt;
    }
     
    public void DelateAllItem()
    {
        for (int i = listItems.Count - 1; i >= 0; i--)
        {
            var item = RemoveItemNotRenewUI(i);
            if (item)
                Destroy(item);
        } 
    }
    public void SetMoney(int _money)
    {
        curMoney = _money; 
        cachedUIController.RenewMoney(curMoney);
    }
    public void MakeStorageByCapacity()
    {
        int newMaxCnt = cachedCapacityInfo.capacitySize;
        if (itemMaxCnt == newMaxCnt)
            return;
        
        // 인벤토리 축소 → 초과 아이템 드랍
        if (itemMaxCnt > newMaxCnt)
        {
            for (int i = newMaxCnt; i < itemMaxCnt; i++)
            {
                var item = GetItem(i);
                if (item)
                {
                    RemoveItem(i, true);
                }
            }
        
            // 리스트 크기 줄이기
            listItems.RemoveRange(newMaxCnt, itemMaxCnt - newMaxCnt);
        }
        
        // 인벤토리 확장 → null 공간 추가
        else
        {
            while (listItems.Count < newMaxCnt)
                listItems.Add(null);
        }
        
        itemMaxCnt = newMaxCnt;
        
        // 비워있는 슬롯 추적 
        emptySlotSet.Clear();
        for (int i = 0; i < listItems.Count; i++)
        {
            if (listItems[i] == null)
                emptySlotSet.Add(i); 
        }
    }
    public void CacheCapacityInfo(CapacityInfo _info)
    {
        cachedCapacityInfo = _info;
    }
     
    public void InsertItemNotRenewUI(int _index, ItemBase _item)
    {
        if (!_item)
            return;

        listItems[_index] = _item;
        _item.CacheDuckStorage(this);

        // 들어갈수 있는 인덱스 추적 갱신
        emptySlotSet.Remove(_index);

        // 아이템 추적 
        var itemData = _item.GetItemData();
        InsertTrack(_index, itemData.itemID);

        // 무게 갱신 
        RenewWeight(_item.GetItemWeight());

        // 자식에 넣고 빌보드 없애주기 
        _item.Insert(transform);
    }
    public ItemBase RemoveItemNotRenewUI(int _index)
    { 
        if (_index < 0 || _index >= listItems.Count)
        {
            return null;
        }

        ItemBase item = listItems[_index];
        if (!item)
        {
            return null;
        }

        listItems[_index] = null;
        item.CacheDuckStorage(null);

        // 들어갈수 있는 인덱스 추적 갱신
        emptySlotSet.Add(_index);

        // 아이템 추적 
        var itemData = item.GetItemData();
        RemoveTrack(_index, itemData.itemID);
         
        // 퀵슬롯인지 체크 
        bool isInQuickSlot = cachedQuick.HasSlotByTrackIndex(_index);
        if (isInQuickSlot)
        {
            int quickSlotIndex = cachedQuick.GetSlotByTrackId(_index);
            cachedQuick.DisableOrChangeTrack(quickSlotIndex);
        }

        // 무게 갱신 
        RenewWeight(item.GetItemWeight() * -1);
         
        return item;

        {
            // ItemBase item = base.RemoveItem(_index, false);
            // if (!item)
            //     return;
            //   
            //if (item && item is ConsumBase consum)
            //{
            //    EConsumableType targetConsumType = DuckUtill.GetConsumTypeByItemID(itemData.itemID);
            //    // 총알임  
            //    if (targetConsumType == EConsumableType.Bullet)
            //    {
            //        // 총알 퀵슬롯 갱신
            //        if (itemData.itemID == curSelectBulletId)
            //        {
            //            cachedUIController.HUD_RenewMaxBullet(GetTotalConsumCnt(itemData.itemID));
            //        }
            //    }
            //}
            // 
            // // 무게 갱신 
            // RenewWeight(item.GetItemWeight() * -1);
            //  
            // Destroy(item);
        }
    }
    public void InsertItem(int _index, ItemBase _item)
    {
        InsertItemNotRenewUI(_index, _item);
        
        // UI 갱신 
        RenewUI(_index, _item);

        // 가방 정보 갱신 
        RenewBackpackDesc();

        // 이펙트 출력 
        PlayInsertEffect();

    }
    public ItemBase RemoveItem(int _index, bool _isDrop)
    {
        var item = RemoveItemNotRenewUI(_index);

        bool isDestroy = false;
        var itemData = item.GetItemData();
         
        if (item && item is ConsumBase consum)
        {
            EConsumableType targetConsumType = DuckUtill.GetConsumTypeByItemID(itemData.itemID);
            // 다 소진되었거나, 드랍일 경우
            if (!consum.IsRemain() || _isDrop)
            {
                isDestroy = true;

                // 총알임  
                if (targetConsumType == EConsumableType.Bullet)
                {
                    // 총알 퀵슬롯 갱신
                    if (itemData.itemID == curSelectBulletId)
                    {
                        cachedUIController.HUD_RenewMaxBullet(GetTotalConsumCnt(itemData.itemID));
                    }
                }
            }
        }
         
        // UI갱신
        cachedUIController.INVEN_DisableBackpackSlot(_index);

        // item 삭제 
        if (isDestroy && !_isDrop)
        {
            Destroy(item.gameObject);
        }

        // Drop인지
        if (_isDrop)
        {
            item.Drop();
        }
         
        // UI갱신 
        RenewDisableUI(_index, item);

        return item;
    } 
    
    public PlayerStorageItemData CreateStorageItems()
    {
        PlayerStorageItemData data = new();

        for (int i = 0; i < listItems.Count; i++)
        {
            ItemBase item = listItems[i];
            if (!item)
                continue;
             
            data.items.Add(new FItemShellIncludeIndex
            {
                index = i,
                shell = item.GetItemShell()
            });
        }

        return data;
    }
    public bool CanStore(float _weight)
    {
        if (curWeight + _weight > cachedCapacityInfo.maxWeight)
            return false;

        return true;
    }
    private void RenewWeight(float _add)
    { 
        curWeight += _add;
        if (curWeight < 0)
            curWeight = 0f;
         
        EWeightState weightState = EWeightState.None;
        if (curWeight >= cachedCapacityInfo.maxWeight)
        {
            weightState = EWeightState.OverWeight;
        }
        else if (curWeight > cachedCapacityInfo.maxWeight * DuckDefine.WARNING_WEIGHT_RATIO)
        {
            weightState = EWeightState.Heavy;
        }
         
        // weightBuff 
        cachedDuckBuff.InsertWeightBuff(weightState);
         
        // UI갱신
        cachedUIController.INVEN_RenewWeight(curWeight, cachedCapacityInfo.maxWeight);
    }

    public void AutoInsertItem(ItemBase _item)
    { 
        // 총알이나, 소모품 같은 경우에는 Marge해줘야함
        if (_item is ConsumBase margeConsum)
        {
            // 개수기반인 경우
            if (!margeConsum.IsCapacity())
            { 
                int curMargeCnt = margeConsum.GetCurCnt();
                int left = curMargeCnt; 
                EItemID id = margeConsum.GetItemData().itemID;
                float weight = margeConsum.GetItemData().weight;
                   
                // hashTrackCntConsum를 가져옴 개수가 없어질때까지 합쳐줄예정
                if (hashTrackConsum.TryGetValue(id, out List<int> listIndex))
                {
                    foreach (int index in listIndex)
                    {
                        // 합쳐질 예정임
                        if (GetItem(index) is ConsumBase beMergedConsum)
                        {
                            left = beMergedConsum.PushAndCheckRemain(left);
                            // UI갱신
                            cachedUIController.INVEN_RenewBackpackSlotConsumCnt(index, beMergedConsum.GetCurCnt()); 
                             
                            // 남은 개수가 없다면, 끝
                            if (left == 0)
                            {
                                break;
                            }
                        }
                    }
                     
                    // 무게 갱신 
                    RenewWeight(weight * (curMargeCnt - left));
                    if (left == 0)
                        return;
                }
            }
        }
          
        InsertItem(emptySlotSet.Min, _item);
    }  
    public int RemoveByItemId(EItemID itemId, int removeCount)
    {
        if (!hashTrackConsum.TryGetValue(itemId, out var indexList))
            return 0;

        int removed = 0;
        var copy = new List<int>(indexList);
         
        foreach (int idx in copy)
        {
            if (removed >= removeCount)
                break;
             
            var item = listItems[idx];

            // 소비 아이템 (개수 기반)
            if (item is ConsumBase consum)
            {
                bool isInQuickSlot = cachedQuick.HasSlotByTrackIndex(idx);
                 
                int cur = consum.GetCurCnt();

                if (removed + cur <= removeCount)
                {
                    removed += cur;
                    RemoveItemNotRenewUI(idx);
                }
                else
                {
                    int left = removeCount - removed;
                    consum.SetCurCnt(cur - left);
                    removed += left;
                }
                 
                if (isInQuickSlot)
                {
                    int quickSlotIndex = cachedQuick.GetSlotByTrackId(idx);
                    cachedQuick.DisableOrChangeTrack(quickSlotIndex); 
                }
            } 
            // 비소비 아이템 (1개 = 1)
            else
            {
                removed += 1;
                RemoveItemNotRenewUI(idx);
            }
        }

        return removed;
    }

    public void RenewAllBackpackInfo() 
    {
        for (int i= 0; i < listItems.Count; i++)
        {
            RenewUI(i, listItems[i]);
        }
    }  
    public void RenewAttachState(Weapon _weapon)
    {
        EAttachSlotState muzzle = _weapon.GetAttachSlotState(EAttachmentType.Muzzle);
        EAttachSlotState scople = _weapon.GetAttachSlotState(EAttachmentType.Scope);
        EAttachSlotState stock = _weapon.GetAttachSlotState(EAttachmentType.Stock);
        cachedUIController.ITEMINFO_RenewSlotAttachment(muzzle, scople, stock);
    }
    public void ReduceMoney(int _reduce)
    {
        curMoney -= _reduce;
        if (curMoney < 0)
            curMoney = 0;

        cachedUIController.RenewMoney(curMoney);
    }
    public void AcquireMoney(int _add)
    {
        curMoney += _add;
        cachedUIController.RenewMoney(curMoney);
    } 
    public void RenewBackpackSlot() 
    { 
        // 인벤토리 유아이 갱신  
        cachedUIController.INVEN_MakeBackpackSlot(itemMaxCnt);
        // 가방정보갱신  
        RenewBackpackDesc();
    }
    public void Sort()
    {
        if (itemMaxCnt == 0)
            return;
         
        var slotcontroller = GameInstance.Instance.SLOT_GetSlotController();
        slotcontroller.Action_Sort(emptySlotSet, GetFillSlotSet());
    }

    public bool IsEmpty(int _index)
    { 
        if (!listItems[_index]) 
            return false; 
         
        return true;
    }
    public bool IsEmptySlotSet()
    {
        if (emptySlotSet.Count == 0)
            return false;

        return true;
    }
      
    public SortedSet<int> GetFillSlotSet()
    {
        SortedSet<int> filledSlotSet = new();
        for (int i = 0; i < listItems.Count; i++)
        {
            if (!emptySlotSet.Contains(i))
                filledSlotSet.Add(i);
        }
        return filledSlotSet;
    }
    public int GetEmptyIndex()
    {
        if (!IsEmptySlotSet())
            return -1;
         
        return emptySlotSet.Min;
    }
    public int GetMoney() { return curMoney; } 
    public int GetItemMaxCnt() { return itemMaxCnt; }
     
    public int GetTotalConsumCnt(EItemID _id)
    {
        if (!hashTrackConsum.TryGetValue(_id, out List<int> listBulletIndex))
            return 0;

        // 현재 총합 계산
        int totalBulletCnt = 0;
        foreach (int idx in listBulletIndex)
        {
            if (listItems[idx] is ConsumBase bullet)
            {
                totalBulletCnt += bullet.GetCurCnt();
            }
        }
        return totalBulletCnt;
    }
    public int GetTrackIndexConsum(EItemID id)
    {
        if (!hashTrackConsum.TryGetValue(id, out var list))
            return -1;

        if (list == null || list.Count == 0)
            return -1;

        return list[0];
    }

    public bool CanScrollBullet()
    {
        if (!activeBulletSelect)
            return false;

        if (listSelectBulletID.Count <= 1)
            return false;
         
        // 0.051초 틱 
        if (Time.time - lastScrollTime < 0.05f)
            return false;

        lastScrollTime = Time.time;
        return true;
    }
    public bool CanChangeBullet()
    {
        if (cachedEquip.GetWeapon() == null)
            return false;

        return true;
    } 
    public bool CanCancleChangeBullet()
    {
        if (cachedEquip.GetWeapon() == null)
            return false;
         
        if (!activeBulletSelect)
            return false;

        return true;
    }
    public bool CanReload()
    {
        var weapon = cachedEquip.GetWeapon();
        if (!weapon)
            return false;

        if (!weapon.CanReload())
            return false;

        if (0 == GetTotalConsumCnt(cachedEquip.GetWeapon().GetCurBulletId()))
        {
            return false;
        }
         
        return true;
    }
      
    public void ChangeSelectBullet(float _value)
    {
        EItemID prevId = curSelectBulletId;
        int curIndex = listSelectBulletID.IndexOf(curSelectBulletId);
        if (curIndex < 0)
            curIndex = 0;

        int dir = _value > 0 ? 1 : -1;
        int nextIndex = curIndex + dir;

        if (nextIndex < 0 || nextIndex >= listSelectBulletID.Count)
            return;

        curSelectBulletId = listSelectBulletID[nextIndex];
        cachedUIController.HUD_NotSelectAvaiableBullet(prevId);
        cachedUIController.HUD_SelectAvaiableBullet(curSelectBulletId);
    } 
    public void ChangeBullet()
    {
        if (activeBulletSelect)
        {
            activeBulletSelect = false;
            TryChangeBullet();
        }   

        else
        {
            activeBulletSelect = true;
            TrackAndRenewAvaiableBullet();
        } 
    }
    public void CancleChangeBullet()
    { 
        activeBulletSelect = false;
        curSelectBulletId = EItemID._END;
        cachedUIController.HUD_ExitAvaiableBullet();
    }

    protected override void AfterUseConsum(bool isRemain, ConsumBase consum)
    {
        // 아이템 박스에서 사용 
        if (cachedItemBox)
        { 
            int prevIndex = curUseConsumIndex;
            ItemBoxBase prevItemBox = cachedItemBox;

            CancleUseConsum();
            cachedDuckState.ChangeState(EDuckState.Default);
            if (isRemain)
            {
                prevItemBox.GetComponent<HandleItemBox>().RenewItemRenderInfo(prevIndex, consum);
            }

            else
            {
                prevItemBox.RemoveItem(prevIndex);
            }
        }
         
        // 인벤토리에서 사용
        else
        {
            int prevIndex = curUseConsumIndex;
            CancleUseConsum();
            cachedDuckState.ChangeState(EDuckState.Default);

            if (isRemain) 
            {
                RenewConsum(prevIndex, consum);

                bool isInQuickSlot = cachedQuick.HasSlotByTrackIndex(prevIndex);
                if (isInQuickSlot)
                {
                    int quickSlotIndex = cachedQuick.GetSlotByTrackId(prevIndex);
                    cachedQuick.RenewQuickSlotInfo(quickSlotIndex);
                }
            } 
            else
            {
                RemoveItem(prevIndex, false);
            }
        }
    }

    private void InsertTrack(int _index, EItemID _id)
    { 
        if (!hashTrackConsum.TryGetValue(_id, out var list))
        {
            list = new List<int>();
            hashTrackConsum[_id] = list;
        }
         
        // 중복 추가 방지
        if (!list.Contains(_index))
            list.Add(_index);
    } 
    private void RemoveTrack(int _index, EItemID _id)
    {
        if (!hashTrackConsum.TryGetValue(_id, out var list))
            return;

        // index 제거
        list.Remove(_index);
    }
    private void TryChangeBullet()
    {
        // 현재 선택된 Bullet이 있다면 
        if (curSelectBulletId != EItemID._END)
        {
            // 1개 이상 존재 할 때 
            int selectTotalCnt = GetTotalConsumCnt(curSelectBulletId);
            if (selectTotalCnt > 0)
            { 
                var weapon = cachedEquip.GetWeapon();
                if (weapon.CanReload(curSelectBulletId))
                {
                    cachedEquip.TryReload(curSelectBulletId);
                }  
            } 
        }
          
        CancleChangeBullet();
    }
    private void TrackAndRenewAvaiableBullet()
    {
        // 선택중인 총알 추적용 리스트 
        listSelectBulletID.Clear();

        // 현재 전체 총알개수 넣어주기 
        bool isEmptyBullet = true;
        foreach (var kv in hashTrackConsum)
        { 
            EItemID itemId = kv.Key;
             
            // 총알이 아니라면 무시
            if (DuckUtill.GetConsumTypeByItemID(itemId) != EConsumableType.Bullet)
                continue;

            // 총알이 있는지 체크
            int totalCnt = GetTotalConsumCnt(itemId);
            if (totalCnt <= 0)
                continue;

            // 같은 총알일 경우에는 무시
            EItemID CurBulletID = cachedEquip.GetWeapon().GetCurBulletId();
            if (CurBulletID == itemId)
                continue; 

            // 다른 총알이 발견이됨, 선택할 수 있는 불렛 리스트에 추가 
            listSelectBulletID.Add(itemId);
            if (curSelectBulletId == EItemID._END)
            {
                // 처음에 선택되게끔 설정해주기 위해서
                curSelectBulletId = itemId;
            }
              
            isEmptyBullet = false;
            cachedUIController.HUD_InsertAvaiableBullet(itemId, totalCnt);
        } 

        // 총이 없으면 _END넣어주기 -> 자체적으로 미착용 표시 
        if (isEmptyBullet)
        {
            cachedUIController.HUD_InsertAvaiableBullet(EItemID._END, 0);
        }
         
        // Enter
        cachedUIController.HUD_EnterAvaiableBullet();

        // 해당 UI Select하기
        cachedUIController.HUD_SelectAvaiableBullet(curSelectBulletId);
    }
     
    private void RenewBackpackDesc()
    {
        // 현재 개수 갱신
        int curCount = listItems.Count(item => item != null);
        int maxCount = itemMaxCnt;
        cachedUIController.INVEN_RenewBackpackDesc($"<b>가방 <size=65%>({curCount}/{maxCount})");

        // 무개 중량 갱신 
        cachedUIController.INVEN_RenewWeight(curWeight, cachedCapacityInfo.maxWeight);
    } 
    private void RenewUI(int _index, ItemBase _item)
    {
        if (!_item)
        {
            cachedUIController.INVEN_DisableBackpackSlot(_index);
            return;
        } 

        var itemData = _item.GetItemData();
        var visualData = _item.GetItemVisualData();
         
        // 목표 슬롯 갱신
        cachedUIController.INVEN_RenewBackpackSlot(_index, itemData.itemID, itemData.grade, itemData.itemName, visualData.iconSprite);

        // 무기일 경우
        bool isWeapon = false;

        // 장비일 경우
        if (_item is EquipBase equip)
        {
            if (equip is Weapon weapon)
            {
                isWeapon = true;
                var muzzle = weapon.GetAttachSlotState(EAttachmentType.Muzzle);
                var scope = weapon.GetAttachSlotState(EAttachmentType.Scope);
                var stock = weapon.GetAttachSlotState(EAttachmentType.Stock);
                cachedUIController.INVEN_RenewBackpackAttachment(_index, muzzle, scope, stock);
            }

            if (equip.IsUseDuration())
            {
                cachedUIController.INVEN_RenewBackpackSlotDurability(_index, equip.GetCurDurabilityRatio());
            }

            else
            {
                // 장비는 한개일꺼니깐 이렇게하자
                cachedUIController.INVEN_RenewBackpackSlotConsumCnt(_index, 1);
            }
        }

        // 부착물일 경우  
        else if (_item is Attachment attachment)
        {
            cachedUIController.INVEN_HideGageAndCnt(_index);
        }

        // 소비템일 경우
        else if (_item is ConsumBase consume)
        {
            if (consume.IsCapacity())
            {
                // 내구도 기반 소비
                cachedUIController.INVEN_RenewBackpackSlotDurability(_index, consume.GetCurDurabilityRatio());
            }
            else
            {
                // 개수 기반 소비
                cachedUIController.INVEN_RenewBackpackSlotConsumCnt(_index, consume.GetCurCnt());
            }

            // 총을 장착중이라면 - 총알일 경우, 총 퀵슬롯을 갱신해줘야함
            var weapon = cachedEquip.GetWeapon();
            if (weapon)
            {
                if (DuckUtill.GetConsumTypeByItemID(itemData.itemID) == EConsumableType.Bullet)
                {
                    int curBulletMaxCnt = GetTotalConsumCnt(itemData.itemID);

                    // 퀵슬롯에 갱신해야함 현재 장착중인 총알이 추가되었음, 퀵슬롯에 갱신해줘야함
                    if (weapon.GetCurBulletId() == itemData.itemID)
                    {
                        cachedUIController.HUD_RenewMaxBullet(curBulletMaxCnt);
                    }

                    // 아니라면 활용가능한 총알에 갱신시켜줘야함
                    else
                    {
                        cachedUIController.HUD_RenewAvaiableBulletCnt(itemData.itemID, curBulletMaxCnt);
                    }
                }
            }
        }

        // 무기 아니면 -> 부착물 없음, 일단 이렇게 설계함, 나중에 부착물이 다른 아이템에도 생기면 수정해야하긴하는데, 계획에 없음
        if (!isWeapon)
        {
            cachedUIController.INVEN_HideBackpackAttachment(_index);
        }
    }
    private void RenewConsum(int _index, ConsumBase _heal)
    {
        if (_heal.IsCapacity())
        {
            cachedUIController.INVEN_RenewBackpackSlotDurability(_index, _heal.GetCurDurabilityRatio());
        }
        else
        {
            cachedUIController.INVEN_RenewBackpackSlotConsumCnt(_index, _heal.GetCurCnt());
        }
    }
    private void RenewDisableUI(int _index, ItemBase _item)
    { 
        RenewBackpackDesc();
        cachedUIController.INVEN_DisableBackpackSlot(_index);
    } 

    private void PlayInsertEffect()
    {
        GameInstance.Instance.SOUND_PlaySoundSfx(ESoundSfxType.PickUp, transform.position);
    }
    private void ApplyStorageItems(PlayerStorageItemData data)
    {
        foreach (var itemInfo in data.items)
        {
            ItemBase created = GameInstance.Instance.SPAWN_MakeItem(itemInfo.shell);

            if (!created)
                continue;

            InsertItemNotRenewUI(itemInfo.index, created);
        } 
    } 
}  
   
  
 