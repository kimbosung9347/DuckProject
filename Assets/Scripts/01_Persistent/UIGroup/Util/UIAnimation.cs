using UnityEngine;

public enum EUIAnimationType
{
    GrewBigger,
    ShakeHorizontal,
    FadeIn,          
     
    End,
}

[System.Serializable]
public struct FUIAnimationInfo_GrewBigger
{
    [Tooltip("커질 때 배율 (ex: 1.2 = 20% 커짐)")]
    public float growScaleRatio;
     
    [Tooltip("전체 재생 시간 (커졌다가 원래로 돌아오기까지 총 시간)")]
    public float totalTime;

    public FUIAnimationInfo_GrewBigger(float _ratio = 1.1f, float _time = 0.1f)
    {
        growScaleRatio = _ratio;
        totalTime = _time;
    } 
    public static FUIAnimationInfo_GrewBigger Default => new FUIAnimationInfo_GrewBigger(1.1f, 0.1f);
} 

[System.Serializable]
public struct FUIAnimationInfo_ShakeHorizontal
{
    [Tooltip("좌우로 흔들리는 강도")]
    public float amplitude;

    [Tooltip("한 번 흔들리는 속도")]
    public float frequency;

    [Tooltip("총 지속 시간")]
    public float totalTime;
     
    public FUIAnimationInfo_ShakeHorizontal(float _amp = 10f, float _freq = 8f, float _time = 0.3f)
    {
        amplitude = _amp;
        frequency = _freq;
        totalTime = _time;
    }
    public static FUIAnimationInfo_ShakeHorizontal Default => new FUIAnimationInfo_ShakeHorizontal(10f, 8f, 0.3f);
}

[System.Serializable]
public struct FUIAnimationInfo_Fade
{
    [Tooltip("시작 알파")]
    public float startAlpha;

    [Tooltip("끝 알파")]
    public float endAlpha;

    [Tooltip("총 지속 시간")]
    public float totalTime;

    public FUIAnimationInfo_Fade(float _start = 0f, float _end = 1f, float _time = 0.2f)
    {
        startAlpha = _start;
        endAlpha = _end;
        totalTime = _time;
    }

    public static FUIAnimationInfo_Fade Default => new FUIAnimationInfo_Fade(0f, 1f, 0.2f);
}
public class UIAnimation : MonoBehaviour
{
    [SerializeField] private EUIAnimationType animationType = EUIAnimationType.End;
    [SerializeField] private FUIAnimationInfo_GrewBigger info_GrewBigger;
    [SerializeField] private FUIAnimationInfo_ShakeHorizontal info_ShakeHorizontal;
    [SerializeField] private FUIAnimationInfo_Fade info_Fade;

    private CanvasGroup canvasGroup;
    private bool bIsActive = false;
    private RectTransform rectTransform;
    private Vector3 startScale = Vector3.one;
    private Vector2 startPos = Vector3.zero;
    private float startAlpha;
    private float timer;
        
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        startScale = rectTransform.localScale;
        startPos = rectTransform.anchoredPosition;
         
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        startAlpha = canvasGroup.alpha;
    }
    private void Update()
    {
        if (!bIsActive)
            return;

        switch (animationType)
        {
            case EUIAnimationType.GrewBigger:
                Update_GrewBigger();
                break;

            case EUIAnimationType.ShakeHorizontal:
                Update_ShakeHorizontal();
                break;

            case EUIAnimationType.FadeIn:        // ✅ 추가
                Update_Fade();
                break;
        } 
    }

    /* 외부 호출 */
    public void Action_Animation()
    {
        ClearAnimation();
        bIsActive = true;
        timer = 0f;
    }
    public void Action_Animation(EUIAnimationType _type)
    {
        animationType = _type;
        Action_Animation();
    }  

    public void StopAnimation()
    {
        ClearAnimation();
        bIsActive = false;
        timer = 0f; 
    }  
     
    /* Update */
    private void Update_GrewBigger()
    {
        if (!rectTransform)
        {
            StopAnimation();
            return;
        }
         
        timer += Time.unscaledDeltaTime;
        float half = info_GrewBigger.totalTime * 0.5f;
        float ratio;

        if (timer <= half)
        {
            // 커지는 구간
            ratio = Mathf.Lerp(1f, info_GrewBigger.growScaleRatio, timer / half);
        }
        else
        {
            // 원래로 돌아오는 구간
            float t = (timer - half) / half;
            ratio = Mathf.Lerp(info_GrewBigger.growScaleRatio, 1f, t);
        }

        rectTransform.localScale = startScale * ratio;
        if (timer >= info_GrewBigger.totalTime)
        {
            StopAnimation();
        }
    }
    private void Update_ShakeHorizontal()
    {
        if (!rectTransform)
        {
            StopAnimation();
            return;
        }

        timer += Time.unscaledDeltaTime;
         
        // 0 ~ totalTime 동안 sin파로 좌우 이동
        float offset = Mathf.Sin(timer * info_ShakeHorizontal.frequency * Mathf.PI * 2f) * info_ShakeHorizontal.amplitude;
        rectTransform.anchoredPosition = startPos + new Vector2(offset, 0f);

        if (timer >= info_ShakeHorizontal.totalTime)
        {
            StopAnimation();
        }
    }
    private void Update_Fade()
    {
        if (!canvasGroup)
        {
            StopAnimation();
            return;
        }
          

        timer += Time.unscaledDeltaTime;
        float t = Mathf.Clamp01(timer / info_Fade.totalTime);
        canvasGroup.alpha = Mathf.Lerp(info_Fade.startAlpha, info_Fade.endAlpha, t);

        if (timer >= info_Fade.totalTime)
        {
            StopAnimation();
        }
    }
    /* Clear */
    private void ClearAnimation()
    {
        switch (animationType)
        {
            case EUIAnimationType.GrewBigger:
            {
                ClearGrewBigger();
            }
            break;

            case EUIAnimationType.ShakeHorizontal:
            {
                ClearShakeHorizontal();
            }
            break;
                 
            case EUIAnimationType.FadeIn:
            {
                ClearFade();
            }
            break;
        } 
    }
    private void ClearGrewBigger()
    {
        if (rectTransform)
        {
            rectTransform.localScale = startScale;
        }
    }
    private void ClearShakeHorizontal()
    {
        if (rectTransform)
        {
            rectTransform.anchoredPosition = startPos;
        }
    }
    private void ClearFade()
    {
        if (canvasGroup)
        {
            canvasGroup.alpha = info_Fade.endAlpha;
        }
    } 
} 
