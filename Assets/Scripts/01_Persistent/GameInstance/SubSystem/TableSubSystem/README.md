using UnityEngine;

public class TableSubSystem : GameInstanceSubSystem
{
    BattleTable battle;
    ItemTable item;
    DuckTable duck;
    BuffTable buff;
    QuestTable quest;
    PlayerTable player;

    public override void Init()
    {
        CacheBattleTable();
        CacheItemTable();
        CacheDuckTable();
        CacheBuffTable(); 
        CacheQuestTable();
        CachePlayerTable();
    } 
    public override void LevelStart(ELevelType _type)
    {
  
    } 
    public override void LevelEnd(ELevelType _type)
    {

    }
     
    public BattleTable GetBattleTable()
    {
        return battle;
    }
    public ItemTable GetItemTable()
    {
        return item; 
    }
    public DuckTable GetDuckTable() { return duck; }  
    public BuffTable GetBuffTable() { return buff; }
    public QuestTable GetQuestTable() { return quest; }
    public PlayerTable GetPlayerTable() { return player; }
      
    private void CacheBattleTable()
    {
        var tables = Object.FindObjectsByType<BattleTable>(FindObjectsSortMode.None);
        if (tables.Length > 0)
        {
            battle = tables[0];
        } 
        else
        {
            Debug.LogWarning("[TableSubSystem] NotFound - battleTable");
        } 
    }  
    private void CacheItemTable()
    {
        var tables = Object.FindObjectsByType<ItemTable>(FindObjectsSortMode.None);
        if (tables.Length > 0)
        {
            item = tables[0]; 
        }
        else
        {  
            Debug.LogWarning("[TableSubSystem] NotFound - itemTable");
        }
    } 
    private void CacheDuckTable()
    {
        var tables = Object.FindObjectsByType<DuckTable>(FindObjectsSortMode.None);
        if (tables.Length > 0)
        {
            duck = tables[0];
            duck.Init();
        } 
        else 
        {
            Debug.LogWarning("[TableSubSystem] NotFound - Ducktable");
        }
    }
    private void CacheBuffTable()
    {
        var tables = Object.FindObjectsByType<BuffTable>(FindObjectsSortMode.None);
        if (tables.Length > 0)
        {
            buff = tables[0];
        } 
        else
        {
            Debug.LogWarning("[TableSubSystem] NotFound - Bufftable");
        } 
    }
    private void CacheQuestTable()
    {
        var tables = Object.FindObjectsByType<QuestTable>(FindObjectsSortMode.None);
        if (tables.Length > 0)
        { 
            quest = tables[0];
        }
        else
        { 
            Debug.LogWarning("[TableSubSystem] NotFound - questTable");
        }
    }
    private void CachePlayerTable() 
    {
        var tables = Object.FindObjectsByType<PlayerTable>(FindObjectsSortMode.None);
        if (tables.Length > 0)
        {
            player = tables[0];
        }
    }
}
