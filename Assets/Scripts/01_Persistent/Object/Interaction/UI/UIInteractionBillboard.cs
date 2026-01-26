using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInteractionBillboard : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private GameObject interactionDesc;
    [SerializeField] private GameObject interactionImage;
       
    private TextMeshProUGUI cacheditemNameText;
    private TextMeshProUGUI cachedinteractionKeyText;
     
    private Transform cachedTransform;
    private Vector3 baseScale;
    private float elapsedTime;
    private bool isComplateGrew;
    private bool isLeft = false;

    private void Awake()
    {
        baseScale = transform.localScale;
        isComplateGrew = true;
        cacheditemNameText = interactionDesc.GetComponent<TextMeshProUGUI>();
        cachedinteractionKeyText = interactionImage.GetComponentInChildren<TextMeshProUGUI>();
        gameObject.SetActive(false);
    } 
    private void LateUpdate()
    {
        if (isComplateGrew)
            return;
         
        transform.position = cachedTransform.position;
        transform.LookAt(Camera.main.transform);
          
        float width = backgroundImage.rectTransform.rect.width; 
        Vector3 offset = Camera.main.transform.right * (width * 0.7f);
        if (isLeft)   
        {
            transform.position -= offset;
        }
        else
        {
            transform.position += offset;
        } 

        elapsedTime += Time.deltaTime;
        if (elapsedTime < 0.1f)
        {
            float t = elapsedTime / 0.1f;
            transform.localScale = baseScale * Mathf.Lerp(1f, 1.2f, t);
        }
        else
        {
            transform.localScale = baseScale;
            isComplateGrew = true;
        }
    }
     
    public void Active(Transform _targetTransform, string _itemName, string _interactionKey)
    {
        gameObject.SetActive(true);
        isComplateGrew = false;
        elapsedTime = 0f;
          
        cachedTransform = _targetTransform;
        cacheditemNameText.text = _itemName;
        cachedinteractionKeyText.text = _interactionKey;

        LayoutRebuilder.ForceRebuildLayoutImmediate(cacheditemNameText.rectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(cachedinteractionKeyText.rectTransform);

        float totalWidth =
            cacheditemNameText.preferredWidth +
            cachedinteractionKeyText.preferredWidth +
            0.6f;  

        RectTransform bgRect = backgroundImage.GetComponent<RectTransform>();
        bgRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, totalWidth);
    }
    public void Dsiable() 
    {
        cachedTransform = null;
        gameObject.SetActive(false);
    }

    public void SetIsLeft(bool _key)
    { 
        isLeft = _key;
    }
}
 