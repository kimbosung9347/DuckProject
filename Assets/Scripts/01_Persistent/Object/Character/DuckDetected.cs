using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.UIElements;

public class DuckDetected : MonoBehaviour
{
    [SerializeField] protected DuckMeshSetter duckMeshSetter;
    [SerializeField] protected DuckHpbar duckHpBar;
       
    // DuckHouse? 
    int curDuckHouseId = -1;
    private EDuckType duckType = EDuckType.End;
    private readonly List<Renderer> listRenderers = new();

    private void Awake()
    {
        Spawn();
    }  
    
    public virtual void Spawn()
    {
        duckType = GetComponentInParent<DuckAbility>().GetDuckType();
        listRenderers.Clear();
        curDuckHouseId = -1;
        var renderers = duckMeshSetter.GetAllRenderers();
        AddRenderers(renderers);
    }    
      
    public void ActiveDuckRenderer()
    {
        foreach (var r in listRenderers)
        {
            r.enabled = true;
        }
        duckHpBar.Active();
    }
    public void DisableDuckRenderer()
    {
        foreach (var r in listRenderers)
        { 
            r.enabled = false; 
        } 
        duckHpBar.Disable();
    }
    public void AddRenderers(Renderer[] arr)
    {
        if (arr == null) 
            return;

        foreach (var r in arr)
        {
            if (r == null) 
                continue;
              
            if (!listRenderers.Contains(r))
                listRenderers.Add(r);
        } 
    }
    public void AddRenderer(Renderer r)
    {
        if (r == null) return;

        if (!listRenderers.Contains(r))
            listRenderers.Add(r);
    }
    
    public void RemoveRenderer(Renderer r)
    {
        if (r == null) return;

        if (listRenderers.Remove(r))
        {
            // 리스트에서 제거된 경우 → 항상 활성화
            r.enabled = true;
        }
    }
    public void RemoveRenderers(Renderer[] arr)
    {
        if (arr == null) return;
         
        foreach (var r in arr)
        {
            RemoveRenderer(r);
        }
    } 
     
    public int GetHouseId() { return curDuckHouseId; }
    public EDuckType GetDuckType() { return duckType; }  
     
    public void CacheDuckHouseId(int _id)
    {
        curDuckHouseId = _id;
    }
    public void DisableDuckHouseId()
    {
        curDuckHouseId = -1;
    }
}
