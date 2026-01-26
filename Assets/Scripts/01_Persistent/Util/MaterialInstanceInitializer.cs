using UnityEngine;
 
public sealed class MaterialInstanceInitializer : MonoBehaviour
{
    private void Awake()
    {
        var renderers = GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers)
        {
            if (r == null || r.sharedMaterial == null)
                continue;

            r.sharedMaterial = new Material(r.sharedMaterial);
        }
    }
}
