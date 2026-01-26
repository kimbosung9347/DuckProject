using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
enum EMenuCanvasDomain
{ 
    Inven,
    Stat,
    Map,
    Quest,
    End
} 
 
public class MenuCanvas : MonoBehaviour
{
    // 스텟칸바스
    [SerializeField] private StatCanvas statCanvas;
    [SerializeField] private SlotController slotController; 

    // 인벤토리 칸바스 Buttons
    [SerializeField] private Button invenButton;
    [SerializeField] private Button statButton;
    [SerializeField] private Button mapButton;
    [SerializeField] private Button questButton;
    [SerializeField] private Color selectedButtonColor;

    private InventoryCanvas cachedInventoryCanvas;
    private InvenQuickCanvas cachedQuickInvenCanvas;
    private QuestCanvas cachedQuestCanvas;
    private MinimapCanvas cachedMinimapCanvas;
     
    private EMenuCanvasDomain curActiveDomain = EMenuCanvasDomain.End;
     
    private void Awake()
    {
        // 버튼 바인딩 
        var uiGroup = GameInstance.Instance.UI_GetPersistentUIGroup();
        cachedInventoryCanvas = uiGroup.GetInventoryCanvas();
        cachedQuickInvenCanvas = uiGroup.GetInvenQuickCanvas();
        cachedQuestCanvas = uiGroup.GetQuestCanvas();
        cachedMinimapCanvas = uiGroup.GetMinimapCanvas(); 

    } 
    private void Start()
    {
        BindInvenButton();
        BindStatButton();
        BindMapButton();
        BindQuestButton();

        curActiveDomain = EMenuCanvasDomain.End;
        gameObject.SetActive(false);
    } 
     
    public void Active()
    { 
        gameObject.SetActive(true);
        invenButton.GetComponent<Image>().color = Color.white;
        statButton.GetComponent<Image>().color = Color.white;
        mapButton.GetComponent<Image>().color = Color.white;
        questButton.GetComponent<Image>().color = Color.white;
        PressInven(); 
    }  
    public void Disable()
    {
        // curActiveDomain -> Disable 
        gameObject.SetActive(false);
        switch (curActiveDomain)
        { 
            case EMenuCanvasDomain.Inven:
            { 
                cachedInventoryCanvas.Disable(false);
                cachedQuickInvenCanvas.Disable();
                slotController.ClearSlotInfo();
                slotController.ClearLSelectSlotInot();
            }  
            break; 

            case EMenuCanvasDomain.Stat:
            {
                statCanvas.gameObject.SetActive(false);
            } 
            break; 

            case EMenuCanvasDomain.Map:
            {
                cachedMinimapCanvas.Disable();
            }  
            break;

            case EMenuCanvasDomain.Quest:
            {
                cachedQuestCanvas.Disable();
            } 
            break;
        }

        curActiveDomain = EMenuCanvasDomain.End;
    } 
     
    public StatCanvas GetStatCanvas()
    {
        return statCanvas;
    } 

     
    private void PressInven()
    {
        if (curActiveDomain == EMenuCanvasDomain.Inven)
            return;
         
        ClearOtherCanvas();
         
        curActiveDomain = EMenuCanvasDomain.Inven;
        invenButton.GetComponent<Image>().color = selectedButtonColor;

        cachedInventoryCanvas.Active(false);
        cachedQuickInvenCanvas.Active(false);
    }  
    private void PressStat()
    {
        if (curActiveDomain == EMenuCanvasDomain.Stat)
            return;

        ClearOtherCanvas();

        curActiveDomain = EMenuCanvasDomain.Stat;
        statButton.GetComponent<Image>().color = selectedButtonColor;
        statCanvas.gameObject.SetActive(true);
    }
    private void PressMap()
    {
        if (curActiveDomain == EMenuCanvasDomain.Map)
            return;

        ClearOtherCanvas();

        curActiveDomain = EMenuCanvasDomain.Map;
        mapButton.GetComponent<Image>().color = selectedButtonColor;
        cachedMinimapCanvas.Active();
    }  
    private void PressQuest() 
    {
        if (curActiveDomain == EMenuCanvasDomain.Quest)
            return;

        ClearOtherCanvas();

        curActiveDomain = EMenuCanvasDomain.Quest;
        questButton.GetComponent<Image>().color = selectedButtonColor;
        cachedQuestCanvas.ActiveInprogressAndComplate();
    }  
     
    private void ClearOtherCanvas()
    {
        if (curActiveDomain == EMenuCanvasDomain.End)
            return; 
         
        if (curActiveDomain == EMenuCanvasDomain.Inven)
        {
            cachedInventoryCanvas.gameObject.SetActive(false);
            cachedQuickInvenCanvas.gameObject.SetActive(false);
            slotController.ClearSlotInfo();
            slotController.ClearLSelectSlotInot();
            invenButton.GetComponent<Image>().color = Color.white;
        } 
        else if (curActiveDomain == EMenuCanvasDomain.Stat)
        {
            statCanvas.gameObject.SetActive(false);
            statButton.GetComponent<Image>().color = Color.white;
        }
        else if (curActiveDomain == EMenuCanvasDomain.Map)
        {
            cachedMinimapCanvas.Disable(); 
            mapButton.GetComponent<Image>().color = Color.white;
        }
        else if (curActiveDomain == EMenuCanvasDomain.Quest)
        {
            cachedQuestCanvas.Disable();
            questButton.GetComponent<Image>().color = Color.white;
        }  
    } 

     
    /* Bind */
    private void BindInvenButton()
    {
        invenButton.onClick.AddListener(PressInven);
    }
    private void BindStatButton()
    {
        statButton.onClick.AddListener(PressStat);
    }
    private void BindMapButton()
    {
        mapButton.onClick.AddListener(PressMap);
    }
    private void BindQuestButton()
    {
        questButton.onClick.AddListener(PressQuest);
    } 
} 
