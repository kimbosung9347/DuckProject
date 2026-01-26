//using System.Collections;
//using UnityEngine;

//public class MeleeAttackVisualController : MonoBehaviour
//{
//    [SerializeField] private Renderer targetRenderer;

//    [Header("Shader Properties")]
//    [SerializeField] private string fillProperty = "_Fill";
//    [SerializeField] private string edgeWidthProperty = "_EdgeWidth";

//    [Header("Edge Width")]
//    [SerializeField] private float edgeWidthMin = 0.01f;
//    [SerializeField] private float edgeWidthMax = 0.06f;

//    [Header("Full Charge Flash")]
//    [SerializeField] private float fullFlashFill = 1.15f;   // 순간 오버슈트
//    [SerializeField] private float fullFlashTime = 0.05f;   // 매우 짧게

//    [Header("Reset")]
//    [SerializeField] private float resetSpeed = 8f;

//    private MaterialPropertyBlock mpb;
//    private float curFill;
//    private float curEdgeWidth;
//    private Coroutine routine;

//    private void Awake()
//    {
//        mpb = new MaterialPropertyBlock();
//        SetValues(0f);
//    }

//    public void Active(float fillDuration, float activeTime)
//    {
//        if (routine != null)
//            StopCoroutine(routine);

//        routine = StartCoroutine(FillRoutine(fillDuration, activeTime));
//    } 

//    private IEnumerator FillRoutine(float fillDuration, float activeTime)
//    {
//        curFill = 0f;

//        // ===== Fill 증가 =====
//        while (curFill < 1f)
//        {
//            curFill += Time.deltaTime / Mathf.Max(0.0001f, fillDuration);
//            float t = Mathf.Clamp01(curFill);

//            curEdgeWidth = Mathf.Lerp(edgeWidthMin, edgeWidthMax, t);
//            SetValues(t);

//            yield return null;
//        }

//        // ===== FULL FLASH (임팩트 포인트) =====
//        curEdgeWidth = edgeWidthMax * 1.35f;
//        SetValues(fullFlashFill);
//        yield return new WaitForSeconds(fullFlashTime);

//        // 정상 값으로 복귀
//        curEdgeWidth = edgeWidthMax;
//        SetValues(1f);

//        // ===== 유지 =====
//        yield return new WaitForSeconds(activeTime);

//        // ===== Reset =====
//        while (curFill > 0f)
//        {
//            curFill -= Time.deltaTime * resetSpeed;
//            float t = Mathf.Clamp01(curFill);

//            curEdgeWidth = Mathf.Lerp(edgeWidthMin, edgeWidthMax, t);
//            SetValues(t);

//            yield return null; 
//        }

//        SetValues(0f);
//        routine = null;
//    }

//    private void SetValues(float fillValue)
//    {
//        targetRenderer.GetPropertyBlock(mpb);
//        mpb.SetFloat(fillProperty, fillValue);
//        mpb.SetFloat(edgeWidthProperty, curEdgeWidth);
//        targetRenderer.SetPropertyBlock(mpb);
//    }
//}
