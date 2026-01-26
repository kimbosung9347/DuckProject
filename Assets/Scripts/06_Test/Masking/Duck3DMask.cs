using UnityEngine;

public class Duck3DMask : MonoBehaviour
{
    [Header("Mask Volume")]
    [SerializeField] private float radius = 1.5f;
    [SerializeField] private float soft = 0.3f;

    private static readonly int MaskCenterWS = Shader.PropertyToID("_MaskCenterWS");
    private static readonly int MaskRadius = Shader.PropertyToID("_MaskRadius");
    private static readonly int MaskSoft = Shader.PropertyToID("_MaskSoft");

    private void LateUpdate()
    {
        Vector3 p = transform.position;

        Shader.SetGlobalVector(MaskCenterWS, new Vector4(p.x, p.y, p.z, 1f));
        Shader.SetGlobalFloat(MaskRadius, radius);
        Shader.SetGlobalFloat(MaskSoft, soft);
    }
     
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 1f, 0.35f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}
