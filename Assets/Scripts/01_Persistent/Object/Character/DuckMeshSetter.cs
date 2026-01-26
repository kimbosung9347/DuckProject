using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class DuckMeshSetter : MonoBehaviour
{
    [Header("Mesh")]
    [SerializeField] protected GameObject body;
    [SerializeField] protected GameObject eyeLeft;
    [SerializeField] protected GameObject eyeRight;
    [SerializeField] protected GameObject eyebrowLeft;
    [SerializeField] protected GameObject eyebrowRight;
    [SerializeField] protected GameObject armLeft;
    [SerializeField] protected GameObject armRight;
    [SerializeField] protected GameObject footLeft;
    [SerializeField] protected GameObject footRight;
    [SerializeField] protected GameObject beak;
    [SerializeField] protected GameObject hair;

    [Header("Colors")]
    [SerializeField] protected Color bodyColor = Color.white;
    [SerializeField] protected Color eyeColor = Color.white;
    [SerializeField] protected Color eyebrowColor = Color.white;
    [SerializeField] protected Color armColor = Color.white;
    [SerializeField] protected Color footColor = Color.white;
    [SerializeField] protected Color beakColor = Color.white;
    [SerializeField] protected Color hairColor = Color.white;

    protected EAdornBodyType bodyType = EAdornBodyType.End;
    protected EAdornEyesType eyesType = EAdornEyesType.End;
    protected EAdornBeekType beakType = EAdornBeekType.End;
    protected EAdornEyesBowType eyesBowType = EAdornEyesBowType.End;
    protected EAdornHairType hairType = EAdornHairType.End;
    protected EAdornFootType footType = EAdornFootType.End;
    protected EAdornHandType handType = EAdornHandType.End;

    private Coroutine bodyPulseRoutine;
    private MaterialPropertyBlock bodyBlock;
     
    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");
    private static readonly int HitPulseID = Shader.PropertyToID("_HitPulse");

    private void Start()
    {
        ApplyAllColors();
    } 

#if UNITY_EDITOR
    private void OnValidate()
    {
        ApplyAllColors();
    }
#endif 
     
    // -----------------------------
    // Type Getter
    // -----------------------------
    public EAdornBodyType GetBodyType() => bodyType;
    public EAdornEyesType GetEyesType() => eyesType;
    public EAdornBeekType GetBeakType() => beakType;
    public EAdornEyesBowType GetEyesBowType() => eyesBowType;
    public EAdornHairType GetHairType() => hairType;
    public EAdornFootType GetFootType() => footType;
    public EAdornHandType GetHandType() => handType;

    public Color GetBodyColor() => bodyColor;
    public Color GetEyeColor() => eyeColor;
    public Color GetEyebrowColor() => eyebrowColor;
    public Color GetArmColor() => armColor;
    public Color GetFootColor() => footColor;
    public Color GetBeakColor() => beakColor;
    public Color GetHairColor() => hairColor;

    // -----------------------------
    // Type Setter 
    // -----------------------------
    public void SetBodyType(EAdornBodyType t) => bodyType = t;
    public void SetEyesType(EAdornEyesType t) => eyesType = t;
    public void SetBeakType(EAdornBeekType t) => beakType = t;
    public void SetEyesBowType(EAdornEyesBowType t) => eyesBowType = t;
    public void SetHairType(EAdornHairType t) => hairType = t;
    public void SetFootType(EAdornFootType t) => footType = t;
    public void SetArmType(EAdornHandType t) => handType = t;

    // ----------------------------- 
    // Color Setter
    // -----------------------------
    public void SetBodyColor(Color c) { bodyColor = c; SetColor(body, c); }
    public void SetEyeColor(Color c) { eyeColor = c; SetColor(eyeLeft, c); SetColor(eyeRight, c); }
    public void SetEyebrowColor(Color c) { eyebrowColor = c; SetColor(eyebrowLeft, c); SetColor(eyebrowRight, c); }
    public void SetArmColor(Color c) { armColor = c; SetColor(armLeft, c); SetColor(armRight, c); }
    public void SetFootColor(Color c) { footColor = c; SetColor(footLeft, c); SetColor(footRight, c); }
    public void SetBeakColor(Color c) { beakColor = c; SetColor(beak, c); }
    public void SetHairColor(Color c) { hairColor = c; SetColor(hair, c); }

    // -----------------------------
    // Renderer Access
    // -----------------------------
    public Renderer[] GetAllRenderers()
    {
        List<Renderer> list = new();
        AddRenderer(body, list);
        AddRenderer(eyeLeft, list);
        AddRenderer(eyeRight, list);
        AddRenderer(eyebrowLeft, list);
        AddRenderer(eyebrowRight, list);
        AddRenderer(armLeft, list);
        AddRenderer(armRight, list);
        AddRenderer(footLeft, list);
        AddRenderer(footRight, list);
        AddRenderer(beak, list);
        AddRenderer(hair, list);
        return list.ToArray();
    }

    // -----------------------------
    // Internal
    // -----------------------------
    private void ApplyAllColors()
    {
        SetColor(body, bodyColor);
        SetColor(eyeLeft, eyeColor);
        SetColor(eyeRight, eyeColor);
        SetColor(eyebrowLeft, eyebrowColor);
        SetColor(eyebrowRight, eyebrowColor);
        SetColor(armLeft, armColor);
        SetColor(armRight, armColor);
        SetColor(footLeft, footColor);
        SetColor(footRight, footColor);
        SetColor(beak, beakColor);
        SetColor(hair, hairColor);
    }

    private void SetColor(GameObject go, Color c)
    {
        if (!go) return;
        if (!go.TryGetComponent(out Renderer r)) return;

        var block = new MaterialPropertyBlock();
        r.GetPropertyBlock(block);
        block.SetColor(BaseColorID, c);
        r.SetPropertyBlock(block);
    }
     
    private void AddRenderer(GameObject go, List<Renderer> list)
    {
        if (!go) return;
        if (go.TryGetComponent(out Renderer r))
            list.Add(r);
    }
}
