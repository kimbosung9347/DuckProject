using System.Collections.Generic;
using Unity.AppUI.Core;
using UnityEngine;

public class Weapon : EquipBase
{
    // 총기 정보  
    private WeaponData weaponData;
    private Transform cachedMuzzleTrans; 

    // 현재 총알정보
    private EItemID curBulletId = EItemID._END;
    private float curBuleltDamage = 0f;
    private int curBullet = 0;
     
    // 장전 중인지
    private EItemID reserveBulletId = EItemID._END;
    private bool isReloading = false;
    private float elaspeReloadTime = 0f;
    private float reloadRatio = 0f;

    //
    private AudioSource reloadSrc;
    private SoundSfx reloadSfx;

    // 공격가능한지 
    private float nextAttackTick = 0f;
     
    // 보정된 샷인포
    private ShotInfo corrShotInfo = new();

    // 장착된 부착물
    private Dictionary<EAttachmentType, Attachment> hashAttachment = new();

    private void Awake()
    { 
        // Sfx 조회
        reloadSfx = GameInstance.Instance.SOUND_GetSfx(ESoundSfxType.Reload);

        // AudioSource 생성
        reloadSrc = gameObject.AddComponent<AudioSource>();

        // 기본 세팅 적용
        reloadSfx.ApplyTo(reloadSrc);
        reloadSrc.playOnAwake = false;
        reloadSrc.loop = false;
    } 
    private void Update()
    {
        UpdateReload();
    } 

    public override void Init(ItemData _data, ItemVisualData _visual)
    {
        base.Init(_data, _visual);
        InitWeaponData(_data);
        InitWeaponVisualData(_visual);
    }
    public override List<FStatPair> GetStats()
    {
        // 부착물의 값을 보정을 해준값을 리턴 
        return new List<FStatPair>
        {
            new ("가격", weaponData.price.ToString()),
            new ("현재 총알", GetBulletStatInfo(curBulletId)),
            new ("공격력", weaponData.damage.ToString()),
            new ("연사 속도", $"{weaponData.attackSpeed * 10f:0.#} rps"), // 0.1 → 초당
            new ("탄창 용량", weaponData.magazine.ToString()),
            new ("조준 속도", (corrShotInfo.toAimSpeed + weaponData.shotInfo.toAimSpeed).ToString()),
            new ("명중률", (corrShotInfo.accControl + weaponData.shotInfo.accControl).ToString()),
            new ("반동",  (corrShotInfo.recoilControl + weaponData.shotInfo.recoilControl).ToString()),
            new ("사거리", (corrShotInfo.attackRange + weaponData.shotInfo.attackRange).ToString())
        }; 
    }
    public override void RemoveFromInven()
    {
        base.RemoveFromInven();
         
        cachedDuckStorage = null;
        cachedDuckEquip = null; 
        cachedItemBoxBase = null; 
    }
    public override void Attach()
    {
        base.Attach();

        // 장착중이라면 - 레이저 빔을 활성화 
        var attach = GetAttach(EAttachmentType.Scope);
        if (attach) 
        {
            attach.ActiveRaserBeam(true, GetMuzzleTransform(), GetAttackRange(), foundDuckAiming());
        }
    }
    public override void Detach() 
    {
        base.Detach();
         
        // 레이저 빔을 비활성화 
        var attach = GetAttach(EAttachmentType.Scope);
        if (attach) 
        { 
            attach.ActiveRaserBeam(false, null, GetAttackRange(), null);
        }
    }
    public override FItemShell GetItemShell()
    { 
        FItemShell entry = base.GetItemShell();

        entry.muzzleState = GetAttachSlotState(EAttachmentType.Muzzle);
        entry.scopeState = GetAttachSlotState(EAttachmentType.Scope);
        entry.stockState = GetAttachSlotState(EAttachmentType.Stock);

        var muzzle = GetAttach(EAttachmentType.Muzzle);
        var scope = GetAttach(EAttachmentType.Scope);
        var stock = GetAttach(EAttachmentType.Stock);

        entry.muzzleAttach = muzzle ? muzzle.GetItemData().itemID : EItemID._END;
        entry.scopeAttach = scope ? scope.GetItemData().itemID : EItemID._END;
        entry.stockeAttach = stock ? stock.GetItemData().itemID : EItemID._END;
         
        entry.bulletId = curBulletId;
        entry.bulletCnt = curBullet;
          
        return entry;
    }
     
    public WeaponData GetWeaponData() { return  weaponData; }
    public Transform GetMuzzleTransform() { return cachedMuzzleTrans; }  
    public EItemID GetCurBulletId() { return curBulletId; }
    public ShotInfo GetShotInfo() { return weaponData.shotInfo + corrShotInfo; }
        
    public void InsertAttachment(Attachment _attach)
    {
        if (!_attach)
            return;

        EAttachmentType type = _attach.GetAttachData().attachType;

        // 이미 있으면 넣지 않음
        // 제거후 Insert라서 무조건 없어야 정상임 
        if (hashAttachment.ContainsKey(type))
            return;
          
        hashAttachment[type] = _attach;

        // 능력치 보정
        RenewCorrShotInfo(_attach.GetAttachData().shotInfo);

        // 레이저 빔 체킹 및 활성화 
        if (cachedDuckEquip)
        {
            var attach = GetAttach(EAttachmentType.Scope);
            if (attach)
            {
                attach.ActiveRaserBeam(true, GetMuzzleTransform(), GetAttackRange(), foundDuckAiming());
            } 
        }
         
        _attach.Insert(transform);
         
        // 유아이반영 시켜주기 
        RenewAttachUI();
    }
    public Attachment RemoveAttach(EAttachmentType _type)
    {
        if (!hashAttachment.TryGetValue(_type, out var attachment))
            return null;

        hashAttachment.Remove(_type);

        // 해당 아이템이 빠짐으로 갱신된 능력치를 다시 캐릭터에 반영해줘야함 
        RenewCorrShotInfo(-attachment.GetAttachData().shotInfo);

        // 레이저 있다면 비활성화 
        attachment.ActiveRaserBeam(false, null, 0, null);

        // 제거 이후 유아이 반영 
        RenewAttachUI();
        // RenewScopeUI();

        return attachment;
    }

    public void RenewAttachUI()
    {  
        // 내 게임에서
        // Attachment를 갱신할 때, IntemInfoCanvas가 활성화 되어 있는 상태이게 때문에 ItemInfo도 갱신 해줘야함
        if (cachedDuckEquip)
        {
            if (cachedDuckEquip is PlayerEquip playerEquip)
            {
                playerEquip.RenewAttachState(this);
                  
                var attach = GetAttach(EAttachmentType.Scope);
                playerEquip.RenewCursorScopeUI((attach == null) ? EItemID._END : attach.GetItemData().itemID);
            }
        } 
     
        else if (cachedDuckStorage)
        {
            if (cachedDuckStorage is PlayerStorage playerStorage)
            {
                playerStorage.RenewAttachState(this);
            }  
        } 
    
        else if (cachedItemBoxBase && cachedItemBoxBase.IsActiveInCanvas())
        { 
            cachedItemBoxBase.RenewAttachState(this);
        } 
    }  

    public bool IsReloading() { return isReloading; }
    public int GetCurBullet() { return curBullet; }
    public float GetReloadRatio() { return reloadRatio; }
    public float GetDamage()
    {
        float baseDamage = weaponData.damage * curBuleltDamage;
        float corrRatio = GetCurDurabilityRatio(); // 0 ~ 1

        // 75% 이하면 그대로
        if (corrRatio >= 0.75f)
            return baseDamage;
         
        float t = corrRatio / 0.75f;            
        float damageRatio = Mathf.Lerp(0.3f, 1f, t);
        return baseDamage * damageRatio;
    } 
   
    public EAttachSlotState GetAttachSlotState(EAttachmentType _type)
    {
        // Attach가 들어갈 수 있고
        if (weaponData.allowedAttachments.Contains(_type))
        {
            // 실제로 들어 있다면 
            if (hashAttachment.TryGetValue(_type, out var attachment))
            {
                return EAttachSlotState.Exist;
            }

            // 없다면
            else
            {
                return EAttachSlotState.Empty;
            }
        }

        return EAttachSlotState.Disable;
    } 
    public Attachment GetAttach(EAttachmentType _type)
    {
        if (hashAttachment.TryGetValue(_type, out var attachment))
        {
            return attachment;
        }

        return null;
    }
    public Bullet RemoveCurBullet()
    {
        if (curBulletId == EItemID._END)
            return null;

        var item = GameInstance.Instance.SPAWN_MakeItem(curBulletId);
        if (item is not Bullet bullet)
            return null;

        bullet.SetCurCnt(GetCurBullet());
         
        curBulletId = EItemID._END;
        SetCurBullet(0);

        cachedDuckEquip?.RenewCurBullet();

        return bullet;
    }

    public void SetCurBulletId(EItemID _id) 
    { 
        curBulletId = _id;
        curBuleltDamage = DuckUtill.GetBulletDamage(_id);
    } 
    public void SetCurBullet(int _cnt) { curBullet = _cnt; }
    public void FillBullet() 
    { 
        curBullet = weaponData.magazine;
    }
    public void ActiveEffect(Vector3 _dir)
    {
        var gameInstance = GameInstance.Instance;

        Transform muzzle = GetMuzzleTransform();
        if (!muzzle)
            return;

        // 이펙트 출력
        {
            // Effect출력
            float yOffsetDeg = -90f;
            Quaternion rot =
                Quaternion.LookRotation(_dir) *
                Quaternion.Euler(0f, yOffsetDeg, 0f);
             
            gameInstance.POOL_Spawn(
                EPoolId.MuzzleFlash,
                muzzle.position,
                rot
            );
        }
           
        // 소리 출력
        {
            gameInstance.SOUND_PlaySoundSfx(ESoundSfxType.Shot, muzzle.transform.position);
            switch (weaponData.weaponType)
            {
                case EWeaponType.Pistol:
                {

                }
                break;

                case EWeaponType.Rifle:
                {

                }
                break;

                case EWeaponType.Shotgun:
                {

                }
                break;

                case EWeaponType.Snife:
                {

                }
                break;
            }
        }
    }  

    public bool CanReload(EItemID _bulletId)
    {
        // 꽉찬 상태 - 같은 총알인데 총알이 꽉찼다면 
        if ((_bulletId == curBulletId) && (curBullet == weaponData.magazine))
            return false;
         
        return true;
    }
    public bool CanReload()
    {
        if ((curBullet == weaponData.magazine))
            return false;
         
        return true;
    }
    public bool CanAttack()
    {
        if (isReloading)
            return false;

        // 아직 쿨 안 돌았음 → 공격 불가
        if (Time.time < nextAttackTick)
            return false; 

        // 공격 성공 → 다음 쿨 세팅
        nextAttackTick = Time.time + weaponData.attackSpeed;

        return true;
    } 
    public bool IsExistBullet()
    {
        if (curBullet == 0)
            return false; 

        return true;
    }

    public void TryReload(EItemID _bulletId)
    {
        reserveBulletId = _bulletId;
        isReloading = true;
        elaspeReloadTime = 0f;
        reloadRatio = 0f;

        ActiveReloadEffect();
    }  
    public void TryReload()
    {
        reserveBulletId = curBulletId;
        isReloading = true;
        elaspeReloadTime = 0f;
        reloadRatio = 0f;

        ActiveReloadEffect(); 
    } 
    public void CancleReload() 
    {
        reserveBulletId = EItemID._END;
        isReloading = false;
        elaspeReloadTime = 0f;
        reloadRatio = 0f;
         
        if (reloadSrc.isPlaying)
            reloadSrc.Stop();
    } 
    public void ReduceBullet()
    {
        curBullet--;
        ReduceDurability();

        cachedDuckEquip?.RenewCurBullet(); 
        cachedDuckEquip?.RenewCurDurability(EEquipSlot.Weapon);
    }   
    private void RenewCorrShotInfo(ShotInfo _shotInfo)
    {
        // 이전 보정값
        ShotInfo prevShotInfo = corrShotInfo.Clone();
           
        // 현재 보정값
        corrShotInfo += _shotInfo; 
          
        // 능력치 반영 
        cachedDuckEquip?.RenewShotInfo(corrShotInfo - prevShotInfo);
    }  
    private void InitWeaponData(ItemData _data)
    {
        weaponData = _data as WeaponData;
        curBullet = 0;
    }  
    private void InitWeaponVisualData(ItemVisualData _data)
    {
        var weaponMesh = meshPrefab.GetComponentInChildren<WeaponMesh>();
        cachedMuzzleTrans = weaponMesh.GetMuzzleTransform();
    }
    private float GetAttackRange()
    {
        return weaponData.shotInfo.attackRange + corrShotInfo.attackRange;
    }

    private void UpdateReload()
    {
        if (!isReloading) 
            return;
          
        elaspeReloadTime += Time.deltaTime;
        
        reloadRatio = (elaspeReloadTime / weaponData.reloadTime);
        if (reloadRatio > 1) 
            reloadRatio = 1;
          
        if (elaspeReloadTime >= weaponData.reloadTime)
        { 
            // 다른 총알로 장전 하는 경우 기 존 총알 롤백
            if ((curBulletId != EItemID._END) && (curBulletId != reserveBulletId))
            {
                cachedDuckEquip?.RollbackBullet(curBulletId, curBullet);
                curBullet = 0;
            } 
             
            int refillCnt = weaponData.magazine - curBullet;

            // 이렇게 한 이유는 장전에 대한 성공여부를 UI로 제공해주고싶어서
            // 미리 isReloading이 false여야만 State의 Clear에서 Cancle할때 성공했다라는 것을
            // 체킹 해줄 수 있음.. 짜치지만 괜찮은듯
            isReloading = false;
             
            cachedDuckEquip?.RefillBullet(reserveBulletId, refillCnt); 
              
            // 장전성공
            CancleReload();  
        }    
    } 
    private string GetBulletStatInfo(EItemID _itemId)
    {
        if (_itemId == EItemID._END)
            return "총알 없음";
         
        string bulletName = DuckUtill.GetItemName(_itemId);
        return $"{bulletName}-{curBullet}개";
    }

    private DuckAiming foundDuckAiming()
    {
        DuckAiming duckAiming = null;
        duckAiming = cachedDuckEquip?.gameObject.GetComponent<DuckAiming>();
        if (!duckAiming)
            duckAiming = cachedDuckStorage?.gameObject.GetComponent<DuckAiming>();
          
        return duckAiming;
    }

    private void ActiveReloadEffect()
    {
        if (reloadSrc.isPlaying)
            reloadSrc.Stop();
         
        reloadSrc.pitch = DuckUtill.CalcTimeFitPitch(
            reloadSfx.clip.length,
            weaponData.reloadTime
        );
          
        reloadSrc.Play();
    }
}     
  