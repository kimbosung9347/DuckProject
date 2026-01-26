using System.Reflection;
using UnityEngine;
using UnityEngine.Splines.ExtrusionShapes;
using UnityEngine.UI;

public class CircleAimCursor : CursorAimBase
{ 
    [SerializeField] private Image circle;
     
    protected override void UpdateGather()
    { 
        base.UpdateGather();
         
        RectTransform rt = circle.GetComponent<RectTransform>();
        float diameter = accRange * 2f;
        rt.sizeDelta = new Vector2(diameter, diameter);
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
         
        ApplyAlpha(circle);
    } 
}
