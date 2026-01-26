//using System;
//using UnityEngine;

//public enum EUIEffectType
//{
//    GrewBigger,
//    End,
//}

//[System.Serializable]
//public struct FUIEffectInfo_GrewBigger
//{
//    [Tooltip("커질 때 배율")]
//    public float growScaleRatio;
//    [Tooltip("전체 재생 시간")]
//    public float totalTime;
//}
//public class UIEffect<T> : MonoBehaviour
//{
//    [SerializeField] private EUIEffectType uiEffectType = EUIEffectType.End;

//    [Header("EffectInfo")]
//    [SerializeField] private FUIEffectInfo_GrewBigger info_GrewBigger;

//    private Action<T> onCompleteWithArg;
//    private Action onCompleteNoArg;
//    private T completeArg;

//    private RectTransform cachedRectTransform;
//    private Vector3 startScale = Vector3.one;
     
//    private bool bIsActive = false;
//    private float timer = 0f;

//    private void Awake()
//    {
//        InitEffect(uiEffectType);
//    }
//    private void Update()
//    {
//        if (!bIsActive)
//            return;

//        switch (uiEffectType)
//        {
//            case EUIEffectType.GrewBigger:
//                Update_GrewBigger();
//                break;
//        }
//    }
     
//    public void SetOnComplete(Action<T> callback, T arg)
//    {
//        onCompleteWithArg = callback;
//        completeArg = arg;
//        onCompleteNoArg = null; // 혼동 방지
//    }
//    public void SetOnComplete(Action callback)
//    {
//        onCompleteNoArg = callback;
//        onCompleteWithArg = null;
//    }
//    public void ActiveEffect(EUIEffectType _type)
//    {
//        ChangeEffectType(_type);
//        bIsActive = true;
//        timer = 0f;
//    }

//    /////////////////////////
//    private void ChangeEffectType(EUIEffectType _type)
//    {
//        if (uiEffectType == _type)
//            return;

//        uiEffectType = _type;
//        InitEffect(uiEffectType);
//    }
//    /* Init */
//    private void InitEffect(EUIEffectType _type)
//    {
//        switch (_type)
//        {
//            case EUIEffectType.GrewBigger:
//                InitGrewBigger();
//                break;
//        }
//    }
//    private void InitGrewBigger()
//    {
//        cachedRectTransform = GetComponent<RectTransform>();
//        startScale = cachedRectTransform.localScale;
//    }

//    /* Update */
//    private void Update_GrewBigger()
//    {
//        timer += Time.unscaledDeltaTime;
//        float half = info_GrewBigger.totalTime * 0.5f;
//        float ratio;

//        if (timer <= half)
//        {
//            // 커지는 구간
//            ratio = Mathf.Lerp(1f, info_GrewBigger.growScaleRatio, timer / half);
//        }
//        else
//        {
//            // 원래로 돌아오는 구간
//            float t = (timer - half) / half;
//            ratio = Mathf.Lerp(info_GrewBigger.growScaleRatio, 1f, t);
//        }

//        cachedRectTransform.localScale = startScale * ratio;

//        if (timer >= info_GrewBigger.totalTime)
//        {
//            CloseEffect();
//        }
//    }

//    /* Clear */ 
//    private void CloseEffect()
//    {
//        bIsActive = false;
//        timer = 0f;
//        ClearEffect();

//        // 콜백 실행
//        if (onCompleteWithArg != null)
//            onCompleteWithArg.Invoke(completeArg);
//        else
//            onCompleteNoArg?.Invoke();
         
//        // 안전한 초기화
//        onCompleteWithArg = null;
//        onCompleteNoArg = null;
//        completeArg = default;
//    } 
//    private void ClearEffect()
//    {
//        switch (uiEffectType)
//        {
//            case EUIEffectType.GrewBigger:
//                ClearGrewBigger();
//                break;
//        }
//    }
//    private void ClearGrewBigger()
//    {
//        cachedRectTransform.localScale = startScale;
//    }
//}
