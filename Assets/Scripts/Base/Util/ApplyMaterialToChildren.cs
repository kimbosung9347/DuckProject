using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif 

public class ApplyMaterialToChildren : MonoBehaviour
{
    [SerializeField] private Material targetMaterial;

    public void Apply()
    {
        if (!targetMaterial)
            return;

        var renderers = GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers)
        {
            r.sharedMaterial = targetMaterial;

            #if UNITY_EDITOR
            Undo.RecordObject(r, "Apply Material To Children"); 
            EditorUtility.SetDirty(r);
            #endif
             
        }
    } 
} 

#if UNITY_EDITOR
[CustomEditor(typeof(ApplyMaterialToChildren))]
public class ApplyMaterialToChildrenEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var t = (ApplyMaterialToChildren)target;
        GUILayout.Space(10);

        if (GUILayout.Button("Apply Material To All Children"))
        {
            t.Apply();
        }
    }
}
#endif
