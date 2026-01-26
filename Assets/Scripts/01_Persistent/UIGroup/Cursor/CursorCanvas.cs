using System.Collections.Generic;
using UnityEngine;
 
 
[System.Serializable]
public struct FAimCursorPair
{  
    public EItemID scopeId;
    public CursorAimBase cursor;
}

public class CursorCanvas : MonoBehaviour
{
    [SerializeField] private CursorInUI uiCursor;
    [SerializeField] private CursorInPlay playCursor;
      
    public CursorInUI GetUICursor()
    {
        return uiCursor;
    }
    public CursorInPlay GetPlayCursor()
    {
        return playCursor;
    }
}
