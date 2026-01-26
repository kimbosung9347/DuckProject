using UnityEngine;

[CreateAssetMenu(fileName = "SoundSfx", menuName = "Scriptable Objects/SoundSfx")]
public class SoundSfx : ScriptableObject
{
    // =========================
    // Audio Clip
    // =========================
    // 실제 재생될 사운드 파일
    [Header("Clip")]
    public AudioClip clip;

    // =========================
    // Volume / Pitch
    // =========================
    // volume        : 최종 음량 (0 = 무음, 1 = 원본 볼륨)
    // pitch         : 재생 속도 / 음높이 (1 = 원본)
    // randomizePitch: 재생 시 pitch를 랜덤 범위로 적용할지 여부
    // pitchRange    : 랜덤 pitch 최소/최대 범위
    [Header("Volume / Pitch")]
    [Range(0f, 1f)] public float volume = 1f;
    public float pitch = 1f;
    public bool randomizePitch = false;
    public Vector2 pitchRange = new Vector2(0.95f, 1.05f);

    // =========================
    // 3D Spatial Settings
    // =========================
    // spatialBlend  : 0 = 완전 2D, 1 = 완전 3D 사운드
    // rolloffMode   : 거리 감쇠 곡선 방식
    //   - Logarithmic : 일반적인 현실적인 감쇠 (권장)
    //   - Linear       : 거리 따라 선형 감소
    //   - Custom       : AnimationCurve 사용
    //
    // minDistance   : 이 거리까지는 최대 볼륨 유지
    // maxDistance   : 이 거리 이후에는 거의 들리지 않음
    //
    // dopplerLevel  : 도플러 효과 강도 (이동 시 피치 변화)
    // spread        : 3D 사운드의 입체 확산 각도 (0 = 점음원)
    [Header("3D Spatial Settings")]
    [Range(0f, 1f)] public float spatialBlend = 1f;
    public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;
    public float minDistance = 1f;
    public float maxDistance = 15f;
    [Range(0f, 5f)] public float dopplerLevel = 0f;
    [Range(0f, 360f)] public float spread = 0f;

    // =========================
    // Priority
    // =========================
    // priority : 오디오 우선순위
    // 값이 낮을수록 우선 재생됨 (0 = 최우선)
    // 많은 사운드가 동시에 재생될 때 컷 기준
    [Header("Priority")]
    [Range(0, 256)] public int priority = 128;

    // =========================
    // Reverb
    // =========================
    // reverbZoneMix :
    // 리버브 존(Audio Reverb Zone)에 얼마나 영향을 받을지
    // 0 = 영향 없음, 1 = 완전 적용
    [Header("Reverb")]
    [Range(0f, 1.1f)] public float reverbZoneMix = 1f;

    // =========================
    // Internal
    // =========================
    // 랜덤 Pitch 적용 계산
    public float GetPitch()
    {
        if (!randomizePitch)
            return pitch;

        return Random.Range(pitchRange.x, pitchRange.y);
    }

    // =========================
    // Apply Settings to AudioSource
    // =========================
    // AudioSource 하나에 이 SoundSfx 설정을 그대로 적용
    public void ApplyTo(AudioSource src)
    {
        src.clip = clip;
        src.volume = volume;
        src.pitch = GetPitch();
        src.spatialBlend = spatialBlend;
        src.rolloffMode = rolloffMode;
        src.minDistance = minDistance;
        src.maxDistance = maxDistance;
        src.dopplerLevel = dopplerLevel;
        src.spread = spread;
        src.priority = priority;
        src.reverbZoneMix = reverbZoneMix;

        // SFX 기본값
        src.loop = false;
        src.playOnAwake = false;
    }
}
