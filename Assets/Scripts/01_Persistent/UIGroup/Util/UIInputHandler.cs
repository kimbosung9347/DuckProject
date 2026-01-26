using UnityEngine;

public enum EUIInputKey
{
    ESC,
       
    End,
} 

public abstract class UIInputHandler : MonoBehaviour
{
    public virtual void OnInput(EUIInputKey _key) { }
}
