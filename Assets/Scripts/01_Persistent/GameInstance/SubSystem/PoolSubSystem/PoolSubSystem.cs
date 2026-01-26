using UnityEngine;

// 이펙트 같은것들 풀링해서 쓸꺼임
public class PoolSubSystem : GameInstanceSubSystem
{
    PlayPoolSystem playPoolSystem; 
     
    public override void Init()
    {
        playPoolSystem = Object.FindAnyObjectByType<PlayPoolSystem>();
    }
    public override void LevelStart(ELevelType _type)
    {
        switch (_type)
        {
            case ELevelType.Mainmenu:
            {
                // 모든 풀링 비워줄꺼임
                // PlayPoolSystem->풀링비워주기
                playPoolSystem.ClearAllPools();
            } 
            break;

            case ELevelType.Test:
            case ELevelType.Home:
            {
                // 집에들어가면 풀링해줄꺼임
                // PlayPoolSystem-> 풀링시작
                playPoolSystem.PreloadPools();
            } 
            break;
        }
    }
    public override void LevelEnd(ELevelType _type)
    {
        switch (_type)
        {
            case ELevelType.Mainmenu:
            {
                // 모든 풀링 
            }
            break;
        }
    }

    public PlayPoolSystem GetPlayPoolSystem() { return playPoolSystem; }
     
}
