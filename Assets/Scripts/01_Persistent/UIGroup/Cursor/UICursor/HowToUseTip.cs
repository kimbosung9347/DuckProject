using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HowToUseTip : MonoBehaviour
{
    [SerializeField] GameObject keyAndDescPrefab;
    
    public void Active()
    {
        gameObject.SetActive(true);
        RectTransform rt = (RectTransform)transform;
        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
    }
    public void ClearTip()
    {
        // 자식 UI 전부 제거
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
    public void InsertUseTip(string _key, string _desc)
    {
        GameObject gameObject = Instantiate(keyAndDescPrefab, transform);
        var keyAndDesc = gameObject.GetComponent<HowToUseKeyAndDesc>();
        keyAndDesc.keyText.text = _key;
        keyAndDesc.descText.text = _desc;
    }
      
    public List<(string key, string desc)> GetAllTips()
    {
        List<(string key, string desc)> list = new();

        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i).GetComponent<HowToUseKeyAndDesc>();
            if (child)
                list.Add((child.keyText.text, child.descText.text));
        }

        return list;
    }
}
 