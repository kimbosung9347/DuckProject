using UnityEngine;
using System.Collections.Generic;

public class FallingLeaves : MonoBehaviour
{
    public enum ELeafType
    {
        Straight,
        Sway,
        Spin
    }

    public enum EDropDirection
    {
        Down,
        LeftDown,
        RightDown
    }

    [System.Serializable]
    public struct Range
    {
        public float min;
        public float max;
        public float Random() => UnityEngine.Random.Range(min, max);
        public float Center => (min + max) * 0.5f;
        public float Size => Mathf.Abs(max - min);
    }

    [Header("Prefab / Pool")]
    [SerializeField] private GameObject leafPrefab;
    [SerializeField] private int poolSize = 40;

    [Header("Spawn Area (Local)")]
    [SerializeField] private Range spawnX;
    [SerializeField] private Range spawnY;
    [SerializeField] private Range spawnZ;

    [Header("Scale")]
    [SerializeField] private Range scaleRange;

    [Header("Fall Speed")]
    [SerializeField] private Range fallSpeed;

    [Header("Rotation Speed")]
    [SerializeField] private Range swayRotSpeed;
    [SerializeField] private Range spinRotSpeed;

    [Header("Despawn")]
    [SerializeField] private float despawnY = -3f;

    [Header("Gizmos")]
    [SerializeField] private Color spawnAreaColor = new Color(0f, 1f, 0f, 0.2f);
    [SerializeField] private bool drawGizmos = true;

    class Leaf
    {
        public Transform tf;
        public Vector3 velocity;
        public Vector3 angularVelocity;
    }

    private readonly Queue<Leaf> pool = new();
    private readonly List<Leaf> active = new();

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            var go = Instantiate(leafPrefab, transform);
            go.SetActive(false);
            pool.Enqueue(new Leaf { tf = go.transform });
        }
    }

    private void OnEnable()
    {
        ResetAll();
    }

    private void Update()
    {
        for (int i = active.Count - 1; i >= 0; i--)
        {
            var l = active[i];

            l.tf.localPosition += l.velocity * Time.deltaTime;
            l.tf.Rotate(l.angularVelocity * Time.deltaTime, Space.Self);

            if (l.tf.localPosition.y <= despawnY)
            {
                Recycle(i);
            }
        }
    }

    private void ResetAll()
    {
        while (active.Count > 0)
        {
            var l = active[^1];
            active.RemoveAt(active.Count - 1);
            l.tf.gameObject.SetActive(false);
            pool.Enqueue(l);
        }

        for (int i = 0; i < poolSize; i++)
        {
            Spawn();
        }
    }

    private void Spawn()
    {
        if (pool.Count == 0)
            return;

        var l = pool.Dequeue();
        var tf = l.tf;

        tf.gameObject.SetActive(true);
        tf.localPosition = new Vector3(
            spawnX.Random(),
            spawnY.Random(),
            spawnZ.Random()
        );

        float scale = scaleRange.Random();
        tf.localScale = new Vector3(tf.localScale.x, 0.005f, tf.localScale.z);
        tf.localRotation = Quaternion.identity; 
         
        InitLeaf(l);
        active.Add(l);
    }

    private void InitLeaf(Leaf l)
    {
        var type = (ELeafType)Random.Range(0, 3);
        var dir = (EDropDirection)Random.Range(0, 3);

        Vector3 dropDir = dir switch
        {
            EDropDirection.Down => Vector3.down,
            EDropDirection.LeftDown => (Vector3.down + Vector3.left).normalized,
            EDropDirection.RightDown => (Vector3.down + Vector3.right).normalized,
            _ => Vector3.down
        };

        l.velocity = dropDir * fallSpeed.Random();

        l.angularVelocity = type switch
        {
            ELeafType.Straight => Vector3.zero,
            ELeafType.Sway => new Vector3(0f, 0f, swayRotSpeed.Random()),
            ELeafType.Spin => Random.onUnitSphere * spinRotSpeed.Random(),
            _ => Vector3.zero
        };
    }

    private void Recycle(int index)
    {
        var l = active[index];
        active.RemoveAt(index);

        l.tf.gameObject.SetActive(false);
        pool.Enqueue(l);

        Spawn();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos)
            return;

        Gizmos.color = spawnAreaColor;

        Vector3 localCenter = new Vector3(
            spawnX.Center,
            spawnY.Center,
            spawnZ.Center
        );

        Vector3 size = new Vector3(
            spawnX.Size,
            spawnY.Size,
            spawnZ.Size
        );
         
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.DrawCube(localCenter, size);
        Gizmos.color = new Color(spawnAreaColor.r, spawnAreaColor.g, spawnAreaColor.b, 1f);
        Gizmos.DrawWireCube(localCenter, size);

        Gizmos.matrix = oldMatrix;
    }
#endif
}
