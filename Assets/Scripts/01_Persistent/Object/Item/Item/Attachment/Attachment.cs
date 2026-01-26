using System.Collections.Generic;
using UnityEngine;

public class Attachment : ItemBase
{
    // 총기 정보   
    private AttachmentData attachData;
    private GameObject raserBeam;

    private void OnDestroy()
    {
        if (raserBeam)
        {
            Destroy(raserBeam);
            raserBeam = null;
        }
    }
    public override void Init(ItemData _data, ItemVisualData _visual)
    {
        base.Init(_data, _visual);

        InitAttachData(_data);
         
        // 레이저일때, 이렇게 하자 계획상으로 하나밖에 없을 예정이라 간단하게 처리
        if (_data.itemID == EItemID.SCOPE_C)
        {
            raserBeam = GameInstance.Instance.SPAWN_RaserBeam();
            raserBeam.GetComponent<LaserBeam>().Disable(transform);
        } 
    }
    
    public EAttachmentType GetAttachType()
    {
        return attachData.attachType;
    }
    public AttachmentData GetAttachData()
    {
        return attachData;
    } 


    // 구지긴한데 귀찮음
    public bool HasRaser()
    { 
        return raserBeam;
    }
    public void ActiveRaserBeam(bool _isActive, Transform _muzzleTransform, float _distance, DuckAiming _duckAiming)
    {
        if (!raserBeam)
            return;  

        if (_isActive) 
        { 
            raserBeam.GetComponent<LaserBeam>().Active(_muzzleTransform, _distance, _duckAiming);
        } 
          
        else 
        {
            raserBeam.GetComponent<LaserBeam>().Disable(transform);
        }
    }
    public void ActiveRaserBeam(bool _isActive)
    {
        if (!raserBeam)
            return;
         
        raserBeam.GetComponent<LaserBeam>().ActiveRaiser(_isActive);
    }

    private void InitAttachData(ItemData _data)
    {
        attachData = _data as AttachmentData;
    } 
} 
