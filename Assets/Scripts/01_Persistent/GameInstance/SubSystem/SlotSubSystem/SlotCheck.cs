using System;
using UnityEngine;

public class SlotCheck
{  
    // 캐싱 
    private StoreCanvas cachedStoreCanvas;
    private InventoryCanvas cachedInvenCanvas;
    private PlayerStorage cachedPlayerStorage;

    public void Init()
    {
        var gameInstance = GameInstance.Instance;
        var uiGroup = gameInstance.UI_GetPersistentUIGroup();

        cachedStoreCanvas = uiGroup.GetStoreCanvas();
        cachedInvenCanvas = uiGroup.GetInventoryCanvas();
        cachedPlayerStorage = gameInstance.PLAYER_GetPlayerStorage();
    }

    public bool CanBuy(int _index)
    {
        var store = cachedStoreCanvas.GetStore();

        var item = store.GetItem(_index);
        if (!item)
            return false;

        if (!CanBuyItem(store.GetStoreSellPrice(item.GetPrice()))) 
            return false;
         
        if (!CanInsertInventory())
            return false;

        return true;
    }
    public bool CanBuyItem(int _targetPrice)
    {
        int curMoney = cachedPlayerStorage.GetMoney();
        if (_targetPrice > curMoney)
        {
            cachedInvenCanvas.ShakeMoney();
            return false;
        }

        return true;
    }
    public bool CanStore(float _weight)
    {
        if (!cachedPlayerStorage.CanStore(_weight))
        {
            cachedInvenCanvas.ShakeWeight();
            return false;
        }

        return true;
    }
    public bool CanInsertInventory()
    {
        if (!cachedPlayerStorage.IsEmptySlotSet())
        {
            cachedInvenCanvas.ShakeBackpack();
            return false;
        }

        return true;
    }

    public bool IsActiveStore()
    {
        bool isActiveStore = (cachedStoreCanvas.GetStore() != null);
        return isActiveStore; 
    }
    public bool IsMouseInside(Vector2 screenPos) 
    {
        return cachedStoreCanvas.IsMouseInside(screenPos);
    } 
}
 