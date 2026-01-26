//using System.Collections.Generic;
//using UnityEngine;

//public class UiGroup : MonoBehaviour 
//{
//    [SerializeField] private List<Canvas> listCanvas;
     
//    protected GameInstance cachedGameInstance;
//    protected Dictionary<string, Canvas> mapCanvas = new();
//    protected KeyValuePair<string, Canvas> activeTopCanvas;
     
//    protected virtual void Awake()
//    {
//        cachedGameInstance = GameInstance.Instance;
//        CacheCanvases();
//    } 
//    protected virtual void OnEnable()
//    {
//    } 
     
//    public Canvas GetCanvas(string name)
//    {
//        if (mapCanvas.TryGetValue(name, out var canvas))
//            return canvas;

//        Debug.LogWarning($"[UiGroup] {name} Canvas를 찾을 수 없습니다.");
//        return null;
//    }
//    public KeyValuePair<string, Canvas> GetCurTopCanvas()
//    {
//        return activeTopCanvas;
//    }
//    public void ActiveCanvas(string key, bool active, bool keepOthersActive = false)
//    {
//        // 전체 비활성화
//        if (!keepOthersActive)
//        {
//            foreach (var canvas in mapCanvas.Values)
//            {
//                if (canvas != null)
//                    canvas.gameObject.SetActive(false);
//            }
//        } 

//        // 해당 캔버스 활성/비활성
//        if (mapCanvas.TryGetValue(key, out var targetCanvas) && targetCanvas)
//        {
//            targetCanvas.gameObject.SetActive(active);
//        } 

//        RenewActiveTopCanvas();
//    }
//    public void DisableCanvas(string key)
//    {
//        if (mapCanvas.TryGetValue(key, out var targetCanvas) && targetCanvas != null)
//        {
//            targetCanvas.gameObject.SetActive(false);
//            RenewActiveTopCanvas();
//        }
//    }

//    public void DisableAllCanvas()
//    {
//        foreach (var canvas in mapCanvas.Values)
//        {
//            canvas.gameObject.SetActive(false);
//        }
//        RenewActiveTopCanvas(); 
//    }
//    public void RenewActiveTopCanvas()
//    {
//        Canvas topCanvas = null;
//        string topKey = string.Empty;
//        int highestOrder = int.MinValue;
//        foreach (var kvp in mapCanvas)
//        {
//            var canvas = kvp.Value;
//            if (canvas == null || !canvas.gameObject.activeSelf)
//                continue;

//            if (canvas.sortingOrder > highestOrder)
//            {
//                highestOrder = canvas.sortingOrder;
//                topCanvas = canvas;
//                topKey = kvp.Key;
//            }
//        }
//        activeTopCanvas = new KeyValuePair<string, Canvas>(topKey, topCanvas);
//    }
     
//    private void CacheCanvases()
//    {
//        mapCanvas.Clear();

//        foreach (var canvas in listCanvas)
//        {
//            if (canvas == null) continue;

//            string key = canvas.name;
//            if (!mapCanvas.ContainsKey(key))
//            {
//                mapCanvas.Add(key, canvas);
//                ActiveCanvas(key, false);
//            }
//        }
//    }
//} 

 