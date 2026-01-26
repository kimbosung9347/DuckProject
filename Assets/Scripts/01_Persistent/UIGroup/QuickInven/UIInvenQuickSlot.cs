using UnityEngine;

public class UIInvenQuickSlot : UIItemSlotBase
{
    private int originIndex;
     
    protected override void Awake()
    {
        base.Awake();
        slotType = EItemSlotType.Quick;
    }

    public override void ChangeEmpty()
    { 
        slotDesc.ChangeEmpty();

        sprite.gameObject.SetActive(false);
        border.gameObject.SetActive(false);
    }  
    public override void ChangePush()
    {
        slotDesc.ChangePush();
         
        border.gameObject.SetActive(true);
        sprite.gameObject.SetActive(true);
        sprite.gameObject.GetComponent<UIAnimation>().Action_Animation();
    }

    public int GetOriginIndex()
    {
        return originIndex;
    }
    public void SetOriginIndex(int _index)
    {
        originIndex = _index;
    } 
}  
   