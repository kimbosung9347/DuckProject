using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestObjective : MonoBehaviour
{ 
    [SerializeField] private Image Imageborder;
    [SerializeField] private Image checkImage;
    [SerializeField] private Image itemSpriteImage;
    [SerializeField] private TextMeshProUGUI itemTextCnt;
     
    public void ActiveBorderImage(bool _isActive)
    {
        Imageborder.gameObject.SetActive(_isActive);
    }
    public void ActiveCheckImage(bool _isActive)
    {
        checkImage.gameObject.SetActive(_isActive);
    }
    public void ActiveSpriteImage(bool _isActive)
    {
        itemSpriteImage.gameObject.SetActive(_isActive);
    }  

    public void RenewSpriteImage(Sprite _sprite)
    {
        itemSpriteImage.sprite = _sprite;  
    }
    public void RenewObjectiveText(string _text)
    {
        itemTextCnt.text = _text;
    } 
}
 