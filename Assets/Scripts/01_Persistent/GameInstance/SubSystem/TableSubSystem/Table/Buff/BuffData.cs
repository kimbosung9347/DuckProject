using UnityEngine;

public enum EBuffID
{
    // 버프 1~100;
    [InspectorName(null)]
    _BUFF_START = 0,
     
    Joy = 1, 
    VisionUp,
    Small,
     
    [InspectorName(null)]
    _BUFF_END = 99,

    // 디버프 101~200; 
    [InspectorName(null)]
    _DEBUFF_START = 100,
       
    Heavy = 101,
    OverWeight,
    Thirst,
    Hungry,
     
    [InspectorName(null)]
    _DEBUFF_END = 199,
    _END,
}

[CreateAssetMenu(fileName = "BuffData", menuName = "Scriptable Objects/BuffData")]
public class BuffData : ScriptableObject
{
    [Header("공통")]
    public EBuffID buffId;
    public string buffName;
    public Sprite buffSprite;
    public bool isUseDuration;
    public float duration;

    [Header("스텟버프")]
    public bool isModifyMoveStat;
    public bool isModifyShotStat;
     
    public LocoMoveInfo moveInfo;
    public ShotInfo shotInfo;
} 
 