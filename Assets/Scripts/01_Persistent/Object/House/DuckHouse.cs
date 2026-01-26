using UnityEngine;
using System.Collections.Generic;

public class DuckHouse : MonoBehaviour
{
    [Header("Roof")]
    [SerializeField] private HouseHideRoof cachedRoof;

    [Header("Hide Objects")]
    [SerializeField] private Transform hideProbTransform;

    [Header("Door Objects")]
    [SerializeField] private Transform doorParentTransform;
     
    private int houseId = 0;

    private readonly HashSet<Collider> activeHideRoof = new();
    private readonly HashSet<Collider> activeHideProb = new();

    // 캐시
    private readonly List<GameObject> listhideProb = new();
    private readonly List<HandleDoor> listDoor = new();

    private void Awake()
    {
        if (!cachedRoof)
            cachedRoof = GetComponentInChildren<HouseHideRoof>(true);

        CacheHideProbRenderers();
        CacheDoors();
         
        SetProbVisible(false); // 기본 상태
    }

    // ======================
    // Id
    // ======================
    public int GetId() { return houseId; }
    public void SetId(int _id) { houseId = _id; }

    // ======================
    // Roof
    // ======================
    public void EnterHideRoof(Collider trigger)
    {
        cachedRoof.SetRoofVisible(false);
    }
    public void ExitHideRoof(Collider trigger)
    {
        cachedRoof.SetRoofVisible(true);
    }
     
    // ====================== 
    // Prob / Objects
    // ====================== 
    public void EnterHideProb(Collider trigger, Collider _other)
    {
        var detected = _other.GetComponent<DuckDetected>();
        if (detected)
            detected.CacheDuckHouseId(GetId());

        if (detected is not AiDetected aiDetected)
        {
            SetProbVisible(true);
        }
    
    }  
    public void ExitHideProb(Collider trigger, Collider _other)
    {
        var detected = _other.GetComponent<DuckDetected>();
        if (detected)  
            detected.DisableDuckHouseId();

        if (detected is not AiDetected aiDetected)
        {
            SetProbVisible(false);
        }
    }    
    private void SetProbVisible(bool visible)
    {
        for (int i = 0; i < listhideProb.Count; i++)
        {
            if (listhideProb[i])
                listhideProb[i].SetActive(visible);
        }
    }

    // ======================
    // Door
    // ======================
    public HandleDoor GetNearestDoor(Transform _transform)
    {
        if (listDoor.Count == 0 || !_transform)
            return null;

        HandleDoor nearest = null;
        float minSqr = float.MaxValue;
        Vector3 pos = _transform.position;

        for (int i = 0; i < listDoor.Count; i++)
        {
            var door = listDoor[i];
            if (!door)
                continue;

            float sqr = (door.transform.position - pos).sqrMagnitude;
            if (sqr < minSqr)
            {
                minSqr = sqr;
                nearest = door;
            }
        }

        return nearest;
    }
    public HandleDoor GetNearestDoor(Vector3 _pos)
    {
        if (listDoor.Count == 0)
            return null;

        HandleDoor nearest = null;
        float minSqr = float.MaxValue;

        for (int i = 0; i < listDoor.Count; i++)
        {
            var door = listDoor[i];
            if (!door)
                continue;

            float sqr = (door.transform.position - _pos).sqrMagnitude;
            if (sqr < minSqr)
            {
                minSqr = sqr;
                nearest = door;
            }
        }

        return nearest;
    }

    // ======================
    // Cache
    // ====================== 
    private void CacheHideProbRenderers()
    {
        listhideProb.Clear();

        if (!hideProbTransform)
            return;

        var transforms = hideProbTransform.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < transforms.Length; i++)
        {
            if (transforms[i] != hideProbTransform)
                listhideProb.Add(transforms[i].gameObject);
        }
    }
    private void CacheDoors()
    {
        listDoor.Clear();

        if (!doorParentTransform)
            return;

        doorParentTransform.GetComponentsInChildren(true, listDoor);
    }

}
