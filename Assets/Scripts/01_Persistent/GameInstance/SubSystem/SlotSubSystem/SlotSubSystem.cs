using UnityEngine;

public class SlotSubSystem : GameInstanceSubSystem
{
    SlotController slotController;
 
    public override void Init()
    {
        var controllers = Object.FindObjectsByType<SlotController>(FindObjectsSortMode.None);
        slotController = controllers[0];
    }  
    public override void LevelStart(ELevelType _type)
    {
    }
    public override void LevelEnd(ELevelType _type)
    {
    } 

    public SlotController GetSlotController() {  return slotController; }
     
}
