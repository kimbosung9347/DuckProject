using UnityEngine;
using UnityEngine.InputSystem;

public class BackgroundMove : MonoBehaviour
{
    [SerializeField] private float moveXRange = 8f; 
    [SerializeField] private float moveYRange = 5f;
    [SerializeField] private float moveSpeed = 5f;

    private Vector3 startLocation;

    void Start()
    {
        startLocation = transform.position;
    }

    void Update()
    {
        MoveByMousePos();
    } 

    private void MoveByMousePos()
    {
        if (Mouse.current == null)
            return;

        // 마우스 스크린 위치
        Vector2 mousePos = Mouse.current.position.ReadValue();

        // 화면 크기
        float screenW = Screen.width;
        float screenH = Screen.height;

        // -1 ~ 1 정규화
        float normalizedX = (mousePos.x / screenW - 0.5f) * 2f;
        float normalizedY = (mousePos.y / screenH - 0.5f) * 2f;

        // 목표 위치 계산 (좌우, 상하 반대)
        Vector3 targetPos = startLocation + new Vector3(-normalizedX * moveXRange, -normalizedY * moveYRange, 0);

        // 부드럽게 이동
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * moveSpeed);
    }
}
