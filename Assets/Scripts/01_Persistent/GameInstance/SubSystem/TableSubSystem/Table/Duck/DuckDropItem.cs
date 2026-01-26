using UnityEngine;
using System.Collections.Generic;

    
[CreateAssetMenu(fileName = "DuckDropItem", menuName = "Scriptable Objects/DuckDropItem")]
public class DuckDropItem : ScriptableObject
{
    public EDuckType duckType = EDuckType.End;
     
    public List<FWeightedItemID> list = new();
}
 