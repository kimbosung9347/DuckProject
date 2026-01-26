using UnityEngine;

public class BattleTable : MonoBehaviour 
{ 
    [Header("반동 명중률 회복 계수")]
    [SerializeField] private float accReduceCoefficient = 0.5f;
       
    [Header("반동 카메라 반동 세기 계산 계수")]
    [SerializeField] private float cameraRecoilCoefficient = 0.0025f;
      
    [Header("반동 명중률 제어 계수")]
    [SerializeField] private float cursorRecoilSize = 5f;

    [Header("반동 이동 크기 계수")]
    [SerializeField] private float cursorMoveCoefficient = 5f;


    public float Calculate_AccReduceByRecoilSize(float _recoilSize)
    {
        return -(_recoilSize * accReduceCoefficient);
    }
    public float Calculate_CameraRecoilStrengh(float _recoilSize)
    {
        return (100 - _recoilSize) * cameraRecoilCoefficient;
    }
    public float Calculate_CursorRecoilSize(float _recoilSize)
    {  
        return (100 - _recoilSize) * cursorRecoilSize;
    } 
    public float Calculate_CursorMoveSize(float _recoilSize)
    {
        return (100 - _recoilSize) * cursorMoveCoefficient;
    }

    // 여기는 
    public float Calculate_HitDamage(bool _isHead, float _damage, float _armor)
    { 
        float headMultiplier = _isHead ? 1.6f : 1.0f;
           
        // armorRatio: 0~1
        float reducedDamage = _damage * (1f - Mathf.Clamp01(_armor));

        float finalDamage = reducedDamage * headMultiplier;

        return Mathf.Max(1f, finalDamage);
    }
}
  