using UnityEngine;
using UnityEngine.UIElements;

public class PrefabSpawner : MonoBehaviour
{
    [SerializeField] private GameObject itemBillboardPrefab;
    [SerializeField] private GameObject raserBeamPrefab;
    [SerializeField] private GameObject hitValuePrefab;
    [SerializeField] private GameObject chickenPrefab;
 
    [Header("Bodys : Default")]
    [SerializeField] private GameObject[] bodyMap;
     
    [Header("Eyes : Default / Button / Heart / BottleCup / KeyCap / Shining")]
    [SerializeField] private GameObject[] eyesMap; 
     
    [Header("Beak : Default / ShoeBill / StraightShort / Toucan / BlackFacedSpoonbillShort / Pacifier")]
    [SerializeField] private GameObject[] beakMap;
     
    [Header("EyesBow : RoundBow / Unibrow / JJangu")]
    [SerializeField] private GameObject[] eyesBowMap;
     
    [Header("Hair : Low / PingBa / Ground / Parting")]
    [SerializeField] private GameObject[] hairMap;

    [Header("Foot : Default / Foot")]
    [SerializeField] private GameObject[] footMap;

    [Header("Hand : Default / Boxing / Hand")]
    [SerializeField] private GameObject[] handMap;
     

    public GameObject SpawnItemBillboard()
    {
        return Instantiate(itemBillboardPrefab);
    } 
    public GameObject SpawnRaserBeam()
    {
        return Instantiate(raserBeamPrefab);
    }
    public GameObject SpawnHitValue() 
    {
        return Instantiate(hitValuePrefab);
    }
    public GameObject SpawnChickien(Vector3 _pos, Quaternion _rot)
    { 
        return Instantiate(chickenPrefab, _pos, Quaternion.identity);
    }


    public GameObject SpawnBody(EAdornBodyType type)
        => SpawnFromMap(bodyMap, (int)type); 
    public GameObject SpawnEyes(EAdornEyesType type)
        => SpawnFromMap(eyesMap, (int)type);
    public GameObject SpawnBeak(EAdornBeekType type)
        => SpawnFromMap(beakMap, (int)type);
    public GameObject SpawnEyesBow(EAdornEyesBowType type)
        => SpawnFromMap(eyesBowMap, (int)type);
    public GameObject SpawnHair(EAdornHairType type)
        => SpawnFromMap(hairMap, (int)type);
    public GameObject SpawnFoot(EAdornFootType type)
        => SpawnFromMap(footMap, (int)type);
    public GameObject SpawnHand(EAdornHandType type)
    => SpawnFromMap(handMap, (int)type);
      

    private GameObject SpawnFromMap(GameObject[] map, int index)
    {
        if (map == null || index < 0 || index >= map.Length)
            return null;

        var go = Object.Instantiate(map[index]);
        InitMaterialInstance(go);
        return go;
    }
    private void InitMaterialInstance(GameObject root)
    {
        var renderers = root.GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers)
        {
            if (r == null || r.sharedMaterial == null) continue;
            r.sharedMaterial = new Material(r.sharedMaterial);
        }
    }
}
 