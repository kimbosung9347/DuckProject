using UnityEngine;

public class HouseSystem : MonoBehaviour
{
    [SerializeField] private BoxCollider mapBoundary;
    [SerializeField] private Sprite minimapSprite;

    [SerializeField] private Transform playerRespawnTP;
    [SerializeField] private AdornDuck adornDuck;

    private void Start()
    {
        var instance = GameInstance.Instance;
        var uiGroup = instance.UI_GetPersistentUIGroup();
        uiGroup.GetDisableCanvas().DisableInstant(); 
        uiGroup.GetMinimapCanvas().CacheMinimapInfo(mapBoundary, minimapSprite, "집");
        instance.PLAYER_GetPlayerRespawn().SpawnInHouse(playerRespawnTP);
    } 
      
    private void OnDestroy()
    {
        var instance = GameInstance.Instance;
        var uiGroup = instance.UI_GetPersistentUIGroup();
        uiGroup.GetMinimapCanvas().DisableMinimapInfo();
    }   

    public BoxCollider GetMapBoundary() { return mapBoundary; } 
    public Transform GetPlayerRespawnTP() {  return playerRespawnTP; }
    public AdornDuck GetAdornDuck() {  return adornDuck; }
} 
   