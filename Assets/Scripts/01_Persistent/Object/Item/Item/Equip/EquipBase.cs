using UnityEngine;
public enum EEquipSlot
{
    Weapon,
    Helmet, 
    Armor,
    Backpack,

    End,
}  

public class EquipBase : ItemBase
{
    protected DuckEquip cachedDuckEquip;
     
    // 장착할 수 있는 ItemSlot 자식에서 초기화해야함 
    protected EEquipSlot equipSlot = EEquipSlot.End;
    
    // 내구도 
    protected float curDurability = 0f;
    protected GameObject meshPrefab;

    private bool isUseDuration = true;
    private EquipData equipData;
    private EquipVisualData equipVisualData;

    public override void Init(ItemData _data, ItemVisualData _visual)
    {
        base.Init(_data, _visual);

        // 장비 전용 데이터 초기화,
        // Mesh 같은 것들..
        InitEquipData(_data);
        InitEquipVisualData(_visual);
    }  
    public override void Attach()
    {
        base.Attach();
         
        // 빌보드는 끄고  
        billboardObject.SetActive(false);
        // mesh를 보이게 해줘야함
        meshPrefab.SetActive(true);
    }  
    public override void Detach()
    {
        base.Detach();

        // mesh를 숨겨줘야함
        meshPrefab.SetActive(false);  
    } 
    public override int GetPrice()
    {
        float multiplier = Mathf.Lerp(0.2f, 1f, GetCurDurabilityRatio());
        return Mathf.RoundToInt(base.GetPrice() * multiplier);
    }
     
    public override FItemShell GetItemShell()
    {  
        FItemShell entry = base.GetItemShell();
        if (isUseDuration)
        {
            entry.durabilityRatio = GetCurDurabilityRatio();
        } 
        return entry; 
    }
    public virtual void SetCurDrabilityRtio(float _durabilityRatio)
    {
        curDurability = equipData.maxDurability * _durabilityRatio;
    }

    public GameObject GetMesh()
    {
        return meshPrefab;
    } 
    public float GetCurDurabilityRatio()
    {
        return curDurability / equipData.maxDurability;
    } 
    public bool IsUseDuration() { return isUseDuration; }
    public void CacheDuckEquip(DuckEquip _equip)
    {
        cachedDuckEquip = _equip;
    }
     
    protected void ReduceDurability()
    {
        if (equipData.reduceDurability <= 0f)
            return; 
         
        curDurability -= equipData.reduceDurability;
        if (curDurability < 0f)
            curDurability = 0f;
    }
    private void InitEquipData(ItemData _data)
    {
        equipData = _data as EquipData;
        if (equipData)
        {
            curDurability = equipData.maxDurability;
        }
        
        equipSlot = DuckUtill.GetEquipTypeByItemID(_data.itemID);
        if (equipSlot == EEquipSlot.Backpack)
        {
            isUseDuration = false;
        }
     
    }
    private void InitEquipVisualData(ItemVisualData _data)
    {
        // Equip은 장착되므로 어떤 Mesh정보가 필요함
        equipVisualData = _data as EquipVisualData;
        if (equipVisualData && equipVisualData.equipMeshPrefab)
        {
            meshPrefab = Instantiate(equipVisualData.equipMeshPrefab);
            meshPrefab.transform.SetParent(transform);
            meshPrefab.transform.localPosition = Vector3.zero;
            meshPrefab.transform.localRotation = Quaternion.identity;
            meshPrefab.SetActive(false);
        } 
    } 
}  
 