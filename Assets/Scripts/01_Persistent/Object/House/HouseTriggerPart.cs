using UnityEngine;

public enum EHouseTriggerType
{
    HideRoof,
    HideProb
}

[RequireComponent(typeof(BoxCollider))]
public class HouseTriggerPart : MonoBehaviour
{
    [SerializeField] private EHouseTriggerType type;

    private DuckHouse house;
    private Collider myCollider;

    private void Awake()
    {
        house = GetComponentInParent<DuckHouse>();
        myCollider = GetComponent<Collider>();
        myCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    { 
        switch (type)
        {
            case EHouseTriggerType.HideRoof:
                house.EnterHideRoof(myCollider);
                break;

            case EHouseTriggerType.HideProb:
                house.EnterHideProb(myCollider, other);
                break;
        } 
    }  

    private void OnTriggerExit(Collider other)
    { 
        switch (type)
        {
            case EHouseTriggerType.HideRoof:
                house.ExitHideRoof(myCollider);
                break;

            case EHouseTriggerType.HideProb:
                house.ExitHideProb(myCollider, other);
                break;
        }
    } 
}
