using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
 
public class CursorInUI : MonoBehaviour
{
    [SerializeField] private GameObject dragItem;
    [SerializeField] private GameObject toolTip;
    [SerializeField] private CursorToolTip cursorToolTip;
    [SerializeField] private HowToUseTip howToUseTip;
    [SerializeField] private UIItemSlotSelectList uiItemSlotSelectList;

    private RectTransform cachedRectTransform; 

    private void Awake()
    {
        cachedRectTransform = GetComponent<RectTransform>();
        DisableDragItem(); 
        DisableItemTooltip();
        DisableItemSlotSelectList();
    }
    private void OnEnable()
    {

    } 
    private void Update()
    {
        UpdateCursorPosition();
    }  
    private void UpdateCursorPosition()
    {          
        Vector2 mousePos = Mouse.current.position.ReadValue();
        cachedRectTransform.position = mousePos;
    }
      
    public UIItemSlotSelectList GetItemSlotList() { return uiItemSlotSelectList; }
    public GraphicRaycaster GetRaycaster() { return GetComponentInParent<GraphicRaycaster>(); }
      
    public List<(string key, string desc)> GetAllTips()
    {
        return howToUseTip.GetAllTips();
    }
    public void ActiveDrageItem(EItemID _itemId)
    { 
        DisableItemTooltip();
         
        // Sprite를 가져와야함
        Sprite icon = DuckUtill.GetItemSprite(_itemId);
        dragItem.gameObject.GetComponent<Image>().sprite = icon;
        dragItem.SetActive(true);
    }
    public void ActiveTooltipInfo(EItemID _itemId, EItemSlotType _itemSlot)
    {
        DisableDragItem();
        toolTip.SetActive(true);
        cursorToolTip.RenewToopTipInfo(_itemId);
    }
    public void ActiveItemSlotSelect(RectTransform _slotTransform)
    {
        uiItemSlotSelectList.Active(_slotTransform); 
    } 
    public void ActiveHowToUse()
    {
        howToUseTip.Active();
    } 

    public void InseretUseTip(string _key, string _name)
    {
        howToUseTip.InsertUseTip(_key, _name);
    } 
    public void InsertItemSlotSelect(string _key, string _desc)
    { 
        uiItemSlotSelectList.InsertItemSlotSelect(_key, _desc);
    }
  
    public void ClearUseTip()
    { 
        howToUseTip.ClearTip();
    }
    public void ClearSlotSelectList()
    {
        uiItemSlotSelectList.ClearSlotSelect();
    } 
     
    public void DisableDragItem()
    {
        dragItem.SetActive(false);
    }
    public void DisableItemTooltip()
    {
        toolTip.SetActive(false);
    }
    public void DisableItemSlotSelectList()
    {
        uiItemSlotSelectList.gameObject.SetActive(false);
    } 
}
  