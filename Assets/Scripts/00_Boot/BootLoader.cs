using UnityEngine;
using UnityEngine.SceneManagement;

public class BootLoader : MonoBehaviour
{
    [SerializeField] private ELevelType loadLevel = ELevelType.Mainmenu;

    private void Awake()
    {
        // Persistent 씬 로드
        SceneManager.LoadScene(DuckDefine.GetSceneName(ELevelType.Persistent), LoadSceneMode.Single);
           
        // 동기 로드
        string loadName = DuckDefine.GetSceneName(loadLevel);
        SceneManager.LoadSceneAsync(loadName, LoadSceneMode.Additive);
    }    
} 
