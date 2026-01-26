using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
  
public class StoreCanvas : SlotCanvasBase
{
    [Header("Sound")]
    [SerializeField] private AudioClip sellOrBuySuccessClip;
 
    [SerializeField] private StoreSellBoundary storeSellBoundary;
    [SerializeField] private UIStoreSlot[] storeSlots = new UIStoreSlot[SLOT_COUNT];
    [SerializeField] private GameObject refreshButtonObj;
    [SerializeField] private GameObject refreshTimeObj;
    [SerializeField] private TextMeshProUGUI refreshRemainText;
    [SerializeField] private TextMeshProUGUI complateBuyText;
    [SerializeField] private Color ActiveRefreshColor;
    [SerializeField] private Color DisableRefreshColor;
     
    private AudioSource uiAudioSource;
    private const int SLOT_COUNT = 30;
    private Store cachedStore = null;
    private int prevRefreshTime = -1;
        
    private void Awake()
    {
        gameObject.SetActive(false);
        complateBuyText.gameObject.SetActive(false);
        uiAudioSource = GetComponent<AudioSource>(); 

        for (int i = 0; i < storeSlots.Length; i++)
        {
            if (storeSlots[i] == null)
                continue;

            storeSlots[i].SetIndex(i);
        }
         
        // 새로 고침 버튼
        {
            Button btn = refreshButtonObj.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(TryRefresh);
        }
    }  
    private void Update()
    {
        if (!cachedStore)
            return;
         
        RenewStoreRefreshTime();
    }
    public Store GetStore()
    {
        return cachedStore;
    }
    public void Active(Store store)
    {
        if (!store)
            return;

        cachedStore = store;
        prevRefreshTime = -1; 
        gameObject.SetActive(true);

        var move = GetComponent<UIMovement>();
        if (move)
            move.PlayAppear();
         
        Refresh();
    } 
    public void Disable() 
    {
        cachedStore = null;

        var move = GetComponent<UIMovement>();
        if (move)
            move.PlayDisappear();
         
        complateBuyText.gameObject.SetActive(false);
    }
    public void Refresh()
    {
        if (!cachedStore)
            return;
         
        ItemBase[] itemList = cachedStore.GetItemList();
        for (int i = 0; i < SLOT_COUNT; i++)
        {
            // 슬롯 자체가 없으면 스킵
            if (storeSlots[i] == null)
                continue;

            // 아이템 인덱스 초과 → 빈 슬롯
            if (i >= itemList.Length)
            {
                RenewEmpty(i); 
                continue;
            }

            ItemBase item = itemList[i];
             
            if (item == null)
            {
                RenewEmpty(i);
                continue;
            }

            var itemData = item.GetItemData();
            var visualData = item.GetItemVisualData();

            RenewRenderInfo(
                i,
                itemData.itemID,
                itemData.grade,
                itemData.itemName,
                visualData != null ? visualData.iconSprite : null
            );

            bool isWeapon = false;

            // 장비
            if (item is EquipBase equip)
            {
                if (equip is Weapon weapon)
                {
                    isWeapon = true;
                    RenewStoreAttachment(
                        i,
                        weapon.GetAttachSlotState(EAttachmentType.Muzzle),
                        weapon.GetAttachSlotState(EAttachmentType.Scope),
                        weapon.GetAttachSlotState(EAttachmentType.Stock)
                    );
                }

                if (equip.IsUseDuration())
                {
                    RenewStoreSlotDurability(i, equip.GetCurDurabilityRatio());
                }
                else
                {
                    RenewStoreSlotCnt(i, 1);
                }
            }
            // 부착물
            else if (item is Attachment)
            {
                RenewStoreHideGuageAndCnt(i);
            }
            // 소비템
            else if (item is ConsumBase consume)
            {
                if (consume.IsCapacity())
                {
                    RenewStoreSlotDurability(i, consume.GetCurDurabilityRatio());
                }
                else
                {
                    RenewStoreSlotCnt(i, consume.GetCurCnt());
                }
            }

            if (!isWeapon)
            {
                RenewHideBackpackAttachment(i);
            }
        }

        refreshButtonObj.GetComponent<Image>().color = DisableRefreshColor;
        RenewStoreRefreshTime();
          
        GameInstance.Instance.SLOT_GetSlotController().ClearLSelectSlotInot();
    }
    public void ComplateBuyOrSell(bool _isBuy)
    {
        // Sound 재생  
        uiAudioSource.PlayOneShot(sellOrBuySuccessClip); 

        complateBuyText.text = _isBuy ? "구매성공" : "판매성공";
        StopAllCoroutines();
        StartCoroutine(Co_ComplateBuy());
    }
    public void RenewEmpty(int index)
    {
        storeSlots[index].ChangeEmpty();
    }
    public void DisableGameObject()
    {
        gameObject.SetActive(false);
    }
    public bool IsMouseInside(Vector2 screenPos) 
    { 
        if (!gameObject.activeInHierarchy) 
            return false;
         
        return storeSellBoundary.IsMouseInside(screenPos);
    }
     

    private void RenewRenderInfo(int _index, EItemID _id, EItemGrade _grade, string _name, Sprite _sprite)
    {
        var slot = storeSlots[_index];
        RenewSlotRenderInfo(slot, _id, _grade, _name, _sprite);
         
        slot.ChangePush(); 
    }
    private void RenewStoreSlotDurability(int index, float durabilityRatio)
    { 
        RenewSlotDurability(storeSlots[index], durabilityRatio);
    }
    private void RenewStoreSlotCnt(int index, int cnt)
    { 
        RenewSlotCnt(storeSlots[index], cnt);
    }
    private void RenewStoreAttachment(int index, EAttachSlotState muzzle, EAttachSlotState scope, EAttachSlotState stock)
    {
        RenewSlotAttachment(storeSlots[index], muzzle, scope, stock);
    } 
      
    private void RenewStoreHideGuageAndCnt(int index)
    {
        RenewHideGuageAndCnt(storeSlots[index]);
    } 
    private void RenewHideBackpackAttachment(int index)
    {
        RenewSlotHideAttach(storeSlots[index]); 
    }
    private void RenewStoreRefreshTime()
    {
        int cur = Mathf.CeilToInt(cachedStore.GetRefreshTime());
        if (cur == prevRefreshTime)
            return;

        prevRefreshTime = cur;

        if (cur <= 0)
        {
            refreshRemainText.text = "입고 완료";
            refreshButtonObj.GetComponent<Image>().color = ActiveRefreshColor;
            refreshButtonObj.GetComponent<UIAnimation>().Action_Animation();
        }  
        else
        {
            refreshRemainText.text = cur.ToString();
        }
    }  
      
    private IEnumerator Co_ComplateBuy()
    { 
        const float totalTime = 1f;
        const float peakTime = 0.8f;
          
        complateBuyText.gameObject.SetActive(true);

        Color baseColor = complateBuyText.color;
        baseColor.a = 0.1f; 
        complateBuyText.color = baseColor;

        float t = 0f;
        while (t < totalTime)
        {
            t += Time.unscaledDeltaTime;

            float alpha;
            if (t <= peakTime)
            {
                alpha = Mathf.Lerp(0.3f, 1f, t / peakTime);
            }
            else
            { 
                alpha = Mathf.Lerp(1f, 0.3f, (t - peakTime) / (totalTime - peakTime));
            }

            Color c = complateBuyText.color;
            c.a = alpha;
            complateBuyText.color = c;

            yield return null;
        }

        Color end = complateBuyText.color;
        end.a = 0.3f;
        complateBuyText.color = end;

        complateBuyText.gameObject.SetActive(false);
    }
    private void TryRefresh()
    {
        if (!cachedStore)
            return;

        if (!cachedStore.CanInputRefresh())
        {
            refreshTimeObj.GetComponent<UIAnimation>()?.Action_Animation();
            return;
        }
         
        cachedStore.RefreshItems();
    }

#if UNITY_EDITOR
    [ContextMenu("Bind Store Slots")]
    private void BindStoreSlots()
    {
        var found = GetComponentsInChildren<UIStoreSlot>(true);

        storeSlots = new UIStoreSlot[SLOT_COUNT];

        int count = Mathf.Min(SLOT_COUNT, found.Length);
        for (int i = 0; i < count; i++)
        {
            storeSlots[i] = found[i];
            storeSlots[i].SetIndex(i);
        }

        EditorUtility.SetDirty(this);
    }
#endif
}
