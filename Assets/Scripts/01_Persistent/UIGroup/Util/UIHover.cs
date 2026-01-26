using UnityEngine;
using UnityEngine.EventSystems;
 
enum EMainmenuHoverEvent
{ 
    Bright, 
    Animation,
     
    End,  
}
 
public class UIHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private EMainmenuHoverEvent hoverEvent = EMainmenuHoverEvent.End;

    private UIBright cachedBright;
    private UIAnimation cachedUIAnimation;
     
    private void Awake() 
    {
        cachedBright = GetComponent<UIBright>();
        cachedUIAnimation = GetComponent<UIAnimation>();
    }  
     
    public void OnPointerEnter(PointerEventData eventData)
    {
        switch (hoverEvent)
        {
            case EMainmenuHoverEvent.Bright:
            {
                ProcessHoverBright();
            } 
            break;
            case EMainmenuHoverEvent.Animation:
            {
                ProcessHoverAnimation();
            } 
            break; 

        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        switch (hoverEvent)
        {
            case EMainmenuHoverEvent.Bright:
                {
                    OffHoverBright();
                }
                break;
        }
    }
     
    private void ProcessHoverBright()
    {
        cachedBright.RenewBrightness(true);
    }
    private void ProcessHoverAnimation()
    {
        cachedUIAnimation.Action_Animation();
    }
      
    private void OffHoverBright()
    {
        cachedBright.RenewBrightness(false);
    } 
}
