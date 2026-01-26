using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public class HouseHideRoof : MonoBehaviour
{
    [SerializeField] private bool hideOnEnter = true;

    private readonly List<Renderer> listRoof = new();

    private void Awake()
    {
        CacheRenderers();
        ApplyEditorState();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        CacheRenderers();
        ApplyEditorState();
    }
#endif
     
    private void CacheRenderers()
    {
        listRoof.Clear();
        GetComponentsInChildren(true, listRoof);
    }

    private void ApplyEditorState()
    {
        // hideOnEnter == true  → 에디터에서 지붕 숨김
        // hideOnEnter == false → 에디터에서 지붕 보임
        SetRoofVisible(!hideOnEnter);
    } 
     
    public void SetRoofVisible(bool visible)
    {
        for (int i = 0; i < listRoof.Count; i++)
        {
            if (listRoof[i])
                listRoof[i].enabled = visible;
        }
    }
}
