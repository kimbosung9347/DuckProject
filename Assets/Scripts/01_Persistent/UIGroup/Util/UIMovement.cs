using UnityEngine.Events;
using UnityEngine;
using System;

public enum UIMoveType
{
    None, 
    Appear,
    Disappear
}

[System.Serializable]
public class UIMoveInfo
{
    public RectTransform target;

    // ===== Appear =====
    public Vector2 appearOffset = Vector2.right * 700f;
    public float appearDelay = 0f;
    public float appearSpeed = 10f;

    // ===== Disappear =====
    public Vector2 disappearOffset = Vector2.right * 700f;
    public float disappearDelay = 0f;
    public float disappearSpeed = 10f;

    // ===== Internal =====
    [HideInInspector] public Vector2 basePos; 
    [HideInInspector] public bool completed = false; 
}  
 
public class UIMovement : MonoBehaviour
{
    [SerializeField] private UIMoveInfo[] rules;
    [SerializeField] private UnityEvent onDisappearComplete;

    private bool active = false;
    private float elapsed = 0f;
    private UIMoveType moveType = UIMoveType.None;
     
    private void Awake()
    {
        foreach (var rule in rules)
        {
            if (rule.target == null) continue;
            rule.basePos = rule.target.anchoredPosition;
            rule.completed = false;
        }
    }

    // ===========================================================
    // APPEAR
    // ===========================================================
    public void PlayAppear()
    {
        active = true;
        elapsed = 0f;
        moveType = UIMoveType.Appear;

        foreach (var rule in rules)
        {
            if (rule.target == null) continue;

            rule.completed = false;
            rule.target.anchoredPosition = rule.basePos + rule.appearOffset;
        }
    }
    public void SetAppearImmediate()
    {
        active = false;
        moveType = UIMoveType.None; 

        foreach (var rule in rules)
        {
            if (rule.target == null)
                continue;

            rule.target.anchoredPosition = rule.basePos;
            rule.completed = true;
        }
    }

    // ===========================================================
    // DISAPPEAR
    // ===========================================================
    public void PlayDisappear()
    {
        active = true;
        elapsed = 0f;
        moveType = UIMoveType.Disappear;
         
        foreach (var rule in rules)
        {
            if (rule.target == null) continue;

            rule.completed = false;
            rule.target.anchoredPosition = rule.basePos;
        }
    }
    public void SetMoveActive(bool _active) 
    {
        active = _active;
    }


    // ===========================================================
    // UPDATE
    // ===========================================================
    private void Update()
    {
        if (!active) 
            return;

        elapsed += Time.deltaTime;

        bool allCompleted = true;

        foreach (var rule in rules)
        {
            if (rule.completed || rule.target == null)
                continue;

            allCompleted = false;

            switch (moveType)
            {
                case UIMoveType.Appear:
                    UpdateAppearRule(rule);
                    break;

                case UIMoveType.Disappear:
                    UpdateDisappearRule(rule);
                    break;
            }
        }

        // 전체 완료 시
        if (allCompleted)
        {
            active = false;

            if (moveType == UIMoveType.Disappear && onDisappearComplete != null)
            {
                onDisappearComplete?.Invoke();
            }

            moveType = UIMoveType.None;
        }
    }

    // ================== Appear ===================
    private void UpdateAppearRule(UIMoveInfo rule)
    {
        if (elapsed < rule.appearDelay)
            return;

        Vector2 target = rule.basePos;

        rule.target.anchoredPosition = Vector2.Lerp(
            rule.target.anchoredPosition,
            target,
            rule.appearSpeed * Time.deltaTime
        );

        if (Vector2.Distance(rule.target.anchoredPosition, target) < 0.5f)
        {
            rule.target.anchoredPosition = target;
            rule.completed = true;
        }
    } 

    // ================== Disappear ===================
    private void UpdateDisappearRule(UIMoveInfo rule)
    {
        if (elapsed < rule.disappearDelay)
            return;

        Vector2 target = rule.basePos + rule.disappearOffset;

        rule.target.anchoredPosition = Vector2.Lerp(
            rule.target.anchoredPosition,
            target,
            rule.disappearSpeed * Time.deltaTime
        );

        if (Vector2.Distance(rule.target.anchoredPosition, target) < 0.5f)
        {
            rule.target.anchoredPosition = target;
            rule.completed = true;
        }
    }
}
