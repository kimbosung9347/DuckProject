using Unity.Cinemachine;
using UnityEngine;

public struct FFireInfo
{
    public Vector3 targetPoint;
    public int bulletCnt; 
    public bool isHead;  
}   
  
public class DuckAttack : MonoBehaviour
{
    // 임시
    [SerializeField] private GameObject myHeadCollider;
    [SerializeField] private GameObject myBodyCollider;
     
    protected BattleTable cachedBattleTable;
    protected DuckSpeechBubble cachedSpeech;
    protected DuckAiming cachedAiming;
    private DuckAnimation cachedAnimation;

    protected ShotInfo cachedShotInfo;
    protected Weapon weapon; 
        
    protected virtual void Awake()
    {
        cachedBattleTable = GameInstance.Instance.TABLE_GetBattleTable();
        cachedAiming = GetComponent<DuckAiming>();
        cachedSpeech = GetComponent<DuckSpeechBubble>();
        cachedAnimation = GetComponent<DuckAnimation>(); 
    }   
    private void Start() 
    {
    } 
    private void Update()
    {
         
    }

    public void CacheShotInfo(ShotInfo _shotInfo)
    {
        cachedShotInfo = _shotInfo;
    } 
    public Weapon GetWeapon() { return weapon; }
    public void SetWeapon(Weapon _weapon)
    { 
        weapon = _weapon;
    } 
      
    public void DoAttack()
    {
        FFireInfo info = GetShootInfo();
        for (int i= 0; i < info.bulletCnt; i++)
        {
            float acc = cachedShotInfo.accControl; 
            float size = cachedShotInfo.recoilControl;
            float attRange = cachedShotInfo.attackRange;
            Shoot(acc, size, attRange, info);
        }
          
        // 총알 줄이기
        weapon.ReduceBullet();
    }  
    public void DoAimAttack()
    { 
        FFireInfo info = GetShootInfo();
        for (int i = 0; i < info.bulletCnt; i++)
        {
            float acc = cachedShotInfo.accControl;
            float size = cachedShotInfo.recoilControl * 1.8f; // 조준중일때는 반동 크기를 줄여주자
            float attRange = cachedShotInfo.attackRange;
            Shoot(acc, size, attRange, info);
        }
         
        // 총알 줄이기
        weapon.ReduceBullet();
    } 
    public void ChangeSprint(bool _isActive)
    {
        var weapon = GetWeapon();
        if (!weapon)
            return;
          
        var attach = weapon.GetAttach(EAttachmentType.Scope);
        if (!attach)
            return; 
         
        attach.ActiveRaserBeam(!_isActive);
    }

    public bool CanAim()
    {
        if (!weapon)
            return false;
          
        // 샷건은 그냥 조준하지마라 예외처리 귀찮다
        if (weapon.GetWeaponData().weaponType == EWeaponType.Shotgun)
            return false;

        return true;
    }
    public virtual bool CanAttack()
    {
        if (!weapon)
            return false;
         
        if (!weapon.IsExistBullet())
        {  
            // DuckSpeech
            cachedSpeech.ActiveAutoDeleteSpeech("탄약이 없어");
            return false; 
        }
         
        // 쿨타임 
        if (!weapon.CanAttack())
            return false;
         
        return true;
    }  
     
    protected virtual void ActiveShotEffect(Vector3 dir, float _size)
    {
        GetWeapon()?.ActiveEffect(dir);
        cachedAnimation.ActiveShotAnim();
          
        // 반동에 따른 명중률 감소 
        float remain = cachedBattleTable.Calculate_CursorRecoilSize(_size);
        cachedAiming.DoRecoil(remain);  
    }    
    protected virtual FFireInfo GetShootInfo()
    {
        FFireInfo ShotInfo = new FFireInfo();
        ShotInfo.targetPoint = cachedAiming.GetTargetPos();
        ShotInfo.bulletCnt = weapon.GetWeaponData().bulletCnt;
        return ShotInfo; 
    } 
      
    private void Shoot(float acc, float size, float attRange, FFireInfo _fireInfo)
    {
        Vector3 startPos = weapon.GetMuzzleTransform().position;
        Vector3 dir = (_fireInfo.targetPoint - startPos).normalized;
        // 총알 발사 
        Vector3 randomizedDir = GetRandomizedDirection_TopView(dir, acc, attRange, 8f);
        FireGun(startPos, randomizedDir, acc, attRange);
         
        ActiveShotEffect(randomizedDir, size); 
    }  
    private void FireGun(Vector3 _startPos, Vector3 _dir, float _acc, float _attackRange)
    {
        var bulletObject = GameInstance.Instance.POOL_Spawn(EPoolId.Bullet, _startPos, Quaternion.LookRotation(_dir));
        var bullet = bulletObject.GetComponent<BulletProjectile>();
          
        bullet.AddExcludeObject(myBodyCollider);
        bullet.AddExcludeObject(myHeadCollider);
        bullet.Fire(_dir, weapon.GetDamage(), this);
    }

    private Vector3 GetRandomizedDirection_TopView(Vector3 baseDir, float accControl, float maxSpreadRadius, float maxAngleDeg)
    {
        Vector3 flatBase = baseDir;
        flatBase.Normalize();

        float angleRange = Mathf.Lerp(maxAngleDeg, 0f, accControl);
        if (angleRange <= 0.01f)
            return flatBase;

        float angle = Random.Range(-angleRange, angleRange) * Mathf.Deg2Rad;

        Quaternion rot = Quaternion.LookRotation(flatBase, Vector3.up);
        Vector3 dir = rot * new Vector3(
            Mathf.Sin(angle),
            0f,
            Mathf.Cos(angle)
        ); 

        return dir.normalized;
    }
}
