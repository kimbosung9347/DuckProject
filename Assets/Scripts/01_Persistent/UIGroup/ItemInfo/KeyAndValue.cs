using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class KeyAndValue : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textKey;
    [SerializeField] private TextMeshProUGUI textValue;
     
    public void SetKeyText(string _key)
    {
        textKey.text = _key;
    }

    public void SetKeyValue(string _value)
    {
        textValue.text = _value;
    }
}
