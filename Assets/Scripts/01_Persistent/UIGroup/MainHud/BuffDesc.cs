using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffDesc : MonoBehaviour
{
    [SerializeField] Image buffSprite;
    [SerializeField] TextMeshProUGUI buffName;
    [SerializeField] TextMeshProUGUI buffDuration;

    public void ActiveBuff(Color _color, Sprite _sprite, string _buffName, string _duration)
    {
        buffSprite.color = _color;  
        buffSprite.sprite = _sprite;
        buffName.text = _buffName;
        RenewDuration(_duration);
    }
    public void RenewDuration(string _duration)
    { 
        if (_duration != "0")
        { 
            buffDuration.text = _duration + " ";
        } 
    }
}
 