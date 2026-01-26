using UnityEngine;
using UnityEngine.EventSystems;

public class StoreSlotInput : SlotInputBase
{
    protected override bool CanDrag()
    {
        if (!base.CanDrag())
            return false;

        if (!cachedSlotController.CanBuy(cachedSlot.GetIndex()))
            return false;
         
        return true;
    } 
}
    