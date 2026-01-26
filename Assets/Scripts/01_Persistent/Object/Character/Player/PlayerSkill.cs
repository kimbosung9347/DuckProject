using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    [Header("적탐지")]
    [SerializeField] private float radius = 10f;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private float detectorCooldown = 10f;

    [Header("구르기")]
    [SerializeField] private float rollCooldown = 1f;

    private PlayerUIController cachedUIController;
    private PlayerBuff cachedBuff;
    private PlayerStat cachedStat;
    private DuckLocomotion cachedLoco;

    // 적탐지
    private bool isActiveDetector;
    private float lastDetectTime = -999f;
    private readonly Collider[] buffer = new Collider[16];
     
    // 구르기
    private bool isActiveRoll;
    private float lastRollTime = -999f;

    private void Awake()
    {
        cachedUIController = GetComponent<PlayerUIController>();
        cachedBuff = GetComponent<PlayerBuff>();
        cachedLoco = GetComponent<DuckLocomotion>();
        cachedStat = GetComponent<PlayerStat>(); 
    }

    private void Start()
    {
        cachedUIController.HUD_ActiveDetector(false);
        cachedUIController.HUD_ActiveRoll(false); 
    }

    private void Update()
    {
        UpdateDetectorCooldownUI();
        UpdateRollCooldownUI();
    }

    // =====================
    // Cooldown UI 갱신
    // =====================

    private void UpdateDetectorCooldownUI()
    {
        if (!isActiveDetector)
            return;

        float elapsed = Time.time - lastDetectTime;
        float t = Mathf.Clamp01(elapsed / detectorCooldown);
        float remain = Mathf.Max(detectorCooldown - elapsed, 0f);

        cachedUIController.HUD_RenewDetectorCoolTime(remain, detectorCooldown);

        if (t >= 1f)
        { 
            isActiveDetector = false;
            cachedUIController.HUD_ActiveDetector(false);
        }
    }
    private void UpdateRollCooldownUI()
    {
        if (!isActiveRoll)
            return;

        float elapsed = Time.time - lastRollTime;
        float t = Mathf.Clamp01(elapsed / rollCooldown);
        float remain = Mathf.Max(rollCooldown - elapsed, 0f);

        cachedUIController.HUD_RenewRollCoolTime(remain, rollCooldown);
         
        if (t >= 1f)
        {
            isActiveRoll = false;
            cachedUIController.HUD_ActiveRoll(false);
        } 
    } 

    // =====================
    // Can Use
    // =====================

    public bool CanDetector()
    {
        return Time.time >= lastDetectTime + detectorCooldown;
    }
    public bool CanRoll()
    {
        return Time.time >= lastRollTime + rollCooldown;
    }

    // =====================
    // Skill Execute
    // =====================
    public void DetetectorEnemy()
    {
        if (!CanDetector())
            return;

        isActiveDetector = true;
        lastDetectTime = Time.time;
        UpdateDetectorCooldownUI();
         
        ////////
        int count = Physics.OverlapSphereNonAlloc(
            transform.position,
            radius,
            buffer,
            enemyMask
        );
        for (int i = 0; i < count; i++)
            cachedUIController.HUD_MakeEnemyDetector(transform, buffer[i].transform);

        cachedBuff.InsertBuff(EBuffID.Small);
        
        // 쿨타임
        cachedUIController.HUD_ActiveDetector(true);
    }
    public void Roll()
    { 
        isActiveRoll = true;
        lastRollTime = Time.time;
        UpdateRollCooldownUI();
          
        ////////
        cachedLoco.DoRoll();
        cachedStat.ReduceStamina(DuckDefine.STAMINA_ROLL_VALUE);
         
        // 쿨타임 
        cachedUIController.HUD_ActiveRoll(true);
    }
}
