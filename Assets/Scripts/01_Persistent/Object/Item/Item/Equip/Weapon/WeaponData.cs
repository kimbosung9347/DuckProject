using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
/* 무기 */
public class WeaponData : EquipData
{
    [Tooltip("무기 타입")]
    public EWeaponType weaponType;
     
    [Tooltip("총알 개수")] 
    public int bulletCnt = 1;
      
    [Tooltip("총알 한발 당 공격력")]
    public int damage;
     
    [Tooltip("최대 탄창")]
    public int magazine;
      
    [Tooltip("공격 속도 : 초")]
    public float attackSpeed;
      
    [Tooltip("장전 속도 : 초")] 
    public float reloadTime;

    [Tooltip("제어 정보")]
    public ShotInfo shotInfo;
  
    [Header("부착 가능 부착물 타입")]
    [SerializeField] public List<EAttachmentType> allowedAttachments = new();
      
    protected override void AddStats(List<FStatPair> _list)
    {
        base.AddStats(_list);

        _list.Add(new("공격력", (damage * bulletCnt).ToString())); 
        _list.Add(new("발사 개수", bulletCnt.ToString()));
        _list.Add(new("연사 속도", $"{attackSpeed * 10f:0.#} rps"));
        _list.Add(new("탄창 용량", magazine.ToString()));
        _list.Add(new("조준 속도", shotInfo.toAimSpeed.ToString()));
        _list.Add(new("명중률", shotInfo.accControl.ToString()));
        _list.Add(new("반동", (shotInfo.recoilControl * bulletCnt).ToString()));
        _list.Add(new("사거리", shotInfo.attackRange.ToString()));
    }
}  
 
// 무기 세부 타입
public enum EWeaponType
{
    None,
    Pistol,
    Rifle,
    Shotgun,
    Snife, 
}
