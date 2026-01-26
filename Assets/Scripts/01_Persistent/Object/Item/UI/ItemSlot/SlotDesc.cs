using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum EShowSlotDesc
{ 
    Cnt,
    Guage, 
    None,
} 

 
public class SlotDesc : MonoBehaviour
{ 
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemCnt;
    [SerializeField] private GameObject gaugeObj;
    [SerializeField] private GameObject cntObj;
    [SerializeField] private GameObject attachObj;
    [SerializeField] private Image imageGauge;
     
    [SerializeField] private Image muzzleSocket;
    [SerializeField] private Image scopeSocket;
    [SerializeField] private Image stockSocket;
      
    private RectTransform cachedImageGaugeTransform;
    private EShowSlotDesc showSlotDesc = EShowSlotDesc.None;
    private bool isShowAttach = false;
    private float guageMaxWidth;
     
    private void Awake()
    {
        ChangeEmpty();
         
        if (imageGauge)
        {
            cachedImageGaugeTransform = imageGauge.GetComponent<RectTransform>();
            guageMaxWidth = cachedImageGaugeTransform.rect.width;
        }
    }   
       
    public void ChangePush()
    {
        itemName.gameObject.SetActive(true);

        HideByCurActive();
    }   
    public void ChangeEmpty()
    { 
        itemName.gameObject.SetActive(false);
        if (gaugeObj)
        {
            gaugeObj.SetActive(false);
        }
        if (cntObj)
        {
            cntObj.SetActive(false);
        }
        if (attachObj)
        {
            attachObj.SetActive(false);
        } 
    }    
      
    public void RenewItemName(string _name)
    {
        itemName.text = _name; 
    }  
    public void RenewItemCnt(int _cnt)
    {
        if (_cnt > 1)
        {
            itemCnt.text = _cnt.ToString();
        }
        else if (_cnt == 1)
        {
            itemCnt.text = string.Empty;
        } 
         
        showSlotDesc = EShowSlotDesc.Cnt;
        HideByCurActive();
    }  
    public void RenewDurability(float _ratio)
    {  
        Vector2 size = cachedImageGaugeTransform.sizeDelta;
        size.x = guageMaxWidth * _ratio;

        cachedImageGaugeTransform.sizeDelta = size;
        cachedImageGaugeTransform.GetComponent<Image>().color = GetColorByDurabilityRatio(_ratio);
         
        showSlotDesc = EShowSlotDesc.Guage;
        HideByCurActive();
    } 
    public void RenewAttachment(EAttachSlotState _muzzle, EAttachSlotState _scope, EAttachSlotState _stock)
    {
        attachObj.gameObject.SetActive(true);
           
        var itemTable = GameInstance.Instance.TABLE_GetItemTable();
        muzzleSocket.color =  GetColorByAttachSlotState(_muzzle, itemTable.attachAtiveColor, itemTable.attachEmptyColor, itemTable.attachDisableColor);
        scopeSocket.color = GetColorByAttachSlotState(_scope, itemTable.attachAtiveColor, itemTable.attachEmptyColor, itemTable.attachDisableColor);
        stockSocket.color = GetColorByAttachSlotState(_stock, itemTable.attachAtiveColor, itemTable.attachEmptyColor, itemTable.attachDisableColor);
           
        isShowAttach = true;
    }   

    public void HideAttachement()
    {
        attachObj.gameObject.SetActive(false);
         
        isShowAttach = false;
    }
    public void HideGageAndCnt()
    {
        showSlotDesc = EShowSlotDesc.None;
        HideByCurActive();
    }
    public void SetShowSlotDesc(EShowSlotDesc _desc)
    {
        showSlotDesc = _desc;
    }
     
    private void HideByCurActive()
    {
        if (attachObj)
        {
            if (isShowAttach)
            {
                attachObj.SetActive(true);
            }
             
            else
            {
                attachObj.SetActive(false);
            }
        } 

        if (gaugeObj && cntObj)
        {
            if (showSlotDesc == EShowSlotDesc.Guage)
            {
                gaugeObj.SetActive(true);
                cntObj.SetActive(false);
            }
            else if (showSlotDesc == EShowSlotDesc.Cnt)
            {
                gaugeObj.SetActive(false);
                cntObj.SetActive(true);
            }
            else if (showSlotDesc == EShowSlotDesc.None)
            {
                gaugeObj.SetActive(false);
                cntObj.SetActive(false);
            }
        }
    }
    private Color GetColorByAttachSlotState(EAttachSlotState _state, Color _active, Color _empty, Color _disable)
    {
        if (_state == EAttachSlotState.Exist)
        {
            return _active;
        }
        else if (_state == EAttachSlotState.Empty)
        {
            return _empty;
        }
        else if (_state == EAttachSlotState.Disable)
        {
            return _disable;
        }
         
        return Color.white;
    }
    private Color GetColorByDurabilityRatio(float _ratio)
    {
        var itemTable = GameInstance.Instance.TABLE_GetItemTable();
        if (itemTable == null)
            return new Color(56 / 255f, 56 / 255f, 56 / 255f, 207 / 255f);

        if (_ratio >= 0.75)
        {
            return itemTable.durabilityUp75;
        }
        else if (0.25 < _ratio && _ratio < 0.75)
        {
            return itemTable.durabilityBetween25_75;
        }
        else
        {
            return itemTable.durabilityUnder25;
        }
    }
}
 