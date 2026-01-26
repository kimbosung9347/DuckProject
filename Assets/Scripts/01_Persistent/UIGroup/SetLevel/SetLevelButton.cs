using UnityEngine;
using UnityEngine.UI;

public class SetLevelButton : MonoBehaviour
{
    [SerializeField] private Image lockImage;
    private bool isLock = false;
    public bool IsLock()
    {
        return isLock;
    }
     
    public void SetColor(Color _color)
    {
        var image = GetComponent<Image>();
        image.color = _color;
    }
    public void ActiveLock(bool _isActive)
    {
        lockImage.gameObject.SetActive(_isActive);
        isLock = _isActive; 
    }
}
