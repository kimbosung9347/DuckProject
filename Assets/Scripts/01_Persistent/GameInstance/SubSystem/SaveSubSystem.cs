using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using NUnit.Framework.Interfaces;
using System.Threading.Tasks;
using System.Threading;

public class SaveSubSystem : GameInstanceSubSystem
{
    private const string SETTING_FILE = "/setting_data.json";
    private const string LOBBY_FILE = "/lobby_data.json"; 
    private const string USER_FILE = "/user_data.json";
    private const string SAVE_SLOT_FILE_PREFIX = "/save_slot_";

    private SettingData settingData;
    private LobbyData lobbyData;

    private PlayData[] arrPlayDatas = new PlayData[DuckDefine.MAX_SLOT_CNT];
       
    public override void Init()
    {
        // 세팅 데이터 로드
        LoadSettingData();

        // 로비 데이터 로드
        LoadLobbyData();

        // 플레이 데이터 로드
        for (int i = 0; i < DuckDefine.MAX_SLOT_CNT; ++i)
        {
            LoadPlayData(i);
        }
    }
    public override void LevelStart(ELevelType _type)
    {
        switch (_type)
        {
            case ELevelType.Mainmenu:
            { 
            }
            break;

            case ELevelType.Test:
            {
                lobbyData.SetSelectSaveSlotIndex(0);
            } 
            break;
        } 
    }
    public override void LevelEnd(ELevelType _type)
    {
        // 필요시 자동 저장 처리 가능
    }

    /* Delate */
    public void DelateSaveSlot(int _index)
    {
        if (_index < 0 || _index >= DuckDefine.MAX_SLOT_CNT)
            return;
         
        // -------------------------
        // 세이브 슬롯 디렉토리 삭제
        // -------------------------
        string slotDirPath = GetSaveSlotDirectoryPath(_index);

        if (Directory.Exists(slotDirPath))
        {
            Directory.Delete(slotDirPath, true);
            Debug.Log($"[SaveSubSystem] 세이브 슬롯 삭제됨 → {slotDirPath}");
        }

        // -------------------------
        // 메모리 상 PlayData 초기화
        // -------------------------
        arrPlayDatas[_index] = new PlayData();
    } 

    /* Save */
    public void SaveLobbyData(LobbyData _data)
    {
        lobbyData = _data;
        string path = Application.persistentDataPath + LOBBY_FILE;
        string json = JsonUtility.ToJson(lobbyData, true);
        File.WriteAllText(path, json);
        Debug.Log($"[SaveSubSystem] 로비 데이터 저장 완료 → {path}");
    }
    public void SavePlayData(int _index, PlayData _data)
    {
        if (_index < 0 || _index >= DuckDefine.MAX_SLOT_CNT)
        { 
            return;
        }

        arrPlayDatas[_index] = _data;
        string path = Application.persistentDataPath + SAVE_SLOT_FILE_PREFIX + _index + ".json";
        string json = JsonUtility.ToJson(_data, true);
        File.WriteAllText(path, json);
    }
    public void SaveSettingData(SettingData _data)
    { 
        string path = Application.persistentDataPath + SETTING_FILE;
        string json = JsonUtility.ToJson(settingData, true);
        File.WriteAllText(path, json); 
    }
     
    /* Clone & Getter */ 
    public LobbyData GetLobbyData()
    {
        return lobbyData; 
    }
    public SettingData GetSettingData()
    {
        return settingData;
    }
    public PlayData GetCurPlayData()
    {
        return arrPlayDatas[lobbyData.selectedSaveSlotIndex];
    }
    public PlayData GetPlayData(int _index)
    {
        return arrPlayDatas[_index];
    }

    /* load */
    private void LoadSettingData()
    {
        string path = Application.persistentDataPath + SETTING_FILE;

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            settingData = JsonUtility.FromJson<SettingData>(json);
        }
        else
        {
            settingData = new SettingData();
            settingData.path = path;
        } 
    }
    private void LoadLobbyData()
    {
        string path = Application.persistentDataPath + LOBBY_FILE;
        lobbyData = new LobbyData(); 
        lobbyData.Load(path); 
    }   
    private void LoadPlayData(int _index)
    {
        string saveSlotDirectoryPath = GetSaveSlotDirectoryPath(_index);

        PlayData playData = new PlayData();
        playData.LoadFromSaveSlotDirectory(saveSlotDirectoryPath);
        arrPlayDatas[_index] = playData; 
    }   
    private string GetSaveSlotDirectoryPath(int saveSlotIndex)
    {
        return Path.Combine(
            Application.persistentDataPath,
            $"save_slot_{saveSlotIndex}"
        );
    }
}


/* 세팅 데이터 */

[System.Serializable]
public class SettingData
{
    public string path; 
    public EResolutionType resolutionType = EResolutionType.e1920_1080;
    public EScreenModeType screenModeType = EScreenModeType.Window;
    public ELanguageType eLanguageType = ELanguageType.Korean;
    public float fullVolume = 100f; 
    public void Save(SettingData _copy)
    {
        resolutionType = _copy.resolutionType;
        screenModeType = _copy.screenModeType;
        eLanguageType = _copy.eLanguageType;
        fullVolume = _copy.fullVolume;
        System.IO.File.WriteAllText(path, JsonUtility.ToJson(this, true));
    } 
} 


/* 로비 데이터 */
 
// 로비에서 선택한 마지막 세이브슬롯 정보를 저장
[System.Serializable] 
public class LobbyData
{
    [System.NonSerialized] 
    private string path;

    public int selectedSaveSlotIndex = -1;

    public void SetSelectSaveSlotIndex(int index)
    {
        selectedSaveSlotIndex = index;
    }
      
    // 저장
    public void Save()
    {
        if (string.IsNullOrEmpty(path))
            return;

        string json = JsonUtility.ToJson(this, true);
        File.WriteAllText(path, json);
    }

    // 로드 
    public void Load(string _path)
    {
        path = _path;
         
        if (string.IsNullOrEmpty(path))
            return;

        if (!File.Exists(path))
            return;

        string json = File.ReadAllText(path);
        JsonUtility.FromJsonOverwrite(json, this);
    }
} 

/* 플레이 데이터 */

// 여기는 세이브 슬롯 데이터
[System.Serializable]
public class PlayData
{
    [System.NonSerialized]
    private string saveSlotDirectoryPath;

    // Save / Load 동시 접근 방지
    [System.NonSerialized]
    private readonly SemaphoreSlim ioLock = new SemaphoreSlim(1, 1);

    public bool bIsEmpty = true;

    public DifficultData difficultData;
    public CharacterData characterData;
    public CharacterMeshData characterMeshData;
    public QuestDataShell questData;
    public PlayerEquipItemData equipData;
    public PlayerStorageItemData storageData;
    public PlayerQuickSlotData qickSlotData; 
    public PlayerWarehouseItemData wareHouseData;
    public PlayerStatProgress statProgressData;
    public HomeData homeData;
    public FarmData farmData;
      
    // 맵정보를 담은 데이터도 있어야함 

    public PlayData()
    {
        bIsEmpty = true;
         
        difficultData ??= new DifficultData();
        characterData ??= new CharacterData();
        characterMeshData ??= new CharacterMeshData();
        questData ??= new QuestDataShell();
        equipData ??= new PlayerEquipItemData();
        storageData ??= new PlayerStorageItemData();
        qickSlotData ??= new PlayerQuickSlotData(); 
        wareHouseData ??= new PlayerWarehouseItemData();
        statProgressData ??= new PlayerStatProgress();
        homeData ??= new HomeData();
        farmData ??= new FarmData();
    }

    // =========================
    // [LEGACY] 동기 API
    // =========================
    public void LoadFromSaveSlotDirectory(string slotDirectoryPath)
    {
        // 초기 1회
        // Debug.LogError("Use LoadFromSaveSlotDirectoryAsync instead.");
        saveSlotDirectoryPath = slotDirectoryPath;
        bIsEmpty = !Directory.Exists(saveSlotDirectoryPath);
        difficultData = LoadOrNull<DifficultData>("difficult") ?? difficultData;
        characterData = LoadOrNull<CharacterData>("character") ?? characterData;
        characterMeshData = LoadOrNull<CharacterMeshData>("mesh") ?? characterMeshData;
        questData = LoadOrNull<QuestDataShell>("quest") ?? questData;
        equipData = LoadOrNull<PlayerEquipItemData>("equip") ?? equipData;
        storageData = LoadOrNull<PlayerStorageItemData>("storage") ?? storageData;
        qickSlotData = LoadOrNull<PlayerQuickSlotData>("quickSlot") ?? qickSlotData; 
        wareHouseData = LoadOrNull<PlayerWarehouseItemData>("warehouse") ?? wareHouseData;
        statProgressData = LoadOrNull<PlayerStatProgress>("stat") ?? statProgressData;
        homeData = LoadOrNull<HomeData>("home") ?? homeData;
        farmData = LoadOrNull<FarmData>("farm") ?? farmData; 
    } 
    public void Save(object data)
    {
        // Debug.LogError("Use SaveAsync instead.");
        AssignAndSave(data, SaveInternal);
    }

    // =========================
    // 비동기 API 
    // =========================
    public async Task LoadFromSaveSlotDirectoryAsync(string slotDirectoryPath)
    {
        await ioLock.WaitAsync();
        try
        {
            saveSlotDirectoryPath = slotDirectoryPath;
            bIsEmpty = !Directory.Exists(saveSlotDirectoryPath);

            difficultData = await LoadOrDefaultAsync("difficult", difficultData);
            characterData = await LoadOrDefaultAsync("character", characterData);
            characterMeshData = await LoadOrDefaultAsync("mesh", characterMeshData);
            questData = await LoadOrDefaultAsync("quest", questData);
            equipData = await LoadOrDefaultAsync("equip", equipData);
            storageData = await LoadOrDefaultAsync("storage", storageData);
            qickSlotData = await LoadOrDefaultAsync("quickSlot", qickSlotData); 
            wareHouseData = await LoadOrDefaultAsync("warehouse", wareHouseData);
            statProgressData = await LoadOrDefaultAsync("stat", statProgressData);
            homeData = await LoadOrDefaultAsync("home", homeData);
            farmData = await LoadOrDefaultAsync("farm", farmData);
        }
        finally
        {
            ioLock.Release();
        } 
    }
    public async Task SaveAsync(object data)
    {
        // 파일 IO 동시 접근을 방지하기 위한 비동기 락 획득
        await ioLock.WaitAsync();

        try
        {
            // 전달받은 데이터 객체를 PlayData에 할당하고
            // 실제 파일 저장 작업을 비동기적으로 수행 
            await AssignAndSaveAsync(data);
            bIsEmpty = false;
        }
         
        finally
        {
            // 반드시 IO 락을 해제하여 데드락을 방지
            ioLock.Release();
        }
    }

    // =========================
    // 내부 공통 처리
    // =========================
    private void AssignAndSave(object data, Action<string, object> saveFunc)
    {
        // 동기 로드도 지원
        switch (data)
        {
            case DifficultData d: difficultData = d; saveFunc("difficult", d); break;
            case CharacterData c: characterData = c; saveFunc("character", c); break;
            case CharacterMeshData m: characterMeshData = m; saveFunc("mesh", m); break;
            case QuestDataShell q: questData = q; saveFunc("quest", q); break;
            case PlayerEquipItemData e: equipData = e; saveFunc("equip", e); break;
            case PlayerStorageItemData s: storageData = s; saveFunc("storage", s); break;
            case PlayerQuickSlotData q: qickSlotData = q; saveFunc("quickSlot", q); break;
            case PlayerWarehouseItemData w: wareHouseData = w; saveFunc("warehouse", w); break;
            case PlayerStatProgress sp: statProgressData = sp; saveFunc("stat", sp); break;
            case HomeData h: homeData = h; saveFunc("home", h); break;
            case FarmData f: farmData = f; saveFunc("farm", f); break;
        }
         
        bIsEmpty = false;
    }
    private async Task AssignAndSaveAsync(object data)
    { 
        // 데이터 타입에 따라 동기 혹은 비동기 로드를 진행
        switch (data)
        {
            case DifficultData d: difficultData = d; await SaveInternalAsync("difficult", d); break;
            case CharacterData c: characterData = c; await SaveInternalAsync("character", c); break;
            case CharacterMeshData m: characterMeshData = m; await SaveInternalAsync("mesh", m); break;
            case QuestDataShell q: questData = q; await SaveInternalAsync("quest", q); break;
            case PlayerEquipItemData e: equipData = e; await SaveInternalAsync("equip", e); break;
            case PlayerStorageItemData s: storageData = s; await SaveInternalAsync("storage", s); break;
            case PlayerQuickSlotData q: qickSlotData = q; await SaveInternalAsync("quickSlot", q); break; 
            case PlayerWarehouseItemData w: wareHouseData = w; await SaveInternalAsync("warehouse", w); break;
            case PlayerStatProgress sp: statProgressData = sp; await SaveInternalAsync("stat", sp); break;
            case HomeData h: homeData = h; await SaveInternalAsync("home", h); break;
            case FarmData f: farmData = f; await SaveInternalAsync("farm", f); break;
        }
    }

    // ========================= 
    // Save / Load Core
    // =========================
    private void SaveInternal(string fileName, object data)
    {
        if (string.IsNullOrEmpty(saveSlotDirectoryPath))
            return;

        Directory.CreateDirectory(saveSlotDirectoryPath);
        string path = Path.Combine(saveSlotDirectoryPath, $"{fileName}.json");
        File.WriteAllText(path, JsonUtility.ToJson(data, true));
    }
    private async Task SaveInternalAsync<T>(string fileName, T data)
    {
        if (string.IsNullOrEmpty(saveSlotDirectoryPath))
            return;

        Directory.CreateDirectory(saveSlotDirectoryPath);
        string path = Path.Combine(saveSlotDirectoryPath, $"{fileName}.json");
        string json = JsonUtility.ToJson(data, true);

        await File.WriteAllTextAsync(path, json);
    }
    private T LoadOrNull<T>(string fileName) where T : class
    {
        string path = Path.Combine(saveSlotDirectoryPath, $"{fileName}.json");
        if (!File.Exists(path))
            return null;

        return JsonUtility.FromJson<T>(File.ReadAllText(path));
    }
    private async Task<T> LoadOrDefaultAsync<T>(string fileName, T defaultValue) where T : class
    {
        string path = Path.Combine(saveSlotDirectoryPath, $"{fileName}.json");
        if (!File.Exists(path))
            return defaultValue;

        string json = await File.ReadAllTextAsync(path);
        return JsonUtility.FromJson<T>(json) ?? defaultValue;
    }
} 

[System.Serializable]
public class PlayerStatProgress
{
    public LocoInfo loco;
    public ShotInfo shot;
    public ArmorInfo armor;
    public StatInfo stat;
    public CapacityInfo capacity; 
    public AnimInfo anim;

    public PlayerStatProgress()
    {
        DuckDefaultInfo def = GameInstance.Instance.TABLE_GetDuckDefaultInfo(EDuckType.Player);
        loco = def.locoInfo.Clone();
        shot = def.shotInfo.Clone();
        armor = def.armorInfo.Clone();
        stat = def.statInfo.Clone();
        capacity = def.capacityInfo.Clone();
        anim = def.animInfo.Clone();
    }
}

// 현재 난이도 정보를 저장 
public enum EPlayDifficultType
{
    Easy,
    Hard,
    Survival,
    Limit,
    Impossibility,

    End,
}
 
[Flags]
public enum EPlayDifficultFlag
{
    None = 0,
    Easy = 1 << 0,
    Hard = 1 << 1,
    Survival = 1 << 2,
    Limit = 1 << 3,
    Impossibility = 1 << 4,
    All = Easy | Hard | Survival | Limit | Impossibility
}

public static class PlayDifficultFlagExt
{
    public static EPlayDifficultFlag ToFlag(this EPlayDifficultType type)
    {
        return type switch
        {
            EPlayDifficultType.Easy => EPlayDifficultFlag.Easy,
            EPlayDifficultType.Hard => EPlayDifficultFlag.Hard,
            EPlayDifficultType.Survival => EPlayDifficultFlag.Survival,
            EPlayDifficultType.Limit => EPlayDifficultFlag.Limit,
            EPlayDifficultType.Impossibility => EPlayDifficultFlag.Impossibility,
            _ => EPlayDifficultFlag.None
        };
    }

    public static bool Has(this EPlayDifficultFlag flags, EPlayDifficultType type)
        => (flags & type.ToFlag()) != 0;

    public static EPlayDifficultFlag Add(this EPlayDifficultFlag flags, EPlayDifficultType type)
        => flags | type.ToFlag();

    public static EPlayDifficultFlag Remove(this EPlayDifficultFlag flags, EPlayDifficultType type)
        => flags & ~type.ToFlag();
}
 
// 저장용: 데이터만
[System.Serializable]
public class DifficultData
{
    [SerializeField] private EPlayDifficultFlag availableFlags = EPlayDifficultFlag.None;
    [SerializeField] private EPlayDifficultType difficultType = EPlayDifficultType.End;

    public EPlayDifficultFlag AvailableFlags => availableFlags;
    public EPlayDifficultType CurrentDifficult => difficultType;

    public DifficultData()
    {
        availableFlags = EPlayDifficultFlag.None
            .Add(EPlayDifficultType.Easy)
            .Add(EPlayDifficultType.Hard);

        difficultType = EPlayDifficultType.Easy;
    }
    public void CopyFrom(DifficultData src)
    {
        availableFlags = src.availableFlags;
        difficultType = src.difficultType;
    }

    // 데이터 세팅만 담당
    public void SetAvailableFlags(EPlayDifficultFlag flags)
    {
        availableFlags = flags;
    }

    public void SetCurrent(EPlayDifficultType type)
    {
        if (availableFlags.Has(type))
            difficultType = type;
    }
}

// 캐릭터 정보를 저장
[System.Serializable]
public class CharacterData
{
    public float playTime = 0f;
    public float gameTime = 6f;

    public float exp = 0f;
    public int level = 1;
    public int moeny = 0;
    public int wareHouseSize = 100;
} 

// 현재 캐릭터의 퀘스트 정보를 저장
[System.Serializable]
public struct FQuestDeliverRuntime
{
    public EItemID itemID;
    public int curCount;
    public int maxCount;
}
[System.Serializable]
public struct FQuestKillRuntime
{
    public EDuckType duckType;
    public bool isHead;
    public int curCount;
    public int maxCount;
}
[System.Serializable]
public class QuestRuntimeShell
{
    public EQuestID questID;
    public List<FQuestDeliverRuntime> listDeliverRuntimes = new();
    public List<FQuestKillRuntime> listKillRuntimes = new();
}
[System.Serializable]
public class QuestDataShell
{
    public List<EQuestID> complateQuest = new();
    public List<QuestRuntimeShell> inProgressQuest = new();
}

// 캐릭터의 외형 정보를 저장
[System.Serializable]
public class CharacterMeshData
{
    public Color bodyColor = Color.yellow;
    public Color eyeColor = Color.black;
    public Color eyebrowColor = Color.black;
    public Color armColor = Color.yellow;
    public Color footColor = Color.yellow;
    public Color beakColor = Color.yellow;
    public Color hairColor = Color.black;

    public EAdornBodyType bodyType = EAdornBodyType.Default;
    public EAdornEyesType eyesType = EAdornEyesType.Default;
    public EAdornBeekType beakType = EAdornBeekType.Default;
    public EAdornHandType handType = EAdornHandType.Default;
    public EAdornEyesBowType eyesBowType = EAdornEyesBowType.Unibrow;
    public EAdornHairType hairType = EAdornHairType.End;
    public EAdornFootType footType = EAdornFootType.Default;
}
 
// 캐릭터의 장착 아이템 정보를 저장
[System.Serializable] 
public class PlayerEquipItemData
{
    public List<FItemShellIncludeIndex> items = new();
}

// 캐릭터의 인벤토리 아이템을 저장 
[System.Serializable]
public class PlayerStorageItemData
{ 
    public List<FItemShellIncludeIndex> items = new();
}
 
// 플레이어의 퀵슬롯 아이템 정보를 저장
[System.Serializable]
public class PlayerQuickSlotData
{
    public FQucikSlotInfo[] arrQuickSlot = new FQucikSlotInfo[DuckDefine.MAX_QUICKSLOT_CNT];
    public PlayerQuickSlotData()
    {
        arrQuickSlot = new FQucikSlotInfo[DuckDefine.MAX_QUICKSLOT_CNT];
        for (int i = 0; i < arrQuickSlot.Length; i++)
        {
            arrQuickSlot[i].Empty();
        } 
    }
}   

// 창고 아이템을 저장 
[System.Serializable]
public class PlayerWarehouseItemData
{
    public List<FItemShellIncludeIndex> items = new();
}

// Home 집에 있는 데이터 - 가구 설치 그런 정보들
[System.Serializable]
public class HomeData
{
    // 구현안할듯  
}

[System.Serializable]
public struct ChickenBoxShell
{  
    public bool isEmpty;
    public FItemShell[] items; // 항상 10칸
    public Vector3 pos;
    public void Init()
    {
        isEmpty = true;
        items = new FItemShell[10];
        for (int i = 0; i < items.Length; i++)
        {
            items[i].Empty();
        } 

        pos = new Vector3();
    }
    public void Empty() 
    {
        isEmpty = true;
        items = new FItemShell[10];
        pos = Vector3.zero;
    } 
}
 
// Farm 농장에 있는 데이터 - 맵에 설치 하는 그런 정보들, -> 귀찮아서 치킨 정보만 만들어줄 듯 
[System.Serializable]
public class FarmData
{
    public ChickenBoxShell playerChicken;
    public FarmData() 
    {
        playerChicken.Init();
    }
    public void ClearChicken()
    { 
        playerChicken.isEmpty = true;
        for (int i = 0; i <  playerChicken.items.Length; i++)
        {
            playerChicken.items[i].Empty();
        } 
    }
}   

