using TMPro;
using UnityEngine;
using UnityEngine.UI; 
 
public class ItemBoxCanvas : SlotCanvasBase
{ 
    // [SerializeField]
    [SerializeField] private TextMeshProUGUI itemBoxDesc;

    [SerializeField] private Image backgroundImage;
    [SerializeField] private GameObject magnifier; 
    [SerializeField] private UIItemBoxSlot[] arrItemSlot;

    [SerializeField] private RectTransform nameTransform;
    [SerializeField] private RectTransform contentTransform;

    private ItemBoxBase cachedActiveItemBox;

    private void Awake()
    {
        for (int i = 0; i < arrItemSlot.Length; i++)
        {
            arrItemSlot[i].SetIndex(i);
        }  
    } 
    private void Start()
    {
        gameObject.SetActive(false); 
        magnifier.SetActive(false);
        for (int i= 0; i  < arrItemSlot.Length; i++)
        {
            arrItemSlot[i].ChangeItemBoxState(EItemBoxSlotState.Empty); 
            arrItemSlot[i].CacheMagafierTransform(magnifier.GetComponent<RectTransform>());
        }   
    }
    private void Update()
    {
        if (cachedActiveItemBox)
        {
            cachedActiveItemBox.UpdateState();
        }
    }
     
    public void Active(ItemBoxBase _itemBox)
    { 
        gameObject.SetActive(true);
        magnifier.SetActive(false); 
        cachedActiveItemBox = _itemBox;
        _itemBox.SetActiveInCanvas(true);
        GetComponent<UIMovement>().PlayAppear();
    }
    public void Disable()
    {
        cachedActiveItemBox?.SetActiveInCanvas(false); 
        cachedActiveItemBox = null;
        GetComponent<UIMovement>().PlayDisappear();
    }  
    public void Callback_Disable()
    {
        gameObject.SetActive(false);
    } 
      
    public ItemBoxBase GetItemBox() { return cachedActiveItemBox; }

    public void RenewItemBoxNameAndDesc(string _desc)
    {
        itemBoxDesc.text = _desc; 
    }
    public void RenewItemSlotState(int _index, EItemBoxSlotState _state)
    {
        var itemBoxSlot = arrItemSlot[_index];
        itemBoxSlot.ChangeItemBoxState(_state);
    } 
    public void RenewItemSlotRenderInfo(int _index, EItemID _id, EItemGrade _grede, string _name, Sprite _sprite)
    {
        var itemBoxSlot = arrItemSlot[_index];
        RenewSlotRenderInfo(itemBoxSlot, _id, _grede, _name, _sprite);
        itemBoxSlot.SetShowSlotDesc(EShowSlotDesc.None);
    }        
    public void RenewItemSlotConsumCnt(int _index, int _cnt)
    {
        var itemBoxSlot = arrItemSlot[_index];
        RenewSlotCnt(itemBoxSlot, _cnt);
    } 
    public void RenewItemSlotEquipDurability(int _index, float _ratio)
    {
        var itemBoxSlot = arrItemSlot[_index];
        RenewSlotDurability(itemBoxSlot, _ratio);
    }    
    public void RenewItemSlotAttachment(int _index, EAttachSlotState _muzzle, EAttachSlotState _scope, EAttachSlotState _stock)
    {
        var itemBoxSlot = arrItemSlot[_index];
        RenewSlotAttachment(itemBoxSlot,_muzzle,_scope,_stock);
    }   
    public void HideItemSlotAttachment(int _index)
    {
        var itemBoxSlot = arrItemSlot[_index];
        RenewSlotHideAttach(itemBoxSlot); 
    } 
     
    public void RenewBackgroundHeight(float _height)
    {
        RectTransform rect = backgroundImage.rectTransform;
        Vector2 size = rect.sizeDelta;
        size.y = _height; 
        rect.sizeDelta = size;
    }
} 
 