using System;
using UnityEngine;

public enum EAdornBodyType
{
    Default,
     
    End,
}
public enum EAdornEyesType
{ 
    Default,
    Button,
    Heart,
    BottleCup,
    KeyCap,
    Shining,
      
    End,
}
public enum EAdornBeekType
{ 
    Default,
    ShoeBill,
    StraightShort,
    Toucan,
    BlackFacedSpoonbillShort, // 
    Pacifier, 
      
    End,
}
public enum EAdornEyesBowType
{
    RoundBow,
    Unibrow,
    JJangu,

    End,
}
public enum EAdornHairType
{
    Low,
    PingBa,
    Gourd,
    Parting,
 
    End,
}
public enum EAdornFootType
{
    Default,
    Foot,
       
    End,
}
public enum EAdornHandType
{
    Default,
    HandBoxing,
    Hand,
     
    End,
} 
 

public class AdornDuck : MonoBehaviour
{
    [Header("Roots")]
    [SerializeField] private Transform bodyRoot;

    [Header("Eyes (L / R)")]
    [SerializeField] private Transform eyesRootL;
    [SerializeField] private Transform eyesRootR;

    [Header("EyesBow (L / R)")]
    [SerializeField] private Transform eyesBowRootL;
    [SerializeField] private Transform eyesBowRootR;

    [Header("Arms (L / R)")]
    [SerializeField] private Transform armRootL;
    [SerializeField] private Transform armRootR;

    [Header("Legs / Foot (L / R)")] 
    [SerializeField] private Transform footRootL;
    [SerializeField] private Transform footRootR;

    [Header("Single Roots")]
    [SerializeField] private Transform beakRoot;
    [SerializeField] private Transform hairRoot;
     
    [Header("Meshs")]
    [SerializeField] private GameObject curBody;
    [SerializeField] private GameObject curEyesL;
    [SerializeField] private GameObject curEyesR;
    [SerializeField] private GameObject curEyesBowL;
    [SerializeField] private GameObject curEyesBowR;
    [SerializeField] private GameObject curArmL;
    [SerializeField] private GameObject curArmR;
    [SerializeField] private GameObject curFootL;
    [SerializeField] private GameObject curFootR;
    [SerializeField] private GameObject curBeak;
    [SerializeField] private GameObject curHair;

    [Header("Rotate")]
    [SerializeField] private float rotateSpeed = 0.3f;

    private PlayerMeshSetter cachedPlayerDuckColor;
     
    private EAdornBodyType cachedBodyType = EAdornBodyType.End;
    private EAdornEyesType cachedEyesType = EAdornEyesType.End;
    private EAdornEyesBowType cachedEyesBowType = EAdornEyesBowType.End;
    private EAdornBeekType cachedBeakType = EAdornBeekType.End;
    private EAdornHairType cachedHairType = EAdornHairType.End;
    private EAdornFootType cachedFootType = EAdornFootType.End;
    private EAdornHandType cachedHandType = EAdornHandType.End;

    private Color cachedBodyColor;
    private Color cachedEyesColor;
    private Color cachedEyesBowColor;
    private Color cachedBeakColor;
    private Color cachedHairColor;
    private Color cachedFootColor;
    private Color cachedHandColor;

    private Vector3 cachedLocalPos;
    private Quaternion cachedLocalRot;
    private bool isDragging;
    private Vector2 lastDragPos;

    private void Awake()
    {
        cachedLocalPos = transform.localPosition;
        cachedLocalRot = transform.localRotation; 
        cachedPlayerDuckColor = GameInstance.Instance.PLAYER_GetPlayerColorSetter();
        Disable();
    }
      
    // =========================
    // Public API
    // =========================
    public void Active()
    { 
        if (cachedPlayerDuckColor == null)
            return;

        gameObject.SetActive(true);

        // 위치 세팅
        transform.localPosition = cachedLocalPos;
        transform.localRotation = cachedLocalRot;

        ClearAll();

        // =========================
        // Body
        // =========================
        if (cachedPlayerDuckColor.GetBodyType() != EAdornBodyType.End)
        {
            SpawnBody(cachedPlayerDuckColor.GetBodyType()); 
        }
        SetBodyColor(cachedPlayerDuckColor.GetBodyColor());

        // =========================
        // Eyes
        // =========================
        if (cachedPlayerDuckColor.GetEyesType() != EAdornEyesType.End)
        {
            SpawnEyes(cachedPlayerDuckColor.GetEyesType());
        }
        SetEyesColor(cachedPlayerDuckColor.GetEyeColor());

        // =========================
        // EyesBow
        // =========================
        if (cachedPlayerDuckColor.GetEyesBowType() != EAdornEyesBowType.End)
        {
            SpawnEyesBow(cachedPlayerDuckColor.GetEyesBowType());
        }
        SetEyesBowColor(cachedPlayerDuckColor.GetEyebrowColor());

        // =========================
        // Beak
        // =========================
        if (cachedPlayerDuckColor.GetBeakType() != EAdornBeekType.End)
        {
            SpawnBeak(cachedPlayerDuckColor.GetBeakType());
        }
        SetBeakColor(cachedPlayerDuckColor.GetBeakColor());

        // =========================
        // Hair
        // =========================
        if (cachedPlayerDuckColor.GetHairType() != EAdornHairType.End)
        {
            SpawnHair(cachedPlayerDuckColor.GetHairType());
        }
        SetHairColor(cachedPlayerDuckColor.GetHairColor());

        // =========================
        // Foot
        // =========================
        if (cachedPlayerDuckColor.GetFootType() != EAdornFootType.End)
        {
            SpawnFoot(cachedPlayerDuckColor.GetFootType());
        }
        SetFootColor(cachedPlayerDuckColor.GetFootColor());
         
        // =========================
        // Hand 
        // =========================
        if (cachedPlayerDuckColor.GetHandType() != EAdornHandType.End)
        {
            SpawnHand(cachedPlayerDuckColor.GetHandType());
        }
        SetHandColor(cachedPlayerDuckColor.GetArmColor());
    }
    public void Disable()
    {
        ClearAll(); 
         
        gameObject.SetActive(false);
    } 

    public void ApplyToPlayer()
    {
        if (!cachedPlayerDuckColor)
            return;

        cachedPlayerDuckColor.ApplyBody(GetBodyType(), GetBodyColor());
        cachedPlayerDuckColor.ApplyEyes(GetEyesType(), GetEyesColor());
        cachedPlayerDuckColor.ApplyEyesBow(GetEyesBowType(), GetEyesBowColor());
        cachedPlayerDuckColor.ApplyBeak(GetBeakType(), GetBeakColor());
        cachedPlayerDuckColor.ApplyHair(GetHairType(), GetHairColor());
        cachedPlayerDuckColor.ApplyHand(GetHandType(), GetHandColor());
        cachedPlayerDuckColor.ApplyFoot(GetFootType(), GetFootColor());

        // 여기서 세이브 데이터를 실제로 Save시켜주기
        var playerSave = GameInstance.Instance.PLAYER_GetPlayerSave();
        playerSave.SavePlayerMesh();
    }  

    public EAdornBodyType GetBodyType() => cachedBodyType;
    public EAdornEyesType GetEyesType() => cachedEyesType;
    public EAdornEyesBowType GetEyesBowType() => cachedEyesBowType;
    public EAdornBeekType GetBeakType() => cachedBeakType;
    public EAdornHairType GetHairType() => cachedHairType;
    public EAdornFootType GetFootType() => cachedFootType;
    public EAdornHandType GetHandType() => cachedHandType;

    public Color GetBodyColor() => cachedBodyColor;
    public Color GetEyesColor() => cachedEyesColor;
    public Color GetEyesBowColor() => cachedEyesBowColor;
    public Color GetBeakColor() => cachedBeakColor;
    public Color GetHairColor() => cachedHairColor;
    public Color GetFootColor() => cachedFootColor;
    public Color GetHandColor() => cachedHandColor;

    public void BeginDrag(Vector2 screenPos)
    {
        isDragging = true;
        lastDragPos = screenPos;
    }
    public void Drag(Vector2 screenPos)
    {
        if (!isDragging)
            return;

        float deltaX = screenPos.x - lastDragPos.x;
        transform.Rotate(Vector3.up, -deltaX * rotateSpeed, Space.Self);

        lastDragPos = screenPos;
    }
    public void EndDrag()
    {
        isDragging = false;
    }
     
    // =========================
    // Spawn
    // =========================
    public void SpawnBody(EAdornBodyType type)
    {
        curBody = SpawnAndAttach(curBody, bodyRoot, GameInstance.Instance.SPAWN_BodyMesh(type));
        cachedBodyType = type;
        SetBodyColor(cachedBodyColor);
    }
    public void SpawnEyes(EAdornEyesType type)
    {
        curEyesL = SpawnAndAttach(curEyesL, eyesRootL,
            GameInstance.Instance.SPAWN_EyesMesh(type));
        curEyesR = SpawnAndAttach(curEyesR, eyesRootR,
            GameInstance.Instance.SPAWN_EyesMesh(type));

        cachedEyesType = type;
        SetEyesColor(cachedEyesColor);
    }
    public void SpawnEyesBow(EAdornEyesBowType type)
    {
        curEyesBowL = SpawnAndAttach(curEyesBowL, eyesBowRootL,
            GameInstance.Instance.SPAWN_EyesBowMesh(type));
        curEyesBowR = SpawnAndAttach(curEyesBowR, eyesBowRootR,
            GameInstance.Instance.SPAWN_EyesBowMesh(type));

        cachedEyesBowType = type;
        SetEyesBowColor(cachedEyesBowColor);
    }
    public void SpawnBeak(EAdornBeekType type)
    {
        curBeak = SpawnAndAttach(curBeak, beakRoot, GameInstance.Instance.SPAWN_BeakMesh(type));
         
        cachedBeakType = type;
        SetBeakColor(cachedBeakColor);
    }
    public void SpawnHair(EAdornHairType type)
    {
        curHair = SpawnAndAttach(curHair, hairRoot, GameInstance.Instance.SPAWN_HairMesh(type));

        cachedHairType = type;
        SetHairColor(cachedHairColor);
    }
    public void SpawnFoot(EAdornFootType type) 
    {
        curFootL = SpawnAndAttach(curFootL, footRootL, GameInstance.Instance.SPAWN_FootMesh(type));
        curFootR = SpawnAndAttach(curFootR, footRootR, GameInstance .Instance.SPAWN_FootMesh(type));
         
        cachedFootType = type;
        SetFootColor(cachedFootColor);
    }
    public void SpawnHand(EAdornHandType type)
    {
        curArmL = SpawnAndAttach(curArmL, armRootL,
            GameInstance.Instance.SPAWN_HandMesh(type));
        curArmR = SpawnAndAttach(curArmR, armRootR,
            GameInstance.Instance.SPAWN_HandMesh(type));

        cachedHandType = type;
        SetHandColor(cachedHandColor);
    }
     

    // =========================
    // Color
    // =========================
    public void SetBodyColor(Color c)
    {
        cachedBodyColor = c;
        ApplyColor(curBody, c);
    }
    public void SetEyesColor(Color c)
    {
        cachedEyesColor = c;
        ApplyColor(curEyesL, c);
        ApplyColor(curEyesR, c);
    }
    public void SetEyesBowColor(Color c)
    {
        cachedEyesBowColor = c;
        ApplyColor(curEyesBowL, c);
        ApplyColor(curEyesBowR, c);
    }
    public void SetBeakColor(Color c)
    {
        cachedBeakColor = c;
        ApplyColor(curBeak, c); 
    }
    public void SetHairColor(Color c)
    {
        cachedHairColor = c; 
        ApplyColor(curHair, c);
    }
    public void SetFootColor(Color c)
    {
        cachedFootColor = c; 
        ApplyColor(curFootL, c);
        ApplyColor(curFootR, c);
    }
    public void SetHandColor(Color c)
    {
        cachedHandColor = c;
        ApplyColor(curArmL, c); 
        ApplyColor(curArmR, c);
    }
     

    // =========================
    // Internal
    // =========================
    private GameObject SpawnAndAttach(GameObject cur, Transform parent, GameObject next)
    {
        if (cur) 
            Destroy(cur);

        if (!next || !parent) 
            return null;

        next.transform.SetParent(parent, false);
        return next; 
    }
   
    private void ClearAll()
    {
        DestroyAndClear(ref curBody);

        DestroyAndClear(ref curEyesL);
        DestroyAndClear(ref curEyesR);

        DestroyAndClear(ref curEyesBowL);
        DestroyAndClear(ref curEyesBowR);

        DestroyAndClear(ref curArmL);
        DestroyAndClear(ref curArmR);

        DestroyAndClear(ref curFootL);
        DestroyAndClear(ref curFootR);

        DestroyAndClear(ref curBeak);
        DestroyAndClear(ref curHair);

        cachedBodyType = EAdornBodyType.End;
        cachedEyesType = EAdornEyesType.End;
        cachedEyesBowType = EAdornEyesBowType.End;
        cachedBeakType = EAdornBeekType.End;
        cachedHairType = EAdornHairType.End;
        cachedFootType = EAdornFootType.End;
        cachedHandType = EAdornHandType.End; 
    }
    private void ApplyColor(GameObject root, Color c)
    {
        if (!root) return;

        var renderers = root.GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers)
        {
            if (!r) continue;

            var block = new MaterialPropertyBlock();
            r.GetPropertyBlock(block);
            block.SetColor("_BaseColor", c);
            r.SetPropertyBlock(block);
        }
    }
    private void DestroyAndClear(ref GameObject go)
    {
        if (!go) return;

        Destroy(go);
        go = null;
    }
}


