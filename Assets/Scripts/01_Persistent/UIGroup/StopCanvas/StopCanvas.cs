using UnityEngine.UI;
using UnityEngine; 
 
public class StopCanvas : MonoBehaviour
{
    [SerializeField] private Button returnToGame;
    [SerializeField] private Button returnToMenu;
    [SerializeField] private Button Setting;
    [SerializeField] private Button GameQuit;

    private SettingCanvas cachedSettingCanvas;
       
    private void Awake() 
    {
        cachedSettingCanvas = GameInstance.Instance.UI_GetPersistentUIGroup().GetSettingCanvas();
        returnToGame.onClick.AddListener(PressReturnToGame);
        returnToMenu.onClick.AddListener(PressReturnToMenu);
        Setting.onClick.AddListener(PressSetting);
        GameQuit.onClick.AddListener(PressGameQuit);
        gameObject.SetActive(false);
    } 

    public void Active()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;
        GetComponent<UIAnimation>()?.Action_Animation();
    }
    public void Disable()
    { 
        var playerController = GameInstance.Instance.PLAYER_GetPlayerController();
        playerController.ChangePlay(); 

        Time.timeScale = 1f;
        gameObject.SetActive(false);
        cachedSettingCanvas.Disable();
    } 
     
    private void PressReturnToGame()
    {
        Disable(); 
    }  

    private void PressReturnToMenu() 
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
         
        var gameInstance = GameInstance.Instance;
        gameInstance.LOADING_ChangeNextLevel(ELevelType.Mainmenu, ELoadingScreenType.End);
        gameInstance.SOUND_StopBGM();  
    }

    private void PressSetting()
    {
        // 설정 캔버스 활성화
        cachedSettingCanvas.Active(this);
        gameObject.SetActive(false);
    }   

    private void PressGameQuit()
    {
        Time.timeScale = 1f;
        Application.Quit();
    } 
}  
