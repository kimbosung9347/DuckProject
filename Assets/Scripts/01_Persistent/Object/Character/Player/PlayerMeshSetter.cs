using UnityEngine;

public class PlayerMeshSetter : DuckMeshSetter
{ 
    [SerializeField] private DuckDetected cachedDuckDetected;
      
    [Header("Mesh Roots")]
    [SerializeField] private Transform rootBody;

    [SerializeField] private Transform rootEyeL;
    [SerializeField] private Transform rootEyeR;

    [SerializeField] private Transform rootEyeBowL;
    [SerializeField] private Transform rootEyeBowR;

    [SerializeField] private Transform rootArmL;
    [SerializeField] private Transform rootArmR;

    [SerializeField] private Transform rootHair;
    [SerializeField] private Transform rootBeak;

    [SerializeField] private Transform rootFootL;
    [SerializeField] private Transform rootFootR;

    private void Awake()
    {
        EnsureAllRoots();

        var gi = GameInstance.Instance;
        var data = gi.SAVE_GetCurPlayData().characterMeshData;

        ClearAllRoots();

        if (data.bodyType != EAdornBodyType.End)
        {
            var go = gi.SPAWN_BodyMesh(data.bodyType);  
            Attach(go, rootBody, ref body);
              
            SetBodyType(data.bodyType); 
            SetBodyColor(data.bodyColor);
        }  

        // =====================
        // Eyes (L / R)
        // =====================
        if (data.eyesType != EAdornEyesType.End)
        {
            var eyeL = gi.SPAWN_EyesMesh(data.eyesType);
            var eyeR = gi.SPAWN_EyesMesh(data.eyesType); 

            Attach(eyeL, rootEyeL, ref eyeLeft);
            Attach(eyeR, rootEyeR, ref eyeRight);

            SetEyesType(data.eyesType);
            SetEyeColor(data.eyeColor);
        }

        // =====================
        // Beak
        // =====================
        if (data.beakType != EAdornBeekType.End)
        {
            var go = gi.SPAWN_BeakMesh(data.beakType);
            Attach(go, rootBeak, ref beak);

            SetBeakType(data.beakType);
            SetBeakColor(data.beakColor);
        }

        // =====================
        // Eyebrow (L / R)
        // =====================
        if (data.eyesBowType != EAdornEyesBowType.End)
        {
            var bowL = gi.SPAWN_EyesBowMesh(data.eyesBowType);
            var bowR = gi.SPAWN_EyesBowMesh(data.eyesBowType);

            Attach(bowL, rootEyeBowL, ref eyebrowLeft);
            Attach(bowR, rootEyeBowR, ref eyebrowRight);

            SetEyesBowType(data.eyesBowType);
            SetEyebrowColor(data.eyebrowColor);
        }

        // =====================
        // Hair
        // =====================
        if (data.hairType != EAdornHairType.End)
        {
            var go = gi.SPAWN_HairMesh(data.hairType);
            Attach(go, rootHair, ref hair);

            SetHairType(data.hairType);
            SetHairColor(data.hairColor);
        }

        // =====================
        // Arm (L / R)
        // =====================
        if (data.handType != EAdornHandType.End)
        {
            var armL = gi.SPAWN_HandMesh(data.handType);
            var armR = gi.SPAWN_HandMesh(data.handType);

            Attach(armL, rootArmL, ref armLeft);
            Attach(armR, rootArmR, ref armRight);

            SetArmType(data.handType);
            SetArmColor(data.armColor);
        }

        // =====================
        // Foot (L / R)
        // =====================
        if (data.footType != EAdornFootType.End)
        {
            var footL = gi.SPAWN_FootMesh(data.footType);
            var footR = gi.SPAWN_FootMesh(data.footType);

            Attach(footL, rootFootL, ref footLeft);
            Attach(footR, rootFootR, ref footRight);

            SetFootType(data.footType);
            SetFootColor(data.footColor);
        }
    }

    public CharacterMeshData CreateCharacterMeshData()
    {
        return new CharacterMeshData
        {
            bodyColor = bodyColor,
            eyeColor = eyeColor,
            eyebrowColor = eyebrowColor,
            armColor = armColor,
            footColor = footColor,
            beakColor = beakColor,
            hairColor = hairColor,

            bodyType = bodyType,
            eyesType = eyesType,
            beakType = beakType,
            eyesBowType = eyesBowType,
            hairType = hairType,
            footType = footType,
            handType = handType,
        };
    }

    public void ApplyBody(EAdornBodyType type, Color color)
    {
        if (bodyType != type)
        {
            DestroyAndNull(ref body);

            if (type != EAdornBodyType.End)
            {
                var go = GameInstance.Instance.SPAWN_BodyMesh(type);
                Attach(go, rootBody, ref body);
            }

            SetBodyType(type);
        }

        if (bodyColor != color)
            SetBodyColor(color);
    }
    public void ApplyEyes(EAdornEyesType type, Color color)
    {
        if (eyesType != type)
        {
            DestroyAndNull(ref eyeLeft);
            DestroyAndNull(ref eyeRight);

            if (type != EAdornEyesType.End)
            {
                Attach(GameInstance.Instance.SPAWN_EyesMesh(type), rootEyeL, ref eyeLeft);
                Attach(GameInstance.Instance.SPAWN_EyesMesh(type), rootEyeR, ref eyeRight);
            }
            SetEyesType(type);
        } 

        if (eyeColor != color)
            SetEyeColor(color);
    }
    public void ApplyEyesBow(EAdornEyesBowType type, Color color)
    {
        if (eyesBowType != type)
        {
            DestroyAndNull(ref eyebrowLeft);
            DestroyAndNull(ref eyebrowRight);

            if (type != EAdornEyesBowType.End)
            {
                Attach(GameInstance.Instance.SPAWN_EyesBowMesh(type), rootEyeBowL, ref eyebrowLeft);
                Attach(GameInstance.Instance.SPAWN_EyesBowMesh(type), rootEyeBowR, ref eyebrowRight);
            }

            SetEyesBowType(type);
        }

        if (eyebrowColor != color)
            SetEyebrowColor(color);
    }
    public void ApplyBeak(EAdornBeekType type, Color color)
    {
        if (beakType != type)
        {
            DestroyAndNull(ref beak);

            if (type != EAdornBeekType.End)
                Attach(GameInstance.Instance.SPAWN_BeakMesh(type), rootBeak, ref beak);

            SetBeakType(type);
        }

        if (beakColor != color)
            SetBeakColor(color);
    }
    public void ApplyHair(EAdornHairType type, Color color)
    {
        if (hairType != type)
        {
            DestroyAndNull(ref hair);

            if (type != EAdornHairType.End)
                Attach(GameInstance.Instance.SPAWN_HairMesh(type), rootHair, ref hair);

            SetHairType(type);
        }

        if (hairColor != color)
            SetHairColor(color);
    }
    public void ApplyHand(EAdornHandType type, Color color)
    {
        if (handType != type)
        {
            DestroyAndNull(ref armLeft);
            DestroyAndNull(ref armRight);

            if (type != EAdornHandType.End)
            {
                Attach(GameInstance.Instance.SPAWN_HandMesh(type), rootArmL, ref armLeft);
                Attach(GameInstance.Instance.SPAWN_HandMesh(type), rootArmR, ref armRight);
            }

            SetArmType(type);
        }

        if (armColor != color)
            SetArmColor(color);
    }
    public void ApplyFoot(EAdornFootType type, Color color)
    {
        if (footType != type)
        {
            DestroyAndNull(ref footLeft);
            DestroyAndNull(ref footRight);

            if (type != EAdornFootType.End)
            {
                Attach(GameInstance.Instance.SPAWN_FootMesh(type), rootFootL, ref footLeft);
                Attach(GameInstance.Instance.SPAWN_FootMesh(type), rootFootR, ref footRight);
            }

            SetFootType(type);
        }

        if (footColor != color)
            SetFootColor(color);
    }
     
    // ----------------
    // Root 생성 / 보장
    // ----------------
    private void EnsureAllRoots()
    {
        rootBody = Ensure("BodyRoot", rootBody);

        rootEyeL = Ensure("Eye_L", rootEyeL);
        rootEyeR = Ensure("Eye_R", rootEyeR);

        rootEyeBowL = Ensure("EyeBow_L", rootEyeBowL);
        rootEyeBowR = Ensure("EyeBow_R", rootEyeBowR);

        rootArmL = Ensure("Arm_L", rootArmL);
        rootArmR = Ensure("Arm_R", rootArmR);

        rootHair = Ensure("HairRoot", rootHair);
        rootBeak = Ensure("BeakRoot", rootBeak);

        rootFootL = Ensure("Foot_L", rootFootL);
        rootFootR = Ensure("Foot_R", rootFootR);
    }
    private Transform Ensure(string name, Transform t)
    {
        if (t) return t;

        var go = new GameObject(name);
        go.transform.SetParent(transform, false);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        return go.transform;
    }
    private void ClearAllRoots()
    {
        DestroyAndNull(ref body);
         
        DestroyAndNull(ref eyeLeft);
        DestroyAndNull(ref eyeRight);

        DestroyAndNull(ref eyebrowLeft);
        DestroyAndNull(ref eyebrowRight);

        DestroyAndNull(ref armLeft);
        DestroyAndNull(ref armRight);

        DestroyAndNull(ref footLeft);
        DestroyAndNull(ref footRight);

        DestroyAndNull(ref beak);
        DestroyAndNull(ref hair);
    }
    private void DestroyAndNull(ref GameObject go)
    {
        if (!go) return;

        // 🔑 Renderer 먼저 제거
        var renderer = go.GetComponentInChildren<Renderer>(true);
        if (renderer && cachedDuckDetected)
        {
            cachedDuckDetected.RemoveRenderer(renderer);
        }

        Transform root = go.transform.parent;
        if (root)
            Destroy(root.gameObject);
        else
            Destroy(go);

        go = null;
    }
    private void Attach(GameObject spawned, Transform root, ref GameObject target)
    {
        if (!spawned || !root) 
            return;
         
        // 레이어를 Player로 변경 (자식 포함)
        int playerLayer = LayerMask.NameToLayer("Player");
        SetLayerRecursively(spawned, playerLayer);

        spawned.transform.SetParent(root, false);

        var r = spawned.GetComponentInChildren<Renderer>(true);
        if (!r)
            return;

        // DuckDetected에 등록
        if (cachedDuckDetected)
            cachedDuckDetected.AddRenderer(r);

        target = r.gameObject;
    }
    private void SetLayerRecursively(GameObject go, int layer)
    {
        go.layer = layer;

        foreach (Transform child in go.transform)
            SetLayerRecursively(child.gameObject, layer);
    }
}
