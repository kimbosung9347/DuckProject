using System.Collections.Generic;
using UnityEngine;
public enum ECursorState 
{
    Default,
    Aim,
    Reload,
    End,
}
public class CursorInPlay : MonoBehaviour
{ 
    [Header("에임 커서 매핑")]
    [SerializeField] private List<FAimCursorPair> aimCursorList;
    [SerializeField] private CursorHit cachedCursorHit;
    [SerializeField] private RectTransform cachedDefaultBase;   // 기본 상태 커서
    [SerializeField] private CursorRelad cachedReload;          // 장전 상태 커서
     
    private Dictionary<EItemID, CursorAimBase> hashPlayAimCursor = new(); 
     
    private CursorAimBase curAimCursor;
    private RectTransform cachedRectTransform;
    private ECursorState cursorState = ECursorState.End;
     
    protected virtual void Awake()
    {
        cachedRectTransform = GetComponent<RectTransform>();
        cachedDefaultBase.SetParent(cachedRectTransform, false);
        BuildDictionary();
        ChangeCursorState(ECursorState.Default);
    }
      
    public void CacheShotInfo(ShotInfo _shotInfo)
    {
        foreach (var pair in hashPlayAimCursor)
        {
            pair.Value.CacheShotInfo(_shotInfo);
        }
            
        cachedDefaultBase.gameObject.GetComponent<DefaultReticleCursor>().CacheShotInfo(_shotInfo);
    }  
    public void SetAimCursor(EItemID _scopeId)
    { 
        curAimCursor = hashPlayAimCursor[_scopeId];
    }  
    public void ChangeCursorState(ECursorState _state)
    {
        if (cursorState == _state)
            return;
         
        cursorState = _state;

        switch (_state)
        { 
            case ECursorState.Default:
            {
                cachedDefaultBase.gameObject.SetActive(true);
                cachedReload.gameObject.SetActive(false);
                curAimCursor?.Disable();
            }   
            break; 

            case ECursorState.Aim:
            {
                cachedDefaultBase.gameObject.SetActive(false);
                cachedReload.gameObject.SetActive(false);
                curAimCursor?.ActiveAimCursor();
            }  
            break;
                 
            case ECursorState.Reload:
            {
                cachedReload.gameObject.SetActive(true);
                cachedReload.RenewGauge(0f);
                cachedDefaultBase.gameObject.SetActive(false);
                curAimCursor?.Disable();
            }     
            break;
        }
    } 
     
    public void ActiveCursorHit(ECursorHitState _state)
    {
        cachedCursorHit.Active(_state);
    }   
      
    public void RenewMousePos(Vector2 _mousePos) 
    {
        cachedRectTransform.position = _mousePos;
    }
    public void RenewReloadGauge(float _ratio)
    {
        cachedReload.RenewGauge(_ratio);
    } 

    private void BuildDictionary()
    {
        foreach (var pair in aimCursorList)
        {
            if (pair.cursor == null) 
                continue;

            if (!hashPlayAimCursor.ContainsKey(pair.scopeId))
            {
                hashPlayAimCursor.Add(pair.scopeId, pair.cursor);
                pair.cursor.Disable();
            } 
        }
         
        aimCursorList.Clear();
    }
}    
 