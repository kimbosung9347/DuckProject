using UnityEngine;

public class WeaponMesh : MonoBehaviour
{ 
    [SerializeField] private Transform muzzleTrnasform;

    public Transform GetMuzzleTransform()
    {
        return muzzleTrnasform;
    } 
}
  