using TMPro;
using UnityEngine;

public class ToolTipDescAndValue : MonoBehaviour
{ 
    [SerializeField] private TextMeshProUGUI desc;
    [SerializeField] private TextMeshProUGUI value;
    public void SetInfo(string _desc, string _value)
    {
        desc.text = _desc;
        value.text = _value;
    }
}
