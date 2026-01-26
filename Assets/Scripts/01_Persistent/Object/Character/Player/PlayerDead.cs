using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDead : DuckDead
{
    private PlayerDifficult cachedDifficult;
    private PlayerStorage cachedStorage;
    private PlayerEquip cachedEquip;
    private PlayerInteraction cachedInteraction;

    protected override void Awake()
    {
        base.Awake();
        cachedDifficult = GetComponent<PlayerDifficult>();
        cachedStorage = GetComponent<PlayerStorage>();
        cachedEquip = GetComponent<PlayerEquip>();
        cachedInteraction = GetComponent<PlayerInteraction>();
    }
    public override void Dead(bool _isHead, DuckAttack _killedTarget)
    {
        base.Dead(_isHead, _killedTarget);
        ProcessAfterDead();

        var duckType = _killedTarget.GetComponent<DuckAbility>().GetDuckType();
        string duckName = GameInstance.Instance.TABLE_GetDuckName(duckType);
        string weaponName = _killedTarget.GetWeapon().GetItemData().itemName;
        ReturnToHome(duckName, weaponName);
    }
    public override void Dead(bool _isHead, DuckMeleeAttack _killedTarget)
    {
        base.Dead(_isHead, _killedTarget);
        ProcessAfterDead();

        var duckType = _killedTarget.GetComponent<DuckAbility>().GetDuckType();
        string duckName = GameInstance.Instance.TABLE_GetDuckName(duckType);
        ReturnToHome(duckName, "주먹");
    } 

    private void ProcessAfterDead()
    {
        var gi = GameInstance.Instance;
        var mapType = gi.MAP_GetMapType();
         
        switch (mapType)
        {
            case EMapType.Home:
                {
                    // Home에서 사망 시 처리 (현재 없음)
                }
                break; 

            case EMapType.Farm:
                {
                    gi.SPAWN_MakeChickPrefab(transform.position, Quaternion.identity);
                    PlayData playData = gi.SAVE_GetCurPlayData();
                    ChickenBoxShell chickenShell = CreateChickenShellByDifficult();
                    playData.farmData.playerChicken = chickenShell;
                }
                break;
        }

        // 모든 아이템을 삭제 시켜줘야함
        // Storage 먼저 삭제해주자 -> 가방때문에
        cachedStorage.DelateAllItem();
        cachedEquip.DelateAllEquip();
    }      
    private ChickenBoxShell CreateChickenShellByDifficult()
    {
        var shell = new ChickenBoxShell();
        shell.Init(); 
        shell.pos = transform.position;

        switch (cachedDifficult.curDifficultType)
        { 
            case EPlayDifficultType.Easy:
                {
                    shell.isEmpty = false;
                    int index = 0;

                    // ======================
                    // Equip 아이템 먼저
                    // ======================
                    for (int i = 0; i < (int)EEquipSlot.End && index < shell.items.Length; i++)
                    {
                        var equip = cachedEquip.GetEquip((EEquipSlot)i);
                        if (!equip)
                            continue;

                        shell.items[index] = equip.GetItemShell();
                        index++;
                    }

                    // ======================
                    // Storage 아이템
                    // ======================
                    List<ItemBase> listItems = cachedStorage.GetItemList();
                    for (int i = 0; i < listItems.Count && index < shell.items.Length; i++)
                    {
                        var item = listItems[i];
                        if (!item)
                            continue;

                        shell.items[index] = item.GetItemShell();
                        index++;
                    }
                }
                break;

            case EPlayDifficultType.Hard:
                {
                    shell.isEmpty = true;
                }
                break;
        }
        return shell;
    } 
    private void ReturnToHome(string _ai, string _weapon)
    {
        StartCoroutine(ReturnToHomeRoutine(_ai, _weapon));
    }
    private IEnumerator ReturnToHomeRoutine(string _ai, string _weapon)
    { 
        yield return new WaitForSeconds(0.75f);
        cachedInteraction.EnterDeadAndMoveHome(_ai, _weapon);
    } 
}
