using System.Collections.Generic;
using UnityEngine;

public class BuffTable : MonoBehaviour
{
    // 여기서 버프 테이블에 대한 정보를 관리해줘야함
    private Dictionary<EBuffID, BuffData> hashBuffTable = new ();
    private void Awake() 
    {
        var datas = Resources.LoadAll<BuffData>("Data/Buff");
        foreach (var data in datas)
            hashBuffTable[data.buffId] = data;
    }

    public BuffData GetBuffData(EBuffID _buffId)
    {
        return hashBuffTable[_buffId];
    }
      
}
 