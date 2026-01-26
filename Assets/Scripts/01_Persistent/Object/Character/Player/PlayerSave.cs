using UnityEngine;
using System.Threading.Tasks;
 
public class PlayerSave : MonoBehaviour
{ 
    private PlayerDifficult cachedPlayerDifficult;
    private PlayerMeshSetter cachedMeshSetter;
    private PlayerEquip cachedPlayerEquip;
    private PlayerStorage cachedPlayerStorage;
    private PlayerQuest cachedPlayerQuest;
    private PlayerGrow cachedPlayerGrow;
    private PlayerTime cachedPlayerTime;
    private PlayerQuickSlot cachedPlayerQuick; 

    // MapSubSystem;    
    private WareHouse cachedWareHouse;

    private void Awake()
    {
        cachedWareHouse = GameInstance.Instance.PLAYER_GetWareHouse();
        cachedPlayerDifficult = GetComponent<PlayerDifficult>();
        cachedPlayerEquip = GetComponent<PlayerEquip>();
        cachedPlayerStorage = GetComponent<PlayerStorage>();
        cachedPlayerQuest = GetComponent<PlayerQuest>();
        cachedPlayerGrow = GetComponent<PlayerGrow>();
        cachedPlayerTime = GetComponent<PlayerTime>(); 
        cachedPlayerQuick = GetComponent<PlayerQuickSlot>();
        cachedMeshSetter = GetComponentInChildren<PlayerMeshSetter>();
    }        
     
    public Task SaveAllForSallyForthAsync()
    {
        // 출격 시 
        return Task.WhenAll(
            SaveEquipAsync(),         // 장비 정보를 저장
            SaveStorageAsync(),       // 장착 정보를 저장
            SaveQuickAsync(),         // 퀵슬롯 정보를 저장
            SaveWareHouseAsync(),     // 창고 정보를 저장
            SaveCharacterDataAsync(), // 캐릭터 능력치를 저장
            SaveQuestAsync()          // 퀘스트 정보를 저장
        ); 
    }
    public Task SaveAllForWithdrawAsync()
    {
        // 철수 시
        return Task.WhenAll(
            SaveEquipAsync(),
            SaveStorageAsync(),
            SaveQuickAsync(),
            SaveCharacterDataAsync(),
            SaveMapInfo()
        ); 
    } 
    public Task SaveAllForDeadAsync()
    {
        // 사망 시 
        return Task.WhenAll(
            SaveEquipAsync(),
            SaveStorageAsync(),
            SaveQuickAsync(),
            SaveMapInfo()
        );   
    }
     
    public void SavePlayerMesh()
    {
        PlayData playData = GameInstance.Instance.SAVE_GetCurPlayData();
        var data = cachedMeshSetter.CreateCharacterMeshData();
        playData.Save(data); // 동기
    }
    public void SaveDifficult()
    {
        PlayData playData = GameInstance.Instance.SAVE_GetCurPlayData();
        var data = cachedPlayerDifficult.CreateDifficultData();
        playData.Save(data);
    } 
    public void ClearAllSaveChickenPrefab()
    {
        var playData = GameInstance.Instance.SAVE_GetCurPlayData();
        playData.farmData.playerChicken.Empty();
        playData.Save(playData.farmData);
    } 

    private async Task SaveQuestAsync()
    {
        PlayData playData = GameInstance.Instance.SAVE_GetCurPlayData();
        QuestDataShell data = cachedPlayerQuest.CreateQuestData();
        await playData.SaveAsync(data);
    }
    private async Task SaveEquipAsync()
    {
        // 장비 정보 저장
        PlayData playData = GameInstance.Instance.SAVE_GetCurPlayData();
        PlayerEquipItemData data = cachedPlayerEquip.CreateEquipItemData();
        await playData.SaveAsync(data);
    }
    private async Task SaveStorageAsync()
    {
        // 인벤토리 정보 저장
        PlayData playData = GameInstance.Instance.SAVE_GetCurPlayData();
        PlayerStorageItemData data = cachedPlayerStorage.CreateStorageItems();
        await playData.SaveAsync(data);
    }
    private async Task SaveQuickAsync()
    { 
        // 퀵 슬롯 저장
        PlayData playData = GameInstance.Instance.SAVE_GetCurPlayData();
        var data = cachedPlayerQuick.GetPlayerQuickSlotData();
        await playData.SaveAsync(data);
    }  

    private async Task SaveWareHouseAsync()
    {
        PlayData playData = GameInstance.Instance.SAVE_GetCurPlayData();
        var data = cachedWareHouse.CreateWareHouseData();
        await playData.SaveAsync(data);
    }
    private async Task SaveCharacterDataAsync()
    {
        PlayData playData = GameInstance.Instance.SAVE_GetCurPlayData();

        var data = new CharacterData
        {
            playTime = cachedPlayerTime.GetPlayTime(),
            gameTime = cachedPlayerTime.GetGameTime(),
            level = cachedPlayerGrow.GetLevel(),
            exp = cachedPlayerGrow.GetExp(),
            moeny = cachedPlayerStorage.GetMoney(),
            wareHouseSize = cachedWareHouse.GetMaxItemCnt()
        }; 

        await playData.SaveAsync(data);
    }

    private async Task SaveMapInfo()
    {
        var gameInstance = GameInstance.Instance;

        var mapType = gameInstance.MAP_GetMapType();
        PlayData playData = gameInstance.SAVE_GetCurPlayData();

        switch (mapType)
        {
            case EMapType.Home:
                await playData.SaveAsync(playData.homeData);
                break;

            case EMapType.Farm:
                await playData.SaveAsync(playData.farmData);
                break;
                 
            case EMapType.End:
                break;
        }
    }
}  
  