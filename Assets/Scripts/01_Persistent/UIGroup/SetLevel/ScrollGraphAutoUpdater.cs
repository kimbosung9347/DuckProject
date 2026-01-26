using UnityEngine;
using UnityEngine.UI;

public class ScrollGraphAutoUpdater : MonoBehaviour
{
    private Material cachedMaterial;
    private static readonly int TilingID = Shader.PropertyToID("_Tiling");

    private void Awake()
    {
        var Image = GetComponent<Image>();
         
        // 기존 마테리얼 인스턴스화
        if (Image.material != null)
        {
            cachedMaterial = Instantiate(Image.material);
            Image.material = cachedMaterial;
        }
        else
        {
            Debug.LogWarning("[ScrollGraphAutoUpdater] Image에 마테리얼이 없습니다.");
        }
    } 

    private void OnEnable()
    {
        RenewTiling();
    }

    private void RenewTiling()
    { 
        if (cachedMaterial == null)
            return;
         
        // 예시: 1920x1080 → (1.92, 1.08)
        Vector2 tiling = new Vector2(Screen.width / 1000f, Screen.height / 1000f);
        cachedMaterial.SetVector(TilingID, tiling);
    }
}
