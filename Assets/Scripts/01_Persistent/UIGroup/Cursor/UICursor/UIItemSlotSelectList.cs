using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIItemSlotSelectList : MonoBehaviour
{
    [SerializeField] GameObject itemSlotSelectPrefab;

    [SerializeField] Color PickColor;
    [SerializeField] Color DrawColor;
    [SerializeField] Color UseColor;
 
    public void ClearSlotSelect()
    {
        // 자식 UI 전부 제거
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    public void Active(RectTransform target)
    {
        gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
        ApplyPosition(target);
    }  
     
    public void InsertItemSlotSelect(string _key, string _desc)
    {  
        GameObject gameObject = Instantiate(itemSlotSelectPrefab, GetComponent<RectTransform>());
        var ItemSlotSelect = gameObject.GetComponent<UIItemSlotSelect>();
        ItemSlotSelect.keyText.text = _key;
        ItemSlotSelect.descText.text = _desc;
           
        // Pick
        if (_key == "F")
        {
            ItemSlotSelect.imageColor.color = PickColor;
            ItemSlotSelect.SetItemSlotSelectType(EItemSlotSelectType.Pick);
        }
        // Use
        else if (_key == "U") 
        {
            ItemSlotSelect.imageColor.color = UseColor;
            ItemSlotSelect.SetItemSlotSelectType(EItemSlotSelectType.Use);
        }
        // Draw
        else if (_key == "X") 
        {
            ItemSlotSelect.imageColor.color = DrawColor;
            ItemSlotSelect.SetItemSlotSelectType(EItemSlotSelectType.Throw);
        }
    }
     
    private void ApplyPosition(RectTransform target)
    {
        RectTransform self = (RectTransform)transform;

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, target.position);
        bool isRightSide = screenPos.x > Screen.width * 0.5f;
         
        // VerticalLayoutGroup 정렬 보정
        var vlg = GetComponent<VerticalLayoutGroup>();
        if (vlg != null)
        {
            vlg.childAlignment = isRightSide
                ? TextAnchor.MiddleRight   // 왼쪽에 붙을 때
                : TextAnchor.MiddleLeft;   // 오른쪽에 붙을 때
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)self.parent, screenPos, null, out Vector2 targetLocal);

        Vector2 pos = targetLocal;

        float offset = (target.rect.width * 0.5f + self.rect.width * 0.5f);

        if (isRightSide) pos.x -= offset;
        else pos.x += offset;

        pos.y += (target.rect.height * 0.5f);

        self.anchoredPosition = pos;
    }
}
