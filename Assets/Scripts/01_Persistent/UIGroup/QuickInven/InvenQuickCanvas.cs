using System.Collections.Generic;
using UnityEngine;

public class InvenQuickCanvas : MonoBehaviour
{
    [SerializeField] UIMovement quickSlotMovement;

    [Header("퀵슬롯")]
    [SerializeField] private UIInvenQuickSlot[] arrQuickSlot = new UIInvenQuickSlot[DuckDefine.MAX_QUICKSLOT_CNT];
     
    private void Awake()
    {  
        for (int i = 0; i < arrQuickSlot.Length; i++)
        {
            arrQuickSlot[i].SetIndex(i);
            arrQuickSlot[i].ChangeEmpty();
        }
        gameObject.SetActive(false);
    }
    public void Active(bool _isUseMove = true)
    {  
        gameObject.SetActive(true);

        var quickSlot = GameInstance.Instance.PLAYER_GetPlayerQuickSlot();
        quickSlot.RenewAllQuickSlotInfo();
         
        if (_isUseMove)
        {
            quickSlotMovement.PlayAppear();
        }
        else
        { 
            quickSlotMovement.SetAppearImmediate();
        }  
    }  
    public void Disable()
    { 
        gameObject.SetActive(false);
    }

    public void RenewQuickSlot(int _index, EItemID _id, EItemGrade _grade, string _name, Sprite _sprite)
    {
        if (_index < 0 || _index >= arrQuickSlot.Length)
            return;
          
        var slot = arrQuickSlot[_index];
        slot.RenewItemID(_id);
        slot.RenewItemGrade(_grade);
        slot.RenewItemName(_name);
        slot.RenewSprite(_sprite);
        slot.ChangePush();
    }
    public void RenewQuickSlotDurability(int _index, float _durabilityRatio)
    {
        if (_index < 0 || _index >= arrQuickSlot.Length)
            return;

        var slot = arrQuickSlot[_index];
        slot.RenewDurability(_durabilityRatio);
    } 
    public void RenewQuickSlotConsumCnt(int _index, int _cnt)
    {
        if (_index < 0 || _index >= arrQuickSlot.Length)
            return;
         
        var slot = arrQuickSlot[_index];
        slot.RenewItemCnt(_cnt);
    }
    public void RenewQuickSlotTrackOriginIndex(int _index, int _originTrackIndex)
    {
        arrQuickSlot[_index].SetOriginIndex(_originTrackIndex);
    } 

    // 비워주기 
    public void DisableQuickSlot(int _index)
    {
        arrQuickSlot[_index].ChangeEmpty();
    }
} 
 