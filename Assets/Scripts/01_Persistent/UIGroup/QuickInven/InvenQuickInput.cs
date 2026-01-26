using UnityEngine;
using UnityEngine.EventSystems;

public class InvenQuickInput : SlotInputBase
{
    protected override void OnLClick()
    {
    }
    protected override bool CanLeftButton()
    {
        return false;
    }
 
}
