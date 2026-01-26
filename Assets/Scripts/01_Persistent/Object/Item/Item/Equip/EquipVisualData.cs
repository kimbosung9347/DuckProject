using UnityEngine;
 
[CreateAssetMenu(fileName = "equipVisual", menuName = "Scriptable Objects/EquipVisualData")]
public class EquipVisualData : ItemVisualData 
{ 
    [Tooltip("메쉬 프리팹")]
    public GameObject equipMeshPrefab;
}    