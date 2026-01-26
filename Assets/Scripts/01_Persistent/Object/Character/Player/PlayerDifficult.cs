using UnityEngine;
using static DifficultData;

public class PlayerDifficult : MonoBehaviour
{
    public EPlayDifficultFlag unlockedDifficulties;
    public EPlayDifficultType curDifficultType = EPlayDifficultType.Easy;

    private void Start()
    {
        var playData = GameInstance.Instance.SAVE_GetCurPlayData();
        SetCurDifficultType(playData.difficultData.CurrentDifficult);
        SetCurDifficultFlag(playData.difficultData.AvailableFlags);
    } 

    public void SetCurDifficultType(EPlayDifficultType _type)
    {
        curDifficultType = _type;
    }
    public void SetCurDifficultFlag(EPlayDifficultFlag _type)
    {
        unlockedDifficulties = _type;
    } 

    public EPlayDifficultType GetCurDifficult() { return curDifficultType; }
    public EPlayDifficultFlag GetCurPlayDifficultFlag() { return unlockedDifficulties; }
    public DifficultData CreateDifficultData()
    {
        var data = new DifficultData(); 
        data.SetAvailableFlags(unlockedDifficulties);
        data.SetCurrent(curDifficultType);
        return data;
    }

    // public void SetDifficult(EPlayDifficultType _diffcult) { curDifficult = _diffcult; }


    public float ConvertDamageByDifficult(float _damage)
    {
       return _damage *= DuckDefine.GetHitDamageByDifficult(curDifficultType);
    }
} 
