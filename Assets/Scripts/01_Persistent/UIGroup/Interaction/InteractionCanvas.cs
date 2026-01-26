using UnityEngine;

public class InteractionCanvas : MonoBehaviour
{
    [Header("상호작용게이지")]
    [SerializeField] private InteractionGuage interactionGuage;
      
    ////////////////////////
    /* Interaction Guage */
    public void ActiveInteractionGuage()
    {
        interactionGuage.gameObject.SetActive(true);
        RenewInteactionGuage(0);
    }
    public void DisableInteractioknGuage()
    {
        interactionGuage.gameObject.SetActive(false);
    }
    public void RenewInteactionGuage(float _gauge)
    {
        interactionGuage.RenewGuage(_gauge);
    }

}
