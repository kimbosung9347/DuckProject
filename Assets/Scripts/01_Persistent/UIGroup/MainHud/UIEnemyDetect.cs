using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어 화면 위치를 중심으로
/// 고정 반경(hudRadius)에 적 방향 HUD를 표시하는 UI
/// - 위치: 플레이어 화면 위치 + (방향 * 고정 반경)
/// - 거리: 월드 평면 거리 → 선명도에만 사용
/// - 플레이어 회전: 완전히 무시
/// </summary>
public class UIEnemyDetect : MonoBehaviour
{
    [Header("Ref")]
    [SerializeField] private RectTransform arcRoot; // 회전/이동 기준
    [SerializeField] private Image arcImage;        // 셰이더 적용 이미지

    [Header("Game Distance (for visual only)")]
    [SerializeField] private float maxDistance = 15f; // 선명도 기준 거리
     
    [Header("HUD")]
    [SerializeField] private float hudRadius = 150f;  // HUD 고정 반경

    [Header("Life")]
    [SerializeField] private float lifeTime = 5f;     // 표시 시간

    private Transform playerTransform;
    private Transform enemyTransform;
    private Material arcMat;
    private float timer;

    private Canvas cachedCanvas;
    private RectTransform canvasRect;
      
    private void Awake()
    {
        // UI 머티리얼 인스턴스화 (다중 HUD 안전)
        arcImage.material = new Material(arcImage.material);
        arcMat = arcImage.material;
         
        // 이 UI가 속한 Canvas 캐싱
        cachedCanvas = GetComponentInParent<Canvas>();
        canvasRect = cachedCanvas.transform as RectTransform;
    }

    private void Update()
    {
        // ===== 적 / 플레이어 유효성 =====
        if (!playerTransform || !enemyTransform)
        {
            Destroy(gameObject);
            return;
        } 
         
        if (!cachedCanvas)
        {
            Destroy(gameObject);
            return;
        }

        // ===== 수명 관리 =====
        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
            return;
        }

        Camera cam = Camera.main;
        if (!cam)
            return;

        // ======================================================
        // 1. 게임용 거리 (월드 평면 거리)
        //    → HUD 위치에는 사용 ❌
        //    → 선명도 연출에만 사용 ⭕
        // ======================================================
        Vector3 d = enemyTransform.position - playerTransform.position;
        d.y = 0f; // 높낮이 무시 (중요)
        float dist = d.magnitude;


        // ======================================================
        // 2. Viewport 좌표 (카메라 기준)
        // ======================================================
        Vector3 playerVP = cam.WorldToViewportPoint(playerTransform.position);
        Vector3 enemyVP = cam.WorldToViewportPoint(enemyTransform.position);
         
        // 카메라 뒤에 있으면 표시 안 함
        if (enemyVP.z <= 0f)
        {
            arcRoot.gameObject.SetActive(false);
            return;
        }

        // 플레이어 → 적 방향 (화면 기준)
        Vector2 vpDir = new Vector2(
            enemyVP.x - playerVP.x,
            enemyVP.y - playerVP.y
        ); 

        if (vpDir.sqrMagnitude < 0.0001f)
            return;

        // ======================================================
        // 3. 방향만 사용 (정규화)
        // ======================================================
        Vector2 dir = vpDir.normalized;

        // ======================================================
        // 4. 플레이어의 화면 위치를 UI 좌표로 변환
        // ======================================================
        Vector2 playerUIPos = new Vector2(
            (playerVP.x - 0.5f) * canvasRect.sizeDelta.x,
            (playerVP.y - 0.5f) * canvasRect.sizeDelta.y
        );

        // ======================================================
        // 5. 최종 HUD 위치
        //    = 플레이어 UI 위치 + (방향 * 고정 반경)
        // ======================================================
        arcRoot.gameObject.SetActive(true);
        arcRoot.anchoredPosition = playerUIPos + dir * hudRadius;

        // ======================================================
        // 6. HUD 회전
        //    - 위쪽이 0도 기준
        //    - 플레이어 회전 완전 무시
        // ======================================================
        float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
        arcRoot.localRotation = Quaternion.Euler(0f, 0f, -angle);

        // ======================================================
        // 7. 선명도 연출 (게임용 거리만 반영)
        // ======================================================
        float dist01 = Mathf.Clamp01(dist / maxDistance);
        arcMat.SetFloat("_Distance01", dist01);
    }

    public void Active(Transform _playerTransform, Transform _enemyTransform)
    {
        playerTransform = _playerTransform;
        enemyTransform = _enemyTransform;
        timer = 0f;
    }
}
