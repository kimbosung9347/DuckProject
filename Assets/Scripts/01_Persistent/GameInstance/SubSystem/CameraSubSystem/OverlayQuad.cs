using UnityEngine;

public class OverlayQuad : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float distance;

    void Start()
    {
        if (!targetCamera)
            targetCamera = Camera.main;

        FitQuadToCamera();
    } 

    void LateUpdate()
    {
        FitQuadToCamera();
    }
     
    private void FitQuadToCamera()
    { 
        Camera cam = targetCamera; 

        // near plane 바로 앞
        float dist = cam.nearClipPlane + distance;

        // near plane의 실제 화면 크기 계산
        float height = 2f * dist * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float width = height * cam.aspect;

        // 위치
        transform.position = cam.transform.position + cam.transform.forward * dist;

        // 회전 = 카메라와 완전히 동일해야 "투영 동일"함 
        transform.rotation = cam.transform.rotation;

        // 스케일
        transform.localScale = new Vector3(width, height, 1f);
    }
}
