using UnityEngine;
using UnityEngine.UI;
  
// enum EEquipSlotState
// { 
//     Empty,
// 
// 
//     End,
// }
// 이것도 갱신시켜주는 구조로 만들자   
public class UIEquipSlot : UIItemSlotBase
{  
    [SerializeField] Image equipIconImage;
     
    protected override void Awake()
    {
        base.Awake();
        slotType = EItemSlotType.Equip;
    }  
    public override void ChangeEmpty()
    {
        base.ChangeEmpty();
        equipIconImage.gameObject.SetActive(true);
    } 
    public override void ChangePush()
    {  
        base.ChangePush();
        equipIconImage.gameObject.SetActive(false);
    } 
} 
