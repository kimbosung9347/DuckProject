using UnityEngine;

public class AiDead : DuckDead
{
    private AiStorage cachedAiStorage;
    
    protected override void Awake()
    {
        base.Awake();

        cachedAiStorage = GetComponent<AiStorage>();
    } 
    public override void Dead(bool _isHead, DuckAttack _killedTarget)
    { 
        base.Dead(_isHead, _killedTarget); 
          
        // 일단 치킨프리팹을 드랍시켜줘야함
        DropChicken();
        
        // 후처리 
        ProcessAfterKill(_isHead, _killedTarget);
         
        // 이 오브젝트를 삭제해야함
        Destroy(gameObject); 
    }
    public override void Dead(bool _isHead, DuckMeleeAttack _killedTarget)
    {
        base.Dead(_isHead, _killedTarget);
         
        // 일단 치킨프리팹을 드랍시켜줘야함
        DropChicken();

        // 후처리 
        ProcessAfterKill(_isHead, _killedTarget);

        // 이 오브젝트를 삭제해야함
        Destroy(gameObject);
    } 

    private void DropChicken()
    { 
        // 이시점에는 Awak니깐 아이템 배열 다 초기화 됐을 것이고
        var prefab = GameInstance.Instance.SPAWN_MakeChickPrefab(transform.position, Quaternion.identity);
        ItemBoxBase chickenBox = prefab.GetComponent<ItemBoxBase>();
         
        // 장착 아이템 넣어주고
        chickenBox.AddItem(cachedAiStorage.GetSelectWeapon(),   EItemBoxSlotState.Complate);
        chickenBox.AddItem(cachedAiStorage.GetSelectHelmat(),   EItemBoxSlotState.Complate);
        chickenBox.AddItem(cachedAiStorage.GetSelectArmor(),    EItemBoxSlotState.Complate);
        chickenBox.AddItem(cachedAiStorage.GetSelectBackpack(), EItemBoxSlotState.Complate);
         
        // 총알을 쏘는 경우 총알도 만들어주고
        EItemID buleltId = cachedAiStorage.GetSelectBulletId();
        if (buleltId != EItemID._END)
        {
            var bullet = GameInstance.Instance.SPAWN_MakeItem(buleltId);
            if (bullet is Bullet bulletItem)
            {
                int randCnt = UnityEngine.Random.Range(20, 31); // 20 ~ 30
                bulletItem.SetCurCnt(randCnt);
            }
            chickenBox.AddItem(bullet, EItemBoxSlotState.NotSearch);
        }

        // 추가로 몬스터 별로 드랍가능한 아이템들을 넣어주기
        EDuckType duckType = GetComponent<DuckAbility>().GetDuckType();
        DuckDropItem dropItem = GameInstance.Instance.TABLE_GetDuckDropItem(duckType);
        if (dropItem)
        {
            foreach (var itemInfo in dropItem.list)
            {
                // weight 확률 통과 시에만 추가
                if (UnityEngine.Random.value <= itemInfo.weight)
                {
                    chickenBox.PushForceItem(itemInfo.itemId);
                }
            }
        } 
    }
    private void ProcessAfterKill(bool _isHead, DuckAttack _killtarget)
    {
        if (!_killtarget)
            return;

        if (_killtarget is not PlayerAttack playerAttack)
            return; 
          
        EDuckType aiDuckType = GetComponent<DuckAbility>().GetDuckType();
        PlayerAchievement achievements = playerAttack.gameObject.GetComponent<PlayerAchievement>();
        achievements.KillDuck(_isHead, aiDuckType);
    } 
    private void ProcessAfterKill(bool _isHead, DuckMeleeAttack _killtarget)
    {
        if (!_killtarget)
            return;

        if (_killtarget.IsAi())
            return;

        EDuckType aiDuckType = GetComponent<DuckAbility>().GetDuckType();
        PlayerQuest playerQuest = _killtarget.gameObject.GetComponent<PlayerQuest>();
        playerQuest.SuccessKillTarget(false, aiDuckType);
    } 
}    
   