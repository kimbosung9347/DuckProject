using System.Collections.Generic;
using Unity.AppUI.Redux;
using UnityEngine;
using UnityEngine.SceneManagement;
  
[DefaultExecutionOrder(-1000)]
public class GameInstance : MonoBehaviour
{
    public static GameInstance Instance { get; private set; }

    List<GameInstanceSubSystem> arrSubSystem;
    SaveSubSystem save;
    SettingSubSystem setting;
    LoadingSubSystem loading;
    TableSubSystem table;
    SpawnSubSystem spawn; 
    UISubSystem ui;
    CameraSubSystem cam; 
    PlayerSubSystem player;
    SoundSubSystem sound;
    SlotSubSystem slot;
    MapSubSystem map;
    PoolSubSystem pool; 
     
    void Awake() 
    {
        if (Instance)
            return;
          
        Instance = this;
        arrSubSystem = new List<GameInstanceSubSystem>();
        table = new TableSubSystem();
        save = new SaveSubSystem();
        setting = new SettingSubSystem();
		loading = new LoadingSubSystem();
        spawn = new SpawnSubSystem();
        ui = new UISubSystem();
        cam = new CameraSubSystem();
        player = new PlayerSubSystem();
        sound = new SoundSubSystem();  
        slot = new SlotSubSystem();
        map = new MapSubSystem();
        pool = new PoolSubSystem();
          
        // 구조 상에서 순서 의존성이 생김, Awake시점에서 해당 순서를 지켜줘야함
        // #. Table > Save > ...  > Spawn >  map > player
        arrSubSystem.Add(table);
        arrSubSystem.Add(save); 
        arrSubSystem.Add(setting); 
        arrSubSystem.Add(loading);
        arrSubSystem.Add(spawn); 
        arrSubSystem.Add(ui);  
        arrSubSystem.Add(cam);
        arrSubSystem.Add(map);
        arrSubSystem.Add(player);
        arrSubSystem.Add(sound);
        arrSubSystem.Add(slot);
        arrSubSystem.Add(pool); 
         
        foreach (var subSystem in arrSubSystem) 
        {
            subSystem.Init();
        } 

        /////////////
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
 
        Cursor.visible = false; 
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ELevelType levelType = (ELevelType)(scene.buildIndex);
        foreach (var subSystem in arrSubSystem)
        {
            subSystem.LevelStart(levelType);
        } 
    }
    private void OnSceneUnloaded(Scene scene)
    {
        ELevelType levelType = (ELevelType)(scene.buildIndex);
        foreach (var subSystem in arrSubSystem)
        {
            subSystem.LevelEnd(levelType);
        }
    }

        
    /* ========== 
        TABLE
    ========== */
    public BattleTable TABLE_GetBattleTable()
    {
        return table.GetBattleTable();
    }
    public ItemTable TABLE_GetItemTable()
    {
        return table.GetItemTable();
    }
    public DuckTable TABLE_GetDuckTable()
    {
        return table.GetDuckTable();
    } 
    public DuckDefaultInfo TABLE_GetDuckDefaultInfo(EDuckType _type)
    {
        return table.GetDuckTable().GetDuckDefaultInfo(_type);
    } 
    public DuckDropItem TABLE_GetDuckDropItem(EDuckType _type)
    {
        return table.GetDuckTable().GetDuckDropItem(_type);
    }  
    public EDuckRelation TABLE_GetDuckRelation(EDuckType _a, EDuckType _b)
    {
        return table.GetDuckTable().GetDuckRelation(_a, _b);
    } 


    public BuffData TABLE_GetBuffData(EBuffID _id)
    { 
        return table.GetBuffTable().GetBuffData(_id); 
    } 
    public QuestData TABLE_GetQuestData(EQuestID _id)
    {
        return table.GetQuestTable().GetQuestData(_id);
    }
    public QuestUnlockData TABLE_GetQuestUnlockData(EQuestID _id)
    { 
        return table.GetQuestTable().GetQuestUnlockData(_id);
    }
     
    public int TABLE_GetRequireExp(int _lv)
    {
        return table.GetPlayerTable().GetRequireExp(_lv);
    }
    public string TABLE_GetDuckName(EDuckType _type)
    { 
        return table.GetDuckTable().GetDuckName(_type);
    }
    public string TABLE_GetAdornName(EAdornBodyType type) 
    {
        return table.GetDuckTable().GetAdornName(type);
    }
    public string TABLE_GetAdornName(EAdornEyesType type)
    {
        return table.GetDuckTable().GetAdornName(type);
    }
    public string TABLE_GetAdornName(EAdornBeekType type)
    {
        return table.GetDuckTable().GetAdornName(type);
    }
    public string TABLE_GetAdornName(EAdornEyesBowType type)
    {
        return table.GetDuckTable().GetAdornName(type);
    }
    public string TABLE_GetAdornName(EAdornHairType type)
    {
        return table.GetDuckTable().GetAdornName(type);
    }
    public string TABLE_GetAdornName(EAdornFootType type)
    {
        return table.GetDuckTable().GetAdornName(type);
    }
    public string TABLE_GetAdornName(EAdornHandType type)
    {
        return table.GetDuckTable().GetAdornName(type);
    }

    /* ========== 
        UI
     ========== */
    public PersistentUIGroup UI_GetPersistentUIGroup()
    {
        return ui.GetPeristentUIGroup();
    }

    public void UI_ActiveSettingCanvas()
    { 
        ui.GetPeristentUIGroup().GetSettingCanvas().gameObject.SetActive(true);
    }
    public void UI_ActiveRadiusCollapseCanvas(FRadiuseCollaspeInfo info)
    {
        ui.GetPeristentUIGroup().GetRadiusCollaspeCanvas().Active(info);
    }  
      
     
    /* ==========  
        SAVE
   ========== */ 
    public void SAVE_DelateSaveSlot(int _index)
    {
        save.DelateSaveSlot(_index); 
    }
    public PlayData SAVE_GetCurPlayData()
    { 
        return save.GetCurPlayData(); 
    }
    public PlayData SAVE_GetPlatData(int _index)
    {
        return save.GetPlayData(_index);
    } 
    public SettingData SAVE_GetSettingData()
    {
        return save.GetSettingData();
    }
    public LobbyData SAVE_GetLobbyData()
    {
        return save.GetLobbyData();
    } 
     
    public void SAVE_PlayData(int _index, PlayData _data)
    {
        save.SavePlayData(_index, _data);
    } 
    public void SAVE_SettingData(SettingData _data)
    {
        save.SaveSettingData(_data);
    }
       
    /* ==========  
        Player
   ========== */  
  
    public GameObject MakePlayer()
    { 
        return player.GetPlayerSystem().MakePlayer();
    }
    public void DelatePlayer()
    {
        player.GetPlayerSystem().DelatePlayer();
    } 

    public Transform PLAYER_GetPlayerTransform()
    {
        return player.GetPlayerSystem().GetPlayerTransform();
    }
    public PlayerInteraction PLAYER_GetPlayerInteraction()
    {
        return player.GetPlayerSystem().GetPlayerInteraction();  
    }
    public PlayerStorage PLAYER_GetPlayerStorage()
    {
        return player.GetPlayerSystem().GetPlayerStorage();
    }
    public PlayerEquip PLAYER_GetPlayerEquip()
    {
        return player.GetPlayerSystem().GetPlayerEquip();
    } 
    public PlayerQuickSlot PLAYER_GetPlayerQuickSlot()
    {
        return player.GetPlayerSystem().GetPlayerQuickSlot();
    } 
    public PlayerController PLAYER_GetPlayerController()
    {
        return player.GetPlayerSystem().GetPlayerController();
    }
    public PlayerTime PLAYER_GetPlayerTime() 
    {
        return player.GetPlayerSystem().GetPlayerObj().GetComponent<PlayerTime>();
    }
    public PlayerQuest PLAYER_GetPlayerQuest()
    { 
        return player.GetPlayerSystem().GetPlayerObj().GetComponent<PlayerQuest>();
    } 
    public PlayerStat PLAYER_GetPlayerStat()
    {
        return player.GetPlayerSystem().GetPlayerObj().GetComponent<PlayerStat>();
    }
    public PlayerMeshSetter PLAYER_GetPlayerColorSetter()
    {
        return player.GetPlayerSystem().GetPlayerObj().GetComponentInChildren<PlayerMeshSetter>();
    }  
    public PlayerSpawn PLAYER_GetPlayerRespawn()
    {
        return player.GetPlayerSystem().GetPlayerObj().GetComponentInChildren<PlayerSpawn>();
    } 
    public PlayerAchievement PLAYER_GetAchievment()
    {
        return player.GetPlayerSystem().GetPlayerObj().GetComponent<PlayerAchievement>();
    }
    public PlayerGrow PLAYER_GetPlayerGrow()
    {
        return player.GetPlayerSystem().GetPlayerObj().GetComponent<PlayerGrow>();
    }
    public PlayerDifficult PLAYER_GetPlayerDifficult()
    {
        return player.GetPlayerSystem().GetPlayerObj().GetComponent<PlayerDifficult>();
    }
    public PlayerSave PLAYER_GetPlayerSave()
    {
        return player.GetPlayerSystem().GetPlayerObj().GetComponent<PlayerSave>();
    } 
    public WareHouse PLAYER_GetWareHouse()
    {
        return player.GetPlayerSystem().GetWareHouse();
    } 

     
    /* ============= 
        Setting
   ============= */
    public void SETTING_ChangeResolution(EResolutionType _type)
    {
        setting.ChangeResolution(_type);
    }
    public void SETTING_ChangeScreenMode(EScreenModeType _type)
    {
        setting.ChangeScreenMode(_type);
    }
    public void SETTING_ChangeLanguage(ELanguageType _type)
    {
        setting.ChangeLanguage(_type);
    }
    public void SETTING_ChangeVolume(float _volume)
    {
        setting.ChangeVolume(_volume);
    } 
    public void SETTING_SaveCurSetting()
    {
        setting.SaveCurSetting();
    }
    public void SETTING_CancleCurSetting()
    {
        setting.CancleCurSetting();
    }
    public SettingData SETTING_GetSettingData()
    {
        return setting.GetSettingData();
    }

    /* ============= 
	    Loading
    ============= */
    public void LOADING_ChangeNextLevel(ELevelType _next, ELoadingScreenType _screenType)
    {
        loading.ChangeNextLevel(_next, _screenType);
    } 
    public void LOADING_StartNextLevel()
    {
        loading.StartNextLevel();
    } 
    public bool LOADING_IsComplateLoad()
    {
        return loading.IsComplateLoad();
    }
	public ELoadingScreenType LOADING_GetReserveLoadingScreenType()
	{
        return loading.GetReserveLoadingScreenType();
    } 

    /* ========== 
        Camera
    ========== */
    public void CAMERA_SetTopViewCameraDistance(float _distance)
    {
        cam.GetCameraSystem().SetTopViewCameraDistance(_distance);
    }  
    public void CAMERA_SetCameraFocus(Transform _tp)
    {
        cam.GetCameraSystem().SetCameraFocus(_tp); 
    } 
    public void CAMERA_ActievMainCamera(EDuckCameraType _type, bool _isActive)
    {
        cam.GetCameraSystem().ActievMainCamera(_type, _isActive); 
    }
    public void CAMERA_ActiveCinemachinCamera(EPlayCameraType _type)
    {
		cam.GetCameraSystem().ActiveCinemachinCamera(_type);
	}
    public void CAMERA_MoveCameraFocus(Vector3 _pos) 
    {
        cam.GetCameraSystem().MoveCameraFocuse(_pos);
    }
    public void CAMERA_ClearColor()
    {
        cam.GetCameraSystem().ClearColor();
    }
     
    /* =========== 
        Spawn
    =========== */
    public ItemBase SPAWN_MakeItem(EItemID _id)
    {
        return spawn.MakeItem(_id);
    }
    public ItemBase SPAWN_MakeItem(FItemShell _shell)
    {
        return spawn.MakeItem(_shell); 
    } 
      
    public ItemBase SPAWN_MakeWeapon(EWeaponType _type, EItemGrade _grade)
    { 
        return spawn.MakeWeapon(_type, _grade);
    }
    public ItemBase SPAWN_MakeArmor(EArmorType _type, EItemGrade _grade)
    {  
        return spawn.MakeArmor(_type, _grade);
    }
    public ItemBase SPAWN_MakeBullet(int _cnt, EItemGrade _grade)
    {   
        return spawn.MakeBullet(_cnt, _grade);
    }

    public ItemBase SPAWN_MakeRandomHeal(EItemGrade _grade)
    {
        return spawn.MakeRandomHeal(_grade); 
    }
    public ItemBase SPAWN_MakeRandomFood(EItemGrade _grade)
    {
        return spawn.MakeRandomFood(_grade);
    }
    public ItemBase SPAWN_MakeRandomBackpack(EItemGrade _grade)
    {
        return spawn.MakeRandomBackpack(_grade);
    }
    public ItemBase SPAWN_MakeRandomAttachment(EItemGrade _grade)
    {
        return spawn.MakeRandomAttachment(_grade);
    }
    public ItemBase SPAWN_MakeRandomStuff(EItemGrade _grade)
    {
        return spawn.MakeRandomStuff(_grade);
    } 

    public GameObject SPAWN_ItemBilloard()
    {
        return spawn.MakeBillboardPrefab();
    }
    public GameObject SPAWN_RaserBeam()
    {
        return spawn.MakeRaserBeamPrefab();
    }
    public GameObject SPAWN_MakeHitValuePrefab()
    {
        return spawn.MakeHitValuePrefab();
    }
    public GameObject SPAWN_MakeChickPrefab(Vector3 _pos, Quaternion _rot)
    {
        return spawn.GetPrefebSpawner().SpawnChickien(_pos, _rot);
    } 

    public GameObject SPAWN_BodyMesh(EAdornBodyType type)
	{
		return spawn.GetPrefebSpawner().SpawnBody(type);
	}  
	public GameObject SPAWN_EyesMesh(EAdornEyesType type)
    {
        return spawn.GetPrefebSpawner().SpawnEyes(type);
    }
    public GameObject SPAWN_BeakMesh(EAdornBeekType type)
    {
        return spawn.GetPrefebSpawner().SpawnBeak(type);
    }
    public GameObject SPAWN_EyesBowMesh(EAdornEyesBowType type)
    {
        return spawn.GetPrefebSpawner().SpawnEyesBow(type);
    }
    public GameObject SPAWN_HairMesh(EAdornHairType type)
    {
        return spawn.GetPrefebSpawner().SpawnHair(type);
    }
    public GameObject SPAWN_FootMesh(EAdornFootType type)
    {
        return spawn.GetPrefebSpawner().SpawnFoot(type);
    }
    public GameObject SPAWN_HandMesh(EAdornHandType type)
    { 
        return spawn.GetPrefebSpawner().SpawnHand(type);
    }

    /* =========== 
        Slot
    =========== */
    public SlotController SLOT_GetSlotController()
    {
        return slot.GetSlotController(); 
    }

    /* =========== 
        Sound
    =========== */
    public void SOUND_PlayBGM(EBgmType _type)
    {
        sound.GetSoundSystem().PlayBgm(_type);
    }   
    public void SOUND_StopBGM()
    {
        sound.GetSoundSystem().StopBgm();
    }

    public void SOUND_PlaySoundSfx(ESoundSfxType _type, Vector3 _worldPos, float duration = -1f)
    {
        sound.GetSoundSystem().PlaySfx(_type, _worldPos, duration);
    } 
    public void SOUND_PlayDuckQuack(EDuckType _type, Vector3 _worldPos)
    {
        sound.GetSoundSystem().PlayDuckQuack(_type, _worldPos);
    }
    public SoundSfx SOUND_GetSfx(ESoundSfxType type)
    {
        return sound.GetSoundSystem().GetSfx(type);
    }



    /* =========== 
        Map
    =========== */
    public Transform MAP_GetHouseRspawnTP()
    {
        return map.GetHouseSystem().GetPlayerRespawnTP();
    } 
    public AdornDuck MAP_GetAdornDuck()
    {
        return map.GetHouseSystem().GetAdornDuck();
    } 
    public DuckHouse MAP_GetHouse(int _id)
    {
        return map.GetHouse(_id);
    }
    public EMapType MAP_GetMapType()
    {
        return map.GetMapType();
    }
    public void MAP_DropItem(ItemBase _item)
    {
        map.DropItem(_item); 
    }

    public void MAP_ChangeDay(bool _instant)
    {
        map.ChangeDay(_instant);
    }
    public void MAP_ChangeNight(bool _instant)
    {
        map.ChangeNight(_instant);
    } 
    public void MAP_ShotInPos(Vector3 _pos)
    {
        map.ShotTargetPos(_pos);
    }

     
    /* =========== 
        Pool
    =========== */
    public GameObject POOL_Spawn(EPoolId _id, Vector3 _pos, Quaternion _rot)
    {
        return pool.GetPlayPoolSystem().Spawn(_id, _pos, _rot);
    } 
    public void POOL_Return(GameObject _gameObject)
    {
        pool.GetPlayPoolSystem().Return(_gameObject);
    } 
}
