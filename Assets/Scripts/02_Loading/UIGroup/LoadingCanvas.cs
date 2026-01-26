using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Threading.Tasks;

public enum ELoadingScreenType
{
    LobbyToHome,
    SallyForth, // 출격
    Withdraw,   // 철수
    Dead,       // 사망
    End,
}

public class LoadingCanvas : MonoBehaviour
{ 
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private TextMeshProUGUI lodingTipText;
    [SerializeField] private ELoadingScreenType loadingScreenType = ELoadingScreenType.End;

    [SerializeField] private GameObject defaultScreen;
    [SerializeField] private GameObject sallyForth;
    [SerializeField] private GameObject withDraw;
    [SerializeField] private GameObject dead;

    private GameInstance cachedGameInstance;

    private bool isLoadCompleteHandled = false;
    private bool isPostDelayExecuted = false;

    private Dictionary<ELoadingScreenType, Func<IEnumerator>> postLoadActions;
    private Coroutine postProcessRoutine;
      
    private void Awake()
    {
        defaultScreen?.SetActive(false);
        sallyForth?.SetActive(false);
        withDraw?.SetActive(false);
        dead?.SetActive(false);

        cachedGameInstance = GameInstance.Instance;
        var ui = cachedGameInstance.UI_GetPersistentUIGroup();
        ui.GetDisableCanvas().ActiveFadeOut();
        ui.GetRadiusCollaspeCanvas().Disable();
        ui.GetCursorCanvas().GetUICursor().gameObject.SetActive(false);
        ui.GetCursorCanvas().GetPlayCursor().gameObject.SetActive(false);

        loadingScreenType = cachedGameInstance.LOADING_GetReserveLoadingScreenType();
        InitPostLoadTable();



        loadingText.text = "로딩 중";
    }

    private void InitPostLoadTable()
    {
        postLoadActions = new()
        {
            { ELoadingScreenType.LobbyToHome, Co_LobbyToHome },
            { ELoadingScreenType.SallyForth,  Co_SallyForth  },
            { ELoadingScreenType.Withdraw,    Co_Withdraw    },
            { ELoadingScreenType.Dead,        Co_Dead        },
            { ELoadingScreenType.End,         Co_End         },
        }; 
    }  

    private void Update()
    {
        // 로딩 완료 여부 체크
        if (cachedGameInstance && !cachedGameInstance.LOADING_IsComplateLoad())
            return;

        // 비동기 적으로 진행되는 세이브 및 로드가 완료되었는지 여부체크
        if (!isLoadCompleteHandled)
        {
            isLoadCompleteHandled = true;

            if (postLoadActions.TryGetValue(loadingScreenType, out var routine))
                postProcessRoutine = StartCoroutine(routine());

            return;
        }

        // 로드가 끝나도 약간 텀을 주기, 로딩 씬이 너무 빨리 끝나면 아쉬워서
        if (isPostDelayExecuted)
            return;
    }

    // ======================
    // 타입별 처리
    // ======================

    private IEnumerator Co_LobbyToHome()
    {
        lodingTipText.text = "열심히 하겠습니다!";
        defaultScreen.gameObject.SetActive(true);

        // 여기서 플레이어를 만들기 
        MakePlayer();

        // 창고는 영구씬에 있어서, 리펙토링 하면 좋긴한데 굳이 안할꺼임
        WareHouse wareHouse = GameInstance.Instance.PLAYER_GetWareHouse();
        wareHouse.Init();
         
        yield return new WaitForSeconds(2f);
        OnPostProcessFinished();  
    }  
     
    private IEnumerator Co_SallyForth()
    { 
        // SallyForth에 맞는 랜더링 화면 오브젝트가 활성화
        lodingTipText.text = "열심히 하겠습니다!";  
        sallyForth.gameObject.SetActive(true);
         
        // 캐릭터가 전투에 나가서 아이템 파밍 정도와 죽인 캐릭터를 기반으로
        // 복귀 했을 때 경험치가 결정이 되기에, 그걸 관리하는 Achievment 컴포넌트를 초기화
        var playerAchievment = cachedGameInstance.PLAYER_GetAchievment();
        playerAchievment.SallyForth();

        // 플레이어가 전투에 나갈 때 자동 저장해야할 항목들을 저장해주기  
        var playerSave = cachedGameInstance.PLAYER_GetPlayerSave();
        // 비동기 Save 묶음
        yield return RunAsync(
            playerSave.SaveAllForSallyForthAsync()
        );

        // 너무 빨리 끝나면 로딩씬이 빨리 종료되서 텀 살짝만 두기
        yield return new WaitForSeconds(2f);
        OnPostProcessFinished();
    }
    private IEnumerator Co_Withdraw()
    {
        lodingTipText.text = "열심히 하겠습니다!"; 
        withDraw.gameObject.SetActive(true);

        var playerAchievment = cachedGameInstance.PLAYER_GetAchievment();
        var playerSave = cachedGameInstance.PLAYER_GetPlayerSave();
        playerAchievment.Withdrawal();
        
        yield return RunAsync(
            playerSave.SaveAllForWithdrawAsync()
        );

        // WareHouse를 가져와야함


        yield return new WaitForSeconds(2f);
        OnPostProcessFinished();
    }

    private IEnumerator Co_Dead()
    { 
        lodingTipText.text = "그런 스트레스도 필요해";
        dead.gameObject.SetActive(true);

        var playerAchievment = cachedGameInstance.PLAYER_GetAchievment();
        var playerSave = cachedGameInstance.PLAYER_GetPlayerSave();
        playerAchievment.Withdrawal();

        yield return RunAsync(
            playerSave.SaveAllForDeadAsync()
        );

        yield return new WaitForSeconds(3f);
        OnPostProcessFinished(); 
    }
    private IEnumerator Co_End()
    {
        lodingTipText.text = "메인메뉴로 이동중";
        GameInstance.Instance.DelatePlayer();
          
        yield return new WaitForSeconds(0f);
        OnPostProcessFinished(); 
    } 
     
    private void OnPostProcessFinished()
    {
        isPostDelayExecuted = true;
         
        loadingText.text = "로딩 완료";
        loadingText.GetComponent<UIAnimation>()?.Action_Animation();
        var disableCanvas = cachedGameInstance.UI_GetPersistentUIGroup()?.GetDisableCanvas();
        disableCanvas?.ActiveFadeIn(StartNextNevel);
        GameInstance.Instance.SOUND_PlaySoundSfx(ESoundSfxType.KeyLock, Vector3.zero);
    }   
    private void StartNextNevel()
    {
        var disableCanvas = cachedGameInstance.UI_GetPersistentUIGroup().GetDisableCanvas();
        disableCanvas.gameObject.SetActive(true);
        cachedGameInstance.LOADING_StartNextLevel();
    }  

    // ======================
    // Task → Coroutine 브릿지
    // ======================

    private IEnumerator RunAsync(Task task)
    {
        while (!task.IsCompleted)
            yield return null;

        if (task.Exception != null)
            Debug.LogException(task.Exception);
    }
     
    private void MakePlayer()
    {
        var gameInstance = GameInstance.Instance;
        var player = gameInstance.MakePlayer();
        gameInstance.SLOT_GetSlotController().Init();
        player.gameObject.SetActive(false);
    }   
}
