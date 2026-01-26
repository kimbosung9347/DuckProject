using UnityEngine;

public enum EMapType
{ 
    Home,
    Farm,
    Test,

    End,
}
 
public class MapSubSystem : GameInstanceSubSystem
{
    // 여기서 껍데기 정보를 가지고 있는게 맞겠다
    private HouseSystem houseSystem;
    private FarmSystem farmSystem; 
    private EMapType curMapType;

    public override void Init()
    {
    } 
    public override void LevelStart(ELevelType _type)
    {
        switch (_type)
        {
            case ELevelType.Home:
            {
                houseSystem = Object.FindAnyObjectByType<HouseSystem>();
                curMapType = EMapType.Home;
            }
            break; 
                 
            case ELevelType.Farm:
            { 
                farmSystem = Object.FindAnyObjectByType<FarmSystem>();
                curMapType = EMapType.Farm;

                // FarmData
                var playData = GameInstance.Instance.SAVE_GetCurPlayData();
                farmSystem.Init(playData.farmData);
            } 
            break;

            case ELevelType.Test:
            { 
                // testSystem = Object.FindAnyObjectByType<TestSystem>();
                farmSystem = Object.FindAnyObjectByType<FarmSystem>();
                curMapType = EMapType.Test; 
            }  
            break;
        } 
    }
    public override void LevelEnd(ELevelType _type)
    {
        switch (_type)
        {
            case ELevelType.Loading:
                {
                }
                break;
                 
            case ELevelType.Home: 
                {
                    houseSystem = null;
                    curMapType = EMapType.End;
                }
                break;
                 
            case ELevelType.Farm:
                {
                    farmSystem = null;
                    curMapType = EMapType.End;
                }
                break;

            case ELevelType.Test:
                { 
                    farmSystem = null;
                    curMapType = EMapType.End;
                }
                break;
        } 
    }
     
    public EMapType GetMapType() { return curMapType; } 
    public HouseSystem GetHouseSystem() { return houseSystem; }
    public FarmSystem GetFarmSystem() { return farmSystem; } 
    public DuckHouse GetHouse(int _index)
    {
        switch (curMapType)
        {
            case EMapType.Farm:
            {
                return farmSystem.GetHouse(_index);
            }
                 
            case EMapType.Test:
            {
                return farmSystem.GetHouse(_index);
            }
        }

        return null; 
    }
     
    public void ChangeDay(bool _isInstant)
    {
        switch (curMapType)
        {
            case EMapType.Farm:
            {
                farmSystem.ChangeDay(_isInstant);
            }
            break;
        }
    }
    public void ChangeNight(bool _isInstant)
    {
        switch (curMapType)
        {
            case EMapType.Farm:
            {
                farmSystem.ChangeNight(_isInstant);
            }
            break;
        } 
    } 
    public void ShotTargetPos(Vector3 _center)
    {
        switch (curMapType)
        {
            case EMapType.Farm:
            case EMapType.Test:
            {
                farmSystem?.ShotTargetPos(_center); 
            }
            break; 
        }
    }
    public void DropItem(ItemBase _item)
    {
        switch (curMapType)
        {
            case EMapType.Home:
            {
                _item.transform.SetParent(houseSystem.transform);
            }
            break;

            case EMapType.Farm:
            {
                _item.transform.SetParent(farmSystem.transform);
            }
            break;
                 
            default:
            {
                _item.transform.SetParent(null);
            }
            break;
        }
    }
}
   