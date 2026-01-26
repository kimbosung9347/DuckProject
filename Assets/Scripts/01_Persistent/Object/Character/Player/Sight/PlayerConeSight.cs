using UnityEngine;

public class PlayerConeSight : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;

    [SerializeField] private int rayCnt = 30;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private LayerMask obstacleLayerMask;
    [SerializeField] private float checkDetectedTime;
     
    private Mesh mesh;
    private float fov = 90f;
    private float viewDistance = 10f;

    // 각도별 시야 거리 테이블
    private float[] rayDistances;
    private float detectedTime = 0f;

    void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        rayDistances = new float[rayCnt + 1];
    }
     
    private void Update()
    {
        CreateMesh();

        detectedTime += Time.deltaTime;
        if (detectedTime > checkDetectedTime)
        {
            detectedTime = 0f;
            DetectEnemiesInFOV();
        }
    }
    private void CreateMesh()
    {
        Vector3[] vertices = new Vector3[rayCnt + 2];
        int[] triangles = new int[rayCnt * 3];
        Vector2[] uvs = new Vector2[rayCnt + 2];

        vertices[0] = Vector3.zero;
        uvs[0] = new Vector2(0.5f, 0.5f);

        float startAngle = -fov / 2f;
        float step = fov / rayCnt;

        int v = 1;
        int t = 0;

        Vector3 origin = playerTransform.position + Vector3.up * 0.2f;

        for (int i = 0; i <= rayCnt; i++)
        {
            float angle = startAngle + step * i;
            Vector3 worldDir = Quaternion.Euler(0, angle, 0) * playerTransform.forward;

            float dist = viewDistance;
            Vector3 hitPos = origin + worldDir * viewDistance;

            if (Physics.Raycast(origin, worldDir, out RaycastHit hit, viewDistance, obstacleLayerMask))
            {
                dist = hit.distance;

                if (hit.collider is BoxCollider bc)
                {
                    // hitPoint를 collider local space로 변환
                    Vector3 localHit = bc.transform.InverseTransformPoint(hit.point);
                    // 다시 world space로 변환 → 완전 정렬된 top edge point
                    hitPos = bc.transform.TransformPoint(localHit);
                } 
                else 
                {
                    // irregular collider → fallback
                    hitPos = hit.point;
                }
            }
             
            // 거리 저장
            rayDistances[i] = dist;

            // 로컬 좌표 변환
            Vector3 localVertex = transform.InverseTransformPoint(hitPos);
            vertices[v] = localVertex;

            // UV
            float u = (Mathf.Sin(angle) * 0.5f) + 0.5f;
            float w = (Mathf.Cos(angle) * 0.5f) + 0.5f;
            uvs[v] = new Vector2(u, w);

            if (i > 0)
            {
                triangles[t++] = 0;
                triangles[t++] = v - 1;
                triangles[t++] = v;
            }

            v++;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
    }

    public void SetFov(float _fov)
    {
        fov = _fov; 
    }
    public void SetDistance(float _distance)
    {
        viewDistance = _distance;
    }  

    // ===========
    //  탐지 로직 
    // ===========
    private void DetectEnemiesInFOV()
    {
        Vector3 origin = playerTransform.position + Vector3.up * 0.2f;

        Collider[] hits = Physics.OverlapSphere(origin, viewDistance, enemyLayerMask);
        if (hits.Length == 0)
            return;

        foreach (var col in hits)
        {
            Transform enemy = col.transform;
            Vector3 pos = enemy.position;
            pos.y = origin.y;

            Vector3 dir = pos - origin;
            float dist = dir.magnitude;

            float angle = Vector3.SignedAngle(playerTransform.forward, dir, Vector3.up);
            if (Mathf.Abs(angle) > fov * 0.5f)
                continue;

            float normalized = (angle + fov * 0.5f) / fov;
            float fIndex = normalized * rayCnt;

            int i0 = Mathf.FloorToInt(fIndex);
            int i1 = Mathf.Clamp(i0 + 1, 0, rayCnt);

            float t = fIndex - i0;
            float maxDist = Mathf.Lerp(rayDistances[i0], rayDistances[i1], t);

            if (dist > maxDist)
                continue;
             
            if (Physics.Raycast(origin, dir.normalized, out RaycastHit hit, dist, obstacleLayerMask))
                continue;
             
            AiDetected detected = enemy.GetComponentInParent<AiDetected>();
            detected?.ActiveAiRenderer();
        } 
    }
}
