using UnityEngine;

[CreateAssetMenu(
    fileName = "DuckMeshSet",
    menuName = "Scriptable Objects/DuckMeshSet"
)] 
public class DuckMeshSet : ScriptableObject
{
    public EDuckType duckType;

    [Header("Adorn Types")]
    public EAdornEyesType eyesType = EAdornEyesType.End;
    public EAdornEyesBowType eyesBowType = EAdornEyesBowType.End;
    public EAdornBeekType beakType = EAdornBeekType.End;
    public EAdornHairType hairType = EAdornHairType.End;
    public EAdornFootType footType = EAdornFootType.End;

    [Header("Colors")]
    public Color bodyColor = Color.white;
    public Color eyeColor = Color.white;
    public Color eyeBowColor = Color.white;
    public Color armColor = Color.white;
    public Color beakColor = Color.white;
    public Color hairColor = Color.white;
    public Color footColor = Color.white;
}
