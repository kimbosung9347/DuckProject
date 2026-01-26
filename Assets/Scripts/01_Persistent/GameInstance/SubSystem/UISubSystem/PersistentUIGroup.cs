using UnityEngine;

public class PersistentUIGroup : MonoBehaviour
{ 
    [SerializeField] private CursorCanvas cursorCanvas;
    [SerializeField] private MainHudCanvas mainHudCanvas; 
    [SerializeField] private ItemBoxCanvas itemBoxCanvas;
    [SerializeField] private InventoryCanvas invenCanvas;
    [SerializeField] private MenuCanvas menuCanvas;
    [SerializeField] private InvenQuickCanvas invenQuickCanvas;
    [SerializeField] private InteractionCanvas interactionCanvas;
    [SerializeField] private ItemInfoCanvas itemInfoCanvas;
    [SerializeField] private StoreCanvas storeCanvas;
    [SerializeField] private WareHouseCanvas warehouseCanvas;
    [SerializeField] private QuestCanvas questCanvas;
    [SerializeField] private SettingCanvas settingCanvas;
    [SerializeField] private AdornCanvas adornCanvas; 
    [SerializeField] private SleepCanvas sleepCanvas;
    [SerializeField] private SetLevelCanvas setLevelCanvas;
    [SerializeField] private DisableCanvas disableCanvas;
    [SerializeField] private EndSallyForthCanvas endSallyForthCanvas;
    [SerializeField] private MinimapCanvas minimapCanvas;
    [SerializeField] private StopCanvas stopCanvas;    

    // 점점 작아지는 원 같은거
    [SerializeField] private RadiusCollapseCanvas radiusCanvas;
    // 빌보드
    [SerializeField] private UIInteractionBillboard interactionBillboard;
     
    public void CachePlayerInfo()
    {

    }


    public CursorCanvas GetCursorCanvas() {  return cursorCanvas; } 
    public ItemBoxCanvas GetItemBoxCanvas() { return itemBoxCanvas; }
    public InventoryCanvas GetInventoryCanvas() { return invenCanvas; }
    public MenuCanvas GetMenuCanvas() { return menuCanvas; }
    public MainHudCanvas GetHudCanvas() { return mainHudCanvas; }
    public InvenQuickCanvas GetInvenQuickCanvas() { return invenQuickCanvas; }
    public InteractionCanvas GetInteractionCanvas() { return interactionCanvas; }
    public UIInteractionBillboard GetInteractionBillboard() { return interactionBillboard; }
    public ItemInfoCanvas GetItemInfoCanvas() { return itemInfoCanvas; }
    public StoreCanvas GetStoreCanvas() { return storeCanvas; }
    public WareHouseCanvas GetWareHouseCanvas() { return warehouseCanvas; }
    public QuestCanvas GetQuestCanvas() { return questCanvas; } 
    public AdornCanvas GetAdornCanvas() { return adornCanvas; }  
    public SleepCanvas GetSleepCanvas() { return sleepCanvas; } 
    public SetLevelCanvas GetSetLevelCanvas() { return setLevelCanvas; }
    public EndSallyForthCanvas GetEndSallyForthCanvas() { return endSallyForthCanvas; }
    public DisableCanvas GetDisableCanvas() { return disableCanvas; }
    public MinimapCanvas GetMinimapCanvas() { return minimapCanvas; }
    public StopCanvas GetStopCanvas() { return stopCanvas; }
     
        
    public RadiusCollapseCanvas GetRadiusCollaspeCanvas() { return radiusCanvas; }  
    public SettingCanvas GetSettingCanvas() { return settingCanvas; }
}  
