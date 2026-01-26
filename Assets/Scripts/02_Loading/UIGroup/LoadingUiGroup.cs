using UnityEngine;

public class LoadingUiGroup : MonoBehaviour
{ 
    // protected override void Awake() 
    // {
    //     base.Awake();
    //     cachedGameInstance = GameInstance.Instance;
    // } 
    // 
    // private void Start()
    // {
    //     ELoadingScreenType screenType = cachedGameInstance.LOADING_GetReserveLoadingScreenType();
    //     switch (screenType)
    //     {
    //         case ELoadingScreenType.Default:
    //         { 
    //             ActiveCanvas("DefaultCanvas", true, false);
    //         }
    //         break;  
    //     }
    //       
    //     cachedGameInstance.LOADING_LoadNextLevelAsync();
    // }
    // 
    // public void OnPressLeftMouse()
    // {
    //     if (!cachedGameInstance.LOADING_IsComplateLoad())
    //         return;
    // 
    //     var Canvas = activeTopCanvas.Value;
    //     var LoadingCanvas = Canvas.GetComponent<LoadingCanvas>();
    //     LoadingCanvas.Action_StartNextLevel();
    // } 
} 
 