using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InteractionGuage : MonoBehaviour
{
    [SerializeField] Image guageImage;

    private void Awake()
    {
        guageImage.material = new Material(guageImage.material); 
    }
    private void Start()
    {
        gameObject.SetActive(false);
    } 
    public void RenewGuage(float _ratio)
    {
        guageImage.material.SetFloat("_ratio", _ratio);

        if (_ratio >= 1)
        {
            gameObject.SetActive(false);
        } 
    } 
}
 