using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
 
public class WareHouseCanvas : SlotCanvasBase
{
    [SerializeField] private UIItemSlotBase[] slots = new UIItemSlotBase[DuckDefine.MAX_WAREHOUSE_COUNT];
    [SerializeField] private Button allInsertBackpackToWare;
    [SerializeField] private GameObject pageButtonPrefab;
    [SerializeField] private Transform pageButtonRoot;

    [SerializeField] private TextMeshProUGUI wareHouseName;
    [SerializeField] private Color activeColor; 
    [SerializeField] private Color disableColor;

    private WareHouse cachedWareHouse;
    private List<WareHousePageButton> listPageButtons = new();

    private void Awake()
    {
        gameObject.SetActive(false); 

        for (int i = 0; i < slots.Length; i++)
        {
            var slot = slots[i];
            slot.SetIndex(i);
        }

        allInsertBackpackToWare.onClick.RemoveAllListeners();
        allInsertBackpackToWare.onClick.AddListener(OnPressPushBackpackToWare);
    } 

    public void Active(WareHouse _wareHouse)
    {  
        gameObject.SetActive(true);

        cachedWareHouse = _wareHouse;

        int buttonCnt = cachedWareHouse.GetButtonCnt();
         
        // 버튼 삭제
        for (int i = pageButtonRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(pageButtonRoot.GetChild(i).gameObject);
        }

        // 버튼 생성 
        listPageButtons.Clear();
        for (int i = 0; i < buttonCnt; i++) 
        {
            GameObject go = Instantiate(pageButtonPrefab, pageButtonRoot);
            var pageBtn = go.GetComponent<WareHousePageButton>();
            pageBtn.Init(i, OnPressPageButton);
            listPageButtons.Add(pageBtn);
        }  
         
        // 페이지 갱신
        ChangePage(cachedWareHouse.GetPageIndex());
         
        cachedWareHouse.RenewWareHouseCnt();

        GetComponent<UIMovement>()?.PlayAppear();

    } 
    public void Disable() 
    {
        GetComponent<UIMovement>()?.PlayDisappear();
        cachedWareHouse = null;
    } 

    public WareHouse GetWareHouse() { return cachedWareHouse; }
    public void DisableGameObject()
    {
        gameObject.SetActive(false);    
    }
    public void ChangePage(int _index)
    {
        if (!cachedWareHouse)
            return;

        for (int i = 0; i < listPageButtons.Count; i++)
            listPageButtons[i].SetColor(disableColor);

        if (_index >= 0 && _index < listPageButtons.Count)
            listPageButtons[_index].SetColor(activeColor);

        cachedWareHouse.ChangePage(_index);
    } 

    public void RenewWareHouseSlotRenderInfo(int _index, EItemID _id, EItemGrade _grade, string _name, Sprite _sprite)
    {
        if (_index < 0 || _index >= slots.Length)
            return;

        var slot = slots[_index];
        RenewSlotRenderInfo(slot, _id, _grade, _name, _sprite);

        slot.SetShowSlotDesc(EShowSlotDesc.None);
        slot.ChangePush();
    } 
    public void RenewWareHouseDurability(int _index, float _durabilityRatio)
    {
        if (_index < 0 || _index >= slots.Length)
            return;
         
        RenewSlotDurability(slots[_index], _durabilityRatio);
    }
    public void RenewWareHouseConsumCnt(int _index, int _cnt)
    {
        if (_index < 0 || _index >= slots.Length)
            return;
         
        RenewSlotCnt(slots[_index], _cnt);
    } 
    public void RenewWareHouseCnt(float _cur, float _max)
    {
        wareHouseName.text = $"<b>창고<size=55%> ({_cur}/{_max})</size></b>";
    } 
    public void RenewWareHouseAttachment(int _index, EAttachSlotState _muzzle, EAttachSlotState _scope, EAttachSlotState _stock)
    {
        RenewSlotAttachment(slots[_index], _muzzle, _scope, _stock);
    } 
    public void RenewButtonColor(int _index)
    {
        for (int i = 0; i < listPageButtons.Count; i++)
            listPageButtons[i].SetColor(i == _index ? activeColor : disableColor);
    } 
     
    public void ActiveSlotObject(int _index)
    {
        slots[_index].gameObject.SetActive(true);
    }
    public void DisableSlotObject(int _index)
    {
        slots[_index].gameObject.SetActive(false); 
    }
    public void DisableWareHouseSlot(int _index)
    {
        RenewChangeEmpty(slots[_index]);
    } 
    public void HideBackpackAttachment(int _index)
    {
        RenewSlotHideAttach(slots[_index]);
    } 
    public void HideGuageAndCnt(int _index)
    {
        RenewHideGuageAndCnt(slots[_index]);
    }
     
    private void OnPressPageButton(int _index)
    {
        if (!cachedWareHouse)
            return;
          
        cachedWareHouse.ChangePage(_index);
    } 
    private void OnPressPushBackpackToWare()
    {
        var slotController = GameInstance.Instance.SLOT_GetSlotController();
        slotController.Action_PushBackpackToWare();
    }   


#if UNITY_EDITOR
    [ContextMenu("Bind Slots")]
    private void BindSlots() 
    {
        var found = GetComponentsInChildren<UIItemSlotBase>(true);

        slots = new UIItemSlotBase[DuckDefine.MAX_WAREHOUSE_COUNT];

        int count = Mathf.Min(DuckDefine.MAX_WAREHOUSE_COUNT, found.Length);
        for (int i = 0; i < count; i++)
        {
            slots[i] = found[i];
            slots[i].SetIndex(i);
        }

        EditorUtility.SetDirty(this);
    }
#endif

}