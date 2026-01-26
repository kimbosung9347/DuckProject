//using UnityEngine;

//[DisallowMultipleComponent]
//public class MovementBounds : MonoBehaviour
//{
//    [SerializeField] private BoxCollider boundsCollider;

//    [Header("Minimap Bake")] 
//    public LayerMask minimapLayerMask;
//    public float heightPadding = 10f;
     
//    public Bounds Bounds => boundsCollider.bounds;

//    private void Reset()
//    {
//        boundsCollider = GetComponent<BoxCollider>();
//        if (!boundsCollider)
//            boundsCollider = gameObject.AddComponent<BoxCollider>();

//        boundsCollider.isTrigger = true; 
//    }

//    public bool Contains(Vector3 worldPos) 
//    {
//        return Bounds.Contains(worldPos);
//    }

//#if UNITY_EDITOR
//    private void OnDrawGizmos()
//    {
//        if (!boundsCollider) return;

//        Gizmos.color = new Color(0f, 1f, 0f, 0.6f);
//        Gizmos.DrawWireCube(Bounds.center, Bounds.size);
//    }
//#endif
//}
