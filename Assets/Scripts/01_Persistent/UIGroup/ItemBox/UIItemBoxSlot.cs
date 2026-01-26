using NUnit.Framework.Interfaces;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
 
public class UIItemBoxSlot : UIItemSlotBase
{  
    [SerializeField] private Image hideSprite;

    private RectTransform cachedMagafierTransform;
    private EItemBoxSlotState curState = EItemBoxSlotState.End;
      
    private float angle = 0f;

    protected override void Awake()
    { 
        base.Awake();
        slotType = EItemSlotType.ItemBox; 
    } 
    private void Start()
    {  
    }
    private void Update()
    {
        switch (curState)
        {
            case EItemBoxSlotState.Searching:
            {
                MoveMagafier();
            }
            break;
        }  
    }

    public override void ChangeEmpty()
    { 
        base.ChangeEmpty();
        hideSprite.gameObject.SetActive(false);
    }

    public bool CanPutIn()
    {
        // Searching일때만가능
        if (curState == EItemBoxSlotState.Searching ||
            curState == EItemBoxSlotState.NotSearch)
        {
            return false;
        }

        return true;
    }
    public void CacheMagafierTransform(RectTransform _transform)
    {
        cachedMagafierTransform = _transform;   
    }
    public void ChangeItemBoxState(EItemBoxSlotState _state)
    {  
        curState = _state;
         
        if (_state == EItemBoxSlotState.NotExist)
        {
            gameObject.SetActive(false);
            ChangeNotExist();
            return;
        }
         
        gameObject.SetActive(true);
        switch (_state)
        {
            case EItemBoxSlotState.NotSearch: ChangeNotSerch(); break;
            case EItemBoxSlotState.Searching: ChangeSeraching(); break;
            case EItemBoxSlotState.Complate: ChangeComplate(); break;
            case EItemBoxSlotState.Empty: ChangeEmpty(); break;
        }
    } 

    private void ChangeNotSerch()
    {
        // 오브젝트 활성화하고, 숨기기 키고, 아이템 + Desc 끄고
        hideSprite.gameObject.SetActive(true);
        sprite.gameObject.SetActive(false); 
        slotDesc.ChangeEmpty();
    }
    private void ChangeSeraching()
    {
        hideSprite.gameObject.SetActive(true);
        sprite.gameObject.SetActive(false);
        slotDesc.ChangeEmpty();
           
        // 돋보기 설정 
        angle = 0f; 
        cachedMagafierTransform.SetParent(GetComponent<RectTransform>(), false);
        cachedMagafierTransform.anchoredPosition = Vector2.zero;
        cachedMagafierTransform.gameObject.SetActive(true);
    }
    private void ChangeComplate()
    {
        base.ChangePush();
        hideSprite.gameObject.SetActive(false);
        cachedMagafierTransform.gameObject.SetActive(false);
    } 
    private void ChangeNotExist()
    {
        gameObject.SetActive(false);
    } 
     

    private void MoveMagafier()
    {
        if (cachedMagafierTransform == null)
            return;
         
        if (!cachedMagafierTransform.gameObject.activeSelf)
            cachedMagafierTransform.gameObject.SetActive(true);
         
        RectTransform slotRect = GetComponent<RectTransform>();
           
        const float radius = 12f; 
        const float speed = 360;    
            
        angle += speed * Time.deltaTime;
        if (angle > 360f)
            angle -= 360f;

        float rad = angle * Mathf.Deg2Rad;
        float x = Mathf.Cos(rad) * radius;
        float y = Mathf.Sin(rad) * radius;

        // 슬롯 중심 좌표(로컬) 가져오기
        Vector2 slotCenter = slotRect.rect.center;

        // 돋보기를 슬롯 기준 좌표로 이동 (같은 Canvas 상에서 동작)
        cachedMagafierTransform.anchoredPosition = slotCenter + new Vector2(x, y);
    }
}
  