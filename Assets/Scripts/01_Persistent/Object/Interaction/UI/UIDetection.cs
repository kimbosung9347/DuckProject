using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct FWaveRange
{
    [Header("반경 설정")]
    [Range(0f, 1f)] public float minRadius;
    [Range(0f, 1f)] public float maxRadius;

    [Header("파동 설정")]
    [Range(0f, 10f)] public float minFrequency;
    [Range(0f, 10f)] public float maxFrequency;
     
    [Header("파동 스피드")]
    [Range(0f, 10f)] public float Speed;

    [Header("페이드 및 경계 설정")]
    [Range(0f, 1f)] public float minFadeRange;
    [Range(0f, 1f)] public float maxFadeRange;

    [Range(0f, 1f)] public float minFeather;
    [Range(0f, 1f)] public float maxFeather;
}

public class UIDetection : MonoBehaviour 
{
    [Header("UI References")]
    [SerializeField] private Image cacheWave;
    [SerializeField] private Image cachedCheck;
    [SerializeField] private FWaveRange waveRange;

    // Shader property IDs
    private static readonly int ID_radius = Shader.PropertyToID("_radius");
    private static readonly int ID_radiusFeather = Shader.PropertyToID("_radiusFeather");
    private static readonly int ID_freq = Shader.PropertyToID("_frequency");
    private static readonly int ID_speed = Shader.PropertyToID("_speed");
    private static readonly int ID_fadeRange = Shader.PropertyToID("_fadeRange");
    private static readonly int ID_isInteracting = Shader.PropertyToID("_isInteraction");
    private static readonly int ID_closeAlpha = Shader.PropertyToID("_closeAlpha");
     
    private Material instWaveMat;
    private float maxDistance;
    private float distance;
    private bool isAllSearch = false;

    private void Awake()
    {
        Material originalMat = cacheWave.material;
        instWaveMat = new Material(originalMat);
        cacheWave.material = instWaveMat;
        instWaveMat.SetFloat(ID_speed, waveRange.Speed);
        cachedCheck.gameObject.SetActive(false); 
    } 
    private void Update()
    { 
        if (isAllSearch)
            return;
         
        UpdateDetecting();
    }
      
    public void ChangeDetection()
    {
        if (isAllSearch)
        {
            cachedCheck.gameObject.SetActive(true);
            cacheWave.gameObject.SetActive(false);
        }
         
        else
        {
            cachedCheck.gameObject.SetActive(false);
            cacheWave.gameObject.SetActive(true);
            instWaveMat.SetFloat(ID_isInteracting, 0f);
            instWaveMat.SetFloat(ID_closeAlpha, 1f);
        } 
    } 
    public void ChangeInteraction()
    {
        if (isAllSearch)
        {
            cacheWave.gameObject.SetActive(false);
            cachedCheck.gameObject.SetActive(true);
        } 

        else
        {
            cacheWave.gameObject.SetActive(true);
            cachedCheck.gameObject.SetActive(false);
            instWaveMat.SetFloat(ID_isInteracting, 1f);
        }
    }
     
    public void SetMaxDistance(float _distance)
    {
        maxDistance = _distance;
    }
    public void SetDistance(float _distance)
    {
        distance = _distance;
    }
    public void ComplateAllSearch()
    {
        isAllSearch = true; 
    } 

    private void UpdateDetecting()
    {
        if (maxDistance <= 0f)
            return;
         
        // 거리 → 0~1 정규화
        float normalized = Mathf.InverseLerp(0f, maxDistance, distance);

        // 거리 기반 보간
        float radius = Mathf.Lerp(waveRange.minRadius, waveRange.maxRadius, normalized);
        float freq = Mathf.Lerp(waveRange.minFrequency, waveRange.maxFrequency, normalized);
        float fade = Mathf.Lerp(waveRange.minFadeRange, waveRange.maxFadeRange, normalized);
        float feather = Mathf.Lerp(waveRange.minFeather, waveRange.maxFeather, normalized);

        float fadeStart = maxDistance * 0.7f; 
        float alphaNormalized = Mathf.InverseLerp(maxDistance, fadeStart, distance);
        float alpha = Mathf.Clamp01(alphaNormalized);
          
        // 셰이더 적용
        instWaveMat.SetFloat(ID_radius, radius);
        instWaveMat.SetFloat(ID_radiusFeather, feather);
        instWaveMat.SetFloat(ID_freq, freq);
        instWaveMat.SetFloat(ID_fadeRange, fade);
        instWaveMat.SetFloat(ID_closeAlpha, alpha);
           
        //// 투명해지게 
        //float uiAlpha = (distance <= fadeStart) ? 1f : Mathf.InverseLerp(maxDistance, fadeStart, distance);
        //Color c = cacheWave.color;
        //c.a = uiAlpha;
        //cacheWave.color = c;
    }
}
