using System;
using UnityEngine;
using UnityEngine.UI;
  
public class UIAttachSlot : UIItemSlotBase
{  
    [SerializeField] private Image attachImage;
    [SerializeField] private Image XImage;
    [SerializeField] private EAttachmentType attachType;
     
    protected override void Awake()
    {
        base.Awake();
        slotType = EItemSlotType.Attach;
         
        SetShowSlotDesc(EShowSlotDesc.None);
        SetIndex((int)attachType); 
    }  
    public override void ChangeEmpty()
    {
        base.ChangeEmpty(); 

        attachImage.gameObject.SetActive(true);
        XImage.gameObject.SetActive(false);
    } 
    public override void ChangePush()
    {  
        base.ChangePush();

        attachImage.gameObject.SetActive(false);
        XImage.gameObject.SetActive(false);
    } 

    public void ChangeX() 
    {
        base.ChangeEmpty(); 
        attachImage.gameObject.SetActive(false);
        XImage.gameObject.SetActive(true);
    }  
}  
