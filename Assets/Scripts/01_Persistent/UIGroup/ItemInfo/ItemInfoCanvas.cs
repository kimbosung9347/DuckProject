using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ETradeType
{ 
    PlayerSell,         
    StoreSell,        
    End,        
}   

public class ItemInfoCanvas : MonoBehaviour
{
    // Default정보
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemWeight;
    [SerializeField] private TextMeshProUGUI itemDesc;
    [SerializeField] private Image ItemSprite;
     
    // Attach 정보
    [SerializeField] private GameObject attachSlotObj;
    [SerializeField] private UIAttachSlot muzzleSlot;
    [SerializeField] private UIAttachSlot scopeSlot;
    [SerializeField] private UIAttachSlot buttStockSlot;

    // Desc정보
    [SerializeField] private GameObject keyAndValuePrefab;
    [SerializeField] private RectTransform scrollViewContent;

    // Buy정보 
    [SerializeField] private GameObject buyObj; 
     
    [SerializeField] private TextMeshProUGUI sellOrBuyText;
    [SerializeField] private TextMeshProUGUI priceText;

    private InventoryCanvas cachedInvenCanvas;
    private ItemBoxCanvas cacheditemBoxCanvas;
    private WareHouseCanvas cachedWareHouseCanvas;

    private EAttachSlotState muzzleState;
    private EAttachSlotState scopleState;
    private EAttachSlotState stockState;

    private ItemBase cachedItem;
    private EItemSlotType cachedItemSlotType = EItemSlotType.End;
    private int cachedIndex;
    
    private bool isSellStore = false;
    private bool isActiveWeapon = false;

     
    private void Awake()
    {
        var uiGroup = GameInstance.Instance.UI_GetPersistentUIGroup();
        cachedInvenCanvas = uiGroup.GetInventoryCanvas();
        cacheditemBoxCanvas = uiGroup.GetItemBoxCanvas();
        cachedWareHouseCanvas = uiGroup.GetWareHouseCanvas();
    }

    // 아이템정보를 캐싱받아야함  
    private void Start()
    {
        gameObject.SetActive(false);

        Button btn = buyObj.gameObject.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(TryBuyItem);
    }    
      
    public void Active(ItemBase _item, EItemSlotType _itemSlotType, ETradeType _tradeType, int _index, Store _store = null)
    { 
        if (!_item)
            return;

        // 무기 인지 체크
        bool isWeapon = _item is Weapon weapon;
        isActiveWeapon = isWeapon ? true : false;
        
        GetComponent<UIAnimation>().Action_Animation();
         
        cachedItem = _item;
        cachedItemSlotType = _itemSlotType;
        cachedIndex = _index;
         
        gameObject.SetActive(true);
         
        // 캐싱
        var itemData = _item.GetItemData();
        var itemVsData = _item.GetItemVisualData(); 

        // Default정보 업데이트 
        {
            UpdateDefaultInfo(itemData, itemVsData);
        }  

        // Attach정보 업데이트
        {
            UpdateAttachInfo(_item); 
        }
         
        // Desc정보 업데이트
        {
            for (int i = scrollViewContent.childCount - 1; i >= 0; i--)
            {
                Destroy(scrollViewContent.GetChild(i).gameObject);
            } 
             
            List<FStatPair> stats = _item.GetStats();
            for (int i = 0; i < stats.Count; i++)
            {
                var go = Instantiate(keyAndValuePrefab, scrollViewContent);
                var kv = go.GetComponent<KeyAndValue>();
                kv.SetKeyText(stats[i].key);
                kv.SetKeyValue(stats[i].value);
            } 
        }
         
        // Store정보 업데이트
        {
            if (_tradeType == ETradeType.PlayerSell)
            {
                if (_store)
                {
                    buyObj.gameObject.SetActive(true);
                    isSellStore = false;

                    int backpackPrice = _store ? _store.GetPlayerSellPrice(_item.GetPrice()) : _item.GetPrice();
                    priceText.text = backpackPrice.ToString();
                    sellOrBuyText.text = "판매";
                }   
            }  
             
            else if (_tradeType == ETradeType.StoreSell)
            {
                if (_store)
                {
                    buyObj.gameObject.SetActive(true);
                    isSellStore = true;
                      
                    int storePrice = _store ? _store.GetStoreSellPrice(_item.GetPrice()) : _item.GetPrice();
                    priceText.text = storePrice.ToString();
                    sellOrBuyText.text = "구매"; 
                }
            }  

            else
            {
                buyObj.gameObject.SetActive(false);
            } 
        }
    } 
    public void ActiveByWareHouse(ref FItemShell _itemShell, int _index)
    {
        if (_itemShell.IsEmpty())
            return;

        // 무기 인지 체크
        isActiveWeapon = (EEquipSlot.Weapon == DuckUtill.GetEquipTypeByItemID(_itemShell.itemID));
          
        GetComponent<UIAnimation>().Action_Animation();

        cachedItemSlotType = EItemSlotType.WareHouse;
        cachedIndex = _index;
        gameObject.SetActive(true);

        var pair = DuckUtill.GetItemPair(_itemShell.itemID);
        if (pair == null)
            return;

        var itemData = pair.data;
        var itemVsData = pair.visual;
         
        // Default정보 업데이트 
        {
            UpdateDefaultInfo(itemData, itemVsData);
        }

        // Attach정보 업데이트
        {
            RenewAttachByItemShell(ref _itemShell); 
        }
         
        // Store정보 
        { 
            buyObj.gameObject.SetActive(false);
        }
    }
    public void Disable()
    {
        cachedItem = null;
        cachedItemSlotType = EItemSlotType.End;
        cachedIndex = -1;
        isActiveWeapon = false;
         
        gameObject.SetActive(false);
    }
    
    public EItemSlotType GetItemSlotType() {  return cachedItemSlotType; }
    public int GetIndex() { return cachedIndex; }
    public EAttachSlotState GetAttachState(EAttachmentType _type)
    { 
        switch (_type)
        {
            case EAttachmentType.Muzzle:
            {
                return muzzleState;
            }
                 
            case EAttachmentType.Scope:
            {
                return scopleState;
            }

            case EAttachmentType.Stock:
            {
                return stockState;
            }
        } 

        return EAttachSlotState.End;
    }
    public ItemBase GetItem() { return cachedItem; }
    public ItemBase GetAttach(EAttachmentType _type)
    {
        if (cachedItem is Weapon weapon)
        {
            return weapon.GetAttach(_type);
        } 

        return null;
    }
    public ItemBase RemoveAttach(EAttachmentType _type) 
    {
        if (!isActiveWeapon) 
            return null;

        if (cachedItem is not Weapon weapon)
            return null;

        ItemBase removecAttach = weapon.RemoveAttach(_type);
        RenewAttachment(weapon);
         
        return removecAttach;
    }
    public bool IsActiveWeapon() { return isActiveWeapon; }

    public void RenewAttachment(Weapon _weapn)
    { 
        var muzzleState = _weapn.GetAttachSlotState(EAttachmentType.Muzzle);
        var scopeState = _weapn.GetAttachSlotState(EAttachmentType.Scope);
        var stockState = _weapn.GetAttachSlotState(EAttachmentType.Stock);
        RenewAttachment(muzzleState, scopeState, stockState);
    }
    public void RenewAttachment(EAttachSlotState _muzzle, EAttachSlotState _scope, EAttachSlotState _stock)
    { 
        if (!isActiveWeapon)
            return;  
          
        switch (cachedItemSlotType)
        {
            case EItemSlotType.Equip:
            {
                cachedInvenCanvas.RenewEquipSlotAttachment(_muzzle, _scope, _stock);
            }
            break;

            case EItemSlotType.Backpack:
            {
                cachedInvenCanvas.RenewBackpackAttachment(cachedIndex, _muzzle, _scope, _stock);
            } 
            break;

            case EItemSlotType.ItemBox:
            {
                cacheditemBoxCanvas.RenewItemSlotAttachment(cachedIndex, _muzzle, _scope, _stock);
            }
            break;
                 
            case EItemSlotType.WareHouse:
            {
                cachedWareHouseCanvas.RenewWareHouseAttachment(cachedIndex, _muzzle, _scope, _stock);
            }
            break;
        }  

        if (cachedItem)
        {
            UpdateAttachInfo(cachedItem);
        }
    }
    public void RenewRenderInfoSlot(EAttachmentType _type, EItemID _id, EItemGrade _grede, string _name, Sprite _sprite)
    { 
        if (_type == EAttachmentType.End)
            return;

        if (_type == EAttachmentType.End)
            return;

        var slot = GetAttachSlot(_type);
        if (slot == null)
            return;

        slot.RenewItemID(_id);
        slot.RenewItemGrade(_grede);
        slot.RenewItemName(_name);
        slot.RenewSprite(_sprite);
        slot.ChangePush();
    }
    public void RenewAttachByItemShell(ref FItemShell _shell)
    {
        if (EEquipSlot.Weapon != DuckUtill.GetEquipTypeByItemID(_shell.itemID))
        {
            attachSlotObj.SetActive(false);
            return;
        }

        attachSlotObj.SetActive(true);

        muzzleState = _shell.muzzleState;
        scopleState = _shell.scopeState;
        stockState = _shell.stockState;

        UpdateAttachSlot(_shell.muzzleAttach, EAttachmentType.Muzzle, muzzleSlot, muzzleState);
        UpdateAttachSlot(_shell.scopeAttach, EAttachmentType.Scope, scopeSlot, scopleState);
        UpdateAttachSlot(_shell.stockeAttach, EAttachmentType.Stock, buttStockSlot, stockState);
    }
    public void CheckAndInsertAttachment(int _index, ItemBase _item)
    {
        if (_item is not Attachment attach)
            return;

        if (cachedItem is not Weapon weapon)
            return;
  
        // 비워있을때만 부착
        if (weapon.GetAttachSlotState(attach.GetAttachType()) == EAttachSlotState.Empty)
        {
            weapon.InsertAttachment(attach);
            var muzzleState = weapon.GetAttachSlotState(EAttachmentType.Muzzle);
            var scopeState = weapon.GetAttachSlotState(EAttachmentType.Scope);
            var stockState = weapon.GetAttachSlotState(EAttachmentType.Stock);
            RenewAttachment(muzzleState, scopeState, stockState);
        } 
    }


    public void ChangeEmpty(EAttachmentType _type)
    {
        if (_type == EAttachmentType.End)
            return;

        var slot = GetAttachSlot(_type);
        if (slot == null)
            return;

        slot.ChangeEmpty();
    } 
    public void ChangeX(EAttachmentType _type)
    {
        if (_type == EAttachmentType.End)
            return;

        var slot = GetAttachSlot(_type);
        if (slot == null)
            return;

        slot.ChangeX();
    }
    
    private void UpdateDefaultInfo(ItemData _itemData, ItemVisualData _vsData)
    { 
        // Default정보 업데이트 
        itemName.text = _itemData.itemName;
        itemWeight.text = _itemData.weight.ToString() + "kg";
        itemDesc.text = _itemData.itemDesc;
        ItemSprite.sprite = _vsData.iconSprite;
    }
    private void UpdateAttachInfo(ItemBase item)
    {
        if (item is not Weapon weapon)
        {
            attachSlotObj.SetActive(false);
            return;
        }
         
        attachSlotObj.SetActive(true);

        // 상태 캐싱
        muzzleState = weapon.GetAttachSlotState(EAttachmentType.Muzzle);
        scopleState = weapon.GetAttachSlotState(EAttachmentType.Scope);
        stockState = weapon.GetAttachSlotState(EAttachmentType.Stock);
         
        // 슬롯 갱신
        UpdateAttachSlot(weapon, EAttachmentType.Muzzle, muzzleSlot, muzzleState);
        UpdateAttachSlot(weapon, EAttachmentType.Scope, scopeSlot, scopleState);
        UpdateAttachSlot(weapon, EAttachmentType.Stock, buttStockSlot, stockState);
    }
      
    private void UpdateAttachSlot(Weapon weapon, EAttachmentType type, UIAttachSlot slot, EAttachSlotState state)
    {
        switch (state)
        {
            case EAttachSlotState.Exist:
                {
                    var attach = weapon.GetAttach(type);
                    var data = attach.GetItemData();
                    var vsData = attach.GetItemVisualData();

                    slot.RenewItemID(data.itemID);
                    slot.RenewItemGrade(data.grade);
                    slot.RenewItemName(data.itemName);
                    slot.RenewSprite(vsData.iconSprite);
                    slot.ChangePush();
                }
                break;

            case EAttachSlotState.Empty:
                {
                    slot.ChangeEmpty();
                }
                break;

            case EAttachSlotState.Disable:
                {
                    slot.ChangeX();
                } 
                break;
        }
    }
    private void UpdateAttachSlot(EItemID _attachId, EAttachmentType type, UIAttachSlot slot, EAttachSlotState state)
    {
        switch (state)
        {
            case EAttachSlotState.Exist:
                {
                    var pairData = DuckUtill.GetItemPair(_attachId);
                    var data = pairData.data;
                    var vsData = pairData.visual;
                     
                    slot.RenewItemID(data.itemID);
                    slot.RenewItemGrade(data.grade);
                    slot.RenewItemName(data.itemName);
                    slot.RenewSprite(vsData.iconSprite);
                    slot.ChangePush();
                }
                break;

            case EAttachSlotState.Empty:
                {
                    slot.ChangeEmpty();
                }
                break;

            case EAttachSlotState.Disable:
                {
                    slot.ChangeX();
                }
                break;
        }
    }
     
    private UIAttachSlot GetAttachSlot(EAttachmentType type)
    {
        return type switch
        {
            EAttachmentType.Muzzle => muzzleSlot,
            EAttachmentType.Scope => scopeSlot,
            EAttachmentType.Stock => buttStockSlot,
            _ => null
        };
    }
    private void TryBuyItem()
    { 
        GameInstance.Instance.SLOT_GetSlotController().Action_PreesBuyButton(isSellStore);
    }   
} 
 