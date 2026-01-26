#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class CreateMinimapCamera
{
    [MenuItem("Tools/Minimap/Create Minimap Camera")]
    public static void CreateMinimapCameraWithComponent()
    {
        if (GameObject.Find("MinimapCamera"))
        {
            Debug.LogWarning("MinimapCamera already exists.");
            return;
        }
         
        GameObject go = new GameObject("MinimapCamera");

        Camera cam = go.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 50f;
        cam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0f, 0f, 0f, 0f); // 투명

        int mask = LayerMask.GetMask("Minimap");
        cam.cullingMask = mask == 0 ? ~0 : mask;

        cam.depth = 10;
        cam.enabled = true;

        go.AddComponent<MinimapCamera>();

        Selection.activeGameObject = go;
    }
}
#endif
