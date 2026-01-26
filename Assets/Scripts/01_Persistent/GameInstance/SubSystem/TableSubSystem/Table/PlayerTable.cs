using System.Collections.Generic;
using UnityEngine;

public class PlayerTable : MonoBehaviour
{
    private Dictionary<int, int> hashLevelRequireExp = new();

    private void Awake()
    {
        MakePlayerTable();
    }
     
    public int GetRequireExp(int level)
    {
        if (hashLevelRequireExp.TryGetValue(level, out var exp))
            return exp;

        return int.MaxValue;
    } 
    private void MakePlayerTable()
    {
        hashLevelRequireExp.Clear();
         
        for (int level = 1; level <= DuckDefine.PLAYER_MAX_LEVEL; level++)
        {
            hashLevelRequireExp[level] = 100 + level * level * 20;
        }
    }

}
