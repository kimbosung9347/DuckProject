using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public enum EBgmType
{
    DuckFunk,
    IrisOut,
    WalcomeToLosSantos,
    End
}

public enum ESoundSfxType
{
    Duck_Quack,
    Duck_Quack2,

    Reload,
    Shot,
    Punch,
    Hit,
    Dead,
    PickUp,
    Roll,
    Found_Normal,
    Found_Big,
    Walk,
    KeyLock,
         
    End
}

[System.Serializable]
public struct BgmEntry
{
    public EBgmType type;
    public AudioClip clip;
}

[System.Serializable]
public struct SfxEntry
{
    public ESoundSfxType type;
    public SoundSfx sfx; // AudioClip → SoundSfx

    [Min(0)]
    public int poolSize; // 타입별 풀 크기
}
  
public class SoundSystem : MonoBehaviour
{
    [Header("BGM (Type별 매핑)")]
    [SerializeField] private BgmEntry[] bgmEntries = new BgmEntry[(int)EBgmType.End];

    [Header("SFX (Type별 매핑 + Type별 PoolSize)")]
    [SerializeField] private SfxEntry[] sfxEntries = new SfxEntry[(int)ESoundSfxType.End];

    private readonly Dictionary<EBgmType, AudioClip> bgmTable = new();
    private readonly Dictionary<ESoundSfxType, SoundSfx> sfxTable = new();

    // SFX 타입별 풀
    private readonly Dictionary<ESoundSfxType, List<AudioSource>> sfxPools = new();

    private AudioSource bgmSource;

    private void Awake()
    {
        // BGM Source (2D)
        bgmSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.spatialBlend = 0f;

        BuildTables();
        BuildSfxPools(); // 타입별 풀 생성
    }

    private void BuildTables()
    {
        bgmTable.Clear();
        sfxTable.Clear();

        foreach (var e in bgmEntries)
        {
            if (e.type == EBgmType.End || !e.clip)
                continue;

            bgmTable[e.type] = e.clip;
        }

        foreach (var e in sfxEntries)
        {
            if (e.type == ESoundSfxType.End || !e.sfx || !e.sfx.clip)
                continue;

            sfxTable[e.type] = e.sfx;
        }
    }

    private void BuildSfxPools()
    {
        sfxPools.Clear();

        foreach (var e in sfxEntries)
        {
            if (e.type == ESoundSfxType.End)
                continue;

            // poolSize 0이면: 해당 타입은 풀 없이 항상 Temp로만 처리됨
            if (e.poolSize <= 0)
                continue;

            var list = new List<AudioSource>(e.poolSize);

            for (int i = 0; i < e.poolSize; i++)
            {
                var src = gameObject.AddComponent<AudioSource>();
                src.loop = false;
                src.playOnAwake = false;
                src.spatialBlend = 1f;
                list.Add(src);
            }

            sfxPools[e.type] = list;
        }
    }

    public SoundSfx GetSfx(ESoundSfxType type)
    {
        if (sfxTable.TryGetValue(type, out var sfx))
            return sfx;

        return null;
    }

    public void PlayBgm(EBgmType type)
    {
        if (!bgmTable.TryGetValue(type, out var clip))
            return;

        bgmSource.Stop();
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void StopBgm()
    { 
        if (bgmSource)
        {
            bgmSource.Stop();
            bgmSource.clip = null;
        }
    }

    public void PlaySfx(ESoundSfxType type, Vector3 worldPos, float duration)
    {
        if (!sfxTable.TryGetValue(type, out var sfx))
            return;
         
        // 타입별 풀에서 먼저 가져옴
        var src = GetFreeSfxSource(type);
        bool isTemp = false;

        // 해당 타입 풀에 여유가 없거나 poolSize=0이면 Temp 생성
        if (!src)
        {
            var go = new GameObject($"TempSFX_{type}");
            go.transform.position = worldPos;
            src = go.AddComponent<AudioSource>();
            isTemp = true;
        }

        src.transform.position = worldPos;
        sfx.ApplyTo(src);
        src.Play();

        float lifeTime =
            duration > 0f
                ? duration
                : sfx.clip.length / Mathf.Abs(src.pitch);

        if (isTemp)
            Destroy(src.gameObject, lifeTime);
        else
            StartCoroutine(StopSfxAfter(src, lifeTime));
    }

    public void PlayDuckQuack(EDuckType _type, Vector3 worldPos)
    {
        switch (_type)
        {
            case EDuckType.Farmer:
            case EDuckType.Boxer:
                PlaySfx(ESoundSfxType.Duck_Quack, worldPos, -1f);
                break;

            case EDuckType.Mercenary:
                PlaySfx(ESoundSfxType.Duck_Quack2, worldPos, -1f);
                break;
        }
    }

    // 타입별 풀에서 빈 AudioSource 찾기
    private AudioSource GetFreeSfxSource(ESoundSfxType type)
    {
        if (!sfxPools.TryGetValue(type, out var pool))
            return null;

        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].isPlaying)
                return pool[i];
        }

        return null;
    }

    private IEnumerator StopSfxAfter(AudioSource src, float time)
    {
        yield return new WaitForSeconds(time);

        if (src && src.isPlaying)
            src.Stop();
    }
}
