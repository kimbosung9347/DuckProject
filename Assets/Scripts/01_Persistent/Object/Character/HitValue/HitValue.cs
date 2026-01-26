using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HitValue : MonoBehaviour
{
    [SerializeField] private Image headSprite;
    [SerializeField] private TextMeshProUGUI valueText;

    [Header("Move")]
    [SerializeField] private float lifeTime = 1.2f;
    [SerializeField] private float moveHeight = 60f;
    [SerializeField] private float moveSide = 40f;
     
    private static readonly float[] dirTable =
    {
        -1.5f,   // 왼쪽
        -0.75f,  // 약간 왼쪽
         0f,     // 가운데
         0.75f,  // 약간 오른쪽
         1.5f    // 오른쪽
    };

    private float timer;
    private Vector3 startPos;
    private Vector3 endPos;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (!canvasGroup)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        float t = timer / lifeTime;

        float y = Mathf.Sin(t * Mathf.PI) * moveHeight;
        transform.localPosition = Vector3.Lerp(startPos, endPos, t) + Vector3.up * y;

        if (t >= 0.5f)
        {
            float fadeT = (t - 0.5f) / 0.5f;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, fadeT);
        }

        if (t >= 1f) 
            Destroy(gameObject);
    }

    public void Active(bool _isHead, float _value)
    {
        timer = 0f;
        canvasGroup.alpha = 1f;

        headSprite.enabled = _isHead;
        valueText.text = _value.ToString("F1");

        startPos = transform.localPosition;

        int index = Random.Range(0, dirTable.Length);
        float dir = dirTable[index];

        endPos = startPos + new Vector3(dir * moveSide, 0f, 0f);
    }
}
