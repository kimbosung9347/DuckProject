using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 버리기 
public enum EItemSlotSelectType
{ 
    Pick,
    Use,
    Throw, 
 
    End,
}

public class UIItemSlotSelect : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI keyText;
    [SerializeField] public TextMeshProUGUI descText;
    [SerializeField] public Image imageColor;

    private EItemSlotSelectType itemSlotSelectType = EItemSlotSelectType.End;

    public void SetItemSlotSelectType(EItemSlotSelectType _itemSlotSelectType)
    {
        itemSlotSelectType = _itemSlotSelectType;
    }

    public EItemSlotSelectType GetItemSlotSelectType() { return itemSlotSelectType; }
      
} 
