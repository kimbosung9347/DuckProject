using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

public class SaveSlotDesc : MonoBehaviour
{
    [SerializeField] private TMP_Text emptyText;
    [SerializeField] private TMP_Text difficutText;
    [SerializeField] private TMP_Text difficutDescText;
    [SerializeField] private TMP_Text playTimeText;
    [SerializeField] private TMP_Text playTimeDescText;

    private LocalizeStringEvent cachedDifficutLocalize;
     
    private void Awake()
    {
        cachedDifficutLocalize = difficutDescText.GetComponent<LocalizeStringEvent>();
    } 
     
    public void Active(EPlayDifficultType _difficult, float _playTime)
    {
        difficutText.gameObject.SetActive(true);
        difficutDescText.gameObject.SetActive(true);
        playTimeText.gameObject.SetActive(true);
        playTimeDescText.gameObject.SetActive(true);
        emptyText.gameObject.SetActive(false);
         
        // 난이도 Localize 갱신
        RenewDifficult(_difficult);
        RenewPlayTime(_playTime);

        GetComponent<UIAnimation>()?.Action_Animation();
    }
    public void Disable()
    {
        difficutText.gameObject.SetActive(false);
        difficutDescText.gameObject.SetActive(false);
        playTimeText.gameObject.SetActive(false);
        playTimeDescText.gameObject.SetActive(false);
        emptyText.gameObject.SetActive(true);
         
        GetComponent<UIAnimation>()?.Action_Animation();
    }

    private void RenewDifficult(EPlayDifficultType _difficult)
    {
        if (cachedDifficutLocalize != null)
        {
            cachedDifficutLocalize.StringReference.TableReference = "Difficult";
            cachedDifficutLocalize.StringReference.TableEntryReference =
                _difficult == EPlayDifficultType.Easy ? "Easy" : "Hard";

            // 즉시 새 텍스트로 반영
            cachedDifficutLocalize.RefreshString();
        }
    }
    private void RenewPlayTime(float _playTime)
    {
        int totalSec = Mathf.FloorToInt(_playTime);
        int totalMin = totalSec / 60;
        int hour = totalMin / 60;
        int min = totalMin % 60;
        string result = $"{hour}:{min:D2}";
        playTimeDescText.text = result;
    }    
}
