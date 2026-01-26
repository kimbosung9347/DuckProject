using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttachmentData", menuName = "Scriptable Objects/AttachmentData")]
public class AttachmentData : ItemData
{ 
    [Tooltip("부착 타입")]
    public EAttachmentType attachType;
     
    [Tooltip("제어 정보")]
    public ShotInfo shotInfo;

    protected override void AddStats(List<FStatPair> _list)
    {
        base.AddStats(_list);

        _list.Add(new("조준 속도", shotInfo.toAimSpeed.ToString()));
        _list.Add(new("명중률", shotInfo.accControl.ToString()));
        _list.Add(new("반동", shotInfo.recoilControl.ToString()));
        _list.Add(new("사거리", shotInfo.attackRange.ToString()));
    } 
} 

public enum EAttachmentType
{ 
    Muzzle, 
    Scope,  
    Stock,  
    End 
}
public enum EAttachSlotState
{
    Exist,
    Empty,
    Disable,
    End, 
} 