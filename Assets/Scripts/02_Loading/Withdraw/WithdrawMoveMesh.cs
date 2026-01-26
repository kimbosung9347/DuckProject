using UnityEngine;
using System.Collections.Generic;

public class WithdrawMoveMesh : MonoBehaviour
{
    [Header("Meshes")]
    [SerializeField] private List<GameObject> moveMesh = new();

    [Header("Move")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float resetX = -10f; // 이 x보다 왼쪽으로 가면 리스폰

    [Header("Respawn Area (Local)")]
    [SerializeField] private Vector2 respawnX = new(8f, 12f);
    [SerializeField] private Vector2 respawnY = new(-1f, 1f);
    [SerializeField] private Vector2 respawnZ = new(-1f, 1f);

    [Header("Gizmos")]
    [SerializeField] private bool drawGizmos = true;
    [SerializeField] private Color respawnAreaColor = new Color(1f, 0.6f, 0f, 0.2f);

    private void Update()
    {
        float dx = moveSpeed * Time.deltaTime;

        for (int i = 0; i < moveMesh.Count; i++)
        {
            var go = moveMesh[i];
            if (!go) continue;

            var tf = go.transform;

            // 왼쪽 이동 (Local)
            var p = tf.localPosition;
            p.x -= dx;
            tf.localPosition = p;

            // 리스폰
            if (p.x <= resetX)
            {
                tf.localPosition = new Vector3(
                    Random.Range(respawnX.x, respawnX.y),
                    Random.Range(respawnY.x, respawnY.y),
                    Random.Range(respawnZ.x, respawnZ.y)
                );
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        Vector3 localCenter = new Vector3(
            (respawnX.x + respawnX.y) * 0.5f,
            (respawnY.x + respawnY.y) * 0.5f,
            (respawnZ.x + respawnZ.y) * 0.5f
        );

        Vector3 size = new Vector3(
            Mathf.Abs(respawnX.y - respawnX.x),
            Mathf.Abs(respawnY.y - respawnY.x),
            Mathf.Abs(respawnZ.y - respawnZ.x)
        );

        var old = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = respawnAreaColor;
        Gizmos.DrawCube(localCenter, size);
        Gizmos.color = new Color(respawnAreaColor.r, respawnAreaColor.g, respawnAreaColor.b, 1f);
        Gizmos.DrawWireCube(localCenter, size);

        // reset 라인 표시 (X = resetX)
        Gizmos.DrawLine(
            new Vector3(resetX, localCenter.y - size.y * 0.5f, localCenter.z - size.z * 0.5f),
            new Vector3(resetX, localCenter.y + size.y * 0.5f, localCenter.z + size.z * 0.5f)
        );
        Gizmos.DrawLine(
            new Vector3(resetX, localCenter.y - size.y * 0.5f, localCenter.z + size.z * 0.5f),
            new Vector3(resetX, localCenter.y + size.y * 0.5f, localCenter.z - size.z * 0.5f)
        );

        Gizmos.matrix = old;
    }
#endif 
}
