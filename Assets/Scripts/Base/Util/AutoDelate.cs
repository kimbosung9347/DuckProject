using UnityEngine;

public class AutoDelate : MonoBehaviour
{ 
    private void Start()
    {
        Destroy(gameObject, 1f);
    }
}
