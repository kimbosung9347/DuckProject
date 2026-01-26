using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSubSystem : GameInstanceSubSystem
{
    private ELevelType reserveLoadLevel = ELevelType.End;
    private ELoadingScreenType reserveScreenType = ELoadingScreenType.End;
    private bool bIsComplateLoad = false;
    private AsyncOperation asyncOperation = null;

    public override void Init() { }

    public override void LevelStart(ELevelType _type)
    {
        if (_type == ELevelType.Loading)
        {
            GameInstance.Instance.StartCoroutine(UnloadAllAdditiveScenes());
        }
        else
        {
            GameInstance.Instance.StartCoroutine(UnloadLoadingScene());
        }
    } 
    public override void LevelEnd(ELevelType _type) { }
    public ELoadingScreenType GetReserveLoadingScreenType() => reserveScreenType;
    public bool IsComplateLoad() => bIsComplateLoad;

    // ================================
    // Entry
    // ================================
    public void ChangeNextLevel(ELevelType _next, ELoadingScreenType _screenType)
    {
        // 중복 요청 방지
        if (asyncOperation != null)
            return;

        reserveLoadLevel = _next;
        reserveScreenType = _screenType;
        bIsComplateLoad = false;

        GameInstance.Instance.StartCoroutine(ChangeNextLevelSequence());
    }

    public void StartNextLevel()
    {
        if (!bIsComplateLoad || asyncOperation == null)
            return;

        asyncOperation.allowSceneActivation = true;
        GameInstance.Instance.StartCoroutine(WaitForSceneActivation());
    }

    // ================================
    // Core Sequence
    // ================================
    private IEnumerator ChangeNextLevelSequence()
    {
        // 1. Loading 씬 보장
        string loadingScene = DuckDefine.GetSceneName(ELevelType.Loading);
        if (!SceneManager.GetSceneByName(loadingScene).isLoaded)
        {
            SceneManager.LoadScene(loadingScene, LoadSceneMode.Additive);
            yield return null; // 로딩 UI 1프레임 보장
        }

        // 2. 다음 씬 비동기 로드
        yield return GameInstance.Instance.StartCoroutine(LoadNextLevelCoroutine());
    }
    private IEnumerator LoadNextLevelCoroutine()
    {
        string reserveSceneName = DuckDefine.GetSceneName(reserveLoadLevel);

        asyncOperation = SceneManager.LoadSceneAsync(reserveSceneName, LoadSceneMode.Additive);
        if (asyncOperation == null)
        {
            Debug.LogError($"씬 '{reserveSceneName}'을(를) 찾을 수 없습니다. Build Settings 확인!");
            yield break;
        }

        asyncOperation.allowSceneActivation = false;

        while (asyncOperation.progress < 0.9f)
            yield return null;

        bIsComplateLoad = true;
    }
    private IEnumerator WaitForSceneActivation()
    {
        while (!asyncOperation.isDone)
            yield return null;

        string sceneName = DuckDefine.GetSceneName(reserveLoadLevel);
        Scene scene = SceneManager.GetSceneByName(sceneName);

        if (scene.IsValid())
            SceneManager.SetActiveScene(scene);

        // 상태 정리 
        asyncOperation = null;
        reserveLoadLevel = ELevelType.End;
        bIsComplateLoad = false;
    }

    // ================================
    // Scene Utilities
    // ================================
     
    private IEnumerator UnloadAllAdditiveScenes()
    {
        for (int i = SceneManager.sceneCount - 1; i >= 0; i--)
        {
            Scene scene = SceneManager.GetSceneAt(i);

            if (scene.name == DuckDefine.GetSceneName(ELevelType.Persistent) ||
                scene.name == DuckDefine.GetSceneName(ELevelType.Loading))
                continue;

            if (scene.isLoaded)
                yield return SceneManager.UnloadSceneAsync(scene);
        }

        yield return Resources.UnloadUnusedAssets();
    }
    private IEnumerator UnloadLoadingScene()
    {
        Scene loadingScene = SceneManager.GetSceneByName(DuckDefine.GetSceneName(ELevelType.Loading));

        if (loadingScene.IsValid() && loadingScene.isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync(loadingScene);
            yield return Resources.UnloadUnusedAssets(); 
        }
    }
}
