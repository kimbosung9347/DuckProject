using UnityEngine;

public class DisableOnPlay : MonoBehaviour
{ 
    private void Awake()
    { 
        gameObject.SetActive(false);
    }
}  
