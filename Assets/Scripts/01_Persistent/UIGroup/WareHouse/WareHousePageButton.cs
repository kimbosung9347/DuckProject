using TMPro;
using UnityEngine;
using UnityEngine.UI;
 
public class WareHousePageButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonIndexText;

    private int pageNum; 
    private System.Action<int> onClick;
     
    public void Init(int _pageNum, System.Action<int> _onClick)
    {
        pageNum = _pageNum; 
        onClick = _onClick;
           
        buttonIndexText.text = (pageNum+1).ToString();
        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(OnClick);
    }  

    public void SetColor(Color _color)
    { 
        GetComponent<Image>().color = _color;
    }
     
    private void OnClick()
    {
        onClick?.Invoke(pageNum);
    }
}
