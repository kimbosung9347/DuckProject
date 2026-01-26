using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;
   
public class SlotInputBase : MonoBehaviour,
    // Hover
    IPointerEnterHandler, IPointerExitHandler, 
    // Drag
    IBeginDragHandler, IDragHandler, IEndDragHandler
    // Click
    , IPointerClickHandler

{
    [SerializeField] private RectTransform borderTransform;
     
    protected UIItemSlotBase cachedSlot;
    protected SlotController cachedSlotController;  
     
    private void Awake()
    {
        cachedSlot = GetComponent<UIItemSlotBase>();
        cachedSlotController = GameInstance.Instance.SLOT_GetSlotController();
    } 
     
    public virtual void OnPointerClick(PointerEventData eventData)
    { 
        // 좌클릭인 경우 
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (!CanLeftButton())
                return;

            OnLClick();
        } 

        // 우클릭일 경우
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (!CanRightButton())
                return;
             
            cachedSlotController.Action_OpenItemSelect(borderTransform);
        }
    } 
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 시작 -> 드래그 관련 정보를 전달
        if (!CanDrag())
            return;

        var slotType = cachedSlot.GetItemSlotType();
        var slotIndex = cachedSlot.GetIndex();
        var itemId = cachedSlot.GetItemID();
        cachedSlotController.RenewBeginDragSlot(slotType, slotIndex, itemId);
    }
    public virtual void OnDrag(PointerEventData eventData) 
    { 
    }
    public virtual void OnEndDrag(PointerEventData eventData)
    {
        cachedSlotController.Action_EndDrag(eventData);
    }  
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (!CanHover())
            return;
         
        var slotType = cachedSlot.GetItemSlotType();
        var slotIndex = cachedSlot.GetIndex();
        var itemId = cachedSlot.GetItemID();
        cachedSlotController.RenewHoverSlot(slotType, slotIndex, itemId);
    } 
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        cachedSlotController.ClearHoverInfo();
    }

    protected virtual void OnLClick()
    {
        if (!CanLeftButton())
            return;
         
        var slotType = cachedSlot.GetItemSlotType();
        var slotIndex = cachedSlot.GetIndex();
        var itemId = cachedSlot.GetItemID();
         
        FItemSlotInfo slotInfo = new FItemSlotInfo();
        slotInfo.slotType = slotType;
        slotInfo.slotIndex = slotIndex;
        slotInfo.itemId = itemId;
         
        cachedSlotController.RenewLClickSlot(slotInfo, cachedSlot);
    }
    protected virtual bool CanDrag()
    { 
        if (!IsExistItem())
            return false;

        return true;
    } 
    protected virtual bool CanHover()
    {
        if (!cachedSlotController.CanHover())
            return false;

        if (!IsExistItem())
            return false;

        return true;
    }
    protected virtual bool CanLeftButton()
    {
        if (!cachedSlotController.CanLeftButton())
            return false;

        if (!IsExistItem())
            return false;
          
        return true;
    }
      
    protected virtual bool IsExistItem()
    {
        // 해당 아이템이 존재하는지
        if (!cachedSlotController.IsExistItem(cachedSlot.GetItemSlotType(), cachedSlot.GetIndex()))
            return false;
         
        return true;
    }
    private bool CanRightButton()
    {
        if (!cachedSlotController.CanRightButton())
            return false;

        return true;
    }
}
