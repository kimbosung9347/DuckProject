using UnityEngine;
 
[CreateAssetMenu(fileName = "ItemVisual", menuName = "Scriptable Objects/ItemVisualData")]
public class ItemVisualData : ScriptableObject 
{
    [Tooltip("고유 아이디")]
    public EItemID itemID;
     
    [Tooltip("아이콘")] 
    public Sprite iconSprite;  
}  
 