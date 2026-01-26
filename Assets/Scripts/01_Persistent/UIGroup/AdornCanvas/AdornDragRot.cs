using UnityEngine;
using UnityEngine.EventSystems;
 
public class AdornDragRot : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    [SerializeField] private Transform adronTarget;      // 회전시킬 캐릭터
    [SerializeField] private float rotateSpeed = 0.3f;

    private bool isDragging;
    private Vector2 lastPos;

    private void Awake()
    { 
        // if (adronTarget == null)
        // {
        // }
    }

    public void SetAdornTransform(Transform _adronTarget)
    {
        adronTarget = _adronTarget;
    }
      
    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        lastPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || !adronTarget)
            return;
         
        float deltaX = eventData.position.x - lastPos.x;
        adronTarget.Rotate(Vector3.up, -deltaX * rotateSpeed, Space.World);
         
        lastPos = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }
}
