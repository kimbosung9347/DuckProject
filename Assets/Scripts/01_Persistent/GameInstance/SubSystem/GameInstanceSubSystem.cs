using UnityEngine;
 
public abstract class GameInstanceSubSystem
{ 
    public virtual void Init()
    {
    } 
    public virtual void LevelStart(ELevelType _type)
    {
    }
    public virtual void LevelEnd(ELevelType _type)
    {
    }
}
