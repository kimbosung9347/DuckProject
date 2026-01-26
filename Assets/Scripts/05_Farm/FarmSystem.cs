using UnityEngine;
using System.Collections;
using System.Collections.Generic;

enum ELightTime
{
    Day,
    Night
} 

public class FarmSystem : MonoBehaviour
{ 
    [Header("BGM")]
    [SerializeField] private AudioSource bgmSource; 
    [SerializeField] private AudioClip windBgm; 
    [SerializeField] private float dayBgmVolume = 0.6f;
    [SerializeField] private float nightBgmVolume = 0.3f;

    [Header("Directional Light")]
    [SerializeField] private Light DirectionLight;

    [Header("Day Light")]
    [SerializeField] private Color dayLightColor = Color.white;
    [SerializeField] private float dayLightIntensity = 1.55f;
    [SerializeField] private Vector3 dayLightEuler = new Vector3(60f, 192.49f, 0f);

    [Header("Night Light")]
    [SerializeField] private Color nightLightColor = new Color(0.85f, 0.85f, 0.85f);
    [SerializeField] private float nightLightIntensity = 0.25f;
    [SerializeField] private Vector3 nightLightEuler = new Vector3(60f, 30f, 0f);

    [Header("Light Transition")]
    [SerializeField] private float lightTransitionDuration = 3f;
    [SerializeField] private AnimationCurve lightTransitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Map / Minimap")]
    [SerializeField] private BoxCollider mapBoundary;
    [SerializeField] private Sprite minimapSprite;
    [SerializeField] private Transform houseSpawnTP;

    [Header("Shot")] 
    [SerializeField] private float shotNotifyRadius = 15f;
    [SerializeField] private LayerMask aiLayerMask;

    public Transform GetHouseSpawnTP() { return houseSpawnTP; }

    private readonly Dictionary<int, DuckHouse> houseById = new();
     
#if UNITY_EDITOR
    private ELightTime _prevTestTime;
#endif

    private void Awake()
    {
        CacheDuckHouses();

        // BGM 초기화
        if (bgmSource && windBgm)
        {
            bgmSource.clip = windBgm;
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
        }
    }
 
    private void OnDestroy()
    {
        var instance = GameInstance.Instance;
        var uiGroup = instance.UI_GetPersistentUIGroup(); 
        uiGroup.GetMinimapCanvas().DisableMinimapInfo(); 
    }
     
    public void Init(FarmData _data)
    {
        var gameInstance = GameInstance.Instance;

        // 플레이어 관련
        {
            var uiGroup = gameInstance.UI_GetPersistentUIGroup();
            var disableCanvas = uiGroup.GetDisableCanvas();
            disableCanvas.DisableInstant();
            uiGroup.GetMinimapCanvas().CacheMinimapInfo(mapBoundary, minimapSprite, "농장");
            gameInstance.PLAYER_GetPlayerRespawn().SpawnInFarm(houseSpawnTP);
        } 
         
        // 치킨 만들어주기 
        { 
            if (!_data.playerChicken.isEmpty)
            {
                var chicken = gameInstance.SPAWN_MakeChickPrefab(_data.playerChicken.pos, Quaternion.identity);
                var chickenBox = chicken.GetComponent<ItemBoxBase>();
                chickenBox.SetBoxName("누군가의 시체");
                chickenBox.GetComponent<DetectionTarget>().SetInteractionDesc("누군가의 시체");
                 
                foreach (var itemInfo in _data.playerChicken.items)
                {
                    if (itemInfo.IsEmpty())
                        continue;

                    var item = gameInstance.SPAWN_MakeItem(itemInfo);
                    chickenBox.AddItem(item, EItemBoxSlotState.Complate);
                }
                 
                // 다음 씬에서만 치킨이 생기게끔 
                _data.ClearChicken();
            } 
        }

        // 날씨 갱신
        {
            PlayerTime playerTime = GameInstance.Instance.PLAYER_GetPlayerTime();
            playerTime.RenewTimeAndStateInstant();
            var timeState = playerTime.GetTimeState();
            switch (timeState)
            {
                case ETimeState.Day:
                    {
                        ChangeDay(true);
                        PlayBgm(dayBgmVolume);
                    }
                    break;

                case ETimeState.Night:
                    {
                        ChangeNight(true);
                        PlayBgm(nightBgmVolume);
                    }
                    break;
            }
        }
    }   

    public DuckHouse GetHouse(int index)
    {
        houseById.TryGetValue(index, out var house);
        return house;
    }
    public void ChangeNight(bool isInstant)
    {
        StopAllCoroutines();

        if (isInstant)
        {
            ApplyLight(
                nightLightColor,
                nightLightIntensity,
                nightLightEuler
            );
            SetBgmVolume(nightBgmVolume);
        }
        else
        {
            StartCoroutine(
                TransitionLight(
                    DirectionLight.color,
                    DirectionLight.intensity,
                    DirectionLight.transform.eulerAngles,
                    nightLightColor,
                    nightLightIntensity,
                    nightLightEuler
                )
            );
            SetBgmVolume(nightBgmVolume);
        }
    }
    public void ChangeDay(bool isInstant)
    {
        StopAllCoroutines();

        if (isInstant)
        {
            ApplyLight(
                dayLightColor,
                dayLightIntensity,
                dayLightEuler
            );
            SetBgmVolume(dayBgmVolume);
        }
        else
        {
            StartCoroutine(
                TransitionLight(
                    DirectionLight.color,
                    DirectionLight.intensity,
                    DirectionLight.transform.eulerAngles,
                    dayLightColor,
                    dayLightIntensity,
                    dayLightEuler
                )
            );
            SetBgmVolume(dayBgmVolume);
        }
    }
    public void ShotTargetPos(Vector3 _pos)
    {
        // =========================
        // AI 탐지
        // =========================
        Collider[] hits = Physics.OverlapSphere(
            _pos,
            shotNotifyRadius,
            aiLayerMask,
            QueryTriggerInteraction.Collide   // Trigger 포함
        );  

        for (int i = 0; i < hits.Length; i++)
        {
            Collider col = hits[i];
            AiController ai = col.GetComponentInParent<AiController>();

            if (!ai)
            {
                continue;
            }

            if (ai.GetShotPos() == _pos)
                continue;

            ai.SetShotPos(_pos);
        }
    }

    private void CacheDuckHouses()
    {
        houseById.Clear();

        var found = Object.FindObjectsByType<DuckHouse>(FindObjectsSortMode.None);
        for (int i = 0; i < found.Length; i++)
        {
            found[i].SetId(i);
            houseById.Add(i, found[i]);
        }
    }
    private void ApplyLight(Color color, float intensity, Vector3 euler)
    {
        if (!DirectionLight)
            return;

        DirectionLight.color = color;
        DirectionLight.intensity = intensity;
        DirectionLight.transform.rotation = Quaternion.Euler(euler);
    }
    private IEnumerator TransitionLight(Color fromColor, float fromIntensity, Vector3 fromEuler, Color toColor, float toIntensity, Vector3 toEuler)
    {
        float time = 0f;

        Quaternion fromRot = Quaternion.Euler(fromEuler);
        Quaternion toRot = Quaternion.Euler(toEuler);

        while (time < lightTransitionDuration)
        {
            float t = lightTransitionCurve.Evaluate(time / lightTransitionDuration);

            DirectionLight.color = Color.Lerp(fromColor, toColor, t);
            DirectionLight.intensity = Mathf.Lerp(fromIntensity, toIntensity, t);
            DirectionLight.transform.rotation = Quaternion.Slerp(fromRot, toRot, t);

            time += Time.deltaTime;
            yield return null;
        }

        ApplyLight(toColor, toIntensity, toEuler);
    }
    private void PlayBgm(float volume)
    {
        if (!bgmSource || !windBgm)
            return;
         
        bgmSource.volume = volume;

        if (!bgmSource.isPlaying)
            bgmSource.Play();
    }
    private void SetBgmVolume(float volume)
    {
        if (!bgmSource)
            return;

        bgmSource.volume = volume;
    }
}
  