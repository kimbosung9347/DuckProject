using UnityEngine;
 
public enum ETimeState 
{ 
    Day, 
    Night,
    End,
}

public class PlayerTime : MonoBehaviour
{
    [SerializeField] private float gameTimeScale = 48f; // 48배속
    [SerializeField] private ETimeState curTimeState = ETimeState.End;
      
    private PlayerUIController cachedUIController;
    private bool isActiveGameTime = true;

    // 실제 시간 
    private float curPlayTime = 0f;
    // 게임 내 적용 시간
    private float curGameTime = 0f;
     
    private int lastHour = -1;
    private int lastMinute = -1;

    private void Awake()
    {
        cachedUIController = GetComponent<PlayerUIController>();
    }
    private void Start()
    {
        PlayData playData = GameInstance.Instance.SAVE_GetCurPlayData();
        curPlayTime = playData.characterData.playTime;
        curGameTime = playData.characterData.gameTime;
    } 

    private void Update()
    {
        float dt = Time.deltaTime;
        curPlayTime += dt;

        // 실제시간보다 48배빨라야함 즉 30분 -> 하루
        if (isActiveGameTime)
        {
            AdvanceGameTime(dt * gameTimeScale);
        }
    }
      
    public ETimeState GetTimeState() { return curTimeState; }
    public float GetGameTime()
    {
        return curGameTime;
    }
    public float GetPlayTime() { return curPlayTime; }
        
    public void ActiveCalculateGameTime(bool _isActive)
    {
        isActiveGameTime = _isActive;
    }
    public void RenewTimeAndStateInstant()
    {
        // curTimeState;
        const int dayStartHour = 6;
        const int nightStartHour = 18;
        
        int hour = (int)curGameTime;
        if (dayStartHour <= hour && hour <= nightStartHour)
        {
            curTimeState = ETimeState.Day; 
        }
        else
        {
            curTimeState = ETimeState.Night;
        }

        RenewTimeState(true);
    } 

    public void SetGameTime(float _gameTime)
    {
        curGameTime = _gameTime;

        RenewTime();
         
        int hour = (int)curGameTime;
        UpdateTimeState(hour);
    }
    private void AdvanceGameTime(float scaledSeconds)
    {
        curGameTime += scaledSeconds / 3600f;
        if (curGameTime >= 24f)
            curGameTime -= 24f;

        int hour = (int)curGameTime;
        int minute = (int)((curGameTime - hour) * 60f);
        // UI 갱신
        if (hour != lastHour || minute != lastMinute)
        { 
            RenewTime();
        } 

        // 시간 상태 갱신 (hour 바뀔 때만)
        if (hour != lastHour)
        {
            UpdateTimeState(hour);
        }
    }
    private void UpdateTimeState(int hour, bool _instant = false)
    {
        if (!CheckTimeState(hour))
            return;
           
        RenewTimeState(_instant);
    }
    private void RenewTime()
    {
        cachedUIController.HUD_RenewTime(curGameTime);
    }
    private bool CheckTimeState(int hour)
    { 
        const int dayStartHour = 6;
        const int nightStartHour = 18;
 
        ETimeState next =
            (hour >= dayStartHour && hour < nightStartHour)
            ? ETimeState.Day
            : ETimeState.Night;
        if (curTimeState == next)
            return false;

        curTimeState = next;
        return true;
    }
    private void RenewTimeState(bool _instant)
    {
        cachedUIController.HUD_RenewTimeState(curTimeState);

        // 여기서 조명, 포스트프로세스, AI, BGM 등 처리
        switch (curTimeState)
        {
            case ETimeState.Day:
            {
                // PlayerSight
                GetComponent<PlayerSight>().ChangeDay(_instant);
                GameInstance.Instance.MAP_ChangeDay(_instant);
            }  
            break;

            case ETimeState.Night:
            {
                // PlayerSight
                GetComponent<PlayerSight>().ChangeNight(_instant);
                GameInstance.Instance.MAP_ChangeNight(_instant);
            } 
            break;
        }
    }

}
