using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PlayerCircleSight : MonoBehaviour
{
    private Mesh mesh;

    [SerializeField] private Transform playerTransform;
    [SerializeField] private int segments = 180;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private LayerMask obstacleLayerMask;
    [SerializeField] private float checkDetectedTime;
     
    private float radius;

    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;

    private float[] rayAngles;
    private float[] rayDistances;

    private float detectedTime;

    private void Awake()
    {
        mesh = new Mesh();
        mesh.MarkDynamic();
        GetComponent<MeshFilter>().mesh = mesh;

        vertices = new Vector3[segments + 2];
        uvs = new Vector2[segments + 2];
        triangles = new int[segments * 3];

        rayAngles = new float[segments + 1];
        rayDistances = new float[segments + 1];

        for (int i = 0; i <= segments; i++)
            rayAngles[i] = (float)i / segments * Mathf.PI * 2f;

        BuildVisibilityMesh();
    }

    private void Update()
    {
        BuildVisibilityMesh();

        detectedTime += Time.deltaTime;
        if (detectedTime < checkDetectedTime)
            return;

        detectedTime = 0f;
        DetectEnemiesByVisibilityPolygon();
    }

    public void SetRadius(float _radius)
    {
        radius = _radius;
    }

    private void BuildVisibilityMesh()
    {
        Vector3 origin = playerTransform.position + Vector3.up * 0.4f;
        transform.position = origin;
        // transform.rotation = Quaternion.identity;  
            
        vertices[0] = Vector3.zero; 
        uvs[0] = new Vector2(0.5f, 0.5f); 

        for (int i = 0; i <= segments; i++)
        {
            float angle = rayAngles[i];
            Vector3 dir = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));

            float dist = radius;
            if (Physics.Raycast(origin, dir, out RaycastHit hit, radius, obstacleLayerMask))
                dist = hit.distance;

            rayDistances[i] = dist;

            vertices[i + 1] = dir * dist;
            uvs[i + 1] = new Vector2(dir.x * 0.5f + 0.5f, dir.z * 0.5f + 0.5f);
        }

        int t = 0;
        for (int i = 1; i <= segments; i++)
        {
            triangles[t++] = 0;
            triangles[t++] = i;
            triangles[t++] = (i == segments) ? 1 : i + 1;
        } 

        mesh.Clear(false);
        mesh.vertices = vertices;
        mesh.triangles = triangles; 
        mesh.uv = uvs;
    }

    private void DetectEnemiesByVisibilityPolygon()
    {
        Vector3 origin = playerTransform.position + Vector3.up * 0.4f;
         
        Collider[] hits = Physics.OverlapSphere(origin, radius, enemyLayerMask);
        if (hits.Length == 0)
            return;

        foreach (var h in hits)
        {
            Vector3 pos = h.transform.position;
            pos.y = origin.y;

            Vector3 dir = pos - origin;
            float dist = dir.magnitude;
            if (dist <= 0.01f || dist > radius)
                continue;
             
            float angle = Mathf.Atan2(dir.z, dir.x);
            if (angle < 0f)
                angle += Mathf.PI * 2f;
             
            float fIndex = angle / (Mathf.PI * 2f) * segments;
            int i0 = Mathf.FloorToInt(fIndex);
            int i1 = i0 + 1;
            float t = fIndex - i0;

            if (i1 > segments) 
                i1 = segments; 

            float maxDist = Mathf.Lerp(rayDistances[i0], rayDistances[i1], t); 
            if (dist > maxDist + 0.05f)
                continue;

            if (Physics.Raycast(origin, dir.normalized, dist, obstacleLayerMask))
                continue;

            if (h.GetComponentInParent<AiDetected>() is { } detected)
                detected.ActiveAiRenderer();
        }
    }
}
