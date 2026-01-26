using UnityEngine;

public class SoundSubSystem : GameInstanceSubSystem
{
    private SoundSystem soundSystem;
     
    public override void Init()
    {
        CacheSoundSystem();
    }
    public override void LevelStart(ELevelType _type)
    {
    }
    public override void LevelEnd(ELevelType _type)
    { 
    }

    private void CacheSoundSystem()
    {
        var systems = Object.FindObjectsByType<SoundSystem>(FindObjectsSortMode.None);
        if (systems.Length > 0)
            soundSystem = systems[0];
    } 
    public SoundSystem GetSoundSystem()
    {
        return soundSystem;
    }
}
