using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct FItemShellIncludeIndex
{
    public int index;
    public FItemShell shell;
}
 
[System.Serializable] 
public struct FItemShell
{
    public EItemID itemID;
    public int cnt;
    public float durabilityRatio;

    public EAttachSlotState muzzleState;
    public EAttachSlotState scopeState;
    public EAttachSlotState stockState;

    public EItemID muzzleAttach;
    public EItemID scopeAttach;
    public EItemID stockeAttach;

    public EItemID bulletId;
    public int bulletCnt;
     
    public void Empty()
    {
        itemID = EItemID._END;
        cnt = 0;
        durabilityRatio = 0f;

        muzzleState = EAttachSlotState.End;
        scopeState = EAttachSlotState.End;
        stockState = EAttachSlotState.End;

        muzzleAttach = EItemID._END;
        scopeAttach = EItemID._END;
        stockeAttach = EItemID._END;

        bulletId = EItemID._END;
        bulletCnt = 0;
    }
    public void InsertAttach(EAttachmentType _attachType, EItemID _itemID)
    {
        if (_attachType == EAttachmentType.Muzzle)
        {
            muzzleState = EAttachSlotState.Exist;
            muzzleAttach = _itemID;
        }
        else if (_attachType == EAttachmentType.Scope)
        {
            scopeState = EAttachSlotState.Exist;
            scopeAttach = _itemID;
        }
        else if (_attachType == EAttachmentType.Stock)
        {
            stockState = EAttachSlotState.Exist;
            stockeAttach = _itemID;
        }
    }

    public bool IsEmpty()
    {
        return itemID == EItemID._END;
    }
    public EAttachSlotState GetAttachSlotState(EAttachmentType _attachType)
    {
        if (_attachType == EAttachmentType.Muzzle)
        {
            return muzzleState;
        }
        else if (_attachType == EAttachmentType.Scope)
        {
            return scopeState;
        }
        else if (_attachType == EAttachmentType.Stock)
        {
            return stockState;
        }

        return EAttachSlotState.End;
    }
    public EItemID GetAttachID(EAttachmentType _attachType)
    {
        if (_attachType == EAttachmentType.Muzzle)
        {
            return muzzleAttach;
        }
        else if (_attachType == EAttachmentType.Scope)
        {
            return scopeAttach;
        }
        else if (_attachType == EAttachmentType.Stock)
        {
            return stockeAttach;
        }
        return EItemID._END;
    }
}
 
public class ItemBase : MonoBehaviour
{ 
    // 런타임 캐싱 
    protected GameObject billboardObject;
    protected ItemBillboard itemBillboard;
     
    // 캐싱 
    protected DuckStorage cachedDuckStorage;
    protected ItemBoxBase cachedItemBoxBase;
      
    private ItemData itemData;
    private ItemVisualData itemVisualData;

    private void Start()
    {  
    }
    public virtual void Init(ItemData _data, ItemVisualData _visual)
    {
        itemData = _data;
        itemVisualData = _visual; 

        // 상호작용 할떄 필요한 빌보드 객체 
        CreateBillboardObject();
    } 
    public virtual void Drop()
    {
        Vector3 pos = transform.position;
         
        // 살짝 랜덤 오프셋 (원형)
        Vector2 rand = Random.insideUnitCircle * 0.7f; 
        pos.x += rand.x;
        pos.z += rand.y;
        pos.y += 0f;  
         
        transform.position = pos; 
        gameObject.SetActive(true);

        // 빌보드 활성화 
        billboardObject.SetActive(true);

        // 탐지도 켜주기 
        billboardObject.GetComponent<DetectionTarget>().ActiveCollider();
         
        // Drop
        GameInstance.Instance.MAP_DropItem(this);
    }
    public virtual void Attach() 
    {
        gameObject.SetActive(true);
        // 상호작용도 끄고
        billboardObject.GetComponent<DetectionTarget>().DisableCollider();
    }
    public virtual void Detach()
    { 
        gameObject.SetActive(false);
    }
    public virtual float GetItemWeight()
    {
        return itemData.weight;
    }
    public virtual int GetPrice()
    { 
        return itemData.price;
    } 

    public virtual List<FStatPair> GetStats()
    { 
        return itemData.GetStats();
    }
    public virtual void RemoveFromInven()
    { 

    }
    public virtual FItemShell GetItemShell()
    {  
        var entry = new FItemShell();
        entry.itemID = itemData.itemID;
        return entry; 
    }
    public virtual bool IsMerge()
    {
        return false;
    } 

    public void Insert(Transform _transform)
    { 
        transform.SetParent(_transform, false);
        transform.localPosition = Vector3.zero;
        billboardObject.SetActive(false);
    } 
      
    public ItemData GetItemData() { return itemData; }
    public ItemVisualData GetItemVisualData() { return itemVisualData; }    
    public ItemBillboard GetItemBillobard() { return itemBillboard; }
    public EItemID GetItemID() { return GetItemData().itemID; }
     
    public void CacheDuckStorage(DuckStorage _storage)
    {
        cachedDuckStorage = _storage;
    }
    public void CacheItemBox(ItemBoxBase _itemBox)
    {
        cachedItemBoxBase = _itemBox;
    }
     
    public void RemovedInSlot()
    {
        cachedDuckStorage = null;
        cachedItemBoxBase = null;
    } 
    private void CreateBillboardObject()
    { 
        billboardObject = GameInstance.Instance.SPAWN_ItemBilloard();
        billboardObject.transform.SetParent(transform, false);

        itemBillboard = billboardObject.GetComponent<ItemBillboard>();
        var detectionTarget = itemBillboard.GetDetectionTarget(); 
         
        // 글자 갱신
        detectionTarget.SetInteractionKey("F");
        detectionTarget.SetInteractionDesc(itemData.itemName);
        
        // 이미지 갱신
        if (!itemVisualData)
        {
            Debug.Log($"CreateBillboardObject - {itemData.itemName}");
        }  
        itemBillboard.SetSprite(itemVisualData.iconSprite);
        
        // 아이템 캐싱
        var handleItem = billboardObject.GetComponent<HandleItem>();
        handleItem.CacheItem(this); 
    }   
}   
 
 