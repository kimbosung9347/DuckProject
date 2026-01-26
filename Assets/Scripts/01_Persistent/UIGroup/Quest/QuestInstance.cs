using System.Collections.Generic;

public class QuestInstance
{
    private QuestData questData;

    private List<FQuestDeliverRuntime> listDeliverRuntime = new();
    private List<FQuestKillRuntime> listKillRuntime = new();
      
    // 저장 데이터 기반
    public void Init(QuestRuntimeShell shell)
    {
        questData = GameInstance.Instance.TABLE_GetQuestData(shell.questID);

        // 저장된 진행도 → 런타임 복사
        listDeliverRuntime.Clear();
        listDeliverRuntime.AddRange(shell.listDeliverRuntimes);

        listKillRuntime.Clear();
        listKillRuntime.AddRange(shell.listKillRuntimes); 
    }
    // 새로 시작하는 퀘스트용
    public void Init(EQuestID questID)
    {
        questData = GameInstance.Instance.TABLE_GetQuestData(questID);

        listDeliverRuntime.Clear();
         
        foreach (var objective in questData.listObjective)
        {
            EObjectiveType objectiveType = objective.ObjectiveType;
            switch (objectiveType)
            {
                case EObjectiveType.Deliver:
                {
                    if (objective is DeliverQuestObjectiveData deliverData)
                    {
                        var runtime = new FQuestDeliverRuntime();
                        runtime.itemID = deliverData.ItemID;
                        runtime.curCount = 0;
                        runtime.maxCount = deliverData.RequiredCount;
                        listDeliverRuntime.Add(runtime);
                    } 
                }
                break; 
                     
                case EObjectiveType.Kill:
                {
                    if (objective is KillQuestObjectiveData killData)
                    {
                        var runtime = new FQuestKillRuntime();
                        runtime.duckType = killData.TargetDuckType;
                        runtime.isHead = killData.IsHeadShot;
                        runtime.curCount = 0;
                        runtime.maxCount=killData.RequiredCount;
                        listKillRuntime.Add(runtime);
                    } 
                }  
                break;
            }
        } 
    }
    public bool IsComplate() 
    {
        // Deliver 조건 체크
        for (int i = 0; i < listDeliverRuntime.Count; i++)
        {
            if (listDeliverRuntime[i].curCount < listDeliverRuntime[i].maxCount)
                return false;
        }

        // Kill 조건 체크
        for (int i = 0; i < listKillRuntime.Count; i++)
        {
            if (listKillRuntime[i].curCount < listKillRuntime[i].maxCount)
                return false;
        }

        return true;
    }

    public QuestRuntimeShell CreateRuntimeShell()
    {
        var shell = new QuestRuntimeShell();
        shell.questID = questData.questID;
        shell.listDeliverRuntimes.Clear();
        shell.listDeliverRuntimes.AddRange(listDeliverRuntime);
        shell.listKillRuntimes.Clear();
        shell.listKillRuntimes.AddRange(listKillRuntime);
          
        return shell;
    }
    public QuestData GetQuestDate() { return questData; }
    public List<FQuestDeliverRuntime> GetDeliverRuntimeData() { return listDeliverRuntime; }
    public List<FQuestKillRuntime> GetKillRuntimeData() { return listKillRuntime; }
         
}  
  