using System.Collections.Generic;
using TMPro; 
using UnityEngine;
using UnityEngine.UI;

public class QuickWeaponSlot : MonoBehaviour
{ 
    [SerializeField] GameObject availableBulletList;
    [SerializeField] GameObject availableBulletPrefab;
     
    [SerializeField] GameObject bulletCnt;
    [SerializeField] TextMeshProUGUI curBulletText;
    [SerializeField] TextMeshProUGUI maxBulletText;

    [SerializeField] GameObject bulletType;
    [SerializeField] GameObject buleltTypeInterface;
     
    [SerializeField] TextMeshProUGUI buleltTypeInterfaceName;
    [SerializeField] TextMeshProUGUI bulletName; 
     
    [SerializeField] Image borderImage;
    [SerializeField] Image itemImage;

    private Dictionary<EItemID, KeyValuePair<UIAvaibleBullet, int>> hashAvaiablieBullet = new();
     
    private void Awake()
    {
        DetetchWeapon();
         
        InitMaterial();

        buleltTypeInterface.SetActive(true);
    }     
     
    public void EquipWeapon(EItemID _itemId)
    {
        borderImage.gameObject.SetActive(true);
        bulletType.SetActive(true);
        bulletCnt.SetActive(true);
         
        itemImage.gameObject.SetActive(true);
        itemImage.sprite = DuckUtill.GetItemSprite(_itemId);
    }   
    public void DetetchWeapon()
    {
        borderImage.gameObject.SetActive(false);
        itemImage.gameObject.SetActive(false);

        bulletCnt.SetActive(false);  
        bulletType.SetActive(false); 
        // 인스턴스된 유아이들을 모두 제거해줄예정 
    } 

    /* AvaiableList */
    public void EnterAvaiableBullet()
    {
        buleltTypeInterfaceName.text = "X";
        availableBulletList.SetActive(true);
    }  
    public void ExitAvaiableBullet()
    {
        ClearAvaiableBullet();

        buleltTypeInterfaceName.text = "T";
        availableBulletList.SetActive(false);
    }
     
    public void InsertAvaiableBullet(EItemID _id ,int _cnt)
    {          
        // 이미 있으면 Insert가 아니라 Renew 해야 함
        if (hashAvaiablieBullet.ContainsKey(_id))
        {
            RenewAvaiableBulletCnt(_id, _cnt);
            return;
        }

        // UI 생성
        GameObject obj = Instantiate(availableBulletPrefab, availableBulletList.transform);
        var ui = obj.GetComponent<UIAvaibleBullet>();

        if (_id == EItemID._END)
        { 
            ui.RenewBulletName("탄약 없음", _cnt);
        }
          
        else
        {
            ui.RenewBulletName(DuckUtill.GetItemPair(_id).data.itemName, _cnt);
        }

        ui.NotSelect();
        obj.SetActive(true);

        // hash에 등록
        hashAvaiablieBullet[_id] = new KeyValuePair<UIAvaibleBullet, int>(ui, _cnt);
    }
    public void RenewAvaiableBulletCnt(EItemID _id, int _cnt)
    {
        if (!hashAvaiablieBullet.TryGetValue(_id, out var data))
            return;
         
        UIAvaibleBullet ui = data.Key;
        ui.RenewBulletName(DuckUtill.GetItemPair(_id).data.itemName, _cnt);

        // hash 갱신
        hashAvaiablieBullet[_id] = new KeyValuePair<UIAvaibleBullet, int>(ui, _cnt);
    }
    public void RemoveAvaiableBullet(EItemID _id)
    {
        if (!hashAvaiablieBullet.TryGetValue(_id, out var data))
            return;
         
        UIAvaibleBullet ui = data.Key;
        // UI 제거
        if (ui != null)
            Destroy(ui.gameObject);
         
        // hash 삭제
        hashAvaiablieBullet.Remove(_id);
    }
    public void SelectBullet(EItemID _itemId)
    { 
        if (hashAvaiablieBullet.TryGetValue(_itemId, out var bulletInfo))
        {
            bulletInfo.Key.Select();
        }
    }
    public void NotSelect(EItemID _itemId)
    {
        if (hashAvaiablieBullet.TryGetValue(_itemId, out var bulletInfo))
        {
            bulletInfo.Key.NotSelect();
        }
    } 
     
    /* BulletCnt ex) 100 / 200 */
    public void RenewWeaponBulletType(EItemID _itemId)
    {
        if (_itemId == EItemID._END)
        {
            bulletName.text = "탄약 미착용";
        }  
          
        else
        {
            bulletName.text = DuckUtill.GetItemPair(_itemId).data.itemName;
        }
    }  
    public void RenewCurBullet(int _cnt)
    {
        curBulletText.text = _cnt.ToString();
 
        if (_cnt == 0)
        {
            bulletCnt.GetComponent<Image>().color = new Color(200 / 255f, 0f, 0f, 1f);

            // 현재 총알과 max총알이 없다면 - 이름바꿔주기
            // 아이템에서 갱신해주는게 맞다고 보지만, 간단하게 이렇게 해도 문제는 없을듯 
            if (maxBulletText.text == "0")
            {
                bulletName.text = "탄약 미착용";
            } 
        }  
        else
        {
            bulletCnt.GetComponent<Image>().color = Color.black;
        } 
    }
    public void RenewMaxBullet(int _cnt)
    {
        maxBulletText.text = _cnt.ToString();
    } 
     
    private void InitMaterial()
    {
        borderImage.material = new Material(borderImage.material);
        Color color = Color.white;
        color.a = 0;
        borderImage.material.SetColor("_baseColor", color);
    }
    private void ClearAvaiableBullet()
    {
        // 기존 삭제  
        foreach (var pair in hashAvaiablieBullet)
        {
            if (pair.Value.Key != null) 
                Destroy(pair.Value.Key.gameObject);
        } 
        hashAvaiablieBullet.Clear();
    }
}
    