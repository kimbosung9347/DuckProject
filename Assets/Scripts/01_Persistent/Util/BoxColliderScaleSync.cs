using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(BoxCollider))]
public class BoxColliderScaleSync : MonoBehaviour
{
    private BoxCollider col;
    private Vector3 lastScale;
    private Vector3 baseSize;

    private void OnEnable()
    {
        Cache();
        Apply();
    }

    private void OnValidate()
    {
        Cache();
        Apply();
    }

    private void Cache()
    {
        if (!col)
            col = GetComponent<BoxCollider>();

        if (lastScale == Vector3.zero)
        {
            lastScale = transform.lossyScale;
            baseSize = col.size;
        }
    } 

    private void Apply()
    {
        Vector3 s = transform.lossyScale;

        col.size = new Vector3(
            baseSize.x * SafeDiv(s.x, lastScale.x), 
            baseSize.y * SafeDiv(s.y, lastScale.y),
            baseSize.z * SafeDiv(s.z, lastScale.z)
        );

        col.center = Vector3.zero;
    }

    private float SafeDiv(float a, float b)
    {
        return Mathf.Abs(b) < 0.0001f ? 1f : a / b;
    }
}
