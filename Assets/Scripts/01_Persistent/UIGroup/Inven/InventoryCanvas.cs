using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCanvas : SlotCanvasBase
{
    [Header("돈")]
    [SerializeField] private TextMeshProUGUI moneyText;
     
    [Header("장비")]
    [SerializeField] private List<UIEquipSlot> listEquipSlots;
      
    [Header("가방")]
    [SerializeField] private UIBackpackSlot backpackSlotPrefab;         
    [SerializeField] private Transform backpackListContent;             
    [SerializeField] private TextMeshProUGUI backpackDesc;
    [SerializeField] private Button sortButton;

    [Header("무게")]
    [SerializeField] private UIInvenWeight invenWeight;
      
    private readonly List<UIBackpackSlot> listBackpackSlots = new();

    private void Awake()
    { 
        var gameinstance = GameInstance.Instance;

        for (int i= 0; i < listEquipSlots.Count; i++)
        {
            listEquipSlots[i].ChangeEmpty();
            listEquipSlots[i].SetIndex(i);
        } 
        gameObject.SetActive(false);
        invenWeight.gameObject.SetActive(false);
         
        sortButton.onClick.AddListener(() =>
        {
            gameinstance.PLAYER_GetPlayerStorage().Sort();
        }); 
    }
     
    public void Active(bool _useUIMove = true) 
    {

        gameObject.SetActive(true);
        invenWeight.gameObject.SetActive(true);

        var gameInstance = GameInstance.Instance;
        gameInstance.PLAYER_GetPlayerStorage().RenewAllBackpackInfo();
        gameInstance.PLAYER_GetPlayerEquip().RenewAllEquipInfo();
        // ?.RenewAllBackpackInfo();
          
        if (_useUIMove)
        {
            GetComponent<UIMovement>().PlayAppear();
        }
        else
        {
            GetComponent<UIMovement>().SetAppearImmediate();
        }
    }   
    public void Disable(bool _useUIMove = true)
    {
        invenWeight.gameObject.SetActive(false);

        if (_useUIMove)
        {
            GetComponent<UIMovement>().PlayDisappear();
        }
        else
        {
            gameObject.SetActive(false);
        } 
    }

    /* Money */ 
    public void RenewMoney(int money)
    { 
        moneyText.text = money.ToString();
    }
    public void ShakeMoney()
    {
        GameObject parentObj = moneyText.transform.parent.gameObject;
        parentObj.GetComponent<UIAnimation>().Action_Animation();
    } 
    public void ShakeWeight()
    {

    } 


    /* Equip */
    public void RenewEquipSlot(EEquipSlot _slot, EItemID _id, EItemGrade _grede, string _name, Sprite _sprite)
    {
        var itemBoxSlot = listEquipSlots[(int)_slot]; 
        itemBoxSlot.RenewItemID(_id);
        itemBoxSlot.RenewItemGrade(_grede);
        itemBoxSlot.RenewItemName(_name);
        itemBoxSlot.RenewSprite(_sprite);
        itemBoxSlot.SetShowSlotDesc(EShowSlotDesc.None);
         
        itemBoxSlot.ChangePush();
    }  
    public void RenewEquipSlotDurability(EEquipSlot _slot, float _durabilityRatio)
    {
        var itemBoxSlot = listEquipSlots[(int)_slot];
        itemBoxSlot.RenewDurability(_durabilityRatio);
    } 
    public void RenewEquipSlotAttachment(EAttachSlotState _muzzle, EAttachSlotState _scope, EAttachSlotState _stock)
    {
        var itemBoxSlot = listEquipSlots[(int)EEquipSlot.Weapon];
        itemBoxSlot.RenewAttachment(_muzzle, _scope, _stock);
    }  
    public void DisableEquipSlot(EEquipSlot _slot)
    {
        RenewChangeEmpty(listEquipSlots[(int)_slot]);
    }
       
    
    /* Backpack */
    public void MakeBackpackSlot(int _cnt)
    {
        if (listBackpackSlots.Count == _cnt)
            return;

        // 부족하면 생성
        while (listBackpackSlots.Count < _cnt)
        {
            int index = listBackpackSlots.Count;
            UIBackpackSlot slot = Instantiate(backpackSlotPrefab, backpackListContent);
            slot.SetIndex(index);   
            slot.ChangeEmpty(); 
            listBackpackSlots.Add(slot);
        }
          
        // 넘치면 제거
        // todo 드랍 
        while (listBackpackSlots.Count > _cnt)
        {
            var last = listBackpackSlots[^1];
            Destroy(last.gameObject);
            listBackpackSlots.RemoveAt(listBackpackSlots.Count - 1);
        }
    }
    public void ShakeBackpack()
    {
        backpackListContent.gameObject.GetComponent<UIAnimation>().Action_Animation();
    } 
     
    public void RenewBackpackSlot(int _index, EItemID _id, EItemGrade _grade, string _name, Sprite _sprite)
    {
        if (_index < 0 || _index >= listBackpackSlots.Count)
            return; 

        var slot = listBackpackSlots[_index];
        RenewSlotRenderInfo(slot, _id, _grade, _name, _sprite);
           
        slot.SetShowSlotDesc(EShowSlotDesc.None);
        slot.ChangePush();
    } 
    public void RenewBackpackSlotDurability(int _index, float _durabilityRatio)
    {
        if (_index < 0 || _index >= listBackpackSlots.Count)
            return;

        var slot = listBackpackSlots[_index]; 
        RenewSlotDurability(slot, _durabilityRatio); 
    }
    public void RenewBackpackSlotConsumCnt(int _index, int _cnt)
    {
        if (_index < 0 || _index >= listBackpackSlots.Count)
            return;

        var slot = listBackpackSlots[_index];
        RenewSlotCnt(slot, _cnt);
    }  
    public void RenewBackpackWeight(float _cur, float _max)
    {
        invenWeight.RenewWeight(_cur, _max);    
    }
    public void RenewBackpackAttachment(int _index, EAttachSlotState _muzzle, EAttachSlotState _scope, EAttachSlotState _stock)
    {  
        var itemBoxSlot = listBackpackSlots[_index];
        RenewSlotAttachment(itemBoxSlot, _muzzle, _scope, _stock);
    } 
    public void RenewBackpackDesc(string _desc)
    {
        backpackDesc.text = _desc;
    }
      
    public void DisableBackpackSlot(int _index)
    {
        RenewChangeEmpty(listBackpackSlots[_index]);
    }  
    public void HideBackpackAttachment(int _index)
    {
        var itemBoxSlot = listBackpackSlots[_index];
        RenewSlotHideAttach(itemBoxSlot);
    } 
    public void HideGuageAndCnt(int _index)
    {
        var itemBoxSlot = listBackpackSlots[_index];
        RenewHideGuageAndCnt(itemBoxSlot);
    } 
}
 