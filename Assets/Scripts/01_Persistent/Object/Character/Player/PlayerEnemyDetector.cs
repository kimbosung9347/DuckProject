using UnityEngine;
 
public class PlayerEnemyDetector : MonoBehaviour
{
    [Header("Detect")]
    [SerializeField] private float radius = 10f;
    [SerializeField] private LayerMask enemyMask;

    [Header("Cooldown")]
    [SerializeField] private float cooldown = 10f;

    private PlayerUIController cachedUIController;
    private PlayerBuff cachedBuff;
    private float lastDetectTime = -999f;
    private readonly Collider[] buffer = new Collider[16];

    private void Awake()
    {
        cachedUIController = GetComponent<PlayerUIController>();
        cachedBuff = GetComponent<PlayerBuff>();
    }  

    public bool CanDetector()
    {
        return Time.time >= lastDetectTime + cooldown;
    }
      
    public void DetetectorEnemy()
    {
        lastDetectTime = Time.time;

        int count = Physics.OverlapSphereNonAlloc(
            transform.position,
            radius,
            buffer,
            enemyMask
        );
         
        for (int i = 0; i < count; i++)
        {
            cachedUIController.HUD_MakeEnemyDetector(transform, buffer[i].transform);
        }  

        cachedBuff.InsertBuff(EBuffID.Small);
    }
} 
