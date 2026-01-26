using UnityEngine;
using UnityEngine.UI;
 
public class UIStoreSlot : UIItemSlotBase
{
    [SerializeField] private Image SoldoutSprite;

    protected override void Awake()
    {
        base.Awake();
        slotType = EItemSlotType.Store;
    }
     
    public override void ChangePush()
    {
        base.ChangePush();

        SoldoutSprite.gameObject.SetActive(false);
    }
    public override void ChangeEmpty()
    {
        base.ChangeEmpty();

        SoldoutSprite.gameObject.SetActive(true);
    }
}
 