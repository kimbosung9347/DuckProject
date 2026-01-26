using TMPro;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.UI;

public class StatCanvas : MonoBehaviour
{
    [SerializeField] private Color baseColor = Color.white;
    [SerializeField] private Color fillColor = Color.red;
    [SerializeField] private float radius = 15f;
    [SerializeField] private Vector2 rectSize = new Vector2(700, 100);
      
    [SerializeField] private TextMeshProUGUI textPlayerLevel;
    [SerializeField] private TextMeshProUGUI textPlayerExp;
    [SerializeField] private Image gaueImage;  

    [SerializeField] private TextMeshProUGUI textMoveSpeed;
    [SerializeField] private TextMeshProUGUI textRunSpeed;
    [SerializeField] private TextMeshProUGUI textBodyArmor;
    [SerializeField] private TextMeshProUGUI textHeadArmor;
    [SerializeField] private TextMeshProUGUI textAccControl;
    [SerializeField] private TextMeshProUGUI textRecoilControl;
    [SerializeField] private TextMeshProUGUI textRecoilReocover;
    [SerializeField] private TextMeshProUGUI textAttRange;
    [SerializeField] private TextMeshProUGUI textToAimSpeed;
    [SerializeField] private TextMeshProUGUI textCapacitySize;
    [SerializeField] private TextMeshProUGUI textMaxWeight;
    [SerializeField] private TextMeshProUGUI textMaxHp;
    [SerializeField] private TextMeshProUGUI textMaxMp;
    [SerializeField] private TextMeshProUGUI textHpRecover;
    [SerializeField] private TextMeshProUGUI textMpRecover;

    private LocoInfo cachedLocoInfo;
    private ShotInfo cachedShotInfo;
    private StatInfo cachedStatInfo;
    private ArmorInfo cachedArmorInfo;
    private CapacityInfo cachedCapacityInfo;
     
    private float time = 0f;

    private void Awake()
    { 
        InitGuageMaterial();
    }
    private void Start()
    {
        gameObject.SetActive(false); 
    }
    private void Update()
    {
        time += Time.deltaTime;
        if (time < 1f)
            return; 

        time = 0f;
        UpdateStatText();
    }
    private void OnEnable()
    {
        time = 0f;
        UpdateStatText();
    }

    public void CachePlayerInfo(
        LocoInfo _locoInfo,
        ShotInfo _shotInfo,
        StatInfo _statInfo,
        ArmorInfo _armorInfo,
        CapacityInfo _capacityInfo)
    {
        cachedLocoInfo = _locoInfo;
        cachedShotInfo = _shotInfo;
        cachedStatInfo = _statInfo;
        cachedArmorInfo = _armorInfo;
        cachedCapacityInfo = _capacityInfo;
    }

    private void UpdateStatText()
    {
        if (cachedLocoInfo == null || cachedShotInfo == null || cachedStatInfo == null || cachedArmorInfo == null || cachedCapacityInfo == null)
            return;
         
        var playerGrow = GameInstance.Instance.PLAYER_GetPlayerGrow();
        int intPlayerLevel = playerGrow.GetLevel();
        float curExp = playerGrow.GetExp();
        int maxExp = GameInstance.Instance.TABLE_GetRequireExp(intPlayerLevel);
          
        textPlayerLevel.text = $"LV. {intPlayerLevel}";
        textPlayerExp.text = $"{(int)curExp} / {maxExp}";
        RenewGuageImage(curExp / maxExp);
          
        textMoveSpeed.text = cachedLocoInfo.moveInfo.moveSpeed.ToString();
        textRunSpeed.text = (cachedLocoInfo.moveInfo.moveSpeed + cachedLocoInfo.moveInfo.sprintSpeed).ToString();
        textBodyArmor.text = cachedArmorInfo.GetDefense(false).ToString();
        textHeadArmor.text = cachedArmorInfo.GetDefense(true).ToString();

        textAccControl.text = cachedShotInfo.accControl.ToString();
        textRecoilControl.text = cachedShotInfo.recoilControl.ToString();
        textRecoilReocover.text = cachedShotInfo.recoilRecovery.ToString();
        textAttRange.text = cachedShotInfo.attackRange.ToString();
        textToAimSpeed.text = cachedShotInfo.toAimSpeed.ToString();
        
        textCapacitySize.text = cachedCapacityInfo.capacitySize.ToString();
        textMaxWeight.text = cachedCapacityInfo.maxWeight.ToString();

        textMaxHp.text = cachedStatInfo.maxHp.ToString();
        textMaxMp.text = cachedStatInfo.maxMp.ToString();
        textHpRecover.text = cachedStatInfo.hpRecovery.ToString();
        textMpRecover.text = cachedStatInfo.mpRecovery.ToString();
    } 
    private void RenewGuageImage(float _ratio)
    { 
        var mat = gaueImage.material; 
        mat.SetFloat("_Progress", _ratio);
    }
     
    private void InitGuageMaterial()
    {
        gaueImage.material = new Material(gaueImage.material);
        var mat = gaueImage.material;
         
        mat.SetColor("_BaseColor", baseColor);
        mat.SetColor("_FillColor", fillColor);
        mat.SetFloat("_Radius", radius);
        mat.SetVector("_RectSize", rectSize);
    }
}
