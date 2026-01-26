using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndSallyForthCanvas : MonoBehaviour
{ 
    [SerializeField] private DeathReasonText deadReasonText;
    [SerializeField] private TextMeshProUGUI endtText; 
    [SerializeField] private TextMeshProUGUI lvText;
    [SerializeField] private TextMeshProUGUI expCurMaxText;
    [SerializeField] private Image guageImage;
    [SerializeField] private Button confirmButton;

    [SerializeField] private Color guageFillColor;
    [SerializeField] private float fillDuration = 0.6f;

    [Header("SFX")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip deadSfx;
    [SerializeField] private AudioClip bigRewardSfx;
    [SerializeField] private AudioClip normalRewardSfx;
     
    private Material cachedGuageMat;
    private Coroutine fillRoutine;
    private Action confirmAction;

    private static readonly int ProgressID = Shader.PropertyToID("_Progress");
     
    private void Awake()
    { 
        var gi = GameInstance.Instance;

        cachedGuageMat = guageImage.material = new Material(guageImage.material);
        cachedGuageMat.SetColor("_FillColor", guageFillColor);
        cachedGuageMat.SetFloat(ProgressID, 0f);

        Disable();
    }
    public void Active(string _duck, string _weapon, Action _bindAction)
    {
        var gameInstance = GameInstance.Instance;
        gameInstance.UI_GetPersistentUIGroup().GetRadiusCollaspeCanvas().Disable();
        gameObject.SetActive(true);
         
        // 사망 원인 표시
        deadReasonText.gameObject.SetActive(true);
        deadReasonText.SetText(_duck, _weapon);

        // 버튼 바인딩 (아직 비활성)
        confirmAction = _bindAction;
        BindButton();
        confirmButton.gameObject.SetActive(false);
         
        var playerGrow = gameInstance.PLAYER_GetPlayerGrow();
        // 게이지/레벨 즉시 반영
        int level = playerGrow.GetLevel();
        float curExp = playerGrow.GetExp();
        float requireExp = GameInstance.Instance.TABLE_GetRequireExp(level);

        guageImage.gameObject.SetActive(true);
        cachedGuageMat.SetFloat(ProgressID, curExp / requireExp);
        UpdateTexts(level, curExp, requireExp);

        endtText.text = "사망";

        // 사망 사운드
        if (deadSfx && sfxSource)
            sfxSource.PlayOneShot(deadSfx);
         
        // 1.5초 후 버튼 활성화
        Invoke(nameof(EnableConfirmButton), 1.5f);
    }
    public void Actvie(Action _buttonAction)
    {
        var gameInstance = GameInstance.Instance;
        gameInstance .UI_GetPersistentUIGroup().GetRadiusCollaspeCanvas().Disable();
        gameObject.SetActive(true);
        deadReasonText.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(false);
        confirmAction = _buttonAction;
        BindButton();

        var playerGrow = gameInstance.PLAYER_GetPlayerGrow();
        var playerAchievement = gameInstance.PLAYER_GetAchievment();
         
        int level = playerGrow.GetLevel();
        float curExp = playerGrow.GetExp();
        float addExp = playerAchievement.GetCalculateExp();
        playerGrow.AddExp(addExp); // 실제 레벨업

        if (fillRoutine != null)
            StopCoroutine(fillRoutine);
         
        guageImage.gameObject.SetActive(true);
        cachedGuageMat.SetFloat(ProgressID, 0f);
        fillRoutine = StartCoroutine(Co_FillGaugeWithLevel(level, curExp, addExp));
         
        endtText.text = "철수 완료";
        if (addExp > 100f)
        {
            if (bigRewardSfx) sfxSource.PlayOneShot(bigRewardSfx);
        }
        else
        {
            if (normalRewardSfx) sfxSource.PlayOneShot(normalRewardSfx);
        }
    } 
    public void Disable()
    {
        if (fillRoutine != null)
        {
            StopCoroutine(fillRoutine);
            fillRoutine = null;
        }

        confirmAction = null;
        gameObject.SetActive(false);
    }

    private IEnumerator Co_FillGaugeWithLevel(int startLevel, float startExp, float addExp)
    {
        int level = startLevel;
        float exp = startExp;

        float requireExp = GameInstance.Instance.TABLE_GetRequireExp(level);
         
        // 추가: 획득 경험치 없으면 즉시 반영
        if (addExp <= 0f)
        {
            cachedGuageMat.SetFloat(ProgressID, exp / requireExp);
            UpdateTexts(level, exp, requireExp);

            confirmButton.gameObject.SetActive(true);
            confirmButton.GetComponent<UIAnimation>()?.Action_Animation();
            yield break;
        }

        float remain = addExp;
        UpdateTexts(level, exp, requireExp);

        while (remain > 0f)
        {
            float need = requireExp - exp;
            float gain = Mathf.Min(need, remain);

            float from = exp / requireExp;
            float to = (exp + gain) / requireExp;

            float t = 0f;
            while (t < fillDuration)
            {
                t += Time.deltaTime;
                float v = Mathf.Lerp(from, to, t / fillDuration);
                cachedGuageMat.SetFloat(ProgressID, v);
                yield return null;
            }

            exp += gain;
            remain -= gain;

            if (exp >= requireExp)
            {
                level++;
                exp = 0f;
                requireExp = GameInstance.Instance.TABLE_GetRequireExp(level);

                cachedGuageMat.SetFloat(ProgressID, 0f);
            }

            UpdateTexts(level, exp, requireExp);
        }

        confirmButton.gameObject.SetActive(true);
        confirmButton.GetComponent<UIAnimation>()?.Action_Animation();
    }
    private void UpdateTexts(int level, float exp, float requireExp)
    {
        lvText.text = $"LV. {level}";
        expCurMaxText.text = $"<size=100%>{Mathf.FloorToInt(exp)}</size>" + $"<sub><size=90%> / {Mathf.FloorToInt(requireExp)}</size></sub>\r\n";
    }
    private void BindButton()
    {
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(PressConfirmButton);
    }
    private void PressConfirmButton()
    {
        GameInstance.Instance.UI_GetPersistentUIGroup().GetDisableCanvas().ActiveFadeIn(ExeConfirmAction, 0.75f);
    } 
    private void EnableConfirmButton()
    {
        confirmButton.gameObject.SetActive(true);
        confirmButton.GetComponent<UIAnimation>()?.Action_Animation();
    }
     
    private void ExeConfirmAction()
    {
        confirmAction?.Invoke();
        Disable();
    }

}
 