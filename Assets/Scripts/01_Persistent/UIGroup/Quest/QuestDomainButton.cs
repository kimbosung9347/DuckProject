using UnityEngine;

public class QuestDomainButton : MonoBehaviour
{ 
    // Width를 설정해주는 함수 
    // SetColor
       
    public void Active()
    {
        gameObject.SetActive(true);
    }
    public void Disable()
    {
        gameObject.SetActive(false);
    }  

    public void SetButtonWidth(float width)
    {
        RectTransform rt = GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(width, rt.sizeDelta.y);
    } 

}
