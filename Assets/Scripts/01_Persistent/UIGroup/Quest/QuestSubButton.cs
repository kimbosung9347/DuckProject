using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestSubButton : MonoBehaviour
{ 
    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private TextMeshProUGUI questMasterName;
     
    public void Init(EQuestID _questId, System.Action<EQuestID, Image> onClick)
    {
        var instance = GameInstance.Instance;

        QuestData questData = instance.TABLE_GetQuestData(_questId);
        questName.text = questData.questName;
        questMasterName.text = instance.TABLE_GetDuckName(questData.gaveQuestDuck);
         
        var button = GetComponent<Button>(); 
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke(_questId, GetComponent<Image>()));
    }   
}   
