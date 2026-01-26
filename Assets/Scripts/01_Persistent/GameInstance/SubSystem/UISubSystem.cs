using System.Collections.Generic;
using UnityEngine;
  
public class UISubSystem : GameInstanceSubSystem
{
    // 영구씬에 존재하는 UIGroup
    private PersistentUIGroup cachedPerisitentUIGroup;
        
    public override void Init()
    {
        CacheCanvasesInPersistent();
    }  
    public override void LevelStart(ELevelType _type)
    {
        cachedPerisitentUIGroup.GetInteractionBillboard()?.Dsiable();
         
        switch (_type)
        { 
            case ELevelType.Loading:
            {
            }
            break;
                  
            case ELevelType.Mainmenu:
            {
            } 
            break; 
                 
            case ELevelType.Home:
            { 
            }
            break;

            case ELevelType.Farm:
            {
            }
            break;

        }
    }
    public override void LevelEnd(ELevelType _type)
    {
    }  
      
    /* API */
    public PersistentUIGroup GetPeristentUIGroup()
    {
        return cachedPerisitentUIGroup;
    } 
    private void CacheCanvasesInPersistent()
    {
        cachedPerisitentUIGroup = UnityEngine.Object.FindFirstObjectByType<PersistentUIGroup>();
    }
}  
 