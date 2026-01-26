using UnityEngine;
using UnityEngine.UI;

public enum ECursorHitState
{
    Normal,
    Head,
    Dead
}

public class CursorHit : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private RectTransform leftTop;
    [SerializeField] private RectTransform rightTop;
    [SerializeField] private RectTransform leftDown;
    [SerializeField] private RectTransform rightDown;

    [Header("Layout")]
    [SerializeField] private Vector2 offset = new Vector2(10f, 10f);
    [SerializeField] private Vector2 minOffset = new Vector2(3f, 3f); // ⭐ 최소 수렴 거리
     
    [Header("Life")]
    [SerializeField] private float lifeTime = 0.15f;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color headColor = Color.red;
    [SerializeField] private Color deadColor = Color.gray;

    private RectTransform selfRect;
    private CanvasGroup canvasGroup;
    private Image[] images;

    private float timer;
    private bool isActive;

    private void Awake()
    {
        selfRect = GetComponent<RectTransform>();

        images = new[]
        {
            leftTop.GetComponent<Image>(),
            rightTop.GetComponent<Image>(),
            leftDown.GetComponent<Image>(),
            rightDown.GetComponent<Image>()
        };

        canvasGroup = GetComponent<CanvasGroup>();
        if (!canvasGroup)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isActive)
            return;
         
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / lifeTime);

        // ===== 이동 : 가속 수렴 =====
        float easeT = EaseOutCubic(t); 
        Vector2 cur = Vector2.Lerp(offset, minOffset, easeT);

        leftTop.anchoredPosition = new Vector2(-cur.x, cur.y);
        rightTop.anchoredPosition = new Vector2(cur.x, cur.y);
        leftDown.anchoredPosition = new Vector2(-cur.x, -cur.y);
        rightDown.anchoredPosition = new Vector2(cur.x, -cur.y);

        // ===== 알파 : 0 → 1 → 0 =====
        canvasGroup.alpha = Mathf.Sin(t * Mathf.PI);

        if (timer >= lifeTime)
            Deactive();
    }

    // =========================
    // API
    // =========================
    public void Active(ECursorHitState state)
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        timer = 0f;
        isActive = true;

        // 항상 부모 커서 중심
        selfRect.anchoredPosition = Vector2.zero;

        leftTop.anchoredPosition =
        rightTop.anchoredPosition =
        leftDown.anchoredPosition =
        rightDown.anchoredPosition = Vector2.zero;

        SetColor(state);
        canvasGroup.alpha = 1f;
    } 

    private void Deactive()
    {
        isActive = false;
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    private void SetColor(ECursorHitState state)
    {
        Color c = state switch
        {
            ECursorHitState.Head => headColor,
            ECursorHitState.Dead => deadColor,
            _ => normalColor
        };

        for (int i = 0; i < images.Length; i++)
            images[i].color = c;
    }

    private static float EaseOutCubic(float t)
    {
        t = Mathf.Clamp01(t);
        return 1f - Mathf.Pow(1f - t, 3f);
    }
}
