using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainHudCanvas : MonoBehaviour
{
    [Header("음식")]
    [SerializeField] private CircleGauge cachedFoodGuage;
    [SerializeField] private CircleGauge cachedWaterGuage;
      
    [Header("체력")]
    [SerializeField] private Image hpBar;
    [SerializeField] private TextMeshProUGUI hpText;

    [Header("퀵슬롯")]
    [SerializeField] private QuickWeaponSlot quickWeaponSlot;
    [SerializeField] private QuickConsum[] arrQuickConsums = new QuickConsum[DuckDefine.MAX_QUICKSLOT_CNT];

    [Header("퀵스킬")]
    [SerializeField] private QuickSkill quickDetectorSkill;
    [SerializeField] private QuickSkill quickRollSkill;

    [Header("상호작용게이지")]
    [SerializeField] private InteractionGuage interactionGuage;

    [Header("버프")]
    [SerializeField] private GameObject buffList;
    [SerializeField] private GameObject deBuffList; 
    [SerializeField] private GameObject buffUiPrefab;
    [SerializeField] private Color buffColor;
    [SerializeField] private Color DeBuffColor;

    [Header("적탐지")]
    [SerializeField] private RectTransform enemyDetectorList;
    [SerializeField] private GameObject enemyDetector;

    [Header("시간")]
    [SerializeField] private TextMeshProUGUI textHour;
    [SerializeField] private TextMeshProUGUI textMinit;
    [SerializeField] private TextMeshProUGUI textTimeState;
     
    private Dictionary<EBuffID, BuffDesc> cachedBuffDesc = new();
    private float maxHpWidth;
      
    private void Awake()
    {
        // 현재 width가 1최대치임 hpBar.flexibleWidth; maxHpWidth로 넣어준다 
        maxHpWidth = hpBar.rectTransform.rect.width;
        gameObject.SetActive(false);
    } 
     
    ////////////////
    /* Hp And Food*/
    public void RenewFoodGauge(float _gauge)
    {
        cachedFoodGuage.RenewGauge(_gauge);
    }
    public void RenewWaterGauge(float _gauge)
    {
        cachedWaterGuage.RenewGauge(_gauge);
    }
    public void RenewHp(float _cur, float _max)
    {
        float ratio = _cur / _max;
        hpText.text = $"{_cur:F1} / {_max}";
         
        Vector2 size = hpBar.rectTransform.sizeDelta;
        size.x = maxHpWidth * ratio;
        hpBar.rectTransform.sizeDelta = size;
    }
     
    ///////////////////////
    /* QuickSlot - Weapon*/
    public void EquipWeapon(EItemID _itemID)
    {
        quickWeaponSlot.EquipWeapon(_itemID);
    }
    public void DetachWeapon()
    {
        quickWeaponSlot.DetetchWeapon();
    }
    public void EnterAvaiableBullet()
    {
        quickWeaponSlot.EnterAvaiableBullet();
    } 
    public void ExitAvaiableBullet()
    {
        quickWeaponSlot.ExitAvaiableBullet();
    }
    public void InsertAvaiableBullet(EItemID _id, int _cnt)
    {
        quickWeaponSlot.InsertAvaiableBullet(_id, _cnt);
    }
    public void RenewAvaiableBulletCnt(EItemID _id, int _cnt)
    {
        quickWeaponSlot.RenewAvaiableBulletCnt(_id, _cnt);
    }
    public void RemoveAvaiableBullet(EItemID _id)
    {
        quickWeaponSlot.RemoveAvaiableBullet(_id);
    }

    public void SelectAvaiableBullet(EItemID _itemId)
    {
        quickWeaponSlot.SelectBullet(_itemId);
    } 
    public void NotSelectAvaiableBullet(EItemID _itemId)
    {
        quickWeaponSlot.NotSelect(_itemId);
    } 
    public void RenewBulletType(EItemID _itemID)
    {
        quickWeaponSlot.RenewWeaponBulletType(_itemID);
    } 
    public void RenewCurBullet(int _cnt)
    {
        quickWeaponSlot.RenewCurBullet(_cnt);
    }
    public void RenewMaxBullet(int _cnt)
    {
        quickWeaponSlot.RenewMaxBullet(_cnt);
    }
    
    public void ActiveDetectorSkill(bool _isActive)
    {
        quickDetectorSkill.ActiveCoolTime(_isActive); 
    }
    public void ActiveRollSkill(bool _isActive)
    {
        quickRollSkill.ActiveCoolTime(_isActive); 
    }  

    public void RenewDetectorSkillTime(float _curtime, float _maxTime)
    {
        quickDetectorSkill.SetCoolTimeFill(_curtime / _maxTime);
        quickDetectorSkill.SetCoolTimeText(_curtime); 
    }
    public void RenewRollSkillTime(float _curtime, float _maxTime)
    {
        quickRollSkill.SetCoolTimeFill(_curtime / _maxTime);
        quickRollSkill.SetCoolTimeText(_curtime);
    }  


    ////////////////////////
    /* QuickSlot - Consum */
    public void RenewItemSlotRenderInfo(int _index, EItemGrade _grede, string _name, Sprite _sprite)
    {
        var slot = arrQuickConsums[_index];
        slot.RenewItemSlotRenderInfo(_grede, _name, _sprite);
    }
    public void RenewConsumCnt(int _index, int _cnt)
    {
        var slot = arrQuickConsums[_index];
        slot.RenewConsumCnt(_cnt);
    }
    public void RenewCapacity(int _index, float _ratio)
    {
        var slot = arrQuickConsums[_index];
        slot.RenewCapacity(_ratio);
    }
    public void DisableConsumQuick(int _index)
    {
        var slot = arrQuickConsums[_index];
        slot.RemoveItem();
    }


    ///////////
    /* Buff */
    public void InsertBuff(EBuffID _buffId, Sprite _sprite, string _name, float _duration)
    {
        if (cachedBuffDesc.ContainsKey(_buffId))
        {
            RenewBuffDuration(_buffId, _duration);
            return;
        } 

        GameObject obj = Instantiate(buffUiPrefab, buffList.transform);
        var BuffDesc = obj.GetComponent<BuffDesc>(); 
        BuffDesc.ActiveBuff(buffColor, _sprite, _name, _duration.ToString());
        cachedBuffDesc.Add(_buffId, BuffDesc);
    } 
    public void InsertDeBuff(EBuffID _buffId, Sprite _sprite, string _name, float _duration)
    {
        if (cachedBuffDesc.ContainsKey(_buffId))
        {
            RenewBuffDuration(_buffId, _duration);
            return;
        }  
         
        GameObject obj = Instantiate(buffUiPrefab, deBuffList.transform);
        var BuffDesc = obj.GetComponent<BuffDesc>();
        BuffDesc.ActiveBuff(DeBuffColor, _sprite, _name, _duration.ToString());
        cachedBuffDesc.Add(_buffId, BuffDesc);
    }
    public void RemoveBuff(EBuffID _buffId)
    {
        if (cachedBuffDesc.TryGetValue(_buffId, out var buffDesc))
        {
            cachedBuffDesc.Remove(_buffId);
            Destroy(buffDesc.gameObject);
        }
    } 
    public void RenewBuffDuration(EBuffID _buffId, float _duration)
    {
        if (cachedBuffDesc.TryGetValue(_buffId, out var buffDesc))
        {
            buffDesc.RenewDuration(_duration.ToString());
        } 
    }

    //////////////
    /* Detector */
    public void MakeEnemyDetector(Transform _playerTransform, Transform _enemyTransform)
    {
        if (!enemyDetector || !_playerTransform || !_enemyTransform)
            return;
         
        // 생성 (HUD면 Canvas 하위, 월드면 적당한 부모로)
        GameObject go = Instantiate(enemyDetector, enemyDetectorList);
       
        // DetectEnemy 초기화
        UIEnemyDetect detect = go.GetComponent<UIEnemyDetect>();
        if (detect)
        {
            detect.Active(_playerTransform, _enemyTransform);
        }
    }

    ///////////
    /* Time */
    public void RenewTime(float _gameTime)
    {
        int hour = (int)_gameTime;
        int minute = (int)((_gameTime - hour) * 60f);
        textHour.SetText(hour.ToString("00"));
        textMinit.SetText(minute.ToString("00"));
    }
    public void RenewTimeState(ETimeState _state)
    {
        string text = (_state == ETimeState.Day) ? "낮" : "밤";
        textTimeState.SetText(text);
    } 
}  
