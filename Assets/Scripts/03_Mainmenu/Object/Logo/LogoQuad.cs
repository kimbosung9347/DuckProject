using UnityEngine;

public enum ELogoState
{
    None = 0,
    Appear,
    Disable,
}
 
public class LogoQuad : MonoBehaviour
{
    [SerializeField] private float blendTime = 0.5f;


    private Renderer cachedRenderer; 
    private Material cachedMaterial;

    private ELogoState state = ELogoState.None;
    private float stateTime = 0f;
    
    private const float MIN_ALPHA = 0f;
    private const float MAX_ALPHA = 1f;

    private void Awake()
    {
        cachedMaterial = GetComponent<Renderer>().material;
    }  
    public void Active(ELogoState _state)
    {
        if (state == _state) 
            return;
         
        gameObject.SetActive(true);
        state = _state;
        stateTime = 0f;

        switch (state)
        {
            case ELogoState.Appear:
                SetAlpha(MIN_ALPHA);
                break;

            case ELogoState.Disable:
                SetAlpha(MAX_ALPHA);
                break;
        }
    }

    void Update()
    {
        switch (state)
        {
            case ELogoState.Appear:
                UpdateAppear();
                break;

            case ELogoState.Disable:
                UpdateDisable();
                break;
        }
    }

    private void UpdateAppear()
    {
        stateTime += Time.deltaTime;
        float t = Mathf.Clamp01(stateTime / blendTime);

        // 알파 0 → 1
        SetAlpha(Mathf.Lerp(MIN_ALPHA, MAX_ALPHA, t));

        if (t >= 1f)
            state = ELogoState.None;
    }
    private void UpdateDisable()
    {
        stateTime += Time.deltaTime;
        float t = Mathf.Clamp01(stateTime / blendTime);
         
        // 알파 1 → 0
        SetAlpha(Mathf.Lerp(MAX_ALPHA, MIN_ALPHA, t));

        if (t >= 1f)
        {
            state = ELogoState.None;
            gameObject.SetActive(false);
        }
    }
    private void SetAlpha(float a)
    {
        if (cachedMaterial == null)
            return; 

        Color c = cachedMaterial.color;
        c.a = a;
        cachedMaterial.color = c;
    }
}
