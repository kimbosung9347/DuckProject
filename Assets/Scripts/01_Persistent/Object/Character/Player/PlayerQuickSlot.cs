using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FQucikSlotInfo
{
    public int trackIndex;
    public EItemID itemID;
    public FQucikSlotInfo(int trackIndex, EItemID itemID)
    {
        this.trackIndex = trackIndex;
        this.itemID = itemID;
    }  
    public void Empty()
    {
        trackIndex = -1;
        itemID = EItemID._END;
    }
    public bool IsEmpty()
    {
        return (trackIndex == -1);
    }
}
 
public class PlayerQuickSlot : MonoBehaviour
{
    private PlayerUIController cachedUIController;
    private PlayerStorage cachedStorage;
    private PlayerStat cachedStat;
    private DuckSpeechBubble cachedSpeech;

    private FQucikSlotInfo[] arrQuickSlot = new FQucikSlotInfo[DuckDefine.MAX_QUICKSLOT_CNT];
    private Dictionary<EItemID, int> itemIdToSlot = new();
    private Dictionary<int, int> trackIndexToSlot = new(); 
     
    private void Awake()
    {
        cachedUIController = GetComponent<PlayerUIController>();
        cachedStorage = GetComponent<PlayerStorage>();
        cachedStat = GetComponent<PlayerStat>();
        cachedSpeech = GetComponent<DuckSpeechBubble>();

        for (int i = 0; i < DuckDefine.MAX_QUICKSLOT_CNT; i++)
        {
            var slotInfo = new FQucikSlotInfo();
            slotInfo.Empty();
            arrQuickSlot[i] = slotInfo;
        }
    }
    private void Start()
    {
        var playData = GameInstance.Instance.SAVE_GetCurPlayData();
        var src = playData.qickSlotData.arrQuickSlot;
         
        // 런타임 슬롯은 무조건 Empty로 시작
        arrQuickSlot = new FQucikSlotInfo[src.Length];
        for (int i = 0; i < arrQuickSlot.Length; i++)
        {
            arrQuickSlot[i].Empty();
        }

        itemIdToSlot.Clear();
        trackIndexToSlot.Clear();
         
        // 저장 데이터 기준으로 복구
        for (int i = 0; i < src.Length; i++)
        {
            var info = src[i];
            if (info.IsEmpty())
                continue;
             
            arrQuickSlot[i] = info;
            itemIdToSlot[info.itemID] = i;
            trackIndexToSlot[info.trackIndex] = i;
        }
    } 

    public PlayerQuickSlotData GetPlayerQuickSlotData()
    {
        var data = new PlayerQuickSlotData();
        for (int i =0; i < arrQuickSlot.Length; i++)
        {
            data.arrQuickSlot[i] = arrQuickSlot[i];
        }
        return data;
    }
    public void TryPushQuick(int quickSlotIndex, int trackIndex, ItemBase item)
    {
        if (item is not UseConsumBase)
            return;
         
        var itemID = item.GetItemData().itemID;

        // 대상 슬롯에 이미 다른 아이템이 있으면 제거
        if (!arrQuickSlot[quickSlotIndex].IsEmpty())
        {
            DisableSlot(quickSlotIndex);
        }
         
        // 같은 itemID가 다른 슬롯에 있으면 제거
        if (itemIdToSlot.TryGetValue(itemID, out int prevSlot)
            && prevSlot != quickSlotIndex)
        {
            DisableSlot(prevSlot);
        }
            
        // 삽입
        InsertItem(quickSlotIndex, trackIndex, item);
    }

    public bool CanUseQuickSlot(int _index)
    {
        if (IsEmpty(_index))
            return false; 

        // 추적아이템에 접근하고
        var item = cachedStorage.GetItem(arrQuickSlot[_index].trackIndex);
        if (!item)
            return false;

        // Heal일 경우 
        if (item is HealItem heal)
        {
            // 체력이 다찼는지 체크
            if (cachedStat.GetCurHpRatio() >= 1)
            {
                cachedSpeech.ActiveAutoDeleteSpeech("체력이 다찼어");
                return false;
            }
        }

        return true;
    }
    public bool CanInput(int _index)
    {
        if (IsEmpty(_index))
            return false;

        return true;
    }

    public bool HasSlotByItemId(EItemID itemId)
    {
        return itemIdToSlot.ContainsKey(itemId);
    }
    public bool HasSlotByTrackIndex(int trackIndex)
    {
        if (trackIndex < 0)
            return false;
         
        return trackIndexToSlot.ContainsKey(trackIndex);
    }
    public bool HasSlotBySlotIndex(int _slotIndex)
    {
        return !arrQuickSlot[_slotIndex].IsEmpty();
    }

    public int GetSlotByItemId(EItemID itemId)
    {
        return itemIdToSlot.TryGetValue(itemId, out int slotIndex)
            ? slotIndex
            : -1;
    }
    public int GetSlotByTrackId(int trackIndex)
    {
        return trackIndexToSlot.TryGetValue(trackIndex, out int slotIndex)
            ? slotIndex
            : -1;
    }
    public int GetEmptyIndex()
    {
        for (int i = 0; i < arrQuickSlot.Length; i++)
        {
            if (arrQuickSlot[i].IsEmpty())
                return i; 
        } 

        return -1;
    }
     
    public void TryUseConsum(int _index)
    {
        cachedStorage.TryUseConsum(arrQuickSlot[_index].trackIndex);
    }
    public void DisableSlot(int _slotIndex)
    {
        var info = arrQuickSlot[_slotIndex];

        arrQuickSlot[_slotIndex].Empty();
        itemIdToSlot.Remove(info.itemID);
        trackIndexToSlot.Remove(info.trackIndex);
          
        RenewDisableQuickSlotInfo(_slotIndex);
    }
    public void DisableOrChangeTrack(int _slotIndex)
    {
        var info = arrQuickSlot[_slotIndex];
        int newTrackIndex = cachedStorage.GetTrackIndexConsum(info.itemID);
        if (newTrackIndex == -1)
        {
            DisableSlot(_slotIndex);
        }
          
        else
        {
            ChangeTrackID(_slotIndex, newTrackIndex);
        }
    }
    public void RenewAllQuickSlotInfo()
    {
        for (int i= 0; i < arrQuickSlot.Length; i++)
        {
            FQucikSlotInfo info = arrQuickSlot[i];
            if (info.IsEmpty())
            {
                RenewDisableQuickSlotInfo(i);
            }

            else
            {
                RenewQuickSlotInfo(i);
            }
        }
    }

    public void ChangeTrackID(int slotIndex, int newTrackIndex)
    {
        // 슬롯이 비어있으면 무시 
        if (IsEmpty(slotIndex))
            return;   

        EItemID prevId      = arrQuickSlot[slotIndex].itemID;
        int prevTrackIndex  = arrQuickSlot[slotIndex].trackIndex;
        trackIndexToSlot.Remove(prevTrackIndex);
         
        arrQuickSlot[slotIndex].trackIndex = newTrackIndex;
        trackIndexToSlot[newTrackIndex] = slotIndex;
    }
    public void SwitchBtweenQuickSlot(int _aIndex, int _bIndex)
    {
        if (_aIndex == _bIndex)
            return;

        var aInfo = arrQuickSlot[_aIndex];
        var bInfo = arrQuickSlot[_bIndex];

        // --------------------
        // 기존 매핑 제거
        // --------------------
        if (!aInfo.IsEmpty())
        {
            trackIndexToSlot.Remove(aInfo.trackIndex);
            itemIdToSlot.Remove(aInfo.itemID);
        }

        if (!bInfo.IsEmpty())
        {
            trackIndexToSlot.Remove(bInfo.trackIndex);
            itemIdToSlot.Remove(bInfo.itemID);
        }

        // --------------------
        // 슬롯 데이터 스왑
        // --------------------
        arrQuickSlot[_aIndex] = bInfo;
        arrQuickSlot[_bIndex] = aInfo;

        // --------------------
        // 새 매핑 등록
        // --------------------
        if (!arrQuickSlot[_aIndex].IsEmpty())
        {
            var info = arrQuickSlot[_aIndex];
            trackIndexToSlot[info.trackIndex] = _aIndex;
            itemIdToSlot[info.itemID] = _aIndex;
        }

        if (!arrQuickSlot[_bIndex].IsEmpty())
        {
            var info = arrQuickSlot[_bIndex];
            trackIndexToSlot[info.trackIndex] = _bIndex;
            itemIdToSlot[info.itemID] = _bIndex;
        } 

        // --------------------
        // UI 갱신
        // --------------------
        if (arrQuickSlot[_aIndex].IsEmpty())
            RenewDisableQuickSlotInfo(_aIndex);
        else
            RenewQuickSlotInfo(_aIndex);

        if (arrQuickSlot[_bIndex].IsEmpty())
            RenewDisableQuickSlotInfo(_bIndex);
        else
            RenewQuickSlotInfo(_bIndex);
    }


    public void RenewDisableQuickSlotInfo(int _slotIndex)
    {
        cachedUIController.DisableQuickSlot(_slotIndex);
    } 
    public void RenewQuickSlotInfo(int _slotIndex)
    { 
        var item = cachedStorage.GetItem(arrQuickSlot[_slotIndex].trackIndex);
        if (!item)
        {
            RenewDisableQuickSlotInfo(_slotIndex);
            return;
        }
         
        if (item is not ConsumBase consum)
        {
            RenewDisableQuickSlotInfo(_slotIndex);
            return;
        }

        cachedUIController.RenewQuickSlotRenderInfo(_slotIndex, item);
        if (consum.IsCapacity())
        {
            cachedUIController.RenewQuickDurability(_slotIndex, consum.GetCurDurabilityRatio());
        }
        else
        {
            cachedUIController.RenewQuickSlotConsumCnt(_slotIndex, consum.GetCurCnt());
        }
    }
    
    private bool IsEmpty(int index)
    {
        if ((uint)index >= arrQuickSlot.Length)
            return true;

        return arrQuickSlot[index].IsEmpty();
    }
    private void InsertItem(int _slotIndex, int _itemIndex, ItemBase _item)
    { 
        if (_item is not ConsumBase consum)
            return;

        var itemData = _item.GetItemData();
        arrQuickSlot[_slotIndex].trackIndex = _itemIndex;
        arrQuickSlot[_slotIndex].itemID = itemData.itemID;
        itemIdToSlot[itemData.itemID] = _slotIndex;
        trackIndexToSlot[_itemIndex] = _slotIndex;

        RenewQuickSlotInfo(_slotIndex);
    } 
}