using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MinimapCanvas : MonoBehaviour,
        IPointerDownHandler,
        IPointerUpHandler,
        IDragHandler
{
    [SerializeField] private RectTransform playerImageTransform;
    [SerializeField] private Image minimapImage;
    [SerializeField] private TextMeshProUGUI textMapName;
    [SerializeField] private Slider sliderBar;

    [Header("Zoom")]
    [SerializeField] private float maxScale = 1.5f; // slider 0
    [SerializeField] private float minScale = 0.6f; // slider 1
    [SerializeField] private float scrollZoomSpeed = 0.15f;

    [SerializeField] private float dragSpeed = 1f;
    
    private BoxCollider cachedCurMapBoundary;
    private Vector2 initialMinimapPos;   // Awake 시 최초 위치
    private bool isDragging;

    private void Awake()
    {
        sliderBar.onValueChanged.RemoveAllListeners();
        sliderBar.onValueChanged.AddListener(OnZoomChanged);
        initialMinimapPos = minimapImage.rectTransform.anchoredPosition;
         
        Disable();
    }
    private void Update()
    {
        if (!gameObject.activeSelf)
            return;
         
        Vector2 scroll = Mouse.current?.scroll.ReadValue() ?? Vector2.zero;
        if (Mathf.Approximately(scroll.y, 0f))
            return;

        // 휠 위 = 확대 (slider 감소), 휠 아래 = 축소 (slider 증가)
        sliderBar.value = Mathf.Clamp01(
            sliderBar.value - scroll.y * scrollZoomSpeed * Time.unscaledDeltaTime
        );
    }

    public void CacheMinimapInfo(BoxCollider collider, Sprite minimapSprite, string name)
    {
        cachedCurMapBoundary = collider;
        minimapImage.sprite = minimapSprite;
        textMapName.text = name;
    }

    public void DisableMinimapInfo()
    {
        cachedCurMapBoundary = null; 
        if (minimapImage)
        {
            minimapImage.sprite = null;
        }
        if (textMapName)
        {
            textMapName.text = string.Empty;
        }
    } 

    public void Active()
    {
        if (!cachedCurMapBoundary)
            return;

        gameObject.SetActive(true); 

        minimapImage.rectTransform.anchoredPosition = initialMinimapPos;

        Vector3 min = cachedCurMapBoundary.bounds.min;
        Vector3 size = cachedCurMapBoundary.bounds.size;
        
        var playetTransform = GameInstance.Instance.PLAYER_GetPlayerTransform();
        Vector3 p = playetTransform.position;

        float nx = Mathf.InverseLerp(min.x, min.x + size.x, p.x);
        float nz = Mathf.InverseLerp(min.z, min.z + size.z, p.z);

        Rect rect = minimapImage.rectTransform.rect;
        float x = (nx - 0.5f) * rect.width;
        float y = (nz - 0.5f) * rect.height;

        sliderBar.value = 0f;
        playerImageTransform.anchoredPosition = new Vector2(x, y);
    } 
     
    private void OnZoomChanged(float value)
    {
        // minimap 스케일
        float mapScale = Mathf.Lerp(maxScale, minScale, value);
        minimapImage.rectTransform.localScale = Vector3.one * mapScale;

        // 플레이어 아이콘 크기 고정
        playerImageTransform.localScale = Vector3.one / mapScale;
    }

    public void Disable() 
    {
        gameObject.SetActive(false);
    }

    // ========================
    // Drag Interfaces
    // ========================
    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true; 
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging)
            return;

        minimapImage.rectTransform.anchoredPosition +=
            eventData.delta * dragSpeed;
    }
}
 