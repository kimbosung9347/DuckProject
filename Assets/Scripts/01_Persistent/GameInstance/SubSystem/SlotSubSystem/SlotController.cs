using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public struct FItemSlotInfo
{
    public EItemSlotType slotType; 
    public int slotIndex;
    public EItemID itemId;
     
    public bool IsEmpty()
    {
        // 비워있는지 체크 
        return (slotIndex == -1);
    }
    public void Clear()
    {
        itemId = EItemID._END; 
        slotType = EItemSlotType.End; 
        slotIndex = -1;
    }
     
    public static bool operator ==(FItemSlotInfo a, FItemSlotInfo b)
    {
        return a.slotType == b.slotType &&
               a.slotIndex == b.slotIndex &&
               a.itemId == b.itemId;
    }
    public static bool operator !=(FItemSlotInfo a, FItemSlotInfo b)
    {
        return !(a == b);
    }
    public override bool Equals(object obj)
    {
        if (obj is FItemSlotInfo other)
            return this == other;
        return false;
    }
    public override int GetHashCode()
    {
        int hash = 17;
        hash = hash * 31 + slotType.GetHashCode();
        hash = hash * 31 + slotIndex.GetHashCode();
        hash = hash * 31 + itemId.GetHashCode();
        return hash;
    }
}

public class SlotController : MonoBehaviour 
{
    // SlotInfo 
    private FItemSlotInfo curHoverSlotInfo;
    private FItemSlotInfo curDragSlotInfo;
    private FItemSlotInfo curLSelectSlotInfo;
    private FItemSlotInfo curRSelectSlotInfo;
    private UIItemSlotBase lSlectedSlot;

    // Slot - Util 
    private SlotHandle slotHandle = new();
    private SlotCheck slotCheck = new();
    private SlotCursor slotCursor = new();
     
    private void Awake() 
    {
        gameObject.SetActive(false);
    }      
    public void Init()
    {
        curHoverSlotInfo.Clear();
        curDragSlotInfo.Clear();
        curLSelectSlotInfo.Clear();
        curRSelectSlotInfo.Clear();
        lSlectedSlot = null;
         
        slotHandle = new();
        slotCheck = new();
        slotCursor = new();
        slotCheck.Init();
        slotHandle.Init(slotCheck);
        slotCursor.Init(); 
    } 
    public void ChangeUI()
    { 
        ClearSlotInfo();
        ClearLSelectSlotInot();

        gameObject.SetActive(true);
    }
    public void ChangePlay()
    {
        gameObject.SetActive(false);
         
        ClearSlotInfo();
        ClearLSelectSlotInot();
    }
     
    public bool IsExistItem(EItemSlotType _slotType, int _index)
    {
        return slotHandle.IsExistItem( _slotType, _index );
    }
    public bool CanBuy(int _index)
    {
        return slotCheck.CanBuy(_index); 
    }
    public bool CanHover()
    {
        // Drag중이라면 Hover이 안된다 
        if (!curDragSlotInfo.IsEmpty())
            return false;

        return true;
    } 
    public bool CanInstantAction()
    {
        if (curHoverSlotInfo.IsEmpty())
        {
            return false; 
        }
         
        return true;
    } 
    public bool CanRightButton()
    { 
        if (!curRSelectSlotInfo.IsEmpty())
            return false;
         
        return true;
    }
    public bool CanLeftButton() 
    { 
        return true;
    }
    public bool CanInsertInventory()
    {
        return slotCheck.CanInsertInventory();
    }  
     
    public void Action_Instant(EItemSlotSelectType _type)
    {
        if (curLSelectSlotInfo == curHoverSlotInfo)
        {
            ClearLSelectSlotInot(); 
        }
         
        slotHandle.InteractionBySitulation(ref curHoverSlotInfo, _type);
         
        ClearHoverInfo();
        ClearSelectInfo();
    }  
    public void Action_OpenItemSelect(RectTransform _slotTransform)
    {
        if (curHoverSlotInfo.itemId == EItemID._END)
        {
            return; 
        }

        // 같다면 return;
        if (curRSelectSlotInfo == curHoverSlotInfo)
            return;

        // 현재 Hover중인 슬롯이 선택슬롯으로
        curRSelectSlotInfo = curHoverSlotInfo;
         
        // R클릭 
        slotCursor.RSelect(_slotTransform);
    }
    public void Action_Select()
    {
        if (curLSelectSlotInfo == curRSelectSlotInfo)
        { 
            ClearLSelectSlotInot();
        } 

        // Select체크 
        if (curRSelectSlotInfo.IsEmpty())
        {
            Action_CancleSelct();
            return;
        }
          
        EventSystem eventSystem = EventSystem.current;
        if (eventSystem == null)
        {
            Action_CancleSelct();
            return;
        } 

        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = Mouse.current.position.ReadValue();

        List<RaycastResult> results = new List<RaycastResult>();
        var raycaster = slotCursor.GetRaycaster();
        if (!raycaster) 
        {
            Action_CancleSelct();
            return;
        }

        // 결과 중에서 UIItemSlotSelect를 가진 오브젝트 찾기
        raycaster.Raycast(pointerData, results);
        foreach (var r in results)
        { 
            var slot = r.gameObject.GetComponent<UIItemSlotSelect>();
            if (slot)
            {
                slotHandle.InteractionBySitulation(ref curRSelectSlotInfo, slot.GetItemSlotSelectType());
                Action_CancleSelct();
                return;  
            }   
        }
    }
    public void Action_CancleSelct()
    {
        // 우클릭을 눌르면 현재 Select은 Cancle시켜줘야함
        if (curRSelectSlotInfo.IsEmpty())
            return;
         
        ClearSelectInfo();
         
        lSlectedSlot?.NotSelected();
        lSlectedSlot = null;
    } 
    public void Action_PreesBuyButton(bool _isSellStore)
    {
        if (_isSellStore)
        {
            if (curLSelectSlotInfo.slotType != EItemSlotType.Store)
                return;
             
            if (!slotHandle.TryBuy(ref curLSelectSlotInfo))
                return;
        }

        else
        {
            slotHandle.SellItem(ref curLSelectSlotInfo);
        } 
         
        ClearLSelectSlotInot();
    }
    public void Action_EndDrag(PointerEventData eventData)
    {
        if (curDragSlotInfo.slotType == EItemSlotType.End)
        {
            EndDrag();
            return;
        }

        if (curDragSlotInfo == curLSelectSlotInfo)
        {
            ClearLSelectSlotInot();
        }

        // 상점의 판매 바운더리임
        if (slotCheck.IsMouseInside(eventData.position))
        {
            slotHandle.SellItem(ref curDragSlotInfo);
            EndDrag();
            return;
        }

        var target = eventData.pointerCurrentRaycast.gameObject;
        if (!target)
        {
            EndDrag();
            return;
        }

        // 아이템 슬롯에 전달 되어짐
        var raycastItemSlot = target.GetComponentInParent<UIItemSlotBase>();
        if (!raycastItemSlot)
        {
            // 다른 아이템 슬롯 위에 있었는지 체크 
            EndDrag();
            return;
        }

        FItemSlotInfo raycastSlotInfo = new FItemSlotInfo();
        raycastSlotInfo.slotType = raycastItemSlot.GetItemSlotType();
        raycastSlotInfo.slotIndex = raycastItemSlot.GetIndex();
        raycastSlotInfo.itemId = raycastItemSlot.GetItemID();

        if (raycastSlotInfo == curLSelectSlotInfo)
        {
            ClearLSelectSlotInot();
        }

        // 같은 자리에 두었는지 예외 체크
        if (curDragSlotInfo == raycastSlotInfo)
        {
            EndDrag();
            return;
        }

        // 슬롯간의 이동 체크 
        slotHandle.InteractionBetwwenSlot(ref curDragSlotInfo, ref raycastSlotInfo);
        EndDrag();
    }
    public void Action_Sort(SortedSet<int> _empty, SortedSet<int> _fill)
    {
         slotHandle.SortPlayerInventroy(_empty, _fill);
    } 
    public void Action_PushBackpackToWare()
    {
        ClearLselectWhenChangePage();
        slotHandle.PushBackpackToWare();
    }  

    public void RenewHoverSlot(EItemSlotType _type, int _index, EItemID _itemId)
    {
        // 상점이 활성화 되어있는지 아닌지를 체크 해줘야함
        curHoverSlotInfo.slotType = _type;
        curHoverSlotInfo.slotIndex = _index;
        curHoverSlotInfo.itemId = _itemId;

        bool isActiveStore = slotCheck.IsActiveStore(); 
        slotCursor.RenewHoverSlot(_type, _itemId, isActiveStore);
    }
    public void RenewBeginDragSlot(EItemSlotType _type, int index, EItemID itemId)
    {
        curDragSlotInfo.slotType = _type;
        curDragSlotInfo.slotIndex = index;
        curDragSlotInfo.itemId = itemId;

        slotCursor.ActiveDrageItem(itemId); 
    }    
    public void RenewLClickSlot(FItemSlotInfo _slotInfo, UIItemSlotBase _selectedSlot)
    { 
        if (curLSelectSlotInfo == _slotInfo)
        {
            ClearLSelectSlotInot();
            return;  
        }

        curLSelectSlotInfo = _slotInfo;
        slotHandle.RenewLClickSlot(ref curLSelectSlotInfo, _selectedSlot);
    } 
      
    public void ClearHoverInfo()
    {
        curHoverSlotInfo.Clear();

        slotCursor.ClearHoverInfo();

    }
    public void ClearDragInfo() 
    {
        curDragSlotInfo.Clear();
    }
    public void ClearSelectInfo()
    {
        curRSelectSlotInfo.Clear();
         
        slotCursor.ClearSelectInfo();
    }
    public void ClearSlotInfo()
    { 
        ClearHoverInfo();
        ClearDragInfo();
        ClearSelectInfo();
    }     
    public void ClearLSelectSlotInot()
    {
        curLSelectSlotInfo.Clear();

        slotHandle.DisableLSelectInfo();
    } 
    public void ClearLselectWhenChangePage()
    {
        if (curLSelectSlotInfo.slotType == EItemSlotType.Backpack || 
            curLSelectSlotInfo.slotType == EItemSlotType.WareHouse)
        {
            ClearLSelectSlotInot();
        }
    }
    private void EndDrag()
    {
        slotCursor.DisableDragItem();
         
        ClearSlotInfo(); 
    }
}
 
