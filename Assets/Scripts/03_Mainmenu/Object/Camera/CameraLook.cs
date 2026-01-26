using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLook : MonoBehaviour
{ 
    [SerializeField] private float rotationRange = 20f; 
    [SerializeField] private float rotationSpeed = 5f;  

    private Quaternion startRotation;
    
    void Start()
    {
        startRotation = transform.rotation;
    }  
    void Update()
    {
        LookByMousePos();
    }

    private void LookByMousePos()
    {

        // 마우스 스크린 위치 (Input System)
        Vector2 mousePos = Mouse.current.position.ReadValue();

        // 화면 크기 가져오기
        float screenW = Screen.width;
        float screenH = Screen.height;

        // 0~1로 정규화
        float normalizedX = (mousePos.x / screenW - 0.5f) * 2f;
        float normalizedY = (mousePos.y / screenH - 0.5f) * 2f;

        // 회전 각도 계산 (Y: 좌우, X: 상하)
        float yaw = Mathf.Clamp(normalizedX * rotationRange, -rotationRange, rotationRange);
        float pitch = Mathf.Clamp(-normalizedY * rotationRange, -rotationRange, rotationRange);

        // 목표 회전 
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, yaw * -1, pitch * -1);

        // 부드럽게 회전
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

    }
}  
