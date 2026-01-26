using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CursorToolTip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemWeight;
    [SerializeField] private TextMeshProUGUI itemDesc;
    [SerializeField] private RectTransform itemToolTip;
    [SerializeField] private RectTransform howToUse;
     
    [SerializeField] private Vector2 offset;

    // Prefab
    [SerializeField] GameObject itemDescAndValuePrefab;
    private readonly List<ToolTipDescAndValue> slotList = new();

    private void Awake()
    {

    } 
    public void LateUpdate()
    {
        RectTransform self = transform as RectTransform;
        Canvas canvas = GetComponentInParent<Canvas>();
        const float spacing = 5f;

        // 기본 위치 = 마우스 + 오프셋
        Vector2 mouse = Mouse.current.position.ReadValue();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            mouse + offset,
            canvas.worldCamera,
            out Vector2 localPos);

        self.anchoredPosition = localPos;

        // itemToolTip 우상단 (HowToUse 붙을 위치)
        float tw = itemToolTip.rect.width;
        float th = itemToolTip.rect.height;
        float dx = tw * (1f - itemToolTip.pivot.x);
        float dy = th * (1f - itemToolTip.pivot.y);

        Vector2 topRightLocal = itemToolTip.anchoredPosition + new Vector2(dx + spacing, dy);

        // HowToUse 우측 끝까지 고려한 screen 좌표 계산
        float howW = howToUse.rect.width;
        float howRightOffset = howW * (1f - howToUse.pivot.x);
        Vector3 worldRight = howToUse.TransformPoint(new Vector3(howRightOffset, 0, 0));
        Vector2 screenRight = RectTransformUtility.WorldToScreenPoint(null, worldRight);

        // 오른쪽 보정
        if (screenRight.x > Screen.width)
        {
            self.anchoredPosition += new Vector2(Screen.width - screenRight.x, 0f);
        } 
        
        // 아래 보정
        float tipH = itemToolTip.rect.height;
        float tipBottomOffset = tipH * itemToolTip.pivot.y;

        // itemToolTip 하단 월드 좌표
        Vector3 worldBottom = itemToolTip.TransformPoint(
            new Vector3(0f, -tipBottomOffset, 0f)
        );

        // screen 좌표
        Vector2 screenBottom = RectTransformUtility.WorldToScreenPoint(null, worldBottom);

        // 화면 아래로 넘친 만큼 위로 보정
        if (screenBottom.y < 0f)
        {
            self.anchoredPosition += new Vector2(0f, -screenBottom.y);
        }

        // 최종 HowToUse 부착
        howToUse.anchoredPosition = itemToolTip.anchoredPosition + new Vector2(dx + spacing, dy);
    }

    public void RenewToopTipInfo(EItemID _id)
    {
        // 글자 갱신
        {
            EItemType itemType = DuckUtill.GetItemTypeByItemID(_id);
            ItemPair itemPair = DuckUtill.GetItemPair(_id);

            var itemData = itemPair.data;

            // 이름
            itemName.text = itemData.itemName;
             
            // 무게
            itemWeight.text = itemData.weight.ToString() + "kg";

            // 아이템 설명
            itemDesc.text = itemData.itemDesc;

            ClearSlots();

            // 추가 설명, 
            int i = 0;
            foreach (var stat in itemData.GetStats())
            {
                AddStat(stat.key, stat.value, ref i);
            }
        } 

         
        // 사용 툴팁 갱신
    } 
      
    private void ClearSlots()
    {
        for (int i = 0; i < slotList.Count; i++)
            slotList[i].gameObject.SetActive(false);
    }
     
    private ToolTipDescAndValue GetSlot(int index)
    {
        // 기존 슬롯 재활용
        if (index < slotList.Count)
        {
            slotList[index].gameObject.SetActive(true);
            return slotList[index];
        } 
          
        // 부족하면 새로 생성 
        GameObject go = Instantiate(itemDescAndValuePrefab, itemToolTip.transform);
        go.SetActive(true);

        var slot = go.GetComponent<ToolTipDescAndValue>();
        slotList.Add(slot);

        return slot;
    }
    private void AddStat(string desc, string value, ref int idx)
    {
        var slot = GetSlot(idx);
        slot.SetInfo(desc, value);
        idx++;
    }
}  
