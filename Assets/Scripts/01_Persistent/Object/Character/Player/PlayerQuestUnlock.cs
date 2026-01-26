//using UnityEngine;
//using System.Collections.Generic; 
 
//public class PlayerQuestUnlock : MonoBehaviour
//{
//    private PlayerQuest playerQuest;
//    private PlayerGrow cachedPlayerGrow;   
     
//    private void Awake()
//    {
//        cachedPlayerGrow = GetComponent<PlayerGrow>();
//        playerQuest = GetComponent<PlayerQuest>();
//    } 
      
//    public bool CanUnLock(EQuestID _id)
//    {
//        var unlockData = GameInstance.Instance.TABLE_GetQuestUnlockData(_id);
//        return unlockData.IsSuccesUnLock(this);
//    }  

//    public bool HasItem(EItemID id, int count)
//    {
//        // todo 인벤토리에서 모든 아이템을 추적하게끔 만들어줘야한다 이거떄문에
//        // inventory.GetItemCount(id) >= count; 
//        return false; 
//    } 

//    public bool IsAllClearQuest(List<EQuestID> _listQuest)
//    {
//        foreach (var _quest in _listQuest)
//        {
//            if (!playerQuest.IsComplateQuest(_quest))
//                return false;
//        }
//        return true; 
//    } 
    
//    public bool IsReachLevel(int _targetLevel)
//    {
//        int curLevel = cachedPlayerGrow.GetLevel();
//        return curLevel >= _targetLevel;
//    }  

//}
