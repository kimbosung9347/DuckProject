using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIQeustReward : MonoBehaviour
{ 
    [SerializeField] private Image itemSprite;
    [SerializeField] private TextMeshProUGUI rewaredText;
    public void Init(string _rewaredText, Sprite _itemSprite = null)
    {  
        if (_itemSprite)
        {
            itemSprite.gameObject.SetActive(true);
            itemSprite.sprite = _itemSprite;
        }
        else
        {
            itemSprite.gameObject.SetActive(false);
        }
         
        rewaredText.text = _rewaredText;
    }
}
