using UnityEngine;
using UnityEngine.UI;
 
public class DirectionsAimCursor : CursorAimBase
{
    [SerializeField] private Image leftRect; 
    [SerializeField] private Image upRect;
    [SerializeField] private Image rightRect;
    [SerializeField] private Image downRect;
    [SerializeField] private Image middle; 
    protected override void UpdateGather()
    {
        base.UpdateGather(); 

        Vector2 leftTarget = new Vector2(-accRange, 0f); 
        Vector2 rightTarget = new Vector2(accRange, 0f);
        Vector2 upTarget = new Vector2(0f, accRange);
        Vector2 downTarget = new Vector2(0f, -accRange);
          
        // 즉시 적용
        if (leftRect) leftRect.rectTransform.anchoredPosition = leftTarget;
        if (rightRect) rightRect.rectTransform.anchoredPosition = rightTarget;
        if (upRect) upRect.rectTransform.anchoredPosition = upTarget;
        if (downRect) downRect.rectTransform.anchoredPosition = downTarget;
    }   
    protected override void SetImageAlpha(float alpha)
    {
        void ApplyAlpha(Image img)
        {
            if (img == null) return;
            Color c = img.color;
            c.a = alpha;
            img.color = c;
        }

        ApplyAlpha(leftRect);
        ApplyAlpha(upRect);
        ApplyAlpha(rightRect);
        ApplyAlpha(downRect);
        ApplyAlpha(middle);
    }
}
