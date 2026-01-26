using UnityEngine;

public class UIBackpackSlot : UIItemSlotBase
{
    protected override void Awake()
    {
        base.Awake();
        slotType = EItemSlotType.Backpack;
    }    
 
    public override void ChangeEmpty()
    {
        base.ChangeEmpty();
    }   
    public override void ChangePush()
    {
        base.ChangePush(); 
    } 
}
