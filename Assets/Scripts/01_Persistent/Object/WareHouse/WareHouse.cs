using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
 
public class WareHouse : MonoBehaviour
{
    private List<ItemBase> listItem = new();
    private WareHouseCanvas cachedCanvas;
     
    private int curPageIndex = -1;
    private int curItemCnt = 0;
    private int maxItemCnt = 0; 
    private SortedSet<int> emptySlotSet = new();
 
    private void Awake()
    {
		emptySlotSet.Clear();
        cachedCanvas = GameInstance.Instance.UI_GetPersistentUIGroup().GetWareHouseCanvas();
        curPageIndex = 0;
        gameObject.SetActive(false);
    }
    public void Init()
    {  
        var playData = GameInstance.Instance.SAVE_GetCurPlayData();
        CreateWareHouseByWareHouseData(playData.characterData.wareHouseSize, playData.wareHouseData);
    }
    public int GetButtonCnt()
    {
        if (listItem == null || listItem.Count == 0)
            return 0;

        int perPage = DuckDefine.MAX_WAREHOUSE_COUNT;
        return Mathf.CeilToInt((float)listItem.Count / perPage);
    }
    public int GetPageIndex() { return curPageIndex; }
    public int GetMaxItemCnt() { return maxItemCnt; }
     
    public PlayerWarehouseItemData CreateWareHouseData()
    {
        var data = new PlayerWarehouseItemData();

        for (int i = 0; i < listItem.Count; i++)
        {
            if (listItem[i] == null)
                continue;

            var itemShell = new FItemShellIncludeIndex();
            itemShell.index = i;
            itemShell.shell = listItem[i].GetItemShell();
            data.items.Add(itemShell);
        }
         
        return data;
    }
    public void CreateWareHouseByWareHouseData(int _size, PlayerWarehouseItemData _data)
    {
        DelateAllItem();
         
        curItemCnt = 0; 
        // 슬롯을 만들고
        InitSlots(_size);

        var gameInstance = GameInstance.Instance;

        // 데이터를 기반으로 아이템을 만들어준다
        foreach (var itemData in _data.items)
        {
            int index = itemData.index;
            var item = gameInstance.SPAWN_MakeItem(itemData.shell);
            PushItem(index, item);
        }
 
        cachedCanvas.RenewWareHouseCnt(curItemCnt, listItem.Count);
    }
    public void DelateAllItem()
    {
        for (int i = 0; i < listItem.Count; i++)
        {
            if (listItem[i] != null)
            {
                Destroy(listItem[i].gameObject);
                listItem[i] = null;
            }
        } 

        listItem.Clear();
        emptySlotSet.Clear();
        curItemCnt = 0;
        maxItemCnt = 0;
    } 

    public ItemBase GetItem(int _index)
	{
		int convertIndex = originIndexByPage(_index);
		if (!CheckIndex(convertIndex))
			return null;

		return listItem[convertIndex];
	}
	public ItemBase RemoveItem(int _index)
	{
		int convertIndex = originIndexByPage(_index);
		if (!CheckIndex(convertIndex))
			return null;

		ItemBase item = listItem[convertIndex];
		if (item == null)
			return null;

		listItem[convertIndex] = null;      // 슬롯 비우기
		emptySlotSet.Add(convertIndex);     // 빈 슬롯 등록
		curItemCnt--;                       // 아이템 수 감소

        // 현재 페이지에 속할 경우만 UI 갱신
        EmptyUI(_index);
         
        cachedCanvas.RenewWareHouseCnt(curItemCnt, listItem.Count);

        return item;
	}
	public void InsertItem(int _index, ItemBase _item)
	{
        if (!_item)
            return;
         
        int convertIndex = originIndexByPage(_index);
		if (!CheckIndex(convertIndex))
			return;
          
        // 넣어주기
        PushItem(convertIndex, _item);

        // 현재 페이지에 속할 경우만 UI 갱신
        RenewUI(_index, _item);
         
        //
        cachedCanvas.RenewWareHouseCnt(curItemCnt, listItem.Count);
          
        // Sound 재생
        GameInstance.Instance.SOUND_PlaySoundSfx(ESoundSfxType.PickUp, transform.position);
    }
    public void InsertItemByOriginIndex(int _originindex, ItemBase _item)
    { 
        if (!_item)
            return;

        // _originindex는 "실제 슬롯 인덱스"로 그대로 사용
        if (!CheckIndex(_originindex))
            return;

        listItem[_originindex] = _item;
        emptySlotSet.Remove(_originindex);
        curItemCnt++;
         
        // 현재 페이지에 보일 때만 UI 갱신
        if (TryGetPageSlotIndex(_originindex, out int pageSlotIndex))
        {
            RenewUI(pageSlotIndex, _item);
        }  

        cachedCanvas.RenewWareHouseCnt(curItemCnt, listItem.Count);
    } 

    public void ExpandSlots(int _addSize)
    {
        if (_addSize <= 0)
            return;

        int startIndex = listItem.Count;

        for (int i = 0; i < _addSize; i++)
        {
            listItem.Add(null);                 // 새 슬롯은 비어있음
            emptySlotSet.Add(startIndex + i);   // 빈 슬롯 인덱스 등록
        }
    }
    public void ChangePage(int _pageIndex) 
    {
        int maxPage = GetButtonCnt();
        if (_pageIndex < 0 || _pageIndex >= maxPage)
            return;

        curPageIndex = _pageIndex;
         
        int startIndex = _pageIndex * DuckDefine.MAX_WAREHOUSE_COUNT;
        int endIndex = startIndex + DuckDefine.MAX_WAREHOUSE_COUNT;

        for (int slotIndex = 0; slotIndex < DuckDefine.MAX_WAREHOUSE_COUNT; slotIndex++)
        {
            int itemIndex = startIndex + slotIndex;

            if (itemIndex >= listItem.Count)
            {
                cachedCanvas.DisableSlotObject(slotIndex);
                continue;
            }  

            cachedCanvas.ActiveSlotObject(slotIndex);
             
            ItemBase item = listItem[itemIndex];
            if (item == null)
            {
                cachedCanvas.DisableWareHouseSlot(slotIndex);
            }
            else
            {
                RenewUI(slotIndex, item);
            }
        }

        cachedCanvas.RenewButtonColor(curPageIndex);
         
        // todo : 임시 스읍 
        GameInstance.Instance.SLOT_GetSlotController().ClearLselectWhenChangePage();
    }
    public int GetEmptySlotCount()
    {
        return emptySlotSet.Count;
    }
    public List<int> GetEmptySlotIndices(int _maxCount)
    {
        List<int> result = new();

        if (_maxCount <= 0)
            return result;

        foreach (int index in emptySlotSet)
        {
            result.Add(index);
            if (result.Count >= _maxCount)
                break;
        } 

        return result;
    }

    public bool IsExistItem(int _index)
    {
        int convertIndex = originIndexByPage(_index);
        if (!CheckIndex(convertIndex))
            return false;

        bool isExist = (listItem[convertIndex] != null);
        return isExist;
    }
    public void RenewWareHouseCnt()
    {
        cachedCanvas.RenewWareHouseCnt(curItemCnt, listItem.Count);
    }

    private void RenewUI(int _index, ItemBase _item)
    {
        var itemData = _item.GetItemData();
        var visualData = _item.GetItemVisualData();
          
        // 목표 슬롯 갱신
        cachedCanvas.RenewWareHouseSlotRenderInfo(_index, itemData.itemID, itemData.grade, itemData.itemName, visualData.iconSprite);

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
                cachedCanvas.RenewWareHouseAttachment(_index, muzzle, scope, stock);
            } 

            if (equip.IsUseDuration())
            {
                cachedCanvas.RenewWareHouseDurability(_index, equip.GetCurDurabilityRatio());
            }
              
            else
            {
                cachedCanvas.RenewWareHouseConsumCnt(_index, 1);
            }
        }

        // 부착물일 경우  
        else if (_item is Attachment attachment)
        {
            cachedCanvas.HideGuageAndCnt(_index);
        } 

        // 소비템일 경우
        else if (_item is ConsumBase consume)
        {
            if (consume.IsCapacity())
            {
                // 내구도 기반 소비
                cachedCanvas.RenewWareHouseDurability(_index, consume.GetCurDurabilityRatio());
            }
            else
            {
                // 개수 기반 소비 
                cachedCanvas.RenewWareHouseConsumCnt(_index, consume.GetCurCnt());
            } 
        }

        // 무기 아니면 -> 부착물 없음, 일단 이렇게 설계함, 나중에 부착물이 다른 아이템에도 생기면 수정해야하긴하는데, 계획에 없음
        if (!isWeapon)
        {
            cachedCanvas.HideBackpackAttachment(_index);
        } 
    }
    private void EmptyUI(int _index)
    {
        cachedCanvas.DisableWareHouseSlot(_index);
    }

    private void PushItem(int _originIndex, ItemBase _item)
    {   
        listItem[_originIndex] = _item;
        emptySlotSet.Remove(_originIndex);
        curItemCnt++;

        //  
        _item.Insert(transform); 
        _item.Detach(); 
    }
    private void InitSlots(int _size)
	{
        maxItemCnt = _size;
         
        listItem.Clear();
		emptySlotSet.Clear();
		curItemCnt = 0;

		for (int i = 0; i < _size; i++)
		{
			listItem.Add(null);      
			emptySlotSet.Add(i);     
		}
	}
	private bool CheckIndex(int _index)
    {
		if (_index < 0 || _index >= listItem.Count)
			return false;

        return true;
	}
    private int originIndexByPage(int _index)
    {
        return curPageIndex * DuckDefine.MAX_WAREHOUSE_COUNT + _index;
    }
    private bool TryGetPageSlotIndex(int originIndex, out int pageSlotIndex)
    {
        int pageStart = curPageIndex * DuckDefine.MAX_WAREHOUSE_COUNT;
        int pageEnd = Mathf.Min(pageStart + DuckDefine.MAX_WAREHOUSE_COUNT, listItem.Count);

        if (originIndex < pageStart || originIndex >= pageEnd)
        {
            pageSlotIndex = -1;
            return false;
        }

        pageSlotIndex = originIndex - pageStart;
        return true;
    }
}     
  