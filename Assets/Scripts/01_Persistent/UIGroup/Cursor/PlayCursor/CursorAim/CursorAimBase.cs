using Unity.VisualScripting;
using UnityEngine;
 
public class CursorAimBase : MonoBehaviour, ICursorRecoil
{
    [Header("Reticle Settings")]
    [SerializeField] private float blendDuration = 0.2f;
    [SerializeField] protected float minRange = 30f;
    [SerializeField] protected float maxRange = 120f;
     
    private bool isBlending = false;
    private float blendTime = 0f;
    private float currentAlpha = 0.3f;


    protected ShotInfo cachedShotInfo;


    protected float accAlpha = 0f;
    protected float accRange = 0f; // Mathf.Lerp(120f, 15f, accAlpha); 
     
    protected virtual void Awake()
    { 
    }   
    protected virtual void Start()
    {
    }  
    protected virtual void Update()
    {
        UpdateBlendImage();
        UpdateGather(); 
    }
     
    public void ActiveAimCursor()
    { 
        gameObject.SetActive(true);
         
        accAlpha = 0f;
        accRange = 0f;
        isBlending = true;
        blendTime = 0f;
        SetImageAlpha(0.3f); 
    }
    public void Disable()
    {
        gameObject.SetActive(false);
         
    }


    public void CacheShotInfo(ShotInfo _shotInfo)
    {
        cachedShotInfo = _shotInfo;
    } 

    protected virtual void UpdateGather()
    { 
        // 0~1
        float accAlpha = cachedShotInfo.accControl / 100f;
        accRange = Mathf.Lerp(maxRange, minRange, accAlpha);
    }
    protected virtual void UpdateBlendImage()
    {
        if (!isBlending)
            return;

        blendTime += Time.deltaTime;
        float t = Mathf.Clamp01(blendTime / blendDuration);
        currentAlpha = Mathf.Lerp(0.3f, 1f, t);
        SetImageAlpha(currentAlpha);
        if (t >= 1f)
            isBlending = false;
    }
    protected virtual void SetImageAlpha(float _alpha)
    {
         
    }
   
}
