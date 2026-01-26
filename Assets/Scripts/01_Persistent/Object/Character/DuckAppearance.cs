using UnityEngine;

public class DuckAppearance : MonoBehaviour
{
    [SerializeField] private GameObject hair;
     
    [SerializeField] private Transform weaponSocket;
    [SerializeField] private Transform bodySocket;
    [SerializeField] private Transform useConsumSocket;
    
    private Weapon attachWeapon;
    private Armor attachHelmat;
    private Armor attachArmor;
    private Backpack attachBackpack;
    private UseConsumBase attachConsum;

    public void AttachWeapon(Weapon _weapon) 
    { 
        if (_weapon == null)
            return;  
         
        attachWeapon = _weapon;
        attachWeapon.transform.SetParent(weaponSocket);      
        attachWeapon.transform.localPosition = Vector3.zero;
        attachWeapon.transform.localRotation = Quaternion.identity;
        attachWeapon.Attach();
    }   
    public void DetachWeapon()
    {
        if (attachWeapon == null)
            return;
           
        attachWeapon.transform.SetParent(null);
        attachWeapon.Detach();
        attachWeapon = null; 
    }

    public void AttachHelmat(Armor Helmat)
    {
        if (Helmat == null)
            return; 

        attachHelmat = Helmat;
        attachHelmat.transform.SetParent(bodySocket);
        attachHelmat.transform.localPosition = Vector3.zero;
        attachHelmat.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
         
        attachHelmat.Attach(); 
    }
    public void DetachHelmat()
    {
        if (attachHelmat == null)
            return;
         
        attachHelmat.transform.SetParent(null);
        attachHelmat.Detach();
        attachHelmat = null;
    }
     
    public void AttachArmor(Armor armor)
    {
        if (armor == null)
            return;

        attachArmor = armor;
        attachArmor.transform.SetParent(bodySocket);
        attachArmor.transform.localPosition = Vector3.zero;
        attachArmor.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
         
        attachArmor.Attach(); 
    }
    public void DetachArmor()
    {
        if (attachArmor == null)
            return;

        attachArmor.transform.SetParent(null);
        attachArmor.Detach();
        attachArmor = null;
    }

    public void AttachBackpack(Backpack backpack)
    {
        if (backpack == null)
            return;
          
        attachBackpack = backpack;
        attachBackpack.transform.SetParent(bodySocket);
        attachBackpack.transform.localPosition = Vector3.zero;
        attachBackpack.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);


        attachBackpack.Attach();
    } 
    public void DetachBackpack()
    {
        if (attachBackpack == null)
            return;

        attachBackpack.transform.SetParent(null);
        attachBackpack.Detach();
        attachBackpack = null;
    }
     
    public void AttachConsum(UseConsumBase _consum)
    {
        if (_consum == null)
            return;

        attachConsum = _consum;
        attachConsum.transform.SetParent(useConsumSocket);
        attachConsum.transform.localPosition = Vector3.zero;
        attachConsum.transform.localRotation = Quaternion.identity;
        attachConsum.Attach();
    }
    public void DetachConsum()
    {
        if (attachConsum == null)
            return;

        attachConsum.transform.SetParent(null);
        attachConsum.Detach(); 
        attachConsum = null;
    } 

     
    public void ShowHair()
    {
        hair.gameObject.SetActive(true);
    }
    public void HideHair()
    {
        hair.gameObject.SetActive(false);
    }
}  
