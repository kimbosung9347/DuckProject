using UnityEngine;

public class DefaultReticleCursor : MonoBehaviour
{
    [Header("Reticle UI")] 
    [SerializeField] private RectTransform leftRect;
    [SerializeField] private RectTransform rightRect;
    [SerializeField] private RectTransform upRect;
    [SerializeField] private RectTransform downRect;

    private ShotInfo cachedShotInfo;
     
    private void Awake()
    { 
    }
    private void Start()
    { 
    }
    private void Update()
    { 
        if (cachedShotInfo == null)
            return;

        float clampedAcc = Mathf.Clamp01(cachedShotInfo.accControl / 100f);
        float accRange = Mathf.Lerp(100f, 15f, clampedAcc);
        RenewAnchoredPos(accRange);      
    }    
     
    public void CacheShotInfo(ShotInfo _shotInfo)
    {
        cachedShotInfo = _shotInfo;
    }
    
    private void RenewAnchoredPos(float _offset)
    {
        leftRect.anchoredPosition = new Vector2(-_offset, 0);
        rightRect.anchoredPosition = new Vector2(_offset, 0);
        upRect.anchoredPosition = new Vector2(0, _offset);
        downRect.anchoredPosition = new Vector2(0, -_offset);
    }
}
