using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
 
public struct FStatPair
{
    public string key;
    public string value;

    public FStatPair(string _key, string _value)
    {
        key = _key;
        value = _value;
    }
} 

[System.Serializable]
public struct FWeightedItemID
{
    public EItemID itemId;
    [Min(0f)] public float weight;
}
  