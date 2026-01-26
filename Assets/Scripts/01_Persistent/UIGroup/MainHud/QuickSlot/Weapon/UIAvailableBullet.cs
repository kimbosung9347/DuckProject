using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAvaibleBullet : MonoBehaviour
{
    [SerializeField] Color activeColor;
    [SerializeField] Image bulletBackground;
    [SerializeField] TextMeshProUGUI bulletName;
    [SerializeField] TextMeshProUGUI bulletCnt;
    [SerializeField] GameObject interfaceT;

    private void Awake()
    { 
    } 
     
    public void RenewBulletName(string _name, int _cnt)
    {
        bulletName.text = _name;

        if (_cnt == 0)
        {
            bulletCnt.text = string.Empty;
        } 
        else
        {
            bulletCnt.text = _cnt.ToString();
        } 
    } 
    public void Select()
    {
        interfaceT.SetActive(true);
        bulletBackground.color = activeColor;
    }
    public void NotSelect() 
    {
        interfaceT.SetActive(false);
        bulletBackground.color = new Color(95f / 255f, 95f / 255f, 95f / 255f, 1f);
    } 
}
 