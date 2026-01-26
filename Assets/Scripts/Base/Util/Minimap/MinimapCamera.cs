#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class MinimapCamera : MonoBehaviour
{
    [Header("Boundary")]
    [SerializeField] private BoxCollider boundary;
    [SerializeField] private float heightPadding = 10f;

    [Header("Bake Resolution")]
    [Tooltip("월드 1유닛당 픽셀 수")]
    [SerializeField] private float pixelsPerUnit = 4f;
    [SerializeField] private int minResolution = 512;
    [SerializeField] private int maxResolution = 2048;

    [Header("Output")]
    [SerializeField]
    private string outputDir =
        "Assets/RenderAsset/Texture_Material/Minimap/Output";

    Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        ApplyByBoundary();
    }

    private void OnValidate()
    {
        ApplyByBoundary();
    }

    // ======================
    // Camera Fit By Boundary
    // ======================
    private void ApplyByBoundary()
    {
        if (!boundary)
            return;

        if (!cam)
            cam = GetComponent<Camera>();

        Bounds b = boundary.bounds;

        cam.orthographic = true;
        cam.clearFlags = CameraClearFlags.SolidColor;

        // ★ 투명 배경 핵심
        cam.backgroundColor = new Color(0f, 0f, 0f, 0f);

        cam.transform.position = new Vector3(
            b.center.x,
            b.max.y + heightPadding,
            b.center.z
        );

        cam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        cam.orthographicSize = Mathf.Max(b.extents.x, b.extents.z);
    }

    // ======================
    // Bake
    // ======================
    [ContextMenu("Bake Minimap")]
    public void Bake()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogWarning("Stop Play Mode before baking minimap.");
            return;
        }

        if (!boundary)
        {
            Debug.LogError("MinimapCamera : Boundary not assigned.");
            return;
        }

        if (!cam)
            cam = GetComponent<Camera>();

        ApplyByBoundary();

        // ======================
        // Resolution 계산
        // ======================
        Bounds b = boundary.bounds;
        float worldSize = Mathf.Max(b.size.x, b.size.z);

        int size = Mathf.RoundToInt(worldSize * pixelsPerUnit);
        size = Mathf.Clamp(size, minResolution, maxResolution);

        // ======================
        // RenderTexture (알파 포함)
        // ======================
        RenderTexture rt = new RenderTexture(
            size, size, 24, RenderTextureFormat.ARGB32);
        rt.Create();

        bool prevEnabled = cam.enabled;
        RenderTexture prevRT = cam.targetTexture;

        cam.enabled = true;
        cam.targetTexture = rt;

        cam.Render();

        // ======================
        // ReadPixels (알파 유지)
        // ======================
        RenderTexture prevActive = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D tex = new Texture2D(
            size, size, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, size, size), 0, 0);
        tex.Apply();

        RenderTexture.active = prevActive;
        cam.targetTexture = prevRT;
        cam.enabled = prevEnabled;

        rt.Release();
        DestroyImmediate(rt);
         
        // ======================
        // Save
        // ======================
        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        string sceneName =
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        string path = $"{outputDir}/Minimap_{sceneName}.png";
        File.WriteAllBytes(path, tex.EncodeToPNG());

        DestroyImmediate(tex);
        AssetDatabase.Refresh();

        Debug.Log($"Minimap baked ({size}x{size}, Alpha OK) : {path}");
    }
}
#endif
