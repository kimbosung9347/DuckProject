using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem;

public class PlayerSubSystem : GameInstanceSubSystem
{ 
    private PlayerSystem cachedPlayerSystem;
      
    public override void Init()
    {
        CachePlayerSystem();
    }  
    public override void LevelStart(ELevelType _type)
    {
        switch (_type)
        {
            case ELevelType.Mainmenu:
            {
            }
            break; 

            case ELevelType.Loading:
            {
                GetPlayerSystem().GetPlayerObj()?.SetActive(true);
            }
            break; 
                 
            case ELevelType.Home:
            {
                GetPlayerSystem().GetPlayerObj().SetActive(true);
            } 
            break;
                 
            case ELevelType.Farm:
            {
                GetPlayerSystem().GetPlayerObj().SetActive(true);
            }
            break;
                 
            case ELevelType.Test:
            {
            }
            break;
        }
    }
    public override void LevelEnd(ELevelType _type)
    {

    }
     
    /* API */
    public PlayerSystem GetPlayerSystem() { return cachedPlayerSystem; }
    private void CachePlayerSystem()
    {
        var arr = Object.FindObjectsByType<PlayerSystem>(FindObjectsSortMode.None);
        if (arr.Length > 0)
            cachedPlayerSystem = arr[0];
    }
}
