using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIBright : MonoBehaviour
{
    [Header("UI Bright Settings")]
    [Tooltip("밝기 비율")] 
    [SerializeField] private float brightRatio = 1.5f;
    [SerializeField] private bool bUseTexture; 
    [SerializeField] private Color BaseColor;  
    [SerializeField] private Texture2D texture;
      
    private Image cachedImage;
    private Material runtimeMaterial;
      
    private static readonly int ID_Brightness = Shader.PropertyToID("_Brightness");
    private static readonly int ID_IsUseTexture = Shader.PropertyToID("_IsUseTexture");
    private static readonly int ID_BaseColor = Shader.PropertyToID("_BaseColor");
    private static readonly int ID_MainTex = Shader.PropertyToID("_MainTex");

    private void Awake()
    {  
        cachedImage = GetComponent<Image>();
        cachedImage.color = Color.white;
        MakeRuntimeMaterial(); 
    }
    private void OnDestroy()
    {
        if (runtimeMaterial != null)
            Destroy(runtimeMaterial);
    }
       
    public void RenewBaseColor(Color _newColor)
    { 
        BaseColor = _newColor;
        runtimeMaterial.SetColor(ID_BaseColor, _newColor);
    }
    public void RenewBrightness(bool _isActive)
    {
        float value = _isActive ? brightRatio : 1f;
        runtimeMaterial.SetFloat(ID_Brightness, value);
    }

    /* 런타임 전용 머티리얼 생성 */
    private void MakeRuntimeMaterial()
    {
        var originalMat = cachedImage.material;
        if (originalMat == null)
        {
            Debug.LogWarning($"[{name}] Image에 Brightness Material이 없습니다.");
            return;
        }
         
        runtimeMaterial = Instantiate(originalMat);
        cachedImage.material = runtimeMaterial;

        // 초기 Brightness
        runtimeMaterial.SetFloat(ID_Brightness, 1f);
        runtimeMaterial.SetFloat(ID_IsUseTexture, bUseTexture ? 1f : 0f);
        if (bUseTexture)
        {
            Texture2D tex = texture != null ? texture : cachedImage.sprite?.texture;
            if (tex != null)
                runtimeMaterial.SetTexture(ID_MainTex, tex);
            else
                Debug.LogWarning($"[{name}] 사용할 Texture가 없습니다. (_UseTexture가 true지만 Texture가 비어있음)");
        }
         
        RenewBaseColor(BaseColor);
    }
}
